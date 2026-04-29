namespace Greg.Xrm.Command.Services.Package
{
	public sealed record PacxPackagePublishResult(string PackagePath, string ReleaseManifestPath, string ArchiveSha256);
}
