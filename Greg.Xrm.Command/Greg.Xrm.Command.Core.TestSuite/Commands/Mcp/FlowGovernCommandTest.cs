namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowGovernCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Utility.TestParseCommand<FlowGovernCommand>("mcp", "flow", "govern");
			Assert.IsNotNull(command);
		}
	}
}
