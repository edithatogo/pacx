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
"flows": [
	{
"name": "Flow Studio Debug",
"provider": "Flow Studio",
"category": "Debug",
"kind": "mcp tool",
"summary": "Inspect flow runs and failures.",
"operations": [ "debug", "inspect run history", "trace failure" ]
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


