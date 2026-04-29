namespace Greg.Xrm.Command.Parsing
{
	public static class GlobalCommandOptions
	{
		private static readonly HashSet<string> GlobalOptionNames = new(StringComparer.OrdinalIgnoreCase)
		{
			"--verbose",
			"--quiet",
			"--format",
			"--no-color",
			"--correlation-id",
			"--profile",
			"--watch",
		};

		private static readonly HashSet<string> ValueOptionNames = new(StringComparer.OrdinalIgnoreCase)
		{
			"--format",
			"--correlation-id",
			"--profile",
		};

		public static IReadOnlySet<string> Names => GlobalOptionNames;

		public static bool IsGlobalOption(string optionName) => GlobalOptionNames.Contains(optionName);

		public static bool RequiresValue(string optionName) => ValueOptionNames.Contains(optionName);

		public static IReadOnlyDictionary<string, string> Extract(IDictionary<string, string> options)
		{
			var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			foreach (var optionName in GlobalOptionNames)
			{
				if (!options.TryGetValue(optionName, out var value)) continue;
				result[optionName] = value;
				options.Remove(optionName);
			}

			return result;
		}
	}
}
