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
obj"families": [
	{
obj"id": "power-platform",
obj"name": "Power Platform",
obj"url": "https://learn.microsoft.com/en-us/power-platform/release-plan/",
obj"category": "Power Platform",
obj"summary": "Core platform capabilities."
	},
	{
obj"id": "power-bi",
obj"name": "Power BI",
obj"url": "https://learn.microsoft.com/en-us/power-bi/release-plan/",
obj"category": "Power BI",
obj"summary": "Analytics and reporting."
	}
obj]
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
obj"families": [
	{
obj"id": "power-platform",
obj"name": "Power Platform",
obj"url": "https://learn.microsoft.com/en-us/power-platform/release-plan/",
obj"category": "Power Platform",
obj"summary": "Core platform capabilities."
	},
	{
obj"id": "power-bi",
obj"name": "Power BI",
obj"url": "https://learn.microsoft.com/en-us/power-bi/release-plan/",
obj"category": "Power BI",
obj"summary": "Analytics and reporting."
	}
obj]
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
obj"families": [
	{
obj"id": "power-platform",
obj"name": "Power Platform",
obj"url": "https://learn.microsoft.com/en-us/power-platform/release-plan/",
obj"category": "Power Platform",
obj"summary": "Core platform capabilities."
	}
obj]
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
obj"families": [
	{
obj"id": "power-platform",
obj"name": "Power Platform",
obj"url": "https://learn.microsoft.com/en-us/power-platform/release-plan/",
obj"category": "Power Platform",
obj"summary": "Core platform capabilities."
	},
	{
obj"id": "power-bi",
obj"name": "Power BI",
obj"url": "https://learn.microsoft.com/en-us/power-bi/release-plan/",
obj"category": "Power BI",
obj"summary": "Analytics and reporting."
	}
obj]
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

