using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowInspectCommandExecutorTest
	{
		[TestMethod]
		public void InspectShouldRenderFlowDetails()
		{
			var tempDir = TestTempPath.CreateDirectory("flow_mcp_catalog_inspect");
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
obj"homePage": "https://mcp.flowstudio.app/",
obj"operations": [ "debug", "inspect run history" ]
	}
obj]
}
""");

				var output = new OutputToMemory();
				var executor = new FlowInspectCommandExecutor(output);
				var result = executor.ExecuteAsync(new FlowInspectCommand { CatalogPath = catalogPath, Name = "Flow Studio Debug" }, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "Flow Studio Debug (Debug)");
				StringAssert.Contains(output.ToString(), "Operations:");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}

