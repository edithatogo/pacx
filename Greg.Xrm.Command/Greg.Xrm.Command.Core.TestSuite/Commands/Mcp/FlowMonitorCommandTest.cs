namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowMonitorCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Utility.TestParseCommand<FlowMonitorCommand>("mcp", "flow", "monitor");
			Assert.IsNotNull(command);
		}
	}
}
