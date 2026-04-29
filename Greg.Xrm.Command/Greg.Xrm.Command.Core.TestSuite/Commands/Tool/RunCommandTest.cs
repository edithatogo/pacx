namespace Greg.Xrm.Command.Commands.Tool
{
	[TestClass]
	public class RunCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Utility.TestParseCommand<RunCommand>(
				"tool", "run",
				"--name", "XrmToolBox");

			Assert.AreEqual("conductor/tool-catalog/tools.json", command.CatalogPath);
			Assert.AreEqual("XrmToolBox", command.Name);
			Assert.IsFalse(command.OpenHomePage);
		}
	}
}
