using Greg.Xrm.Command.Services.Output;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Commands.Connector
{
	[TestClass]
	public class ConnectorValidateCommandExecutorTests
	{
		[TestMethod]
		public async Task ExecuteAsync_WithValidDefinition_ShouldSucceed()
		{
			var path = CreateTempConnectorFile("""
			{
			  "openapi": "3.0.1",
			  "info": {
			    "title": "Sample Connector",
			    "version": "1.0.0"
			  },
			  "paths": {
			    "/items": {
			      "get": {
			        "operationId": "ListItems",
			        "responses": {
			          "200": {
			            "description": "OK"
			          }
			        }
			      }
			    }
			  }
			}
			""");

			try
			{
				var output = new OutputToMemory();
				var executor = new ConnectorValidateCommandExecutor(output);

				var result = await executor.ExecuteAsync(new ConnectorValidateCommand
				{
					FilePath = path
				}, CancellationToken.None);

				Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
				StringAssert.Contains(output.ToString(), "Validation passed. No issues found.");
			}
			finally
			{
				File.Delete(path);
			}
		}

		[TestMethod]
		public async Task ExecuteAsync_WithWarningsOnly_ShouldSucceedWhenNotStrict()
		{
			var path = CreateTempConnectorFile("""
			{
			  "paths": {
			    "/items": {
			      "get": {
			        "operationId": "ListItems"
			      }
			    }
			  }
			}
			""");

			try
			{
				var output = new OutputToMemory();
				var executor = new ConnectorValidateCommandExecutor(output);

				var result = await executor.ExecuteAsync(new ConnectorValidateCommand
				{
					FilePath = path,
					Strict = false
				}, CancellationToken.None);

				Assert.IsTrue(result.IsSuccess, result.ErrorMessage);
				StringAssert.Contains(output.ToString(), "WARNING: Missing 'openapi' or 'swagger' version field.");
				StringAssert.Contains(output.ToString(), "WARNING: Missing 'info' section.");
			}
			finally
			{
				File.Delete(path);
			}
		}

		[TestMethod]
		public async Task ExecuteAsync_WithWarningsOnlyAndStrictMode_ShouldFail()
		{
			var path = CreateTempConnectorFile("""
			{
			  "paths": {
			    "/items": {
			      "get": {
			        "operationId": "ListItems"
			      }
			    }
			  }
			}
			""");

			try
			{
				var output = new OutputToMemory();
				var executor = new ConnectorValidateCommandExecutor(output);

				var result = await executor.ExecuteAsync(new ConnectorValidateCommand
				{
					FilePath = path,
					Strict = true
				}, CancellationToken.None);

				Assert.IsFalse(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "Found 2 issue(s).");
			}
			finally
			{
				File.Delete(path);
			}
		}

		[TestMethod]
		public async Task ExecuteAsync_WithInvalidJson_ShouldFail()
		{
			var path = CreateTempConnectorFile("{ not-json");

			try
			{
				var output = new OutputToMemory();
				var executor = new ConnectorValidateCommandExecutor(output);

				var result = await executor.ExecuteAsync(new ConnectorValidateCommand
				{
					FilePath = path
				}, CancellationToken.None);

				Assert.IsFalse(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "Invalid JSON");
			}
			finally
			{
				File.Delete(path);
			}
		}

		[TestMethod]
		public async Task ExecuteAsync_WithSchemaFile_ShouldApplyOrganizationRequiredProperties()
		{
			var definitionPath = CreateTempConnectorFile("""
			{
			  "openapi": "3.0.1",
			  "info": {
			    "title": "Sample Connector"
			  },
			  "paths": {
			    "/items": {
			      "get": {
			        "operationId": "ListItems"
			      }
			    }
			  }
			}
			""");
			var schemaPath = CreateTempConnectorFile("""
			{
			  "required": [
			    "x-ms-connector-metadata"
			  ]
			}
			""");

			try
			{
				var output = new OutputToMemory();
				var executor = new ConnectorValidateCommandExecutor(output);

				var result = await executor.ExecuteAsync(new ConnectorValidateCommand
				{
					FilePath = definitionPath,
					SchemaFilePath = schemaPath
				}, CancellationToken.None);

				Assert.IsFalse(result.IsSuccess);
				Assert.IsInstanceOfType<ValidationException>(result.Exception);
				StringAssert.Contains(output.ToString(), "Organization schema requires root property 'x-ms-connector-metadata'.");
			}
			finally
			{
				File.Delete(definitionPath);
				File.Delete(schemaPath);
			}
		}

		private static string CreateTempConnectorFile(string content)
		{
			var path = Path.Combine(Path.GetTempPath(), $"connector-{Guid.NewGuid():N}.json");
			File.WriteAllText(path, content);
			return path;
		}
	}
}
