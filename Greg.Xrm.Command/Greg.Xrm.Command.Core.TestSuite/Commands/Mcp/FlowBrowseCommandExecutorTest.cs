using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowBrowseCommandExecutorTest
	{
		[TestMethod]
		public void BrowseShouldRenderMatchingFlows()
		{
			var tempDir = TestTempPath.CreateDirectory("flow_mcp_catalog");
			var catalogPath = Path.Combine(tempDir, "flows.json");

			try
			{
				File.WriteAllText(catalogPath, """
{
  "flows": [
    {
      "name": "Flow Studio Debug",
      "provider": "Flow Studio",
      "category": "Debug",
      "kind": "mcp tool",
      "summary": "Inspect flow runs and failures.",
      "homePage": "https://mcp.flowstudio.app/",
      "operations": [ "debug", "inspect run history" ]
    },
    {
      "name": "Flow Studio Govern",
      "provider": "Flow Studio",
      "category": "Governance",
      "kind": "mcp tool",
      "summary": "Review approvals and environment controls.",
      "operations": [ "govern", "audit" ]
    }
  ]
}
""");

				var output = new OutputToMemory();
				var executor = new FlowBrowseCommandExecutor(output);
				var result = executor.ExecuteAsync(new FlowBrowseCommand { CatalogPath = catalogPath, Query = "debug" }, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "Flow Studio Debug");
				Assert.IsFalse(output.ToString().Contains("Flow Studio Govern", StringComparison.OrdinalIgnoreCase));
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}
