using System.ComponentModel.DataAnnotations;

namespace Greg.Xrm.Command.Commands.Solution
{
	[Command("solution", "clone", HelpText = "Clones a solution as a new solution with a new version.")]
	public class CloneCommand
	{
		[Option("uniqueName", "un", HelpText = "The unique name of the solution to clone.")]
		[Required]
		public string? SolutionUniqueName { get; set; }

		[Option("displayName", "n", HelpText = "The display name for the cloned solution.")]
		public string? DisplayName { get; set; }

		[Option("version", "v", HelpText = "The version number for the cloned solution (e.g. 1.0.0.1).")]
		public string? Version { get; set; }
	}
}
