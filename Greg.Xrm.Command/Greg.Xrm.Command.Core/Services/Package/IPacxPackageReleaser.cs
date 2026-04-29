namespace Greg.Xrm.Command.Services.Package
{
	public interface IPacxPackageReleaser
	{
		Task<PacxPackageReleaseResult> ReleaseAsync(Commands.Package.PackageReleaseCommand command, CancellationToken cancellationToken);
	}
}
