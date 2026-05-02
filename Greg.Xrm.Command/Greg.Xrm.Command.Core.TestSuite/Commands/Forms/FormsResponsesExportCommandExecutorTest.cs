using Greg.Xrm.Command.Services.Forms;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsResponsesExportCommandExecutorTest
	{
		[TestMethod]
		public void ExecuteShouldExportCsv()
		{
			var tempDir = TestTempPath.CreateDirectory("forms_export_test");
			var outputPath = Path.Combine(tempDir, "export.csv");
			try
			{
				var apiMock = new Mock<IFormsApiClient>();
				apiMock.Setup(a => a.GetResponsesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
					.ReturnsAsync(new List<FormsResponse>
					{
						new() { Id = 1, SubmittedAt = new DateTime(2026, 1, 1), Answers = "{\"q1\":\"Yes\"}" }
					});

				var output = new OutputToMemory();
				var executor = new FormsResponsesExportCommandExecutor(output, apiMock.Object);
				var result = executor.ExecuteAsync(
					new FormsResponsesExportCommand
					{
						TenantId = "00000000-0000-0000-0000-000000000000",
						FormId = "form123",
						OutputPath = outputPath,
						Format = "csv"
					},
					CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess, result.ErrorMessage ?? "no error");
				Assert.IsTrue(File.Exists(outputPath));
				var content = File.ReadAllText(outputPath);
				StringAssert.Contains(content, "ResponseId");
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public void ExecuteShouldExportJson()
		{
			var tempDir = TestTempPath.CreateDirectory("forms_export_json_test");
			var outputPath = Path.Combine(tempDir, "export.json");
			try
			{
				var apiMock = new Mock<IFormsApiClient>();
				apiMock.Setup(a => a.GetResponsesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 100, 0, It.IsAny<CancellationToken>()))
					.ReturnsAsync(new List<FormsResponse>());

				var executor = new FormsResponsesExportCommandExecutor(new OutputToMemory(), apiMock.Object);
				var result = executor.ExecuteAsync(
					new FormsResponsesExportCommand
					{
						TenantId = "00000000-0000-0000-0000-000000000000",
						FormId = "form123",
						OutputPath = outputPath,
						Format = "json"
					},
					CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				Assert.IsTrue(File.Exists(outputPath));
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}
	}
}
