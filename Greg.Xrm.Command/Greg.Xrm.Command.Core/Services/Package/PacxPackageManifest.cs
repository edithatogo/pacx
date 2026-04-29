namespace Greg.Xrm.Command.Services.Package
{
	public sealed class PacxPackageManifest
	{
		public const string FileName = "pacx.package.json";

		public int SchemaVersion { get; set; } = 1;

		public string PackageId { get; set; } = string.Empty;

		public string Version { get; set; } = string.Empty;

		public string Name { get; set; } = string.Empty;

		public string? Description { get; set; }

		public string Kind { get; set; } = PacxPackageKinds.Bundle;

		public List<PacxPackageArtifact> Artifacts { get; set; } = [];

		public List<PacxPackageDeploymentStep> Deployment { get; set; } = [];

		public Dictionary<string, string>? Metadata { get; set; }
	}
}
