namespace Greg.Xrm.Command.Commands.Tool
{
	[TestClass]
	public class UninstallCommandTest
	{
		[TestMethod]
		public void ParseWithRequiredNameShouldWork()
		{
			var command = Utility.TestParseCommand<UninstallCommand>("tool", "uninstall", "--name", "MyPlugin");
			Assert.AreEqual("MyPlugin", command.Name);
		}
	}
}
