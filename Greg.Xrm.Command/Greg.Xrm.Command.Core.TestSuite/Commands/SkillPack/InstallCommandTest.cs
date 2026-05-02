namespace Greg.Xrm.Command.Commands.SkillPack
{
	[TestClass]
	public class InstallCommandTest
	{
		[TestMethod]
		public void ParseWithRequiredIdShouldWork()
		{
			var command = Utility.TestParseCommand<InstallCommand>("skill", "pack", "install", "--id", "dataverse-devops");
			Assert.AreEqual("dataverse-devops", command.Id);
			Assert.IsFalse(command.DryRun);
		}

		[TestMethod]
		public void ParseWithDryRunShouldWork()
		{
			var command = Utility.TestParseCommand<InstallCommand>("skill", "pack", "install", "--id", "dataverse-devops", "--dry-run");
			Assert.AreEqual("dataverse-devops", command.Id);
			Assert.IsTrue(command.DryRun);
		}
	}
}
