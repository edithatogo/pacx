using Greg.Xrm.Command.Commands.Validate;

namespace Greg.Xrm.Command.Commands.Validate
{
	[TestClass]
	public class ValidateAllCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Utility.TestParseCommand<ValidateAllCommand>(
				"validate", "all");

			Assert.AreEqual("docs/reference/commands/generated/index.md", command.DocsIndexPath);
			Assert.AreEqual(".", command.CatalogRootPath);
		}

		[TestMethod]
		public void ParseWithAllOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<ValidateAllCommand>(
				"validate", "all",
				"--docs-index", "docs/reference/commands/generated/index.md",
				"--catalog-root", "repo");

			Assert.AreEqual("docs/reference/commands/generated/index.md", command.DocsIndexPath);
			Assert.AreEqual("repo", command.CatalogRootPath);
		}
	}
}
