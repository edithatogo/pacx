using System.ComponentModel.DataAnnotations;

namespace Greg.Xrm.Command.Commands.Solution
{
	[Command("solution", "upgrade", HelpText = "Stages and upgrades a solution in the current Dataverse environment.")]
	public class UpgradeCommand
	{
		[Option("uniqueName", "un", HelpText = "The unique name of the solution to upgrade.")]
		[Required]
		public string? SolutionUniqueName { get; set; }

		[Option("async", "a", HelpText = "Run the upgrade asynchronously.", DefaultValue = false)]
		public bool Async { get; set; }
	}
}
