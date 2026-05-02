using System.Text.Json.Serialization;

namespace Greg.Xrm.Command.Services.Config
{
	public sealed class PacxConfig
	{
		[JsonPropertyName("defaultTenantId")]
		public string? DefaultTenantId { get; set; }

		[JsonPropertyName("defaultEnvironmentUrl")]
		public string? DefaultEnvironmentUrl { get; set; }

		[JsonPropertyName("outputFormat")]
		public string? OutputFormat { get; set; } = "table";

		[JsonPropertyName("connections")]
		public Dictionary<string, string> Connections { get; set; } = [];

		[JsonPropertyName("options")]
		public Dictionary<string, object> Options { get; set; } = [];
	}
}
