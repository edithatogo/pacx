using Greg.Xrm.Command.Model;
using Greg.Xrm.Command.Services;
using Greg.Xrm.Command.Services.AttributeDeletion;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Project;
using Greg.Xrm.Command.Services.Settings;
using Greg.Xrm.Command.Commands.Script;
using Microsoft.Extensions.Logging;

namespace Greg.Xrm.Command.Commands.Column
{
	[TestClass]
	public class DeleteCommandExecutorTests
	{
		[TestMethod]
		[TestCategory("Integration")]
		public void Integration_ForceDelete()
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

			var dependencyRepository = new Greg.Xrm.Command.Model.Dependency.Repository(Mock.Of<ILogger<Greg.Xrm.Command.Model.Dependency.Repository>>());
			var workflowRepository = new Greg.Xrm.Command.Model.Workflow.Repository();
			var processTriggerRepository = new Greg.Xrm.Command.Model.ProcessTrigger.Repository();
			var savedQueryRepository = new Greg.Xrm.Command.Model.SavedQuery.Repository();
			var userQueryRepository = new Greg.Xrm.Command.Model.UserQuery.Repository();
			var attributeDeletionService = new AttributeDeletionService(output, []);


			var command = new DeleteCommand
			{
				TableName = "ava_table1",
				SchemaName = "ava_category",
				Force = true,
			};

			var executor = new DeleteCommandExecutor(output, repository.Object, dependencyRepository, attributeDeletionService);

			var task = executor.ExecuteAsync(command, CancellationToken.None);

			task.Wait();
			Assert.IsNotNull(task);
			Assert.IsFalse(task.IsFaulted, task.Exception!.Message);

			var result = task.Result;
			Assert.IsNotNull(result);
			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);

			Assert.IsNotNull(output.ToString());
		}
	}
}
