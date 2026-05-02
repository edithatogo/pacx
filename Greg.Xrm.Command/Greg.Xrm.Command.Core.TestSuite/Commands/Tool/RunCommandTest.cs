namespace Greg.Xrm.Command.Commands.Tool
{
	[TestClass]
	public class RunCommandTest
	{
		[TestMethod]
		public void ParseWithRequiredNameShouldWork()
		{
			var command = Utility.TestParseCommand<RunCommand>("tool", "run", "--name", "my-tool");
			Assert.AreEqual("my-tool", command.Name);
			Assert.IsFalse(command.OpenHomePage);
		}

		[TestMethod]
		public void ParseWithOpenFlagShouldWork()
		{
			var command = Utility.TestParseCommand<RunCommand>("tool", "run", "--name", "my-tool", "--open");
			Assert.AreEqual("my-tool", command.Name);
			Assert.IsTrue(command.OpenHomePage);
		}
	}
}
