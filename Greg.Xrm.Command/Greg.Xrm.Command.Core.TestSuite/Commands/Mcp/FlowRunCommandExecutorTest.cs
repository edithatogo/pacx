using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class FlowRunCommandExecutorTest
	{
		[TestMethod]
		public void RunShouldRenderSelectedFlowDetails()
		{
			var tempDir = TestTempPath.CreateDirectory("flow_mcp_catalog_run");
			var catalogPath = Path.Combine(tempDir, "flows.json");

			try
			{
				File.WriteAllText(catalogPath, """
{
obj"flows": [
	{
obj"name": "Flow Studio Authoring",
obj"provider": "Flow Studio",
obj"category": "Authoring",
obj"kind": "mcp tool",
obj"summary": "Packaged authoring operations for composing flow assets and commands.",
obj"homePage": "https://flowstudio.app/welcome/about",
obj"operations": [ "compose", "package", "preview" ]
	}
obj]
}
""");

				var output = new OutputToMemory();
				var executor = new FlowRunCommandExecutor(output);
				var result = executor.ExecuteAsync(new FlowRunCommand { CatalogPath = catalogPath, Name = "Flow Studio Authoring" }, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "Flow Studio Authoring (Authoring)");
				StringAssert.Contains(output.ToString(), "Operations:");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}

