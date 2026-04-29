namespace Greg.Xrm.Command.Services.Package
{
	public interface IPacxPackageInitializer
	{
		Task<string> InitializeAsync(Commands.Package.PackageInitCommand command, CancellationToken cancellationToken);
	}
}
