using Greg.Xrm.Command.Services;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.SkillPack
{
	[TestClass]
	public class InstallCommandExecutorTest
	{
		[TestMethod]
		public void InstallNonExistentShouldFail()
		{
			var tempDir = TestTempPath.CreateDirectory("skillpack_install_missing_test");
			var catalogPath = Path.Combine(tempDir, "packs.json");
			try
			{
				File.WriteAllText(catalogPath, """{ "packs": [] }""");

				var storageMock = new Mock<IStorage>();
				storageMock.Setup(s => s.GetOrCreateStorageFolder()).Returns(new DirectoryInfo(tempDir));

				var output = new OutputToMemory();
				var executor = new InstallCommandExecutor(output, storageMock.Object);
				var result = executor.ExecuteAsync(
					new InstallCommand { CatalogPath = catalogPath, Id = "nonexistent" },
					CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsFalse(result.IsSuccess);
				StringAssert.Contains(result.ErrorMessage, "not found");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void InstallDryRunShouldSucceed()
		{
			var tempDir = TestTempPath.CreateDirectory("skillpack_install_dry_test");
			var catalogPath = Path.Combine(tempDir, "packs.json");
			try
			{
				File.WriteAllText(catalogPath, """{ "packs": [{"id":"test-pack","name":"Test Pack","version":"1.0.0","author":"PACX","description":"A test pack.","capabilities":["test"],"dependencies":[],"tags":["test"]}] }""");

				var storageMock = new Mock<IStorage>();
				storageMock.Setup(s => s.GetOrCreateStorageFolder()).Returns(new DirectoryInfo(tempDir));

				var output = new OutputToMemory();
				var executor = new InstallCommandExecutor(output, storageMock.Object);
				var result = executor.ExecuteAsync(
					new InstallCommand { CatalogPath = catalogPath, Id = "test-pack", DryRun = true },
					CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "dry-run");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}
