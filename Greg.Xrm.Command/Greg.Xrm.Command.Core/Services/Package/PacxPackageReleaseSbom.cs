namespace Greg.Xrm.Command.Services.Package
{
	public sealed class PacxPackageReleaseSbom
	{
		public string Tool { get; set; } = "PACX";

		public string PackageId { get; set; } = string.Empty;

		public string Version { get; set; } = string.Empty;

		public string PackageKind { get; set; } = string.Empty;

		public string PackagePath { get; set; } = string.Empty;

		public DateTimeOffset GeneratedAtUtc { get; set; }

		public List<PacxPackageSbomComponent> Components { get; set; } = [];
	}

	public sealed class PacxPackageSbomComponent
	{
		public string Type { get; set; } = string.Empty;

		public string Path { get; set; } = string.Empty;

		public string? Hash { get; set; }
	}
}
