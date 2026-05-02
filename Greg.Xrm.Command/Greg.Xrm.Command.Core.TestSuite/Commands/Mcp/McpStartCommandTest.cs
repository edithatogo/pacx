namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class McpStartCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Utility.TestParseCommand<McpStartCommand>("mcp", "start");
			Assert.IsNotNull(command);
		}
	}
}
