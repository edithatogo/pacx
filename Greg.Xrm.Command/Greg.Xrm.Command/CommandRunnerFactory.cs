using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services.CommandHistory;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Telemetry;
using Microsoft.Extensions.Logging;
using Spectre.Console;

namespace Greg.Xrm.Command
{
	sealed class CommandRunnerFactory(
			IOutput output,
			IAnsiConsole ansiConsole,
			ILogger<CommandRunnerFactory> log,
			ICommandRegistry registry,
			ICommandParser parser,
			ICommandExecutorFactory commandExecutorFactory,
			IHistoryTracker historyTracker,
			ICommandLineArguments args,
			ITelemetryService telemetryService,
			Greg.Xrm.Command.Diagnostics.ICorrelationIdProvider correlationIdProvider) : ICommandRunnerFactory
	{
		public ICommandRunner CreateCommandRunner()
		{
			if (args.Count == 0)
			{
				return Cli();
			}

			if (args.Contains("--interactive"))
			{
				if (args.Count == 1)
				{
					return Interactive();
				}
				else
				{
					output.WriteLine("The --interactive flag cannot be used with other arguments.", ConsoleColor.Red);
					return NoOp(ExitCodes.UsageError);
				}
			}

			return Cli();
		}

		private CommandRunnerCli Cli() => new(output, log, parser, commandExecutorFactory, historyTracker, args, telemetryService, correlationIdProvider);

		private Interactive.CommandRunnerInteractive Interactive() => new(output, ansiConsole, log, registry, commandExecutorFactory, historyTracker, args, telemetryService, correlationIdProvider);

		private static CommandRunnerNoOp NoOp(int result) => new(result);
	}
}
