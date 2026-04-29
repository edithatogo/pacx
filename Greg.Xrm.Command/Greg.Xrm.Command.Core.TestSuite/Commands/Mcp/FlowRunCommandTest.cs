namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowRunCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<FlowRunCommand>("mcp", "flow", "run", "--name", "Flow Studio Debug");

			Assert.AreEqual("conductor/flow-mcp-catalog/flows.json", command.CatalogPath);
			Assert.AreEqual("Flow Studio Debug", command.Name);
		}
	}
}
