using System.Reflection;
using Greg.Xrm.Command.Services.CommandHistory;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.History
{
	public class GetCommandExecutor : ICommandExecutor<GetCommand>
	{
		private readonly IOutput output;
		private readonly IHistoryTracker historyTracker;

		public GetCommandExecutor(IOutput output, IHistoryTracker historyTracker)
		{
			ArgumentNullException.ThrowIfNull(output);
			ArgumentNullException.ThrowIfNull(historyTracker);
			this.output = output;
			this.historyTracker = historyTracker;
		}

		public async Task<CommandResult> ExecuteAsync(GetCommand command, CancellationToken cancellationToken)
		{
			if (command.Length.HasValue)
			{
				this.output.Write("Retrieving last ").Write(command.Length).Write(" commands...");
			}
			else
			{
				this.output.Write("Retrieving all commands...");
			}


			var commands = await this.historyTracker.GetLastAsync(command.Length).ConfigureAwait(false);

			this.output.WriteLine("Done", ConsoleColor.Green);

			var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;

			if (commands.Count == 0)
			{
				this.output.WriteLine("No commands found", ConsoleColor.Yellow);
				return CommandResult.Success();
			}

			var i = 0;
			var padding = commands.Count.ToString().Length;
			foreach (var c in commands)
			{
				this.output.Write("  [").Write(i.ToString().PadLeft(padding)).Write("] ").Write(assemblyName).Write(" ").WriteLine(c);
				i++;
			}

			if (!string.IsNullOrWhiteSpace(command.File))
			{
				this.output.Write("Saving to file ").Write(command.File).Write("...");
				await File.WriteAllLinesAsync(command.File, commands.Select(x => $"{assemblyName} {x}"), cancellationToken).ConfigureAwait(false);
				this.output.WriteLine("Done", ConsoleColor.Green);
			}

			return CommandResult.Success();
		}
	}
}
