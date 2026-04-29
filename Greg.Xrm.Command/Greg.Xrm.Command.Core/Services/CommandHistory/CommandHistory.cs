namespace Greg.Xrm.Command.Services.CommandHistory
{
#pragma warning disable MA0049 // Type name should not match containing namespace
	sealed class CommandHistory
#pragma warning restore MA0049 // Type name should not match containing namespace
	{
		public int MaxSize { get; set; } = 1000;

		public List<string> Commands { get; set; } = new List<string>();

		public void Add(params string[] command)
		{
			var commandText = string.Join(" ", command.Select(x => EncloseInQuotesIfContainsSpace(x)));

			var lastCommand = this.Commands.LastOrDefault();
			if (string.Equals(lastCommand, commandText, StringComparison.Ordinal)) return;

			this.Commands.Add(commandText);
			if (this.Commands.Count > this.MaxSize)
			{
				this.Commands.RemoveAt(0);
			}
		}

		private static string EncloseInQuotesIfContainsSpace(string command)
		{
			if (command.Contains(' '))
			{
				return $"\"{command}\"";
			}
			return command;
		}
	}

}
