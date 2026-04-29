using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.ReleasePlan
{
	[TestClass]
	public class ReleasePlanBrowseCommandExecutorTest
	{
		[TestMethod]
		public void BrowseShouldRenderMatchingFamilies()
		{
			var tempDir = TestTempPath.CreateDirectory("release_plan_catalog");
			var catalogPath = Path.Combine(tempDir, "families.json");

			try
			{
				File.WriteAllText(catalogPath, """
{
  "families": [
    {
      "id": "power-platform",
      "name": "Power Platform",
      "url": "https://learn.microsoft.com/en-us/power-platform/release-plan/",
      "category": "Power Platform",
      "summary": "Core platform capabilities."
    },
    {
      "id": "power-bi",
      "name": "Power BI",
      "url": "https://learn.microsoft.com/en-us/power-bi/release-plan/",
      "category": "Power BI",
      "summary": "Analytics and reporting."
    }
  ]
}
""");

				var output = new OutputToMemory();
				var executor = new ReleasePlanBrowseCommandExecutor(output);

				var result = executor.ExecuteAsync(new ReleasePlanBrowseCommand
				{
					CatalogPath = catalogPath,
					Query = "analytics"
				}, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "Power BI");
				Assert.IsFalse(output.ToString().Contains("Power Platform", StringComparison.OrdinalIgnoreCase));
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void BrowseShouldRenderAllWhenNoFilter()
		{
			var tempDir = TestTempPath.CreateDirectory("release_plan_catalog_all");
			var catalogPath = Path.Combine(tempDir, "families.json");

			try
			{
				File.WriteAllText(catalogPath, """
{
  "families": [
    {
      "id": "power-platform",
      "name": "Power Platform",
      "url": "https://learn.microsoft.com/en-us/power-platform/release-plan/",
      "category": "Power Platform",
      "summary": "Core platform capabilities."
    },
    {
      "id": "power-bi",
      "name": "Power BI",
      "url": "https://learn.microsoft.com/en-us/power-bi/release-plan/",
      "category": "Power BI",
      "summary": "Analytics and reporting."
    }
  ]
}
""");

				var output = new OutputToMemory();
				var executor = new ReleasePlanBrowseCommandExecutor(output);

				var result = executor.ExecuteAsync(new ReleasePlanBrowseCommand
				{
					CatalogPath = catalogPath
				}, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "Power Platform");
				StringAssert.Contains(output.ToString(), "Power BI");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void BrowseShouldReportEmptyWhenNoMatch()
		{
			var tempDir = TestTempPath.CreateDirectory("release_plan_catalog_empty");
			var catalogPath = Path.Combine(tempDir, "families.json");

			try
			{
				File.WriteAllText(catalogPath, """
{
  "families": [
    {
      "id": "power-platform",
      "name": "Power Platform",
      "url": "https://learn.microsoft.com/en-us/power-platform/release-plan/",
      "category": "Power Platform",
      "summary": "Core platform capabilities."
    }
  ]
}
""");

				var output = new OutputToMemory();
				var executor = new ReleasePlanBrowseCommandExecutor(output);

				var result = executor.ExecuteAsync(new ReleasePlanBrowseCommand
				{
					CatalogPath = catalogPath,
					Query = "nonexistent"
				}, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "No release-plan families found.");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void BrowseShouldFilterByCategory()
		{
			var tempDir = TestTempPath.CreateDirectory("release_plan_catalog_cat");
			var catalogPath = Path.Combine(tempDir, "families.json");

			try
			{
				File.WriteAllText(catalogPath, """
{
  "families": [
    {
      "id": "power-platform",
      "name": "Power Platform",
      "url": "https://learn.microsoft.com/en-us/power-platform/release-plan/",
      "category": "Power Platform",
      "summary": "Core platform capabilities."
    },
    {
      "id": "power-bi",
      "name": "Power BI",
      "url": "https://learn.microsoft.com/en-us/power-bi/release-plan/",
      "category": "Power BI",
      "summary": "Analytics and reporting."
    }
  ]
}
""");

				var output = new OutputToMemory();
				var executor = new ReleasePlanBrowseCommandExecutor(output);

				var result = executor.ExecuteAsync(new ReleasePlanBrowseCommand
				{
					CatalogPath = catalogPath,
					Category = "Power BI"
				}, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "Power BI");
				Assert.IsFalse(output.ToString().Contains("Power Platform", StringComparison.OrdinalIgnoreCase));
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}
