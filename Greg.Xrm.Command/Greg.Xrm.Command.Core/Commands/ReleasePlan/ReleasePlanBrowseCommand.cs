using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.ReleasePlan
{
	[Command("release-plan", "browse", HelpText = "Browse Microsoft release-plan families for Power Platform, Dynamics 365, and related products.")]
	public class ReleasePlanBrowseCommand
	{
		[Option("catalog", "c", Order = 1, DefaultValue = "conductor/release-plan-catalog/families.json", HelpText = "Path to the release-plan catalog JSON file.")]
		public string CatalogPath { get; set; } = "conductor/release-plan-catalog/families.json";

		[Option("category", Order = 2, HelpText = "Filter by category (e.g. 'Power Platform', 'Dynamics 365', 'Power BI', 'AI').")]
		public string? Category { get; set; }

		[Option("query", "q", Order = 3, HelpText = "Filter by name, category, or summary.")]
		public string? Query { get; set; }
	}
}
