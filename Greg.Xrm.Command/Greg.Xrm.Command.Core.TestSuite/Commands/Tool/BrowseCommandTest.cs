namespace Greg.Xrm.Command.Commands.Tool
{
	[TestClass]
	public class BrowseCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Utility.TestParseCommand<BrowseCommand>("tool", "browse");
			Assert.AreEqual("conductor/tool-catalog/tools.json", command.CatalogPath);
			Assert.IsNull(command.Category);
			Assert.IsNull(command.Query);
		}

		[TestMethod]
		public void ParseWithFiltersShouldWork()
		{
			var command = Utility.TestParseCommand<BrowseCommand>(
				"tool", "browse",
				"--catalog", "catalog.json",
				"--category", "mcp",
				"--query", "flow");

			Assert.AreEqual("catalog.json", command.CatalogPath);
			Assert.AreEqual("mcp", command.Category);
			Assert.AreEqual("flow", command.Query);
		}
	}
}
