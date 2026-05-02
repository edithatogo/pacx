namespace Greg.Xrm.Command.Commands.Tool
{
	[TestClass]
	public class SourceListCommandTest
	{
		[TestMethod]
		public void ParseWithoutOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<SourceListCommand>("tool", "source", "list");
			Assert.IsNotNull(command);
		}
	}
}
