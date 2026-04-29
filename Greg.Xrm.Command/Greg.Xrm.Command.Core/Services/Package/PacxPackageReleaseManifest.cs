namespace Greg.Xrm.Command.Services.Package
{
	public sealed class PacxPackageReleaseManifest
	{
		public string PackageId { get; set; } = string.Empty;

		public string Version { get; set; } = string.Empty;

		public string Name { get; set; } = string.Empty;

		public string Kind { get; set; } = string.Empty;

		public string SourcePath { get; set; } = string.Empty;

		public string ArchivePath { get; set; } = string.Empty;

		public string ArchiveSha256 { get; set; } = string.Empty;

		public int ArtifactCount { get; set; }

		public int DeploymentCount { get; set; }

		public DateTimeOffset PublishedAtUtc { get; set; }
	}
}
