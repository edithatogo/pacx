using System.Reflection;
using System.Runtime.InteropServices;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Greg.Xrm.Command.Commands.Diag
{
	public class DiagCommandExecutor(IOutput output, IOrganizationServiceRepository orgServiceRepo) : ICommandExecutor<DiagCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DiagCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			output.WriteLine("PACX Diagnostics", ConsoleColor.Cyan);
			output.WriteLine();

			var checks = new List<DiagCheckResult>
			{
				CheckDotNetRuntime(),
				CheckPacxVersion(),
				CheckOsPlatform(),
				await CheckDataverseConnectivityAsync(cancellationToken),
				CheckAuthStatus(),
				CheckEnvironmentVariables()
			};

			output.WriteTable(
				checks,
				() => ["#", "Check", "Status", "Detail"],
				check => [check.Index.ToString(), check.Name, check.Status, check.Detail],
				(_, check) => check.Status switch
				{
					"Pass" => ConsoleColor.Green,
					"Fail" => ConsoleColor.Red,
					"Warn" => ConsoleColor.Yellow,
					_ => Console.ForegroundColor
				});

			if (command.Verbose)
			{
				output.WriteLine();
				output.WriteLine("Verbose Details", ConsoleColor.Cyan);
				foreach (var check in checks.Where(c => c.Status != "Pass"))
				{
					output.Write("  [").Write(check.Status, GetColor(check.Status)).Write("] ");
					output.WriteLine(check.Name);
					if (!string.IsNullOrEmpty(check.ErrorDetails))
					{
						output.Write("    Error: ").WriteLine(check.ErrorDetails, ConsoleColor.DarkGray);
					}
				}
			}

			var hasFailures = checks.Any(c => c.Status == "Fail");
			return hasFailures ? CommandResult.Fail("Some diagnostics failed. Review the output above.") : CommandResult.Success();
		}

		private static ConsoleColor GetColor(string status) => status switch
		{
			"Pass" => ConsoleColor.Green,
			"Fail" => ConsoleColor.Red,
			"Warn" => ConsoleColor.Yellow,
			_ => Console.ForegroundColor
		};

		private static DiagCheckResult CheckDotNetRuntime()
		{
			var description = RuntimeInformation.FrameworkDescription;
			return new DiagCheckResult(1, ".NET Runtime", "Pass", description);
		}

		private static DiagCheckResult CheckPacxVersion()
		{
			var version = Assembly.GetEntryAssembly()?.GetName()?.Version;
			if (version == null)
				return new DiagCheckResult(2, "PACX Version", "Warn", "Could not determine version");
			return new DiagCheckResult(2, "PACX Version", "Pass", version.ToString());
		}

		private static DiagCheckResult CheckOsPlatform()
		{
			var osDescription = RuntimeInformation.OSDescription;
			var arch = RuntimeInformation.OSArchitecture;
			var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
			var detail = $"{osDescription} ({arch})";
			return new DiagCheckResult(3, "OS Platform", "Pass", detail);
		}

		private async Task<DiagCheckResult> CheckDataverseConnectivityAsync(CancellationToken cancellationToken)
		{
			try
			{
				var service = await orgServiceRepo.GetCurrentConnectionAsync(cancellationToken);
				var whoAmI = await service.ExecuteAsync(new WhoAmIRequest(), cancellationToken);
				var userId = whoAmI.Results.ContainsKey("UserId") ? whoAmI.Results["UserId"] : null;
				var orgId = whoAmI.Results.ContainsKey("OrganizationId") ? whoAmI.Results["OrganizationId"] : null;
				var detail = $"UserId: {userId}, OrganizationId: {orgId}";
				return new DiagCheckResult(4, "Dataverse Connectivity", "Pass", detail);
			}
			catch (Exception ex)
			{
				return new DiagCheckResult(4, "Dataverse Connectivity", "Fail", "Unable to connect to Dataverse")
				{
					ErrorDetails = ex.Message
				};
			}
		}

		private static DiagCheckResult CheckAuthStatus()
		{
			var connectionName = Environment.GetEnvironmentVariable("PACX_CONNECTION_NAME");
			var envUrl = Environment.GetEnvironmentVariable("PACX_INTEGRATION_ENV_URL");

			if (!string.IsNullOrEmpty(connectionName) || !string.IsNullOrEmpty(envUrl))
			{
				var parts = new List<string>();
				if (!string.IsNullOrEmpty(connectionName))
					parts.Add($"Connection: {connectionName}");
				if (!string.IsNullOrEmpty(envUrl))
					parts.Add($"EnvUrl: {RedactUrl(envUrl)}");
				return new DiagCheckResult(5, "Authentication", "Pass", string.Join(", ", parts));
			}

			return new DiagCheckResult(5, "Authentication", "Warn", "No environment variables set (PACX_CONNECTION_NAME or PACX_INTEGRATION_ENV_URL)");
		}

		private static DiagCheckResult CheckEnvironmentVariables()
		{
			var relevantVars = new[] { "MSAL_CLIENT_ID", "PACX_INTEGRATION_ENV_URL", "PACX_CONNECTION_NAME" };
			var found = new List<string>();

			foreach (var name in relevantVars)
			{
				var value = Environment.GetEnvironmentVariable(name);
				if (!string.IsNullOrEmpty(value))
				{
					found.Add($"{name}={RedactValue(name, value)}");
				}
			}

			if (found.Count > 0)
				return new DiagCheckResult(6, "Environment Variables", "Pass", string.Join(", ", found));

			return new DiagCheckResult(6, "Environment Variables", "Pass", "None set");
		}

		private static string RedactUrl(string url)
		{
			try
			{
				var uri = new Uri(url);
				return $"{uri.Scheme}://{uri.Host}/...";
			}
			catch
			{
				return "[redacted]";
			}
		}

		private static string RedactValue(string varName, string value)
		{
			if (varName.Contains("SECRET", StringComparison.OrdinalIgnoreCase)
				|| varName.Contains("TOKEN", StringComparison.OrdinalIgnoreCase)
				|| varName.Contains("KEY", StringComparison.OrdinalIgnoreCase)
				|| varName.Contains("PASSWORD", StringComparison.OrdinalIgnoreCase)
				|| varName.Contains("CONNECTION", StringComparison.OrdinalIgnoreCase))
			{
				return value.Length > 8 ? value[..4] + "..." + value[^4..] : "****";
			}
			return value;
		}

		private sealed class DiagCheckResult
		{
			public int Index { get; }
			public string Name { get; }
			public string Status { get; }
			public string Detail { get; }
			public string? ErrorDetails { get; set; }

			public DiagCheckResult(int index, string name, string status, string detail)
			{
				Index = index;
				Name = name;
				Status = status;
				Detail = detail;
			}
		}
	}
}
