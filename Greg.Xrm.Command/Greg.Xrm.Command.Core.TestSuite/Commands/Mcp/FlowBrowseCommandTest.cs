namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowBrowseCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Utility.TestParseCommand<FlowBrowseCommand>("mcp", "flow", "browse");
			Assert.AreEqual("conductor/flow-mcp-catalog/flows.json", command.CatalogPath);
			Assert.IsNull(command.Category);
			Assert.IsNull(command.Query);
		}

		[TestMethod]
		public void ParseWithFiltersShouldWork()
		{
			var command = Utility.TestParseCommand<FlowBrowseCommand>(
				"mcp", "flow", "browse",
				"--catalog", "custom.json",
				"--category", "browse",
				"--query", "flow");

			Assert.AreEqual("custom.json", command.CatalogPath);
			Assert.AreEqual("browse", command.Category);
			Assert.AreEqual("flow", command.Query);
		}
	}
}
