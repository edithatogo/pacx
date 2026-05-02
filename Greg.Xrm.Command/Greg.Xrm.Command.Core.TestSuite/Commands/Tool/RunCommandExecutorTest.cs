using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Tool
{
	[TestClass]
	public class RunCommandExecutorTest
	{
		[TestMethod]
		public void ExecuteWithExistingToolShouldSucceed()
		{
			var tempDir = TestTempPath.CreateDirectory("tool_run_test");
			var catalogPath = Path.Combine(tempDir, "tools.json");
			try
			{
				File.WriteAllText(catalogPath, """{ "tools": [{"id":"my-tool","name":"My Tool","provider":"PACX","category":"core","kind":"cli","summary":"A test tool.","capabilities":["test"]}] }""");

				var output = new OutputToMemory();
				var executor = new RunCommandExecutor(output);
				var result = executor.ExecuteAsync(
					new RunCommand { CatalogPath = catalogPath, Name = "my-tool" },
					CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "My Tool");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void ExecuteWithNonExistentToolShouldFail()
		{
			var tempDir = TestTempPath.CreateDirectory("tool_run_missing_test");
			var catalogPath = Path.Combine(tempDir, "tools.json");
			try
			{
				File.WriteAllText(catalogPath, """{ "tools": [] }""");

				var output = new OutputToMemory();
				var executor = new RunCommandExecutor(output);
				var result = executor.ExecuteAsync(
					new RunCommand { CatalogPath = catalogPath, Name = "missing" },
					CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsFalse(result.IsSuccess);
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}
