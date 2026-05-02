using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowBrowseCommandExecutorTest
	{
		[TestMethod]
		public void ExecuteShouldRenderMatchingFlows()
		{
			var tempDir = TestTempPath.CreateDirectory("flow_mcp_test");
			var catalogPath = Path.Combine(tempDir, "flows.json");
			try
			{
				File.WriteAllText(catalogPath, """{ "flows": [{"name":"List Flows","provider":"PACX","category":"browse","kind":"discovery","summary":"Lists flows.","operations":["flow-list"]}] }""");

				var output = new OutputToMemory();
				var executor = new FlowBrowseCommandExecutor(output);
				var result = executor.ExecuteAsync(
					new FlowBrowseCommand { CatalogPath = catalogPath },
					CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "List Flows");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void ExecuteWithNoMatchesShouldShowWarning()
		{
			var tempDir = TestTempPath.CreateDirectory("flow_mcp_empty_test");
			var catalogPath = Path.Combine(tempDir, "flows.json");
			try
			{
				File.WriteAllText(catalogPath, """{ "flows": [] }""");

				var output = new OutputToMemory();
				var executor = new FlowBrowseCommandExecutor(output);
				var result = executor.ExecuteAsync(
					new FlowBrowseCommand { CatalogPath = catalogPath, Query = "nonexistent" },
					CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}
