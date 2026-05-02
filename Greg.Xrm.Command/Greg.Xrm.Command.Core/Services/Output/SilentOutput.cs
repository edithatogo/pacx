namespace Greg.Xrm.Command.Services.Output
{
	public class SilentOutput : IOutput
	{
		public IOutput Write(object? text) => this;

		public IOutput Write(object? text, ConsoleColor color) => this;

		public IOutput WriteLine() => this;

		public IOutput WriteLine(object? text) => this;

		public IOutput WriteLine(object? text, ConsoleColor color) => this;

		public IOutput WriteTable<TRow>(IReadOnlyList<TRow> collection, Func<string[]> rowHeaders, Func<TRow, string[]> rowData, Func<int, TRow, ConsoleColor?>? colorPicker = null) => this;

		public IOutput WriteCorrelationHeader(string correlationId) => this;
	}
}
