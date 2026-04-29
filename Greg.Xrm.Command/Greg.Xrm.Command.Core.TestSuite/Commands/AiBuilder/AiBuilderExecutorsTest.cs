using Greg.Xrm.Command.Services.AiBuilder;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Microsoft.PowerPlatform.Dataverse.Client;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Commands.AiBuilder
{
	[TestClass]
	public class AiModelListCommandExecutorTest
	{
		private Mock<IOutput>? outputMock;
		private Mock<IOrganizationServiceRepository>? orgRepoMock;
		private Mock<IAiBuilderApiClientFactory>? factoryMock;
		private Mock<IAiBuilderApiClient>? clientMock;
		private Mock<IOrganizationServiceAsync2>? serviceMock;

		[TestInitialize]
		public void Setup()
		{
			outputMock = new Mock<IOutput>();
			orgRepoMock = new Mock<IOrganizationServiceRepository>();
			factoryMock = new Mock<IAiBuilderApiClientFactory>();
			clientMock = new Mock<IAiBuilderApiClient>();
			serviceMock = new Mock<IOrganizationServiceAsync2>();

			orgRepoMock
				.Setup(r => r.GetCurrentConnectionAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(serviceMock.Object);

			factoryMock
				.Setup(f => f.CreateAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(clientMock.Object);
		}

		[TestMethod]
		public async Task ExecuteAsync_WithModels_ShouldSucceedAndOutputModels()
		{
			var models = new List<AiModelInfo>
			{
				new() { Id = "guid-1", Name = "Invoice Model", Status = "Published", CreatedOn = DateTimeOffset.UtcNow.UtcDateTime },
				new() { Id = "guid-2", Name = "Receipt Model", Status = "Training", CreatedOn = DateTimeOffset.UtcNow.AddDays(-1).UtcDateTime }
			};

			clientMock!
				.Setup(c => c.ListModelsAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(models);

			var executor = new AiModelListCommandExecutor(
				outputMock!.Object,
				factoryMock!.Object);

			var result = await executor.ExecuteAsync(
				new AiModelListCommand { Format = "table" },
				CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			outputMock.Verify(o => o.WriteLine(It.IsAny<string>(), It.IsAny<ConsoleColor>()), Times.AtLeastOnce);
		}

		[TestMethod]
		public async Task ExecuteAsync_NoModels_ShouldSucceedWithNoModelsMessage()
		{
			clientMock!
				.Setup(c => c.ListModelsAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<AiModelInfo>());

			var executor = new AiModelListCommandExecutor(
				outputMock!.Object,
				factoryMock!.Object);

			var result = await executor.ExecuteAsync(
				new AiModelListCommand(),
				CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			outputMock.Verify(o => o.WriteLine("No AI Builder models found.", ConsoleColor.Yellow), Times.Once);
		}

		[TestMethod]
		public async Task ExecuteAsync_JsonFormat_ShouldOutputJson()
		{
			var models = new List<AiModelInfo>
			{
				new() { Id = "guid-1", Name = "Test Model", Status = "Published" }
			};

			clientMock!
				.Setup(c => c.ListModelsAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(models);

			var executor = new AiModelListCommandExecutor(
				outputMock!.Object,
				factoryMock!.Object);

			var result = await executor.ExecuteAsync(
				new AiModelListCommand { Format = "json" },
				CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			outputMock.Verify(o => o.WriteLine(It.Is<string>(s => s.Contains("Test Model"))), Times.Once);
		}

		[TestMethod]
		public async Task ExecuteAsync_StatusFilter_ShouldReturnOnlyMatchingModels()
		{
			var models = new List<AiModelInfo>
			{
				new() { Id = "guid-1", Name = "Published Model", Status = "Published" },
				new() { Id = "guid-2", Name = "Training Model", Status = "Training" },
				new() { Id = "guid-3", Name = "Draft Model", Status = "Draft" }
			};

			clientMock!
				.Setup(c => c.ListModelsAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(models);

			var executor = new AiModelListCommandExecutor(
				outputMock!.Object,
				factoryMock!.Object);

			var result = await executor.ExecuteAsync(
				new AiModelListCommand { Status = "Completed" },
				CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			outputMock.Verify(o => o.WriteTable(
				It.Is<IReadOnlyList<AiModelInfo>>(rows =>
					rows.Count == 1
					&& rows[0].Name == "Published Model"
					&& rows[0].Status == "Published"),
				It.IsAny<Func<string[]>>(),
				It.IsAny<Func<AiModelInfo, string[]>>(),
				It.IsAny<Func<int, AiModelInfo, ConsoleColor?>?>()), Times.Once);
		}

		[TestMethod]
		public async Task ExecuteAsync_InvalidStatusFilter_ShouldFail()
		{
			clientMock!
				.Setup(c => c.ListModelsAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<AiModelInfo>());

			var executor = new AiModelListCommandExecutor(
				outputMock!.Object,
				factoryMock!.Object);

			var result = await executor.ExecuteAsync(
				new AiModelListCommand { Status = "Archived" },
				CancellationToken.None);

			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual("AI model list error: unsupported status filter. Use NotStarted, Training, Completed, or Failed.", result.ErrorMessage);
		}
	}

	[TestClass]
	public class AiModelTrainCommandExecutorTest
	{
		private Mock<IOutput>? outputMock;
		private Mock<IOrganizationServiceRepository>? orgRepoMock;
		private Mock<IAiBuilderApiClientFactory>? factoryMock;
		private Mock<IAiBuilderApiClient>? clientMock;
		private Mock<IOrganizationServiceAsync2>? serviceMock;

		[TestInitialize]
		public void Setup()
		{
			outputMock = new Mock<IOutput>();
			orgRepoMock = new Mock<IOrganizationServiceRepository>();
			factoryMock = new Mock<IAiBuilderApiClientFactory>();
			clientMock = new Mock<IAiBuilderApiClient>();
			serviceMock = new Mock<IOrganizationServiceAsync2>();

			orgRepoMock
				.Setup(r => r.GetCurrentConnectionAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(serviceMock.Object);

			factoryMock
				.Setup(f => f.CreateAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(clientMock.Object);
		}

		[TestMethod]
		public async Task ExecuteAsync_TrainWithoutWait_ShouldSucceed()
		{
			clientMock!
				.Setup(c => c.TrainModelAsync(
					It.IsAny<string>(),
					It.IsAny<bool>(),
					It.IsAny<TimeSpan>(),
					It.IsAny<TimeSpan>(),
					It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var executor = new AiModelTrainCommandExecutor(
				outputMock!.Object,
				factoryMock!.Object);

			var result = await executor.ExecuteAsync(
				new AiModelTrainCommand { ModelId = "test-model-id", Wait = false },
				CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			clientMock.Verify(c => c.TrainModelAsync(
				"test-model-id",
				false,
				TimeSpan.FromSeconds(5),
				TimeSpan.FromSeconds(600),
				It.IsAny<CancellationToken>()), Times.Once);
			outputMock.Verify(o => o.WriteLine("Use 'ai model list' to check the latest training status.", ConsoleColor.Cyan), Times.Once);
		}

		[TestMethod]
		public async Task ExecuteAsync_TrainWithWait_ShouldSucceed()
		{
			clientMock!
				.Setup(c => c.TrainModelAsync(
					It.IsAny<string>(),
					It.IsAny<bool>(),
					It.IsAny<TimeSpan>(),
					It.IsAny<TimeSpan>(),
					It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var executor = new AiModelTrainCommandExecutor(
				outputMock!.Object,
				factoryMock!.Object);

			var result = await executor.ExecuteAsync(
				new AiModelTrainCommand { ModelId = "test-model-id", Wait = true },
				CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			clientMock.Verify(c => c.TrainModelAsync(
				"test-model-id",
				true,
				TimeSpan.FromSeconds(5),
				TimeSpan.FromSeconds(600),
				It.IsAny<CancellationToken>()), Times.Once);
			outputMock.Verify(o => o.WriteLine("Model training triggered successfully and completed!", ConsoleColor.Green), Times.Once);
		}

		[TestMethod]
		public async Task ExecuteAsync_WithPollingOptions_ShouldPassValuesToClient()
		{
			clientMock!
				.Setup(c => c.TrainModelAsync(
					It.IsAny<string>(),
					It.IsAny<bool>(),
					It.IsAny<TimeSpan>(),
					It.IsAny<TimeSpan>(),
					It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var executor = new AiModelTrainCommandExecutor(
				outputMock!.Object,
				factoryMock!.Object);

			var result = await executor.ExecuteAsync(
				new AiModelTrainCommand
				{
					ModelId = "test-model-id",
					Wait = true,
					PollIntervalSeconds = 7,
					TimeoutSeconds = 120
				},
				CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			clientMock.Verify(c => c.TrainModelAsync(
				"test-model-id",
				true,
				TimeSpan.FromSeconds(7),
				TimeSpan.FromSeconds(120),
				It.IsAny<CancellationToken>()), Times.Once);
		}

		[TestMethod]
		public async Task ExecuteAsync_WithInvalidPollingOptions_ShouldFailBeforeCallingClient()
		{
			var executor = new AiModelTrainCommandExecutor(
				outputMock!.Object,
				factoryMock!.Object);

			var result = await executor.ExecuteAsync(
				new AiModelTrainCommand
				{
					ModelId = "test-model-id",
					Wait = true,
					PollIntervalSeconds = 0,
					TimeoutSeconds = 120
				},
				CancellationToken.None);

			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual("AI model train error: --poll-interval must be greater than zero.", result.ErrorMessage);
			clientMock!.Verify(c => c.TrainModelAsync(
				It.IsAny<string>(),
				It.IsAny<bool>(),
				It.IsAny<TimeSpan>(),
				It.IsAny<TimeSpan>(),
				It.IsAny<CancellationToken>()), Times.Never);
		}
	}

	[TestClass]
	public class AiModelPublishCommandExecutorTest
	{
		private Mock<IOutput>? outputMock;
		private Mock<IOrganizationServiceRepository>? orgRepoMock;
		private Mock<IAiBuilderApiClientFactory>? factoryMock;
		private Mock<IAiBuilderApiClient>? clientMock;
		private Mock<IOrganizationServiceAsync2>? serviceMock;

		[TestInitialize]
		public void Setup()
		{
			outputMock = new Mock<IOutput>();
			orgRepoMock = new Mock<IOrganizationServiceRepository>();
			factoryMock = new Mock<IAiBuilderApiClientFactory>();
			clientMock = new Mock<IAiBuilderApiClient>();
			serviceMock = new Mock<IOrganizationServiceAsync2>();

			orgRepoMock
				.Setup(r => r.GetCurrentConnectionAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(serviceMock.Object);

			factoryMock
				.Setup(f => f.CreateAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(clientMock.Object);
		}

		[TestMethod]
		public async Task ExecuteAsync_Publish_ShouldSucceed()
		{
			clientMock!
				.Setup(c => c.PublishModelAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var executor = new AiModelPublishCommandExecutor(
				outputMock!.Object,
				factoryMock!.Object);

			var result = await executor.ExecuteAsync(
				new AiModelPublishCommand { ModelId = "test-model-id" },
				CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			clientMock.Verify(c => c.PublishModelAsync("test-model-id", It.IsAny<CancellationToken>()), Times.Once);
			outputMock.Verify(o => o.WriteLine("Model published successfully!", ConsoleColor.Green), Times.Once);
			outputMock.Verify(o => o.WriteLine("Use 'ai model list' to confirm publication status.", ConsoleColor.Cyan), Times.Once);
		}

		[TestMethod]
		public async Task ExecuteAsync_DryRun_ShouldNotPublish()
		{
			var executor = new AiModelPublishCommandExecutor(
				outputMock!.Object,
				factoryMock!.Object);

			var result = await executor.ExecuteAsync(
				new AiModelPublishCommand { ModelId = "test-model-id", DryRun = true },
				CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			clientMock.Verify(c => c.PublishModelAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
		}
	}

	[TestClass]
	public class AiFormProcessorConfigureCommandExecutorTest
	{
		private Mock<IOutput>? outputMock;
		private Mock<IOrganizationServiceRepository>? orgRepoMock;
		private Mock<IAiBuilderApiClientFactory>? factoryMock;
		private Mock<IAiBuilderApiClient>? clientMock;
		private Mock<IOrganizationServiceAsync2>? serviceMock;

		[TestInitialize]
		public void Setup()
		{
			outputMock = new Mock<IOutput>();
			orgRepoMock = new Mock<IOrganizationServiceRepository>();
			factoryMock = new Mock<IAiBuilderApiClientFactory>();
			clientMock = new Mock<IAiBuilderApiClient>();
			serviceMock = new Mock<IOrganizationServiceAsync2>();

			orgRepoMock
				.Setup(r => r.GetCurrentConnectionAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(serviceMock.Object);

			factoryMock
				.Setup(f => f.CreateAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(clientMock.Object);
		}

		[TestMethod]
		public async Task ExecuteAsync_Configure_ShouldSucceed()
		{
			clientMock!
				.Setup(c => c.ConfigureFormProcessorAsync(
					It.IsAny<string>(),
					It.IsAny<string>(),
					It.IsAny<string[]>(),
					It.IsAny<string[]>(),
					It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var executor = new AiFormProcessorConfigureCommandExecutor(
				outputMock!.Object,
				factoryMock!.Object);

			var result = await executor.ExecuteAsync(
				new AiFormProcessorConfigureCommand
				{
					ModelId = "test-model-id",
					DocumentType = "Invoice",
					Fields = new[] { "TotalAmount", "InvoiceDate" },
					Tables = new[] { "LineItems" }
				},
				CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			clientMock.Verify(c => c.ConfigureFormProcessorAsync(
				"test-model-id",
				"Invoice",
				new[] { "TotalAmount", "InvoiceDate" },
				new[] { "LineItems" },
				It.IsAny<CancellationToken>()), Times.Once);
			outputMock.Verify(o => o.WriteLine("Form processor configured successfully!", ConsoleColor.Green), Times.Once);
			outputMock.Verify(o => o.WriteLine("Use 'ai model list' to verify the model status before publishing.", ConsoleColor.Cyan), Times.Once);
		}

		[TestMethod]
		public async Task ExecuteAsync_WithBlankField_ShouldFailBeforeCreatingClient()
		{
			var executor = new AiFormProcessorConfigureCommandExecutor(
				outputMock!.Object,
				factoryMock!.Object);

			var result = await executor.ExecuteAsync(
				new AiFormProcessorConfigureCommand
				{
					ModelId = "test-model-id",
					DocumentType = "Invoice",
					Fields = new[] { "TotalAmount", "" }
				},
				CancellationToken.None);

			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual("Form processor configuration error: --fields must not contain blank field names.", result.ErrorMessage);
			factoryMock!.Verify(f => f.CreateAsync(It.IsAny<CancellationToken>()), Times.Never);
			clientMock!.Verify(c => c.ConfigureFormProcessorAsync(
				It.IsAny<string>(),
				It.IsAny<string>(),
				It.IsAny<string[]>(),
				It.IsAny<string[]>(),
				It.IsAny<CancellationToken>()), Times.Never);
		}
	}
}
