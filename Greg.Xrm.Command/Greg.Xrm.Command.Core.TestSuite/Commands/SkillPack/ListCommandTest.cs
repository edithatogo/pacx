namespace Greg.Xrm.Command.Commands.SkillPack
{
	[TestClass]
	public class ListCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Utility.TestParseCommand<ListCommand>("skill", "pack", "list");
			Assert.AreEqual("conductor/skill-pack-catalog/packs.json", command.CatalogPath);
			Assert.IsNull(command.Query);
			Assert.IsNull(command.Tag);
		}

		[TestMethod]
		public void ParseWithFiltersShouldWork()
		{
			var command = Utility.TestParseCommand<ListCommand>(
				"skill", "pack", "list",
				"--catalog", "custom.json",
				"--query", "devops",
				"--tag", "governance");

			Assert.AreEqual("custom.json", command.CatalogPath);
			Assert.AreEqual("devops", command.Query);
			Assert.AreEqual("governance", command.Tag);
		}
	}
}
