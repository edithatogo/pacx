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
			var guidesPath = Path.Combine(docsRoot, "guides", "dataverse-skill-pack.md");
			var recipesPath = Path.Combine(docsRoot, "recipes", "dataverse-skill-pack.md");
			var tocPath = Path.Combine(docsRoot, "toc.yml");
			var recipesTocPath = Path.Combine(docsRoot, "recipes", "toc.yml");
			var catalogPath = Path.Combine(conductorRoot, "skill-pack-catalog", "skill-packs.json");

			Assert.IsTrue(File.Exists(guidesPath), guidesPath);
			Assert.IsTrue(File.Exists(recipesPath), recipesPath);
			Assert.IsTrue(File.Exists(catalogPath), catalogPath);

			var guide = File.ReadAllText(guidesPath);
			var recipe = File.ReadAllText(recipesPath);
			var toc = File.ReadAllText(tocPath);
			var recipesToc = File.ReadAllText(recipesTocPath);
			var catalog = File.ReadAllText(catalogPath);

			StringAssert.Contains(guide, "# Dataverse Skill Pack");
			StringAssert.Contains(guide, "pacx validate all");
			StringAssert.Contains(guide, "skill-pack-catalog/skill-packs.json");
			StringAssert.Contains(recipe, "# Dataverse Skill Pack Workflow");
			StringAssert.Contains(recipe, "pacx connector validate --file connector.json --strict");
			StringAssert.Contains(toc, "Dataverse Skill Pack");
			StringAssert.Contains(recipesToc, "Dataverse Skill Pack");
			StringAssert.Contains(catalog, "\"id\": \"dataverse-skill-pack\"");
		}
	}
}
