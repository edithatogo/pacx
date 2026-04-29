using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.ReleasePlan
{
	[TestClass]
	public class ReleasePlanBrowseCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Utility.TestParseCommand<ReleasePlanBrowseCommand>(
				"release-plan", "browse");

			Assert.AreEqual("conductor/release-plan-catalog/families.json", command.CatalogPath);
			Assert.IsNull(command.Category);
			Assert.IsNull(command.Query);
		}

		[TestMethod]
		public void ParseWithAllOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<ReleasePlanBrowseCommand>(
				"release-plan", "browse",
				"-c", "custom/catalog.json",
				"--category", "Power BI",
				"-q", "reporting");

			Assert.AreEqual("custom/catalog.json", command.CatalogPath);
			Assert.AreEqual("Power BI", command.Category);
			Assert.AreEqual("reporting", command.Query);
		}

		[TestMethod]
		public void ParseWithCategoryOnlyShouldWork()
		{
			var command = Utility.TestParseCommand<ReleasePlanBrowseCommand>(
				"release-plan", "browse",
				"--category", "AI");

			Assert.AreEqual("AI", command.Category);
			Assert.IsNull(command.Query);
		}
	}
}
