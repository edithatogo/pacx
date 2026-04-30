namespace Greg.Xrm.Command.Services.ReleasePlan
{
	/// <summary>
	/// Result of analyzing release plan impact on a Dataverse environment.
	/// </summary>
	public sealed class ReleasePlanImpactReport
	{
		/// <summary>
		/// The impacted items identified during analysis.
		/// </summary>
		public List<ImpactedItem> Items { get; set; } = [];

		/// <summary>
		/// Timestamp when this report was generated.
		/// </summary>
		public DateTime GeneratedAt { get; set; }

		/// <summary>
		/// The environment name or URL that was analyzed.
		/// </summary>
		public string? EnvironmentName { get; set; }
	}

	/// <summary>
	/// A single impacted release plan item with its associated Dataverse component types.
	/// </summary>
	public sealed class ImpactedItem
	{
		/// <summary>
		/// The release plan item that may cause impact.
		/// </summary>
		public ReleasePlanItem? ReleaseItem { get; set; }

		/// <summary>
		/// Category of impact: "Breaking", "New", "Updated", "Deprecated".
		/// </summary>
		public string ImpactCategory { get; set; } = string.Empty;

		/// <summary>
		/// Relevance level: "High", "Medium", "Low".
		/// </summary>
		public string Relevance { get; set; } = string.Empty;

		/// <summary>
		/// Explanation of why this item is relevant.
		/// </summary>
		public string? Rationale { get; set; }

		/// <summary>
		/// Dataverse solution component types that are affected (e.g., "Canvas App", "Flow", "Model-driven App").
		/// </summary>
		public List<string> AffectedComponentTypes { get; set; } = [];
	}
}
