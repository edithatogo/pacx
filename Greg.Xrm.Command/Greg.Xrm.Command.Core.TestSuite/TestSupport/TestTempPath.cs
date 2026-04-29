namespace Greg.Xrm.Command.TestSuite
{
	internal static class TestTempPath
	{
		private static readonly string Root = Path.Combine(Environment.CurrentDirectory, ".test-temp");

		public static string CreateDirectory(string prefix)
		{
			Directory.CreateDirectory(Root);

			var path = Path.Combine(Root, $"{prefix}_{Guid.NewGuid():N}");
			Directory.CreateDirectory(path);
			return path;
		}

		public static string CreateFilePath(string prefix, string extension)
		{
			Directory.CreateDirectory(Root);
			return Path.Combine(Root, $"{prefix}_{Guid.NewGuid():N}{extension}");
		}
	}
}
