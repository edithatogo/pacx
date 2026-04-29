using Greg.Xrm.Command.Commands.Column.Builders;
using Greg.Xrm.Command.Services;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.OptionSet;
using Greg.Xrm.Command.Services.Project;
using Greg.Xrm.Command.Services.Settings;
using Greg.Xrm.Command.Commands.Script;
using Microsoft.Extensions.Logging;

namespace Greg.Xrm.Command.Commands.Column
{
	[TestClass]
	public class CreateCommandExecutorTests
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
			var parser = new OptionSetParser();

			var attributeMetadataBuilderFactory = new AttributeMetadataBuilderFactory(output, parser);

			var command = new CreateCommand
			{
				EntityName = "ava_pippo",
				AttributeType = SupportedAttributeType.Picklist,
				SchemaName = "ava_test",
				DisplayName = "Test",
				GlobalOptionSetName = "ava_riccardo", //"emailserverprofile_authenticationprotocol",
				SolutionName = "master"
			};


			var executor = new CreateCommandExecutor(output, repository.Object, attributeMetadataBuilderFactory);

			executor.ExecuteAsync(command, CancellationToken.None).Wait();

			Assert.IsNotNull(output.ToString());
		}
	}
}
