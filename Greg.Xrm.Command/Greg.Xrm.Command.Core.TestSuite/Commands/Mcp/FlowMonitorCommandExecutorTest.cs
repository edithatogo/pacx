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
obj"flows": [
	{
obj"name": "Flow Studio Watcher",
obj"provider": "Flow Studio",
obj"category": "Monitor",
obj"kind": "mcp tool",
obj"summary": "Observe active flow runs.",
obj"operations": [ "watch runs", "monitor alerts" ]
	},
	{
obj"name": "Flow Studio Debug",
obj"provider": "Flow Studio",
obj"category": "Debug",
obj"kind": "mcp tool",
obj"summary": "Inspect flow runs and failures.",
obj"operations": [ "debug", "inspect run history" ]
	}
obj]
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

