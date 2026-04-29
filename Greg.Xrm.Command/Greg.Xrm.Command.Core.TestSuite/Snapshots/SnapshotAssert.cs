using System.Runtime.CompilerServices;

namespace Greg.Xrm.Command.Snapshots
{
	internal static class SnapshotAssert
	{
		public static void Matches(string actual, [CallerFilePath] string sourceFilePath = "", [CallerMemberName] string snapshotName = "")
		{
			var snapshotPath = Path.Combine(Path.GetDirectoryName(sourceFilePath)!, $"{snapshotName}.verified.txt");
			var normalizedActual = Normalize(actual);

			if (!File.Exists(snapshotPath))
			{
				Assert.Fail($"Missing approved snapshot: {snapshotPath}");
			}

			var expected = Normalize(File.ReadAllText(snapshotPath));
			Assert.AreEqual(expected, normalizedActual);
		}

		private static string Normalize(string value)
		{
			return value.Replace("\r\n", "\n", StringComparison.Ordinal).TrimEnd() + "\n";
		}
	}
}
