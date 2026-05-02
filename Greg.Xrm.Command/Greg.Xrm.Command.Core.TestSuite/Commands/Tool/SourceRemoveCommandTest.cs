namespace Greg.Xrm.Command.Commands.Tool
{
	[TestClass]
	public class SourceRemoveCommandTest
	{
		[TestMethod]
		public void ParseWithNameShouldWork()
		{
			var command = Utility.TestParseCommand<SourceRemoveCommand>("tool", "source", "remove", "--name", "myfeed");
			Assert.AreEqual("myfeed", command.Name);
		}
	}
}
