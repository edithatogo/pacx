using System.ComponentModel.DataAnnotations;

namespace Greg.Xrm.Command.Commands.Solution
{
	[Command("solution", "patch", HelpText = "Creates a patch for an existing solution.")]
	public class PatchCommand
	{
		[Option("uniqueName", "un", HelpText = "The unique name of the parent solution to patch.")]
		[Required]
		public string? ParentSolutionUniqueName { get; set; }

		[Option("displayName", "n", HelpText = "The display name for the patch solution.")]
		public string? DisplayName { get; set; }

		[Option("version", "v", HelpText = "The version number for the patch (e.g. 1.0.0.1).")]
		public string? Version { get; set; }
	}
}
