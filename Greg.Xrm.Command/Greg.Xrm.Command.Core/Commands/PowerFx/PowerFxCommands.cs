using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.PowerFx
{
	[Command("power-fx", "validate", HelpText = "Validate a Power Fx expression or expression file.")]
	public class PowerFxValidateCommand
	{
		[Option("expression", "e", Order = 1, HelpText = "Power Fx expression to validate.")]
		public string? Expression { get; set; }

		[Option("file", "f", Order = 2, HelpText = "Text, JSON, or YAML file containing expressions.")]
		public string? FilePath { get; set; }

		[Option("table", "t", Order = 3, HelpText = "Optional Dataverse table logical name for future binding-aware validation.")]
		public string? TableName { get; set; }
	}

	[Command("power-fx", "format", HelpText = "Format a Power Fx expression or expression file.")]
	public class PowerFxFormatCommand
	{
		[Option("expression", "e", Order = 1, HelpText = "Power Fx expression to format.")]
		public string? Expression { get; set; }

		[Option("file", "f", Order = 2, HelpText = "Text file containing a Power Fx expression.")]
		public string? FilePath { get; set; }

		[Option("in-place", Order = 3, HelpText = "Write formatted output back to the file.")]
		public bool InPlace { get; set; }
	}

	[Command("power-fx", "repl", HelpText = "Print Power Fx REPL startup information.")]
	public class PowerFxReplCommand
	{
	}
}
