namespace Greg.Xrm.Command.Docs
{
	[TestClass]
	public class DataverseSkillPackDocsTest
	{
		[TestMethod]
		public void DataverseSkillPackDocsShouldBeWiredIntoTheSite()
		{
			var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
			var docsRoot = Path.Combine(repoRoot, "docs");
			var conductorRoot = Path.Combine(repoRoot, "conductor");
			var guidesPath = Path.Combine(docsRoot, "guides", "skill-packs.md");
			var catalogPath = Path.Combine(conductorRoot, "skill-pack-catalog", "packs.json");

			Assert.IsTrue(File.Exists(guidesPath), guidesPath);
			Assert.IsTrue(File.Exists(catalogPath), catalogPath);

			var guide = File.ReadAllText(guidesPath);
			var catalog = File.ReadAllText(catalogPath);

			StringAssert.Contains(guide, "Skill Packs");
			StringAssert.Contains(guide, "pacx skill pack list");
			StringAssert.Contains(catalog, "\"id\": \"dataverse-devops\"");
		}
	}
}
