using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowDebugCommandExecutorTest
	{
		[TestMethod]
		public void DebugShouldRenderDebugFlows()
		{
			var tempDir = TestTempPath.CreateDirectory("flow_mcp_catalog_debug");
			var catalogPath = Path.Combine(tempDir, "flows.json");

			try
			{
				File.WriteAllText(catalogPath, """
{
obj"flows": [
	{
obj"name": "Flow Studio Debug",
obj"provider": "Flow Studio",
obj"category": "Debug",
obj"kind": "mcp tool",
obj"summary": "Inspect flow runs and failures.",
obj"operations": [ "debug", "inspect run history", "trace failure" ]
	},
	{
obj"name": "Flow Studio Govern",
obj"provider": "Flow Studio",
obj"category": "Governance",
obj"kind": "mcp tool",
obj"summary": "Review approvals and environment controls.",
obj"operations": [ "govern", "audit" ]
	}
obj]
}
""");

				var output = new OutputToMemory();
				var executor = new FlowDebugCommandExecutor(output);
				var result = executor.ExecuteAsync(new FlowDebugCommand { CatalogPath = catalogPath }, CancellationToken.None).GetAwaiter().GetResult();

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

