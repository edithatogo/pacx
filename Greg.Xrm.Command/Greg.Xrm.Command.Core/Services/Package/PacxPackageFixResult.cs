namespace Greg.Xrm.Command.Services.Package
{
	public sealed record PacxPackageFixResult(int AddedArtifacts, int AddedSteps, int DedupedArtifacts, int DedupedSteps, int PrunedArtifacts, int PrunedSteps);
}
