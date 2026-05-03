using Greg.Xrm.Command.Services.CommandHistory;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.History
{
	public class ClearCommandExecutor : ICommandExecutor<ClearCommand>
	{
		private readonly IOutput output;
		private readonly IHistoryTracker historyTracker;

		public ClearCommandExecutor(IOutput output, IHistoryTracker historyTracker)
		{
			ArgumentNullException.ThrowIfNull(output);
			ArgumentNullException.ThrowIfNull(historyTracker);
			this.output = output;
			this.historyTracker = historyTracker;
		}


		public async Task<CommandResult> ExecuteAsync(ClearCommand command, CancellationToken cancellationToken)
		{
			this.output.Write("Cleaning up command history...");

			await this.historyTracker.ClearAsync().ConfigureAwait(false);

			this.output.WriteLine("Done", ConsoleColor.Green);

			return CommandResult.Success();
		}
	}
}
