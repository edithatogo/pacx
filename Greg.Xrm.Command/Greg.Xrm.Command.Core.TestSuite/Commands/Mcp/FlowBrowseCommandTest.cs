namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowBrowseCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<FlowBrowseCommand>("mcp", "flow", "browse");

			Assert.AreEqual("conductor/flow-mcp-catalog/flows.json", command.CatalogPath);
		}
	}
}
