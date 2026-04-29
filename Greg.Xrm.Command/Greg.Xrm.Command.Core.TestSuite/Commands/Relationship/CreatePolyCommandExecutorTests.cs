using Greg.Xrm.Command.Services;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Project;
using Greg.Xrm.Command.Services.Settings;
using Greg.Xrm.Command.Commands.Script;
using Microsoft.Extensions.Logging;

namespace Greg.Xrm.Command.Commands.Relationship
{
	[TestClass]
	public class CreatePolyCommandExecutorTests
	{
		[TestMethod]
		[TestCategory("Integration")]
		public void Integration_CreateGlobalOptionSetField()
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


			var command = new CreatePolyCommand
			{
				ChildTable = "ava_fundedemployee",
				Parents = "ava_solutionarea,ava_practice,ava_clientbusinessgroup,ava_crossstructure",
				LookupAttributeDisplayName = "Funded By",
				RelationshipNameSuffix = "poly",
				SolutionName = "cop_solutioning"
			};


			var executor = new CreatePolyCommandExecutor(output, repository.Object);

			var task = executor.ExecuteAsync(command, CancellationToken.None);
			Assert.IsNotNull(task);

			task.Wait();

			Assert.IsFalse(task.IsFaulted, task.Exception!.Message);

			var result = task.Result;

			Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
			Assert.IsNull(result.Exception, result.Exception?.Message);
		}
	}
}
