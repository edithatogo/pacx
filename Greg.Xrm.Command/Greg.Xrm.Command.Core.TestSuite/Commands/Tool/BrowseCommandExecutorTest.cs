using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Tool
{
	[TestClass]
	public class BrowseCommandExecutorTest
	{
		[TestMethod]
		public void BrowseShouldRenderMatchingTools()
		{
			var tempDir = TestTempPath.CreateDirectory("tool_catalog");
			var catalogPath = Path.Combine(tempDir, "tools.json");

			try
			{
				File.WriteAllText(catalogPath, """
{
  "tools": [
    {
      "id": "xrmtoolbox",
      "name": "XrmToolBox",
      "provider": "MscrmTools",
      "category": "Dataverse",
      "kind": "desktop app",
      "summary": "Plugin host and tool library for Dataverse admins.",
      "capabilities": [ "discover plugins", "launch tools" ]
    },
    {
      "id": "flowstudio",
      "name": "Flow Studio",
      "provider": "EffortlessMetrics",
      "category": "Power Automate",
      "kind": "mcp service",
      "summary": "MCP workflows for flow authoring and operations.",
      "capabilities": [ "build", "debug" ]
    }
  ]
}
""");

				var output = new OutputToMemory();
				var executor = new BrowseCommandExecutor(output);

				var result = executor.ExecuteAsync(new BrowseCommand { CatalogPath = catalogPath, Query = "flow" }, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "Flow Studio");
				Assert.IsFalse(output.ToString().Contains("XrmToolBox", StringComparison.OrdinalIgnoreCase));
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}
