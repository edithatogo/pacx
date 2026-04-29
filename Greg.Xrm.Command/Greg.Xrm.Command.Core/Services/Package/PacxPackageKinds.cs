namespace Greg.Xrm.Command.Services.Package
{
	public static class PacxPackageKinds
	{
		public const string Bundle = "bundle";
		public const string Solution = "solution";
		public const string Data = "data";

		public static readonly IReadOnlyCollection<string> All = new[]
		{
			Bundle,
			Solution,
			Data
		};

		public static string Describe(string kind)
		{
			return kind.Trim().ToLowerInvariant() switch
			{
				Bundle => "bundle (mixed or release package)",
				Solution => "solution (solution-only deployment package)",
				Data => "data (data-only deployment package)",
				_ => kind
			};
		}

		public static string DescribeContract(string kind)
		{
			return kind.Trim().ToLowerInvariant() switch
			{
				Bundle => "No additional kind-specific constraints.",
				Solution => "Requires at least one solution artifact and one solutionImport step.",
				Data => "Requires at least one data artifact and one dataImport step.",
				_ => "Unknown package kind."
			};
		}
	}
}
