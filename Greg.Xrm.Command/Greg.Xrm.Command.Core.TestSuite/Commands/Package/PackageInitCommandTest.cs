namespace Greg.Xrm.Command.Commands.Package
{
	[TestClass]
	public class PackageInitCommandTest
	{
		[TestMethod]
		public void PackageInitParseShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<PackageInitCommand>(
				"package", "init",
				"-p", "./package",
				"--package-id", "contoso.sample",
				"--version", "1.2.3",
				"--name", "Contoso Sample",
				"--description", "Starter package",
				"--kind", "solution",
				"--force");

			Assert.AreEqual("./package", command.Path);
			Assert.AreEqual("contoso.sample", command.PackageId);
			Assert.AreEqual("1.2.3", command.Version);
			Assert.AreEqual("Contoso Sample", command.Name);
			Assert.AreEqual("Starter package", command.Description);
			Assert.AreEqual("solution", command.Kind);
			Assert.IsTrue(command.Force);
		}
	}
}
