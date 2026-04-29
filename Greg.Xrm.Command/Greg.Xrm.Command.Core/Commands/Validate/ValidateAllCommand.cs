using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Validate
{
	[Command("validate", "all", HelpText = "Validate the generated command reference and catalog contracts against the live command registry.")]
	public class ValidateAllCommand
	{
		[Option("docs-index", "d", Order = 1, DefaultValue = "docs/reference/commands/generated/index.md", HelpText = "Path to the generated command reference index.")]
		public string DocsIndexPath { get; set; } = "docs/reference/commands/generated/index.md";

		[Option("catalog-root", "c", Order = 2, DefaultValue = ".", HelpText = "Repository root used to validate catalog JSON files.")]
		public string CatalogRootPath { get; set; } = ".";
	}
}
