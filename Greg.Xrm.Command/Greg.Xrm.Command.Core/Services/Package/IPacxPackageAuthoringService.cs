namespace Greg.Xrm.Command.Services.Package
{
	public interface IPacxPackageAuthoringService
	{
		Task<string> AddSolutionAsync(Commands.Package.PackageAddSolutionCommand command, CancellationToken cancellationToken);

		Task<string> AddDataAsync(Commands.Package.PackageAddDataCommand command, CancellationToken cancellationToken);

		Task<string> RemoveSolutionAsync(Commands.Package.PackageRemoveSolutionCommand command, CancellationToken cancellationToken);

		Task<string> RemoveDataAsync(Commands.Package.PackageRemoveDataCommand command, CancellationToken cancellationToken);

		Task<PacxPackageSyncResult> SyncAsync(Commands.Package.PackageSyncCommand command, CancellationToken cancellationToken);

		Task<PacxPackageFixResult> FixAsync(Commands.Package.PackageFixCommand command, CancellationToken cancellationToken);
	}
}
