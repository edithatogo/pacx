using Greg.Xrm.Command.Commands;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Commands.Connector
{
	[TestClass]
	public class ConnectorPrevalidationCommandExecutorTests : CommandExecutorTestBase
	{
		[TestMethod]
		public async Task Import_WithInvalidDefinition_ShouldFailBeforeConnecting()
		{
			var path = CreateTempConnectorFile("{ not-json");

			try
			{
				var executor = new ConnectorImportCommandExecutor(this.Output, this.OrganizationServiceRepositoryMock.Object);

				var result = await executor.ExecuteAsync(new ConnectorImportCommand
				{
					FilePath = path
				}, CancellationToken.None);

				Assert.IsFalse(result.IsSuccess);
				StringAssert.Contains(this.Output.ToString(), "Invalid JSON");
				this.OrganizationServiceRepositoryMock.Verify(r => r.GetCurrentConnectionAsync(It.IsAny<CancellationToken>()), Times.Never);
				this.OrganizationServiceMock.Verify(s => s.CreateAsync(It.IsAny<Entity>(), It.IsAny<CancellationToken>()), Times.Never);
			}
			finally
			{
				File.Delete(path);
			}
		}

		[TestMethod]
		public async Task Import_WithValidDefinition_ShouldCreateConnector()
		{
			var definition = ValidConnectorDefinition();
			var path = CreateTempConnectorFile(definition);
			var connectorId = Guid.NewGuid();

			this.OrganizationServiceMock
				.Setup(s => s.CreateAsync(
					It.Is<Entity>(e =>
						e.LogicalName == "connector"
						&& e.GetAttributeValue<string>("name") == Path.GetFileNameWithoutExtension(path)
						&& e.GetAttributeValue<string>("openapidefinition") == definition),
					It.IsAny<CancellationToken>()))
				.ReturnsAsync(connectorId);

			try
			{
				var executor = new ConnectorImportCommandExecutor(this.Output, this.OrganizationServiceRepositoryMock.Object);

				var result = await executor.ExecuteAsync(new ConnectorImportCommand
				{
					FilePath = path
				}, CancellationToken.None);

				Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
				this.OrganizationServiceMock.Verify(s => s.CreateAsync(It.IsAny<Entity>(), It.IsAny<CancellationToken>()), Times.Once);
			}
			finally
			{
				File.Delete(path);
			}
		}

		[TestMethod]
		public async Task Test_WithInvalidStoredDefinition_ShouldFailBeforeBackendCall()
		{
			var connector = new Entity("connector")
			{
				["name"] = "bad_connector",
				["openapidefinition"] = "{ not-json"
			};

			this.OrganizationServiceMock
				.Setup(s => s.RetrieveMultipleAsync(
					It.Is<QueryExpression>(q => q.EntityName == "connector"),
					It.IsAny<CancellationToken>()))
				.ReturnsAsync(new EntityCollection([connector]));

			var executor = new ConnectorTestCommandExecutor(
				this.Output,
				this.OrganizationServiceRepositoryMock.Object,
				this.TokenProviderMock.Object,
				this.HttpClientFactoryMock.Object);

			var result = await executor.ExecuteAsync(new ConnectorTestCommand
			{
				ConnectorName = "bad_connector",
				OperationName = "ListItems"
			}, CancellationToken.None);

			Assert.IsFalse(result.IsSuccess);
			StringAssert.Contains(this.Output.ToString(), "Invalid JSON");
			this.TokenProviderMock.Verify(t => t.GetTokenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
			this.HttpClientFactoryMock.Verify(f => f.CreateClient(It.IsAny<string>()), Times.Never);
		}

		private static string CreateTempConnectorFile(string content)
		{
			var path = Path.Combine(Path.GetTempPath(), $"connector-{Guid.NewGuid():N}.json");
			File.WriteAllText(path, content);
			return path;
		}

		private static string ValidConnectorDefinition()
		{
			return """
			{
			  "openapi": "3.0.1",
			  "info": {
obj"title": "Sample Connector",
obj"version": "1.0.0"
			  },
			  "paths": {
obj"/items": {
			      "get": {
obj"operationId": "ListItems",
obj"responses": {
			          "200": {
obj"description": "OK"
			          }
obj}
			      }
obj}
			  }
			}
			""";
		}
	}
}

