using System.Diagnostics;
using Greg.Xrm.Command.Commands.Table.ExportMetadata;
using Greg.Xrm.Command.Services;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Project;
using Greg.Xrm.Command.Services.Settings;
using Greg.Xrm.Command.Commands.Script;
using Microsoft.Extensions.Logging;

namespace Greg.Xrm.Command.Commands.Table
{
	[TestClass]
	public class ExportMetadataCommandTest
	{
		[TestMethod]
		[TestCategory("Integration")]
		public void Integration_ExecuteExportExcel()
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

			var exportMetadataStrategyFactory = new ExportMetadataStrategyFactory(output);

			var command = new ExportMetadataCommand
			{
				Format = ExportMetadataFormat.Excel,
				OutputFilePath = @"C:\temp\",
				TableSchemaName = "ava_practice",
				AutoOpenFile = true,
			};


			var executor = new ExportMetadataCommandExecutor(output, repository.Object, exportMetadataStrategyFactory);

			executor.ExecuteAsync(command, CancellationToken.None).Wait();


			var outputText = output.ToString();

			Debug.WriteLine(outputText);

			Assert.IsNotNull(outputText);
		}
	}
}
