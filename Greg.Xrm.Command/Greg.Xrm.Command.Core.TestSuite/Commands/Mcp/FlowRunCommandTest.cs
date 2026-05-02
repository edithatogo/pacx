namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowRunCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Utility.TestParseCommand<FlowRunCommand>("mcp", "flow", "run");
			Assert.IsNotNull(command);
		}
	}
}
