using Greg.Xrm.Command.Services.CommandHistory;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.History
{
	public class SetLengthCommandExecutor : ICommandExecutor<SetLengthCommand>
	{
		private readonly IOutput output;
		private readonly IHistoryTracker historyTracker;

		public SetLengthCommandExecutor(IOutput output, IHistoryTracker historyTracker)
		{
			ArgumentNullException.ThrowIfNull(output);
			ArgumentNullException.ThrowIfNull(historyTracker);
			this.output = output;
			this.historyTracker = historyTracker;
		}


		public async Task<CommandResult> ExecuteAsync(SetLengthCommand command, CancellationToken cancellationToken)
		{
			this.output.Write("Setting command history max length to ").Write(command.Length).Write("...");

			await this.historyTracker.SetMaxLengthAsync(command.Length).ConfigureAwait(false);

			this.output.WriteLine("Done", ConsoleColor.Green);
			return CommandResult.Success();
		}
	}
}
