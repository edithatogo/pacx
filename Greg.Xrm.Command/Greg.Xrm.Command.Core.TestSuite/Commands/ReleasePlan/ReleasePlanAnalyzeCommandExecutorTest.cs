using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.ReleasePlan;
using Moq;

namespace Greg.Xrm.Command.Commands.ReleasePlan
{
	[TestClass]
	public class ReleasePlanAnalyzeCommandExecutorTest
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
			new()
			{
				Id = "3",
				Title = "Dynamics 365 Sales insights",
				Product = "Dynamics 365 Sales",
				Status = "RollingOut",
				Category = "New",
				Description = "AI-driven sales insights.",
			},
		];

		private static Mock<IReleasePlanService> CreateServiceMock()
		{
			var serviceMock = new Mock<IReleasePlanService>();
			serviceMock
				.Setup(s => s.GetItemsAsync(It.IsAny<ReleasePlanFilter?>(), It.IsAny<CancellationToken>()))
				.Returns<ReleasePlanFilter?, CancellationToken>((filter, _) =>
				{
					var items = TestItems.AsEnumerable();

					if (filter is not null)
					{
						if (!string.IsNullOrWhiteSpace(filter.Product))
							items = items.Where(i => i.Product.Contains(filter.Product, StringComparison.OrdinalIgnoreCase));
						if (!string.IsNullOrWhiteSpace(filter.Status))
							items = items.Where(i => i.Status.Contains(filter.Status, StringComparison.OrdinalIgnoreCase));
						if (filter.MaxResults.HasValue && filter.MaxResults.Value > 0)
							items = items.Take(filter.MaxResults.Value);
					}

					return Task.FromResult(items.ToList());
				});
			return serviceMock;
		}

		[TestMethod]
		public async Task AnalyzeShouldReturnSuccessWithItems()
		{
			var serviceMock = CreateServiceMock();
			var componentMap = new ReleasePlanComponentMap();
			var output = new OutputToMemory();
			var executor = new ReleasePlanAnalyzeCommandExecutor(serviceMock.Object, componentMap, output);

			var result = await executor.ExecuteAsync(new ReleasePlanAnalyzeCommand
			{
				EnvironmentName = "test-env",
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			var text = output.ToString();
			StringAssert.Contains(text, "Power Platform");
			StringAssert.Contains(text, "New Copilot features");
		}

		[TestMethod]
		public async Task AnalyzeShouldFilterByProduct()
		{
			var serviceMock = CreateServiceMock();
			var componentMap = new ReleasePlanComponentMap();
			var output = new OutputToMemory();
			var executor = new ReleasePlanAnalyzeCommandExecutor(serviceMock.Object, componentMap, output);

			var result = await executor.ExecuteAsync(new ReleasePlanAnalyzeCommand
			{
				EnvironmentName = "test-env",
				Product = "Power BI",
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			var text = output.ToString();
			StringAssert.Contains(text, "Power BI");
			Assert.IsFalse(text.Contains("Power Platform", StringComparison.OrdinalIgnoreCase));
		}

		[TestMethod]
		public async Task AnalyzeShouldFilterByStatus()
		{
			var serviceMock = CreateServiceMock();
			var componentMap = new ReleasePlanComponentMap();
			var output = new OutputToMemory();
			var executor = new ReleasePlanAnalyzeCommandExecutor(serviceMock.Object, componentMap, output);

			var result = await executor.ExecuteAsync(new ReleasePlanAnalyzeCommand
			{
				EnvironmentName = "test-env",
				Status = "Launched",
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			var text = output.ToString();
			StringAssert.Contains(text, "Power BI");
		}

		[TestMethod]
		public async Task AnalyzeShouldReturnEmptyForNoMatchingItems()
		{
			var serviceMock = CreateServiceMock();
			var componentMap = new ReleasePlanComponentMap();
			var output = new OutputToMemory();
			var executor = new ReleasePlanAnalyzeCommandExecutor(serviceMock.Object, componentMap, output);

			var result = await executor.ExecuteAsync(new ReleasePlanAnalyzeCommand
			{
				EnvironmentName = "test-env",
				Product = "Nonexistent",
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			var text = output.ToString();
			StringAssert.Contains(text, "No impacted items found");
		}

		[TestMethod]
		public async Task AnalyzeShouldDisplayEnvironmentName()
		{
			var serviceMock = CreateServiceMock();
			var componentMap = new ReleasePlanComponentMap();
			var output = new OutputToMemory();
			var executor = new ReleasePlanAnalyzeCommandExecutor(serviceMock.Object, componentMap, output);

			var result = await executor.ExecuteAsync(new ReleasePlanAnalyzeCommand
			{
				EnvironmentName = "my-analysis-env",
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			var text = output.ToString();
			StringAssert.Contains(text, "my-analysis-env");
		}

		[TestMethod]
		public async Task AnalyzeShouldShowComponentTypesInOutput()
		{
			var serviceMock = CreateServiceMock();
			var componentMap = new ReleasePlanComponentMap();
			var output = new OutputToMemory();
			var executor = new ReleasePlanAnalyzeCommandExecutor(serviceMock.Object, componentMap, output);

			var result = await executor.ExecuteAsync(new ReleasePlanAnalyzeCommand
			{
				EnvironmentName = "test-env",
				Product = "Power Platform",
			}, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			var text = output.ToString();
			StringAssert.Contains(text, "Canvas App");
			StringAssert.Contains(text, "Flow");
		}
	}
}
