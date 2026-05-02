using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Diag
{
	[Command("diag", HelpText = "Run diagnostics to troubleshoot PACX setup.")]
	public class DiagCommand
	{
		[Option("verbose", "v", Order = 1, HelpText = "Show detailed diagnostic information.")]
		public bool Verbose { get; set; }
	}
}
