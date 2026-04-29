namespace Greg.Xrm.Command.Commands.Package
{
	[TestClass]
	public class PackageReleaseVerifyCommandTest
	{
		[TestMethod]
		public void ParseWithPathShouldWork()
		{
			var command = Utility.TestParseCommand<PackageReleaseVerifyCommand>(
				"package", "release", "verify",
				"--path", "out/release");

			Assert.AreEqual("out/release", command.Path);
		}
	}
}
