using System.ComponentModel.DataAnnotations;

namespace Greg.Xrm.Command.Commands.Solution
{
	[Command("solution", "export", HelpText = "Exports a solution from the current Dataverse environment to a .zip file.")]
	public class ExportCommand
	{
		[Option("uniqueName", "un", HelpText = "The unique name of the solution to export.")]
		[Required]
		public string? SolutionUniqueName { get; set; }

		[Option("managed", "m", HelpText = "Export as a managed solution.", DefaultValue = false)]
		public bool Managed { get; set; }

		[Option("output", "o", HelpText = "Output directory for the exported solution file. Defaults to current directory.")]
		public string? OutputDirectory { get; set; }

		[Option("filename", "fn", HelpText = "Output filename. Defaults to {SolutionUniqueName}_{version}_managed/unmanaged.zip.")]
		public string? FileName { get; set; }
	}
}
