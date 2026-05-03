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
obj"sources": [
	{
obj"name": "NuGet",
obj"provider": "Microsoft",
obj"category": "Packages",
obj"kind": "feed",
obj"summary": "Primary feed for .NET and PACX ecosystem packages.",
obj"homePage": "https://www.nuget.org/",
obj"packages": [ "Microsoft.PowerPlatform.Dataverse.Client", "Microsoft.PowerApps.CLI" ],
obj"capabilities": [ "browse packages", "inspect prerelease versions" ]
	},
	{
obj"name": "PowerShell Gallery",
obj"provider": "Microsoft",
obj"category": "Packages",
obj"kind": "feed",
obj"summary": "PowerShell module discovery for operator workflows.",
obj"homePage": "https://www.powershellgallery.com/packages",
obj"packages": [ "PowerFlowCore", "Microsoft.PowerPlatform.PowerAutomate.Desktop.Actions.SDK" ],
obj"capabilities": [ "browse modules", "inspect release metadata" ]
	}
obj]
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

