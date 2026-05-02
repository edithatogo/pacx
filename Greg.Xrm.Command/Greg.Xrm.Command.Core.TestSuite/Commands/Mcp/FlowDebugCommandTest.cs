namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowDebugCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Utility.TestParseCommand<FlowDebugCommand>("mcp", "flow", "debug");
			Assert.IsNotNull(command);
		}
	}
}
