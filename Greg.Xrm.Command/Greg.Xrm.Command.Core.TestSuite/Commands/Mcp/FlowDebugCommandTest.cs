namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowDebugCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<FlowDebugCommand>("mcp", "flow", "debug");

			Assert.AreEqual("conductor/flow-mcp-catalog/flows.json", command.CatalogPath);
		}
	}
}
