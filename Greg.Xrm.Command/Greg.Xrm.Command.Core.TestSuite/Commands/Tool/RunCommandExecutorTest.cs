using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Tool
{
	[TestClass]
	public class RunCommandExecutorTest
	{
		[TestMethod]
		public void RunShouldRenderToolDetails()
		{
			var tempDir = TestTempPath.CreateDirectory("tool_catalog_run");
			var catalogPath = Path.Combine(tempDir, "tools.json");

			try
			{
				File.WriteAllText(catalogPath, """
{
  "tools": [
    {
      "id": "xrmtoolbox",
      "name": "XrmToolBox",
      "provider": "MscrmTools",
      "category": "Dataverse",
      "kind": "desktop app",
      "summary": "Plugin host and tool library for Dataverse admins.",
      "homePage": "https://www.xrmtoolbox.com/plugins/",
      "capabilities": [ "discover plugins", "launch tools" ]
    }
  ]
}
""");

				var output = new OutputToMemory();
				var executor = new RunCommandExecutor(output);

				var result = executor.ExecuteAsync(new RunCommand { CatalogPath = catalogPath, Name = "xrmtoolbox" }, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "XrmToolBox (Dataverse)");
				StringAssert.Contains(output.ToString(), "Home page: https://www.xrmtoolbox.com/plugins/");
				StringAssert.Contains(output.ToString(), "Capabilities:");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}
