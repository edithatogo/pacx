using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowGovernCommandExecutorTest
	{
		[TestMethod]
		public void GovernShouldRenderGovernanceFlows()
		{
			var tempDir = TestTempPath.CreateDirectory("flow_mcp_catalog_govern");
			var catalogPath = Path.Combine(tempDir, "flows.json");

			try
			{
				File.WriteAllText(catalogPath, """
{
obj"flows": [
	{
obj"name": "Flow Studio Govern",
obj"provider": "Flow Studio",
obj"category": "Governance",
obj"kind": "mcp tool",
obj"summary": "Review approvals and environment controls.",
obj"operations": [ "govern", "approve", "audit" ]
	},
	{
obj"name": "Flow Studio Authoring",
obj"provider": "Flow Studio",
obj"category": "Authoring",
obj"kind": "mcp tool",
obj"summary": "Packaged authoring operations for composing flow assets and commands.",
obj"operations": [ "compose", "package", "preview" ]
	}
obj]
}
""");

				var output = new OutputToMemory();
				var executor = new FlowGovernCommandExecutor(output);
				var result = executor.ExecuteAsync(new FlowGovernCommand { CatalogPath = catalogPath }, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "Flow Studio Govern");
				Assert.IsFalse(output.ToString().Contains("Flow Studio Authoring", StringComparison.OrdinalIgnoreCase));
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}

