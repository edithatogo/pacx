namespace Greg.Xrm.Command.Services.Package
{
	public interface IPacxPackageReader
	{
		IPacxPackageSource Open(string path);
	}
}
