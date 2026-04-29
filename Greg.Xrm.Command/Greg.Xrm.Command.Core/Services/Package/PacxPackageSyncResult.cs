namespace Greg.Xrm.Command.Services.Package
{
	public sealed record PacxPackageSyncResult(int AddedArtifacts, int AddedSteps, int PrunedArtifacts, int PrunedSteps);
}
