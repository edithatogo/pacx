using System.ComponentModel.DataAnnotations;
using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Package
{
	[Command("package", "source", "browse", HelpText = "Browse package and source ecosystems.")]
	public class PackageSourceBrowseCommand
	{
		[Option("catalog", "c", Order = 1, DefaultValue = "conductor/source-catalog/sources.json", HelpText = "Path to the package source catalog JSON file.")]
		public string CatalogPath { get; set; } = "conductor/source-catalog/sources.json";

		[Option("category", Order = 2, HelpText = "Filter by category.")]
		public string? Category { get; set; }

		[Option("query", "q", Order = 3, HelpText = "Filter by name, provider, kind, summary, or package name.")]
		public string? Query { get; set; }
	}
}
