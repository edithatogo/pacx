using Greg.Xrm.Command.Services;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Project;
using Greg.Xrm.Command.Services.Settings;
using Greg.Xrm.Command.Commands.Script;
using Microsoft.Extensions.Logging;

namespace Greg.Xrm.Command.Commands.Auth
{
	[TestClass]
	public class CreateCommandExecutorTests
	{
		[TestMethod]
		[TestCategory("Integration")]
		public void Integration_Execute01()
		{
			if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DATAVERSE_URL"))
				|| string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DATAVERSE_CLIENT_ID"))
				|| string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DATAVERSE_CLIENT_SECRET")))
			{
				Assert.Inconclusive("DATAVERSE_URL, DATAVERSE_CLIENT_ID, and DATAVERSE_CLIENT_SECRET must be set to run this integration test.");
			}

			using var crm = TestConfiguration.CreateServiceClient();
			var output = new OutputToMemory();
			var repository = new Mock<IOrganizationServiceRepository>();
			repository.Setup(r => r.GetCurrentConnectionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(crm);
			var executor = new CreateCommandExecutor(repository.Object, output);

			var command = new CreateCommand
			{
				Name = "test",
				ConnectionString = ""
			};

			var task = executor.ExecuteAsync(command, CancellationToken.None);

			task.Wait();

			Assert.IsNotNull(output.ToString());
		}
	}
}
