namespace Greg.Xrm.Command.Services.Package
{
	public sealed record PacxPackageReleaseResult(
		string ReleaseDirectory,
		string PackagePath,
		string ReleaseManifestPath,
		string ProvenancePath,
		string SbomPath,
		string ReleaseNotesPath,
		string ChecksumsPath);
}
