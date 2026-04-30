namespace Greg.Xrm.Command.Services.ReleasePlan
{
	/// <summary>
	/// Maps release plan product names to Dataverse solution component types.
	/// Provides knowledge about which component types are affected by changes
	/// to specific product areas within the Microsoft Power Platform ecosystem.
	/// </summary>
	public sealed class ReleasePlanComponentMap
	{
		private static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> KnownMappings =
			new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase)
			{
				["Power Platform"] =
				[
					"Canvas App",
					"Model-driven App",
					"Flow",
					"Dataverse Table",
					"Dataverse Column",
					"Connection Reference",
					"Custom Connector",
					"Environment Variable",
					"Solution",
					"Environment",
				],
				["Power Apps"] =
				[
					"Canvas App",
					"Model-driven App",
					"Custom Connector",
					"Connection Reference",
					"Environment Variable",
				],
				["Power Automate"] =
				[
					"Flow",
					"Connection Reference",
					"Custom Connector",
					"Desktop Flow",
				],
				["Power BI"] =
				[
					"Semantic Model",
					"Report",
					"Dashboard",
					"Dataflow",
					"Datamart",
				],
				["Dynamics 365"] =
				[
					"Model-driven App",
					"Business Rule",
					"Workflow",
					"Plugin Assembly",
					"SDK Message Processing Step",
					"System Form",
				],
				["Dynamics 365 Sales"] =
				[
					"Model-driven App",
					"Business Rule",
					"Workflow",
					"System Form",
				],
				["Dynamics 365 Customer Service"] =
				[
					"Model-driven App",
					"Business Rule",
					"Workflow",
					"System Form",
				],
				["Dynamics 365 Finance"] =
				[
					"Model-driven App",
					"Business Rule",
					"Workflow",
				],
				["Dynamics 365 Supply Chain Management"] =
				[
					"Model-driven App",
					"Business Rule",
					"Workflow",
				],
				["Microsoft Copilot"] =
				[
					"Copilot Plugin",
					"AI Builder Model",
					"Custom Connector",
				],
				["Microsoft Teams"] =
				[
					"Canvas App",
					"Custom Connector",
					"Flow",
				],
				["Microsoft Viva"] =
				[
					"Canvas App",
					"Flow",
				],
				["SharePoint"] =
				[
					"Canvas App",
					"Flow",
					"Connection Reference",
				],
				["Microsoft 365"] =
				[
					"Canvas App",
					"Flow",
					"Connection Reference",
					"Custom Connector",
				],
			};

		/// <summary>
		/// Gets the full set of known product-to-component mappings.
		/// </summary>
		public IReadOnlyDictionary<string, IReadOnlyList<string>> Mappings => KnownMappings;

		/// <summary>
		/// Returns the list of Dataverse component types affected by changes to a given product.
		/// Matching is case-insensitive. Returns an empty list if the product is not recognized.
		/// </summary>
		public IReadOnlyList<string> GetComponentTypes(string productName)
		{
			if (string.IsNullOrWhiteSpace(productName))
			{
				return [];
			}

			if (KnownMappings.TryGetValue(productName, out var types))
			{
				return types;
			}

			return [];
		}
	}
}
