using System.Text.Json.Serialization;

namespace Greg.Xrm.Command.Services.ReleasePlan
{
	/// <summary>
	/// Represents a single item from the Microsoft 365 Release Plan / Roadmap.
	/// </summary>
	public sealed class ReleasePlanItem
	{
		[JsonPropertyName("id")]
		public string Id { get; set; } = string.Empty;

		[JsonPropertyName("title")]
		public string Title { get; set; } = string.Empty;

		[JsonPropertyName("description")]
		public string Description { get; set; } = string.Empty;

		[JsonPropertyName("product")]
		public string Product { get; set; } = string.Empty;

		[JsonPropertyName("category")]
		public string Category { get; set; } = string.Empty;

		[JsonPropertyName("status")]
		public string Status { get; set; } = string.Empty;

		[JsonPropertyName("wave")]
		public string Wave { get; set; } = string.Empty;

		[JsonPropertyName("url")]
		public string Url { get; set; } = string.Empty;

		[JsonPropertyName("lastModifiedUtc")]
		public DateTime LastModifiedUtc { get; set; }

		[JsonPropertyName("tags")]
		public List<string> Tags { get; set; } = [];
	}

	/// <summary>
	/// Filter criteria for querying release plan items.
	/// </summary>
	public sealed class ReleasePlanFilter
	{
		public string? Product { get; set; }
		public string? Status { get; set; }
		public string? Category { get; set; }
		public string? Search { get; set; }
		public int? MaxResults { get; set; }
	}

	/// <summary>
	/// Cached snapshot of the release plan data.
	/// </summary>
	public sealed class ReleasePlanSnapshot
	{
		[JsonPropertyName("items")]
		public List<ReleasePlanItem> Items { get; set; } = [];

		[JsonPropertyName("fetchedAtUtc")]
		public DateTime FetchedAtUtc { get; set; }

		[JsonPropertyName("etag")]
		public string? Etag { get; set; }
	}
}
