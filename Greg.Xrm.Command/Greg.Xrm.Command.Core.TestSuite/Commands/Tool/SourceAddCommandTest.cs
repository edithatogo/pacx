namespace Greg.Xrm.Command.Commands.Tool
{
	[TestClass]
	public class SourceAddCommandTest
	{
		[TestMethod]
		public void ParseWithRequiredOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<SourceAddCommand>("tool", "source", "add", "--name", "myfeed", "--url", "https://example.com");
			Assert.AreEqual("myfeed", command.Name);
			Assert.AreEqual("https://example.com", command.Url);
			Assert.AreEqual("nuget", command.Type);
			Assert.IsNull(command.PersonalAccessToken);
		}

		[TestMethod]
		public void ParseWithAllOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<SourceAddCommand>(
				"tool", "source", "add",
				"--name", "private-feed",
				"--url", "https://private.nuget.org/v3/index.json",
				"--type", "nuget",
				"--pat", "my-token");

			Assert.AreEqual("private-feed", command.Name);
			Assert.AreEqual("https://private.nuget.org/v3/index.json", command.Url);
			Assert.AreEqual("nuget", command.Type);
			Assert.AreEqual("my-token", command.PersonalAccessToken);
		}
	}
}
