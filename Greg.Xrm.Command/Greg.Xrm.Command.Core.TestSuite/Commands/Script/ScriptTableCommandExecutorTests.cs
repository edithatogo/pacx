using Greg.Xrm.Command.Commands.Script.MetadataExtractor;
using Greg.Xrm.Command.Commands.Script.Models;
using Greg.Xrm.Command.Commands.Script.Service;
using Greg.Xrm.Command.Services.Connection;
using Microsoft.PowerPlatform.Dataverse.Client;
using Greg.Xrm.Command.TestSuite;

namespace Greg.Xrm.Command.Commands.Script
{
	[TestClass]
	public class ScriptTableCommandExecutorTests
	{
		/// <summary>
		/// Unit test with mocked dependencies
		/// </summary>
		[TestMethod]
		public async Task ExecuteAsync_WithValidTable_ShouldExtractMetadata()
		{
			// Arrange
			var tableName = "incident";
			var customPrefixes = "ava_";
			var scriptFileName = "incident_datamodel.ps1";
			var stateFileName = "incident_state-fields.csv";

			var mockOutput = new Mock<IOutput>();
			var mockMetadataExtractor = new Mock<IScriptMetadataExtractor>();
			var outputDir = TestTempPath.CreateDirectory("script_extract");

			// Setup mock entity metadata
			var mockEntity = new Extractor_EntityMetadata
			{
				SchemaName = tableName,
				DisplayName = "Incident",
				PluralName = "Incidents"
			};

			mockMetadataExtractor
				.Setup(x => x.GetTableAsync(tableName, It.IsAny<List<string>>()))
				.ReturnsAsync(mockEntity);
			mockMetadataExtractor
				.Setup(x => x.GetRelationshipsAsync(It.IsAny<List<string>>(), It.IsAny<List<Extractor_EntityMetadata>>()))
				.ReturnsAsync(new List<Extractor_RelationshipMetadata>());
			mockMetadataExtractor
				.Setup(x => x.GeneratePacxScript(It.IsAny<List<Extractor_EntityMetadata>>(), It.IsAny<List<Extractor_RelationshipMetadata>>(), It.IsAny<List<string>>()))
				.Returns("-- generated script --");
			mockMetadataExtractor
				.Setup(x => x.GetOptionSetsAsync(It.IsAny<List<string>?>()))
				.ReturnsAsync(new List<Extractor_OptionSetMetadata>());
			mockMetadataExtractor
				.Setup(x => x.GenerateStateFieldsCSV(It.IsAny<List<Extractor_OptionSetMetadata>>(), It.IsAny<string>()))
				.Callback<List<Extractor_OptionSetMetadata>, string>((optionSets, path) =>
				{
					File.WriteAllText(path, "EntityName,FieldName,OptionValue,OptionLabel,SourceType\n");
				})
				.Returns(Task.CompletedTask);

			var extractionService = new ScriptExtractionService(
				mockOutput.Object,
				mockMetadataExtractor.Object);

			var executor = new ScriptTableCommandExecutor(extractionService);

			var command = new ScriptTableCommand
			{
				TableName = tableName,
				CustomPrefixes = customPrefixes,
				OutputDir = outputDir,
				PacxScriptName = scriptFileName,
				StateFieldsDefinitionName = stateFileName,
				WithStateFieldsDefinition = true
			};

			// Act
			var result = await executor.ExecuteAsync(command, CancellationToken.None);

			// Assert
			mockMetadataExtractor.Verify(
				x => x.GetTableAsync(tableName, It.IsAny<List<string>>()),
				Times.Once);
			Assert.IsTrue(File.Exists(Path.Combine(outputDir, scriptFileName)));
			Assert.IsTrue(File.Exists(Path.Combine(outputDir, stateFileName)));
		}

		/// <summary>
		/// Unit test for table not found scenario
		/// </summary>
		[TestMethod]
		public async Task ExecuteAsync_WithInvalidTable_ShouldFail()
		{
			// Arrange
			var tableName = "nonexistent_table";

			var mockOutput = new Mock<IOutput>();
			var mockMetadataExtractor = new Mock<IScriptMetadataExtractor>();
			mockMetadataExtractor
				.Setup(x => x.GetRelationshipsAsync(It.IsAny<List<string>>(), It.IsAny<List<Extractor_EntityMetadata>>()))
				.ReturnsAsync(new List<Extractor_RelationshipMetadata>());

			mockMetadataExtractor
				.Setup(x => x.GetTableAsync(tableName, It.IsAny<List<string>>()))
				.ReturnsAsync((Extractor_EntityMetadata?)null);

			var extractionService = new ScriptExtractionService(
				mockOutput.Object,
				mockMetadataExtractor.Object);

			var executor = new ScriptTableCommandExecutor(extractionService);

			var command = new ScriptTableCommand
			{
				TableName = tableName,
				CustomPrefixes = "ava_",
				OutputDir = "C:/output"
			};

			// Act
			var result = await executor.ExecuteAsync(command, CancellationToken.None);

			// Assert
			Assert.IsFalse(result.IsSuccess);
			mockOutput.Verify(
				x => x.WriteLine(
					It.Is<string>(s => s.Contains("Table not found")),
					It.IsAny<ConsoleColor>()),
				Times.Once);
		}

		/// <summary>
		/// Integration test - requires real Dataverse connection
		/// Set [Ignore] attribute to skip during normal test runs
		/// Remove [Ignore] when you want to debug locally
		/// </summary>
		[TestMethod]
		[TestCategory("Integration")]
		public async Task ExecuteAsync_Integration_WithRealConnection()
		{
			// Arrange
			var tableName = "";
			var customPrefixes = "";
			var outputDir = TestConfiguration.GetTestOutputDirectory();
			var scriptFileName = "";
			var stateFileName = "";

			if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DATAVERSE_URL"))
				|| string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DATAVERSE_CLIENT_ID"))
				|| string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DATAVERSE_CLIENT_SECRET")))
			{
				Assert.Inconclusive("DATAVERSE_URL, DATAVERSE_CLIENT_ID, and DATAVERSE_CLIENT_SECRET must be set to run this integration test.");
			}

			var serviceClient = TestConfiguration.CreateServiceClient();

			Assert.IsTrue(serviceClient.IsReady, $"Failed to connect to Dataverse: {serviceClient.LastError}");

			var output = new OutputToMemory(); // Or use OutputToConsole for debugging
			var mockServiceRepository = new Mock<IOrganizationServiceRepository>();
			mockServiceRepository
				.Setup(x => x.GetCurrentConnectionAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(serviceClient);

			var scriptBuilder = new ScriptBuilder();
			var metadataExtractor = new ScriptMetadataExtractor(
				mockServiceRepository.Object,
				scriptBuilder);

			var extractionService = new ScriptExtractionService(
				output,
				metadataExtractor);

			var executor = new ScriptTableCommandExecutor(extractionService);

			var command = new ScriptTableCommand
			{
				TableName = tableName,
				CustomPrefixes = customPrefixes,
				OutputDir = outputDir,
				PacxScriptName = scriptFileName,
				StateFieldsDefinitionName = stateFileName,
				WithStateFieldsDefinition = true
			};

			// Act
			var result = await executor.ExecuteAsync(command, CancellationToken.None);

			// Assert
			Assert.IsTrue(result.IsSuccess, $"Command failed: {result.ErrorMessage}");

			// Verify output files were created
			var scriptFilePath = Path.Combine(outputDir, scriptFileName);
			Assert.IsTrue(File.Exists(scriptFilePath), $"Script file not created at {scriptFilePath}");

			var stateFilePath = Path.Combine(outputDir, stateFileName);
			Assert.IsTrue(File.Exists(stateFilePath), $"State file not created at {stateFilePath}");

			// Clean up
			serviceClient.Dispose();
		}
	}
}
