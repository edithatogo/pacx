using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.SkillPack
{
	[TestClass]
	public class ListCommandExecutorTest
	{
		[TestMethod]
		public void ExecuteShouldRenderMatchingPacks()
		{
			var tempDir = TestTempPath.CreateDirectory("skillpack_list_test");
			var catalogPath = Path.Combine(tempDir, "packs.json");
			try
			{
				File.WriteAllText(catalogPath, """{ "packs": [{"id":"test-pack","name":"Test Pack","version":"1.0.0","author":"PACX","description":"A test pack.","capabilities":["test"],"dependencies":[],"tags":["test"]}] }""");

				var output = new OutputToMemory();
				var executor = new ListCommandExecutor(output);
				var result = executor.ExecuteAsync(
					new ListCommand { CatalogPath = catalogPath },
					CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "Test Pack");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}
