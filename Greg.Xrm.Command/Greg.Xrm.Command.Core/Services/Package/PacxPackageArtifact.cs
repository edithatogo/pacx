namespace Greg.Xrm.Command.Services.Package
{
	public sealed class PacxPackageArtifact
	{
		public string Path { get; set; } = string.Empty;

		public string Role { get; set; } = string.Empty;

		public bool Required { get; set; } = true;

		public string? ContentType { get; set; }

		public string? Sha256 { get; set; }
	}
}
