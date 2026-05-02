namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowInspectCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Utility.TestParseCommand<FlowInspectCommand>("mcp", "flow", "inspect");
			Assert.IsNotNull(command);
		}
	}
}
