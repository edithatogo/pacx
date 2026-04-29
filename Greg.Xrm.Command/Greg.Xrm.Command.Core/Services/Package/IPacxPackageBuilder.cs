namespace Greg.Xrm.Command.Services.Package
{
	public interface IPacxPackageBuilder
	{
		Task<string> BuildAsync(string sourcePath, string? outputPath, CancellationToken cancellationToken);
	}
}
