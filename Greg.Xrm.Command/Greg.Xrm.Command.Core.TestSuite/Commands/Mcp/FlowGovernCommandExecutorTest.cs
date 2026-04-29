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
  "flows": [
    {
      "name": "Flow Studio Govern",
      "provider": "Flow Studio",
      "category": "Governance",
      "kind": "mcp tool",
      "summary": "Review approvals and environment controls.",
      "operations": [ "govern", "approve", "audit" ]
    },
    {
      "name": "Flow Studio Authoring",
      "provider": "Flow Studio",
      "category": "Authoring",
      "kind": "mcp tool",
      "summary": "Packaged authoring operations for composing flow assets and commands.",
      "operations": [ "compose", "package", "preview" ]
    }
  ]
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
