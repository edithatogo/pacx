namespace Greg.Xrm.Command.Services.Package
{
	public interface IPacxPackagePublisher
	{
		Task<PacxPackagePublishResult> PublishAsync(Commands.Package.PackagePublishCommand command, CancellationToken cancellationToken);
	}
}
