using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Package
{
	[TestClass]
	public class PackageSourceBrowseCommandExecutorTest
	{
		[TestMethod]
		public void BrowseShouldRenderMatchingSources()
		{
			var tempDir = TestTempPath.CreateDirectory("package_source_catalog");
			var catalogPath = Path.Combine(tempDir, "sources.json");

			try
			{
				File.WriteAllText(catalogPath, """
{
  "sources": [
    {
      "name": "NuGet",
      "provider": "Microsoft",
      "category": "Packages",
      "kind": "feed",
      "summary": "Primary feed for .NET and PACX ecosystem packages.",
      "homePage": "https://www.nuget.org/",
      "packages": [ "Microsoft.PowerPlatform.Dataverse.Client", "Microsoft.PowerApps.CLI" ],
      "capabilities": [ "browse packages", "inspect prerelease versions" ]
    },
    {
      "name": "PowerShell Gallery",
      "provider": "Microsoft",
      "category": "Packages",
      "kind": "feed",
      "summary": "PowerShell module discovery for operator workflows.",
      "homePage": "https://www.powershellgallery.com/packages",
      "packages": [ "PowerFlowCore", "Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK" ],
      "capabilities": [ "browse modules", "inspect release metadata" ]
    }
  ]
}
""");

				var output = new OutputToMemory();
				var executor = new PackageSourceBrowseCommandExecutor(output);

				var result = executor.ExecuteAsync(new PackageSourceBrowseCommand { CatalogPath = catalogPath, Query = "powershell" }, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "PowerShell Gallery");
				Assert.IsFalse(output.ToString().Contains("NuGet", StringComparison.OrdinalIgnoreCase));
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}
