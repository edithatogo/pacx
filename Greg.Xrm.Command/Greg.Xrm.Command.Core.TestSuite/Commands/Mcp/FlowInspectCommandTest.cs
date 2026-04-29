namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowInspectCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<FlowInspectCommand>("mcp", "flow", "inspect", "--name", "Flow Studio Debug");

			Assert.AreEqual("conductor/flow-mcp-catalog/flows.json", command.CatalogPath);
			Assert.AreEqual("Flow Studio Debug", command.Name);
		}
	}
}
