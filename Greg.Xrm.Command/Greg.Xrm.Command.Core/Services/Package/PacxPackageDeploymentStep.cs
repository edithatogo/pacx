using System.Text.Json;
using System.Text.Json.Serialization;

namespace Greg.Xrm.Command.Services.Package
{
	public sealed class PacxPackageDeploymentStep
	{
		public string Type { get; set; } = string.Empty;

		public string Artifact { get; set; } = string.Empty;

		public string? Table { get; set; }

		public string? Mode { get; set; }

		public bool? OverwriteUnmanagedCustomizations { get; set; }

		public bool? PublishWorkflows { get; set; }

		[JsonExtensionData]
		public Dictionary<string, JsonElement>? Extensions { get; set; }
	}
}
