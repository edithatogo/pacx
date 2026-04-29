using Greg.Xrm.Command.Services.AiBuilder;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Core.TestSuite.Services.AiBuilder
{
	[TestClass]
	public class AiBuilderServiceTests
	{
		[TestMethod]
		public async Task ListModelsAsync_ShouldReturnStructuredSuccess()
		{
			var client = new Mock<IAiBuilderApiClient>();
			client
				.Setup(c => c.ListModelsAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(new[]
				{
					new AiModelInfo { Id = "model-1", Name = "Invoice", Status = "Published" }
				});

			var factory = CreateFactory(client);
			var service = new AiBuilderService(factory.Object);

			var result = await service.ListModelsAsync(CancellationToken.None);

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			Assert.AreEqual(1, result.Value?.Count);
			Assert.AreEqual("Invoice", result.Value?[0].Name);
		}

		[TestMethod]
		public async Task TrainModelWithRetryAsync_WhenClientFails_ShouldReturnStructuredError()
		{
			var client = new Mock<IAiBuilderApiClient>();
			client
				.Setup(c => c.TrainModelAsync(
					It.IsAny<string>(),
					It.IsAny<bool>(),
					It.IsAny<TimeSpan>(),
					It.IsAny<TimeSpan>(),
					It.IsAny<CancellationToken>()))
				.ThrowsAsync(new InvalidOperationException("backend unavailable"));

			var factory = CreateFactory(client);
			var service = new AiBuilderService(factory.Object);

			var result = await service.TrainModelWithRetryAsync(
				"model-1",
				wait: true,
				TimeSpan.FromSeconds(5),
				TimeSpan.FromMinutes(10),
				CancellationToken.None);

			Assert.IsFalse(result.IsSuccess);
			Assert.AreEqual("AI model train error: backend unavailable", result.ErrorMessage);
			Assert.IsInstanceOfType<InvalidOperationException>(result.Exception);
		}

		private static Mock<IAiBuilderApiClientFactory> CreateFactory(Mock<IAiBuilderApiClient> client)
		{
			var factory = new Mock<IAiBuilderApiClientFactory>();
			factory
				.Setup(f => f.CreateAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(client.Object);
			return factory;
		}
	}
}
