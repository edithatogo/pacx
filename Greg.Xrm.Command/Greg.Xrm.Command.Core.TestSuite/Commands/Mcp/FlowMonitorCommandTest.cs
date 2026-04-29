namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowMonitorCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<FlowMonitorCommand>("mcp", "flow", "monitor");

			Assert.AreEqual("conductor/flow-mcp-catalog/flows.json", command.CatalogPath);
		}
	}
}
