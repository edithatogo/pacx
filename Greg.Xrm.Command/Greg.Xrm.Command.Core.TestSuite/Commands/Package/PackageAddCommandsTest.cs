namespace Greg.Xrm.Command.Commands.Package
{
	[TestClass]
	public class PackageAddCommandsTest
	{
		[TestMethod]
		public void PackageAddSolutionParseShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<PackageAddSolutionCommand>(
				"package", "add", "solution",
				"-p", "./package",
				"-s", "./solution.zip",
				"--artifact", "payload/solution.zip",
				"--force");

			Assert.AreEqual("./package", command.Path);
			Assert.AreEqual("./solution.zip", command.SourcePath);
			Assert.AreEqual("payload/solution.zip", command.ArtifactPath);
			Assert.IsTrue(command.Force);
			Assert.IsTrue(command.OverwriteUnmanagedCustomizations);
			Assert.IsTrue(command.PublishWorkflows);
		}

		[TestMethod]
		public void PackageAddDataParseShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<PackageAddDataCommand>(
				"package", "add", "data",
				"-p", "./package",
				"-s", "./accounts.json",
				"-t", "account",
				"--artifact", "data/accounts.json",
				"--mode", "create");

			Assert.AreEqual("./package", command.Path);
			Assert.AreEqual("./accounts.json", command.SourcePath);
			Assert.AreEqual("account", command.Table);
			Assert.AreEqual("data/accounts.json", command.ArtifactPath);
			Assert.AreEqual("create", command.Mode);
			Assert.IsFalse(command.Force);
		}
	}
}
