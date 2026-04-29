using Greg.Xrm.Command.Commands.Help;
using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Updates;
using Microsoft.Extensions.Logging;

namespace Greg.Xrm.Command
{
	public sealed partial class Bootstrapper(
		ILogger<Bootstrapper> logger,
		IOutput output,
		ICommandRegistry registry,
		ICommandRunnerFactory commandRunnerFactory,
		ICommandLineArguments args,
		IAutoUpdater updater,
		IOrganizationServiceRepository organizationServiceRepository,
		Greg.Xrm.Command.Diagnostics.ICorrelationIdProvider correlationIdProvider)
	{
		private readonly ILogger log = logger;

		public async Task<int> StartAsync(CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			await updater.CheckForUpdatesAsync(cancellationToken);
			cancellationToken.ThrowIfCancellationRequested();
			ShowTitleBanner();

			LogStartAsync(log);

			registry.InitializeFromAssembly(typeof(HelpCommand).Assembly);
			registry.ScanPluginsFolder(args);
			await ShowFirstRunAuthGuidanceAsync(cancellationToken).ConfigureAwait(false);

			var commandRunner = commandRunnerFactory.CreateCommandRunner();
			var result = await commandRunner.RunCommandAsync(cancellationToken);

			await updater.LaunchUpdateAsync(cancellationToken);
			return result;
		}






		private void ShowTitleBanner()
		{
			if (args.Contains("--noprompt") || args.Contains("--nologo"))
			{
				args.Remove("--noprompt");
				args.Remove("--nologo");
				return;
			}


			output.Write(">>> Greg PowerPlatform CLI Extended (PACX) <<<", ConsoleColor.Green)
				.WriteLine(" - Dataverse command tool", ConsoleColor.DarkGray);
			output.Write("Version ")
				.Write(updater.CurrentVersion);

			if (updater.UpdateRequired)
			{
				output.Write(" - New version available (will be installed on exit): ", ConsoleColor.Yellow)
					.Write(updater.NextVersion, ConsoleColor.Yellow);
			}

			output.WriteLine();
			output.Write("Online documentation: ").WriteLine("https://github.com/edithatogo/Greg.Xrm.Command");
			output.Write("Package format: ").WriteLine("conductor/pacx-package-format.md");
			output.Write("Feedback, Suggestions, Issues: ").WriteLine("fork-local issues or discussions");
			output.WriteCorrelationHeader(correlationIdProvider.Current);
			output.WriteLine();
		}

		private async Task ShowFirstRunAuthGuidanceAsync(CancellationToken cancellationToken)
		{
			if (args.Count == 0
				|| args.Contains("--noprompt")
				|| args.Contains("--interactive")
				|| IsAuthFreeCommand())
			{
				return;
			}

			var connections = await organizationServiceRepository.GetAllConnectionDefinitionsAsync(cancellationToken).ConfigureAwait(false);
			if (connections.ConnectionStringKeys.Count > 0)
			{
				return;
			}

			output.WriteLine("No authentication profiles found.", ConsoleColor.Yellow);
			output.WriteLine("Create one with: pacx auth create --name <profile> --environment <environment-url>", ConsoleColor.Yellow);
			output.WriteLine("Then confirm it with: pacx auth whoami", ConsoleColor.Yellow);
			output.WriteLine();
		}

		private bool IsAuthFreeCommand()
		{
			if (args.Count == 0) return true;

			var firstVerb = args.FirstOrDefault(arg => !arg.StartsWith("-", StringComparison.Ordinal));
			return firstVerb == null
				|| string.Equals(firstVerb, "auth", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(firstVerb, "help", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(firstVerb, "completions", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(firstVerb, "telemetry", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(firstVerb, "ping", StringComparison.OrdinalIgnoreCase);
		}
	}
}
