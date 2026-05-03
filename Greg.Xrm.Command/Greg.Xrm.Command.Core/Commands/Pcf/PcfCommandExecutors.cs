using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Greg.Xrm.Command.Commands.Pcf
{
	public class PcfTestCommandExecutor(
		IOutput output) : ICommandExecutor<PcfTestCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PcfTestCommand command, CancellationToken cancellationToken)
		{
			var projectPath = command.Path ?? Environment.CurrentDirectory;
			if (!Directory.Exists(projectPath))
			{
				return CommandResult.Fail($"PCF project path not found: {projectPath}");
			}

			output.WriteLine($"Running PCF tests...", ConsoleColor.Cyan);
			output.WriteLine($"  Project: {projectPath}");
			output.WriteLine($"  Browser: {command.Browser}");
			output.WriteLine($"  Reporter: {command.Reporter}");
			output.WriteLine();

			try
			{
				var psi = new ProcessStartInfo
				{
					FileName = "npx",
					Arguments = $"pcf-test --browser {command.Browser} --reporter {command.Reporter}",
					WorkingDirectory = projectPath,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true,
				};

				using var process = Process.Start(psi);
				if (process == null)
				{
					return CommandResult.Fail("Failed to start npx pcf-test. Is Node.js installed?");
				}

				var stdOut = await process.StandardOutput.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
				var stdErr = await process.StandardError.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
				await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

				output.WriteLine(stdOut);

				if (process.ExitCode != 0)
				{
					if (!string.IsNullOrEmpty(stdErr))
						output.WriteLine(stdErr, ConsoleColor.Red);
					output.WriteLine($"Tests failed with exit code {process.ExitCode}", ConsoleColor.Red);
					return CommandResult.Fail($"PCF tests failed (exit code: {process.ExitCode})");
				}

				output.WriteLine("PCF tests passed!", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to run PCF tests: {ex.Message}", ex);
			}
		}
	}

	public class PcfPublishCommandExecutor(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository) : ICommandExecutor<PcfPublishCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PcfPublishCommand command, CancellationToken cancellationToken)
		{
			var projectPath = command.Path ?? Environment.CurrentDirectory;
			if (!Directory.Exists(projectPath))
			{
				return CommandResult.Fail($"PCF project path not found: {projectPath}");
			}

			var manifestPath = Path.Combine(projectPath, "ControlManifest.Input.xml");
			if (!File.Exists(manifestPath))
			{
				return CommandResult.Fail("ControlManifest.Input.xml not found. Are you in a PCF project directory?");
			}

			var doc = XDocument.Load(manifestPath);
			var ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;
			var control = doc.Descendants(ns + "control").FirstOrDefault();
			var version = control?.Attribute("version")?.Value ?? "unknown";
			var name = control?.Attribute("namespace")?.Value + "." + control?.Attribute("name")?.Value;

			output.WriteLine($"PCF Component: {name} v{version}", ConsoleColor.Cyan);

			if (command.DryRun)
			{
				output.WriteLine("[DRY RUN] Would publish:", ConsoleColor.Yellow);
				output.WriteLine($"  Component: {name}");
				output.WriteLine($"  Version: {version}");
				if (!string.IsNullOrEmpty(command.SolutionUniqueName))
					output.WriteLine($"  Solution: {command.SolutionUniqueName}");
				return CommandResult.Success();
			}

			var pacPath = command.PacPath;
			var solutionName = command.SolutionUniqueName ?? $"PcfComponent_{name.Replace('.', '_')}";
			var solutionDir = Path.Combine(Path.GetTempPath(), $"pacx_pcf_{Guid.NewGuid():N}");
			var zipPath = Path.Combine(Path.GetTempPath(), $"{solutionName}.zip");

			try
			{
				Directory.CreateDirectory(solutionDir);

				output.Write("Initializing solution...");
				await RunPacCommandAsync(pacPath, $"solution init --publisherName pacx --customizationPrefix px --outputDirectory \"{solutionDir}\"", cancellationToken).ConfigureAwait(false);
				output.WriteLine(" Done", ConsoleColor.Green);

				output.Write("Adding PCF reference...");
				await RunPacCommandAsync(pacPath, $"solution add-reference --path \"{projectPath}\"", cancellationToken).ConfigureAwait(false);
				output.WriteLine(" Done", ConsoleColor.Green);

				output.Write("Packing solution...");
				await RunPacCommandAsync(pacPath, $"solution pack --zipfile \"{zipPath}\" --folder \"{solutionDir}\"", cancellationToken).ConfigureAwait(false);
				output.WriteLine(" Done", ConsoleColor.Green);

				output.Write("Connecting to Dataverse...");
				var crm = await organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
				output.WriteLine(" Done", ConsoleColor.Green);

				output.Write("Importing solution...");
				var zipBytes = await File.ReadAllBytesAsync(zipPath, cancellationToken).ConfigureAwait(false);
				var importRequest = new ImportSolutionRequest
				{
					CustomizationFile = zipBytes,
					OverwriteUnmanagedCustomizations = true,
					PublishWorkflows = true,
				};
				await crm.ExecuteAsync(importRequest, cancellationToken).ConfigureAwait(false);
				output.WriteLine(" Done", ConsoleColor.Green);

				output.WriteLine($"Successfully published {name} v{version}", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Publish failed: {ex.Message}", ex);
			}
			finally
			{
				try
				{
					if (Directory.Exists(solutionDir))
						Directory.Delete(solutionDir, recursive: true);
					if (File.Exists(zipPath))
						File.Delete(zipPath);
				}
				catch { }
			}
		}

		private static async Task RunPacCommandAsync(string pacPath, string arguments, CancellationToken cancellationToken)
		{
			var psi = new ProcessStartInfo
			{
				FileName = pacPath,
				Arguments = arguments,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true,
			};
			using var process = Process.Start(psi);
			if (process == null)
				throw new InvalidOperationException("pac CLI is not installed or not found.");
			var stdErr = await process.StandardError.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
			await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
			if (process.ExitCode != 0)
				throw new InvalidOperationException($"pac CLI failed: {stdErr}");
		}
	}

	public class PcfVersionBumpCommandExecutor(
		IOutput output) : ICommandExecutor<PcfVersionBumpCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PcfVersionBumpCommand command, CancellationToken cancellationToken)
		{
			var projectPath = command.Path ?? Environment.CurrentDirectory;
			var manifestPath = Path.Combine(projectPath, "ControlManifest.Input.xml");

			if (!File.Exists(manifestPath))
			{
				return CommandResult.Fail("ControlManifest.Input.xml not found.");
			}

			var doc = XDocument.Load(manifestPath);
			var ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;
			var control = doc.Descendants(ns + "control").FirstOrDefault();
			var versionStr = control?.Attribute("version")?.Value ?? "1.0.0";

			var parts = versionStr.Split('.');
			if (parts.Length < 3 || !int.TryParse(parts[0], out var major) ||
obj!int.TryParse(parts[1], out var minor) || !int.TryParse(parts[2], out var patch))
			{
				return CommandResult.Fail($"Invalid version format: '{versionStr}'. Expected 'major.minor.patch'.");
			}

			var newVersion = command.BumpType.ToLowerInvariant() switch
			{
				"major" => $"{major + 1}.0.0",
				"minor" => $"{major}.{minor + 1}.0",
				"patch" => $"{major}.{minor}.{patch + 1}",
				_ => versionStr
			};

			output.WriteLine($"Bumping version: {versionStr} -> {newVersion} ({command.BumpType})", ConsoleColor.Cyan);

			if (control != null)
			{
				control.SetAttributeValue("version", newVersion);
				doc.Save(manifestPath);
				output.WriteLine($"Updated ControlManifest.Input.xml to v{newVersion}", ConsoleColor.Green);
			}

			if (!string.IsNullOrEmpty(command.Message))
			{
				var changelogPath = Path.Combine(projectPath, "CHANGELOG.md");
				var entry = $"## v{newVersion}\n- {command.Message}\n\n";
				if (File.Exists(changelogPath))
				{
					var existing = await File.ReadAllTextAsync(changelogPath, cancellationToken).ConfigureAwait(false);
					await File.WriteAllTextAsync(changelogPath, entry + existing, cancellationToken).ConfigureAwait(false);
				}
				else
				{
					await File.WriteAllTextAsync(changelogPath, $"# Changelog\n\n{entry}", cancellationToken).ConfigureAwait(false);
				}
				output.WriteLine("Updated CHANGELOG.md", ConsoleColor.Green);
			}

			return CommandResult.Success();
		}
	}

	public class PcfDependencyCheckCommandExecutor(
		IOutput output,
		IOrganizationServiceRepository organizationServiceRepository) : ICommandExecutor<PcfDependencyCheckCommand>
	{
		public async Task<CommandResult> ExecuteAsync(PcfDependencyCheckCommand command, CancellationToken cancellationToken)
		{
			output.WriteLine("PCF Dependency Check", ConsoleColor.Cyan);
			output.WriteLine();

			try
			{
				output.Write("Connecting to Dataverse...");
				var crm = await organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
				output.WriteLine(" Done", ConsoleColor.Green);

				// Check 1: Organization connectivity
				output.Write("  [1/3] Organization connectivity...");
				var whoAmI = (WhoAmIResponse)await crm.ExecuteAsync(new WhoAmIRequest(), cancellationToken).ConfigureAwait(false);
				output.WriteLine(" Pass", ConsoleColor.Green);

				// Check 2: Organization version
				output.Write("  [2/3] Organization version...");
				string? version = null;
				if (crm is ServiceClient sc)
				{
					version = sc.OrganizationDetail?.OrganizationVersion;
				}

				if (version == null)
				{
					output.WriteLine(" Unknown (could not determine version)", ConsoleColor.Yellow);
				}
				else
				{
					output.Write($" {version}...");
					var versionParts = version.Split('.');
					if (versionParts.Length >= 1 && int.TryParse(versionParts[0], out var major) && major >= 9)
					{
						output.WriteLine(" Pass (9.0+ supported)", ConsoleColor.Green);
					}
					else
					{
						output.WriteLine(" Fail (PCF requires version 9.0+)", ConsoleColor.Red);
						return CommandResult.Fail($"PCF dependency check failed: Dataverse version {version} is below 9.0. PCF requires version 9.0 or later.");
					}
				}

				// Check 3: User context
				output.Write("  [3/3] User context...");
				output.WriteLine($" User {whoAmI.UserId} in Org {whoAmI.OrganizationId}", ConsoleColor.Green);

				output.WriteLine();
				output.WriteLine("All PCF dependency checks passed!", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"PCF dependency check failed: {ex.Message}", ex);
			}
		}
	}
}

