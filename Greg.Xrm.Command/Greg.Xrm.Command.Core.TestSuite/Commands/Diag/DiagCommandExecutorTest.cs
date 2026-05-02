using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Moq;

namespace Greg.Xrm.Command.Commands.Diag
{
	[TestClass]
	public class DiagCommandExecutorTest
	{
		[TestMethod]
		public void ExecuteShouldReturnSuccessWhenConnected()
		{
			var output = new OutputToMemory();

			var mockService = new Mock<IOrganizationServiceAsync2>();
			mockService
				.Setup(s => s.ExecuteAsync(It.IsAny<WhoAmIRequest>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new OrganizationResponse
				{
					Results = new ParameterCollection
					{
						["UserId"] = Guid.NewGuid(),
						["OrganizationId"] = Guid.NewGuid()
					}
				});

			var mockRepo = new Mock<IOrganizationServiceRepository>();
			mockRepo
				.Setup(r => r.GetCurrentConnectionAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(mockService.Object);

			var executor = new DiagCommandExecutor(output, mockRepo.Object);
			var command = new DiagCommand();
			var result = executor.ExecuteAsync(command, CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsTrue(result.IsSuccess);
		}

		[TestMethod]
		public void ExecuteShouldReturnFailureWhenDataverseUnreachable()
		{
			var output = new OutputToMemory();

			var mockRepo = new Mock<IOrganizationServiceRepository>();
			mockRepo
				.Setup(r => r.GetCurrentConnectionAsync(It.IsAny<CancellationToken>()))
				.ThrowsAsync(new InvalidOperationException("No connection configured"));

			var executor = new DiagCommandExecutor(output, mockRepo.Object);
			var command = new DiagCommand();
			var result = executor.ExecuteAsync(command, CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsFalse(result.IsSuccess);
		}

		[TestMethod]
		public void ExecuteWithVerboseShouldIncludeDetails()
		{
			var output = new OutputToMemory();

			var mockRepo = new Mock<IOrganizationServiceRepository>();
			mockRepo
				.Setup(r => r.GetCurrentConnectionAsync(It.IsAny<CancellationToken>()))
				.ThrowsAsync(new InvalidOperationException("No connection configured"));

			var executor = new DiagCommandExecutor(output, mockRepo.Object);
			var command = new DiagCommand { Verbose = true };
			var result = executor.ExecuteAsync(command, CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsFalse(result.IsSuccess);
			var outputText = output.ToString();
			Assert.IsTrue(outputText.Contains("Fail") || outputText.Contains("Warn"));
		}
	}
}
