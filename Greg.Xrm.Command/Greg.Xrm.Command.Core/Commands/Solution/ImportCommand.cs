using System.ComponentModel.DataAnnotations;

namespace Greg.Xrm.Command.Commands.Solution
{
	[Command("solution", "import", HelpText = "Imports a solution file into the current Dataverse environment.")]
	public class ImportCommand
	{
		[Option("file", "f", HelpText = "Path to the solution .zip file to import.")]
		[Required]
		public string? FilePath { get; set; }

		[Option("overwrite", "ow", HelpText = "Overwrite the solution if it already exists.", DefaultValue = true)]
		public bool Overwrite { get; set; } = true;

		[Option("publish", "p", HelpText = "Publish the solution after import.", DefaultValue = true)]
		public bool Publish { get; set; } = true;
	}
}
