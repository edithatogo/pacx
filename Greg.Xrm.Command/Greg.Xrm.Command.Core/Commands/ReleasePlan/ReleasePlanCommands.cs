using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.ReleasePlan
{
	[Command("release-plan", "list", HelpText = "List available release plans / roadmap items. Filters by product, status, or category.")]
	public class ReleasePlanListCommand
	{
		[Option("product", "p", Order = 1, HelpText = "Filter by product name (e.g., Teams, SharePoint).")]
		public string? Product { get; set; }

		[Option("status", "s", Order = 2, HelpText = "Filter by status (e.g., Launched, RollingOut, InDevelopment).")]
		public string? Status { get; set; }

		[Option("category", "c", Order = 3, HelpText = "Filter by category (e.g., New, Updated, Deprecated).")]
		public string? Category { get; set; }

		[Option("max", "m", Order = 4, HelpText = "Maximum number of results to return (default: 50).")]
		public int MaxResults { get; set; } = 50;
	}

	[Command("release-plan", "get", HelpText = "Get details of a specific release plan item by ID.")]
	public class ReleasePlanGetCommand
	{
		[Option("id", "i", Order = 1, Required = true, HelpText = "Release plan item ID.")]
		public string Id { get; set; } = string.Empty;
	}

	[Command("release-plan", "search", HelpText = "Search release plans for a keyword.")]
	public class ReleasePlanSearchCommand
	{
		[Option("query", "q", Order = 1, Required = true, HelpText = "Search keyword or phrase.")]
		public string Query { get; set; } = string.Empty;

		[Option("product", "p", Order = 2, HelpText = "Filter by product name.")]
		public string? Product { get; set; }

		[Option("max", "m", Order = 3, HelpText = "Maximum results (default: 20).")]
		public int MaxResults { get; set; } = 20;
	}

	[Command("release-plan", "refresh", HelpText = "Force refresh the local release plan cache from the Roadmap API.")]
	public class ReleasePlanRefreshCommand
	{
	}

	[Command("release-plan", "products", HelpText = "List all product names found in the cached release plan data.")]
	public class ReleasePlanProductsCommand
	{
	}
}
