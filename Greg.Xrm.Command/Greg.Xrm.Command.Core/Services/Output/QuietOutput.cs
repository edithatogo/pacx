namespace Greg.Xrm.Command.Services.Output
{
	public class QuietOutput : IOutput
	{
		private readonly IOutput inner;

		private static readonly ConsoleColor[] quietColors =
		[
			ConsoleColor.Yellow,
			ConsoleColor.Red,
			ConsoleColor.DarkYellow,
			ConsoleColor.DarkRed,
			ConsoleColor.Magenta,
			ConsoleColor.DarkMagenta,
		];

		public QuietOutput(IOutput inner)
		{
			ArgumentNullException.ThrowIfNull(inner);
			this.inner = inner;
		}

		public IOutput Write(object? text) => this;

		public IOutput Write(object? text, ConsoleColor color) => IsQuietColor(color) ? inner.Write(text, color) : this;

		public IOutput WriteLine() => this;

		public IOutput WriteLine(object? text) => this;

		public IOutput WriteLine(object? text, ConsoleColor color) => IsQuietColor(color) ? inner.WriteLine(text, color) : this;

		public IOutput WriteTable<TRow>(IReadOnlyList<TRow> collection, Func<string[]> rowHeaders, Func<TRow, string[]> rowData, Func<int, TRow, ConsoleColor?>? colorPicker = null) => this;

		public IOutput WriteCorrelationHeader(string correlationId) => this;

		private static bool IsQuietColor(ConsoleColor color)
		{
			return quietColors.Contains(color);
		}
	}
}
