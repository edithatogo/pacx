namespace Greg.Xrm.Command.Commands.Tool
{
	[TestClass]
	public class ListCommandTest
	{
		[TestMethod]
		public void ParseWithoutOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<ListCommand>("tool", "list");
			Assert.IsNotNull(command);
		}
	}
}
