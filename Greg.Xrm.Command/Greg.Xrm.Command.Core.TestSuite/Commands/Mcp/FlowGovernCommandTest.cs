namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowGovernCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<FlowGovernCommand>("mcp", "flow", "govern");

			Assert.AreEqual("conductor/flow-mcp-catalog/flows.json", command.CatalogPath);
		}
	}
}
