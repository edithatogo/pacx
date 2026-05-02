namespace Greg.Xrm.Command.Commands.Update
{
	[TestClass]
	public class SelfUpdateCommandTest
	{
		[TestMethod]
		public void ParseSelfUpdateWithDefaultsShouldWork()
		{
			var command = Utility.TestParseCommand<SelfUpdateCommand>("self-update");
			Assert.IsFalse(command.CheckOnly);
			Assert.IsNull(command.Version);
			Assert.IsFalse(command.IncludePrerelease);
		}

		[TestMethod]
		public void ParseWithAliasShouldWork()
		{
			var command = Utility.TestParseCommand<SelfUpdateCommand>("update");
			Assert.IsFalse(command.CheckOnly);
		}

		[TestMethod]
		public void ParseWithCheckOnlyShouldWork()
		{
			var command = Utility.TestParseCommand<SelfUpdateCommand>("self-update", "--check");
			Assert.IsTrue(command.CheckOnly);
		}

		[TestMethod]
		public void ParseWithCheckOnlyShortNameShouldWork()
		{
			var command = Utility.TestParseCommand<SelfUpdateCommand>("self-update", "-c");
			Assert.IsTrue(command.CheckOnly);
		}

		[TestMethod]
		public void ParseWithVersionShouldWork()
		{
			var command = Utility.TestParseCommand<SelfUpdateCommand>("self-update", "--version", "1.2.3");
			Assert.AreEqual("1.2.3", command.Version);
		}

		[TestMethod]
		public void ParseWithPreReleaseShouldWork()
		{
			var command = Utility.TestParseCommand<SelfUpdateCommand>("self-update", "--pre-release");
			Assert.IsTrue(command.IncludePrerelease);
		}
	}
}
