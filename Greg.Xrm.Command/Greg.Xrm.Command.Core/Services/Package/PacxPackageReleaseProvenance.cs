namespace Greg.Xrm.Command.Services.Package
{
	public sealed class PacxPackageReleaseProvenance
	{
		public string Tool { get; set; } = "PACX";

		public string PackageId { get; set; } = string.Empty;

		public string Version { get; set; } = string.Empty;

		public string Repository { get; set; } = string.Empty;

		public string CommitSha { get; set; } = string.Empty;

		public string Workflow { get; set; } = string.Empty;

		public string RunId { get; set; } = string.Empty;

		public string RunAttempt { get; set; } = string.Empty;

		public string ArchivePath { get; set; } = string.Empty;

		public string ArchiveSha256 { get; set; } = string.Empty;

		public string ManifestPath { get; set; } = string.Empty;

		public string ManifestSha256 { get; set; } = string.Empty;

		public DateTimeOffset GeneratedAtUtc { get; set; }
	}
}
