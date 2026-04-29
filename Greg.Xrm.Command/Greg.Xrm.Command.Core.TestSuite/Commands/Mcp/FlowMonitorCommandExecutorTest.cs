using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowMonitorCommandExecutorTest
	{
		[TestMethod]
		public void MonitorShouldRenderMonitoringFlows()
		{
			var tempDir = TestTempPath.CreateDirectory("flow_mcp_catalog_monitor");
			var catalogPath = Path.Combine(tempDir, "flows.json");

			try
			{
				File.WriteAllText(catalogPath, """
{
  "flows": [
    {
      "name": "Flow Studio Watcher",
      "provider": "Flow Studio",
      "category": "Monitor",
      "kind": "mcp tool",
      "summary": "Observe active flow runs.",
      "operations": [ "watch runs", "monitor alerts" ]
    },
    {
      "name": "Flow Studio Debug",
      "provider": "Flow Studio",
      "category": "Debug",
      "kind": "mcp tool",
      "summary": "Inspect flow runs and failures.",
      "operations": [ "debug", "inspect run history" ]
    }
  ]
}
""");

				var output = new OutputToMemory();
				var executor = new FlowMonitorCommandExecutor(output);
				var result = executor.ExecuteAsync(new FlowMonitorCommand { CatalogPath = catalogPath }, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "Flow Studio Watcher");
				Assert.IsFalse(output.ToString().Contains("Flow Studio Debug", StringComparison.OrdinalIgnoreCase));
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}
