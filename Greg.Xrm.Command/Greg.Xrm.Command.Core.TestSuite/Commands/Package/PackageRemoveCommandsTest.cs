namespace Greg.Xrm.Command.Commands.Package
{
	[TestClass]
	public class PackageRemoveCommandsTest
	{
		[TestMethod]
		public void PackageRemoveSolutionParseShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<PackageRemoveSolutionCommand>(
				"package", "remove", "solution",
				"-p", "./package",
				"-a", "payload/solution.zip",
				"--force");

			Assert.AreEqual("./package", command.Path);
			Assert.AreEqual("payload/solution.zip", command.ArtifactPath);
			Assert.IsTrue(command.Force);
		}

		[TestMethod]
		public void PackageRemoveDataParseShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<PackageRemoveDataCommand>(
				"package", "remove", "data",
				"-p", "./package",
				"-a", "data/accounts.json");

			Assert.AreEqual("./package", command.Path);
			Assert.AreEqual("data/accounts.json", command.ArtifactPath);
			Assert.IsFalse(command.Force);
		}
	}
}
