using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.ReleasePlan;
using Greg.Xrm.Command.TestSuite;
using Moq;

namespace Greg.Xrm.Command.Commands.ReleasePlan
{
	[TestClass]
	public class ReleasePlanReportCommandExecutorTest
	{
		private static readonly List<ReleasePlanItem> TestItems =
		[
			new()
			{
				Id = "1",
				Title = "New Copilot features",
				Product = "Power Platform",
				Status = "InDevelopment",
				Category = "New",
				Description = "New Copilot capabilities across the platform.",
			},
			new()
			{
				Id = "2",
				Title = "Power BI semantic model enhancements",
				Product = "Power BI",
				Status = "Launched",
				Category = "Updated",
				Description = "Enhanced DAX engine for semantic models.",
			},
		];

		private static Mock<IReleasePlanService> CreateServiceMock()
		{
			var serviceMock = new Mock<IReleasePlanService>();
			serviceMock
				.Setup(s => s.GetItemsAsync(It.IsAny<ReleasePlanFilter?>(), It.IsAny<CancellationToken>()))
				.Returns<ReleasePlanFilter?, CancellationToken>((_, _) =>
				{
					return Task.FromResult(TestItems.ToList());
				});
			return serviceMock;
		}

		[TestMethod]
		public async Task ReportShouldGenerateMarkdownToConsole()
		{
			var serviceMock = CreateServiceMock();
			var componentMap = new ReleasePlanComponentMap();
			var output = new OutputToMemory();
			var executor = new ReleasePlanReportCommandExecutor(serviceMock.Object, componentMap, output);

			var result = await executor.ExecuteAsync(new ReleasePlanReportCommand(), CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			var text = output.ToString();
			StringAssert.Contains(text, "# Release Plan Report");
			StringAssert.Contains(text, "New Copilot features");
			StringAssert.Contains(text, "Power BI");
		}

		[TestMethod]
		public async Task ReportShouldIncludeComponentTypesInMarkdown()
		{
			var serviceMock = CreateServiceMock();
			var componentMap = new ReleasePlanComponentMap();
			var output = new OutputToMemory();
			var executor = new ReleasePlanReportCommandExecutor(serviceMock.Object, componentMap, output);

			var result = await executor.ExecuteAsync(new ReleasePlanReportCommand(), CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			var text = output.ToString();
			StringAssert.Contains(text, "Canvas App");
			StringAssert.Contains(text, "Semantic Model");
		}

		[TestMethod]
		public async Task ReportShouldWorkInHtmlFormat()
		{
			var serviceMock = CreateServiceMock();
			var componentMap = new ReleasePlanComponentMap();
			var output = new OutputToMemory();
			var executor = new ReleasePlanReportCommandExecutor(serviceMock.Object, componentMap, output);

			var result = await executor.ExecuteAsync(new ReleasePlanReportCommand
			{
				Format = "html",
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			var text = output.ToString();
			StringAssert.Contains(text, "<!DOCTYPE html>");
			StringAssert.Contains(text, "Release Plan Report");
		}

		[TestMethod]
		public async Task ReportShouldWriteToFileWhenOutputPathSet()
		{
			var tempDir = TestTempPath.CreateDirectory("release_plan_report");
			var outputPath = Path.Combine(tempDir, "report.md");

			try
			{
				var serviceMock = CreateServiceMock();
				var componentMap = new ReleasePlanComponentMap();
				var output = new OutputToMemory();
				var executor = new ReleasePlanReportCommandExecutor(serviceMock.Object, componentMap, output);

				var result = await executor.ExecuteAsync(new ReleasePlanReportCommand
				{
					OutputPath = outputPath,
				}, CancellationToken.None);

				Assert.IsTrue(result.IsSuccess);
				Assert.IsTrue(File.Exists(outputPath));
				var content = File.ReadAllText(outputPath);
				StringAssert.Contains(content, "# Release Plan Report");
			}
			finally
			{
				if (Directory.Exists(tempDir))
					Directory.Delete(tempDir, true);
			}
		}

		[TestMethod]
		public async Task ReportShouldReturnEmptyMessageWhenNoItems()
		{
			var serviceMock = new Mock<IReleasePlanService>();
			serviceMock
				.Setup(s => s.GetItemsAsync(It.IsAny<ReleasePlanFilter?>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<ReleasePlanItem>());

			var componentMap = new ReleasePlanComponentMap();
			var output = new OutputToMemory();
			var executor = new ReleasePlanReportCommandExecutor(serviceMock.Object, componentMap, output);

			var result = await executor.ExecuteAsync(new ReleasePlanReportCommand(), CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			StringAssert.Contains(output.ToString(), "No release plan items found");
		}
	}
}
