using Greg.Xrm.Command.Services;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Project;
using Greg.Xrm.Command.Services.Settings;
using Greg.Xrm.Command.Commands.Script;
using Microsoft.Extensions.Logging;

namespace Greg.Xrm.Command.Commands.UnifiedRouting
{
	[TestClass]
	public class GetQueueStatusCommandExecutorTest
	{
		[TestMethod]
		[TestCategory("Integration")]
		public async Task TestQuery()
		{
			var queue = "QUEUENAME";
			var hasDataverseEnvironment =
				!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DATAVERSE_URL"))
				&& !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DATAVERSE_CLIENT_ID"))
				&& !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DATAVERSE_CLIENT_SECRET"));

			if (!hasDataverseEnvironment)
			{
				Assert.Inconclusive("DATAVERSE_URL, DATAVERSE_CLIENT_ID, and DATAVERSE_CLIENT_SECRET must be set to run this integration test.");
			}

			using var crm = TestConfiguration.CreateServiceClient();
			var output = new OutputToConsole();
			var repository = new Mock<IOrganizationServiceRepository>();
			repository
				.Setup(r => r.GetCurrentConnectionAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(crm);

			var executor = new GetQueueStatusCommandExecutor(output, repository.Object);

			var result = await executor.ExecuteAsync(new GetQueueStatusCommand
			{
				Queue = queue,
				DateTimeFilter = "28/11/2023 11:00"
			}, new CancellationToken());

			Assert.IsNotNull(result);
			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
		}
	}
}
