namespace Greg.Xrm.Command.Commands.Package
{
	[TestClass]
	public class PackageCommandsTest
	{
		[TestMethod]
		public void PackageShowParseShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<PackageShowCommand>(
				"package", "show",
				"-p", "./sample.pacx");

			Assert.AreEqual("./sample.pacx", command.Path);
		}

		[TestMethod]
		public void PackageDeployParseShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<PackageDeployCommand>(
				"package", "deploy",
				"-p", "./sample.pacx",
				"--dry-run");

			Assert.AreEqual("./sample.pacx", command.Path);
			Assert.IsTrue(command.DryRun);
		}

		[TestMethod]
		public void PackageBuildParseShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<PackageBuildCommand>(
				"package", "build",
				"-p", "./package");

			Assert.AreEqual("./package", command.Path);
			Assert.IsNull(command.OutputPath);
		}

		[TestMethod]
		public void PackageListParseShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<PackageListCommand>(
				"package", "list",
				"-p", "./package");

			Assert.AreEqual("./package", command.Path);
		}

		[TestMethod]
		public void PackageSyncParseShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<PackageSyncCommand>(
				"package", "sync",
				"-p", "./package",
				"--prune-missing");

			Assert.AreEqual("./package", command.Path);
			Assert.IsTrue(command.PruneMissing);
		}

		[TestMethod]
		public void PackageFixParseShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<PackageFixCommand>(
				"package", "fix",
				"-p", "./package");

			Assert.AreEqual("./package", command.Path);
			Assert.IsTrue(command.PruneMissing);
		}

		[TestMethod]
		public void PackagePublishParseShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<PackagePublishCommand>(
				"package", "publish",
				"-p", "./package",
				"-d", "./releases",
				"-v", "2.0.0");

			Assert.AreEqual("./package", command.Path);
			Assert.AreEqual("./releases", command.DestinationPath);
			Assert.AreEqual("2.0.0", command.Version);
			Assert.IsFalse(command.Force);
		}

		[TestMethod]
		public void PackageReleaseParseShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<PackageReleaseCommand>(
				"package", "release",
				"-p", "./package",
				"-d", "./release",
				"-v", "2.0.0");

			Assert.AreEqual("./package", command.Path);
			Assert.AreEqual("./release", command.DestinationPath);
			Assert.AreEqual("2.0.0", command.Version);
			Assert.IsFalse(command.Force);
		}

		[TestMethod]
		public void PackageInitParseShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<PackageInitCommand>(
				"package", "init",
				"-p", "./package",
				"--kind", "solution",
				"--version", "2.0.0");

			Assert.AreEqual("./package", command.Path);
			Assert.AreEqual("solution", command.Kind);
			Assert.AreEqual("2.0.0", command.Version);
			Assert.IsFalse(command.Force);
		}

		[TestMethod]
		public void PackageInitParseDataKindShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<PackageInitCommand>(
				"package", "init",
				"-p", "./package",
				"--kind", "data");

			Assert.AreEqual("./package", command.Path);
			Assert.AreEqual("data", command.Kind);
			Assert.AreEqual("1.0.0", command.Version);
			Assert.IsFalse(command.Force);
		}

		[TestMethod]
		public void PackageInitParseBundleKindShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<PackageInitCommand>(
				"package", "init",
				"-p", "./package",
				"--kind", "bundle",
				"--force");

			Assert.AreEqual("./package", command.Path);
			Assert.AreEqual("bundle", command.Kind);
			Assert.AreEqual("1.0.0", command.Version);
			Assert.IsTrue(command.Force);
		}
	}
}
