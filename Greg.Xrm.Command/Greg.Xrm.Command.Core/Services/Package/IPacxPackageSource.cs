namespace Greg.Xrm.Command.Services.Package
{
	public interface IPacxPackageSource : IDisposable
	{
		string SourcePath { get; }

		PacxPackageManifest Manifest { get; }

		IReadOnlyList<PacxPackageEntry> Entries { get; }

		Stream OpenRead(string relativePath);

		bool Exists(string relativePath);
	}
}
