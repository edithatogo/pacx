using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services.CommandHistory;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Telemetry;
using Microsoft.Extensions.Logging;

namespace Greg.Xrm.Command
{
	sealed class CommandRunnerCli(
			IOutput output,
			ILogger log,
			ICommandParser parser,
			ICommandExecutorFactory commandExecutorFactory,
			IHistoryTracker historyTracker,
			ICommandLineArguments args,
			ITelemetryService telemetryService,
			Greg.Xrm.Command.Diagnostics.ICorrelationIdProvider correlationIdProvider)
#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
		: CommandRunnerBase(output, log, commandExecutorFactory, historyTracker, args, telemetryService, correlationIdProvider), ICommandRunner
#pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
	{
		public async Task<int> RunCommandAsync(CancellationToken cancellationToken)
		{
			object command;
			try
			{
				(command, _) = parser.Parse(args);
			}
			catch (Exception ex) when (ex is CommandException || ex is System.Data.DataException || ex is ArgumentException)
			{
				output.WriteLine(ex.Message, ConsoleColor.Red).WriteLine();
				return ExitCodes.UsageError;
			}

			if (command == null)
			{
				return ExitCodes.UsageError;
			}

			if (!IsValidCommand(command))
			{
				return ExitCodes.ValidationError;
			}

			await TrackCommandIntoHistoryAsync(command);

			var result = await ExecuteCommand(command, cancellationToken);

			return result;
		}
	}
}
