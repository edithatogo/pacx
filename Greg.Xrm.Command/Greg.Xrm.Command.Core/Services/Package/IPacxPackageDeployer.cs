namespace Greg.Xrm.Command.Services.Package
{
	public interface IPacxPackageDeployer
	{
		Task DeployAsync(IPacxPackageSource source, bool dryRun, CancellationToken cancellationToken);
	}
}
