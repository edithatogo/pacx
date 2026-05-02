using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Update
{
	[Command("self-update", HelpText = "Check for and install the latest version of PACX.")]
	[Alias("update")]
	public class SelfUpdateCommand
	{
		[Option("check", "c", Order = 1, HelpText = "Only check for updates without installing.")]
		public bool CheckOnly { get; set; }

		[Option("version", "v", Order = 2, HelpText = "Install a specific version instead of latest.")]
		public string? Version { get; set; }

		[Option("pre-release", "p", Order = 3, HelpText = "Include pre-release versions.")]
		public bool IncludePrerelease { get; set; }
	}
}
