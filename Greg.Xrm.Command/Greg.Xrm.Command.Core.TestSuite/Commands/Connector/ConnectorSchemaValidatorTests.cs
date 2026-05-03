using System.Linq;

namespace Greg.Xrm.Command.Commands.Connector
{
	[TestClass]
	public class ConnectorSchemaValidatorTests
	{
		[TestMethod]
		public void Validate_WithValidOpenApiDefinition_ShouldReturnNoIssues()
		{
			var validator = new ConnectorSchemaValidator();

			var result = validator.Validate("""
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

			Assert.IsTrue(result.IsValid);
			Assert.AreEqual(0, result.Errors.Count);
			Assert.AreEqual(0, result.Warnings.Count);
			Assert.IsNull(result.Exception);
		}

		[TestMethod]
		public void Validate_WithMissingOptionalOpenApiMetadata_ShouldReturnWarnings()
		{
			var validator = new ConnectorSchemaValidator();

			var result = validator.Validate("""
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

			Assert.IsTrue(result.IsValid);
			Assert.AreEqual(0, result.Errors.Count);
			Assert.AreEqual(2, result.Warnings.Count);
			Assert.IsTrue(result.Warnings.Contains("Missing 'openapi' or 'swagger' version field."));
			Assert.IsTrue(result.Warnings.Contains("Missing 'info' section."));
		}

		[TestMethod]
		public void Validate_WithInvalidStructure_ShouldReturnErrors()
		{
			var validator = new ConnectorSchemaValidator();

			var result = validator.Validate("""
			{
				"openapi": 3,
				"info": {
			"title": ""
				},
				"paths": {}
			}
			""");

			Assert.IsFalse(result.IsValid);
			Assert.IsTrue(result.Errors.Contains("'openapi' must be a string."));
			Assert.IsTrue(result.Errors.Contains("'info.title' is required and must be a non-empty string."));
			Assert.IsTrue(result.Errors.Contains("'paths' must contain at least one operation."));
			Assert.AreEqual(0, result.Warnings.Count);
		}

		[TestMethod]
		public void Validate_WithInvalidJson_ShouldReturnParseError()
		{
			var validator = new ConnectorSchemaValidator();

			var result = validator.Validate("{ not-json");

			Assert.IsFalse(result.IsValid);
			Assert.AreEqual(1, result.Errors.Count);
			StringAssert.StartsWith(result.Errors[0], "Invalid JSON:");
			Assert.IsNotNull(result.Exception);
		}
	}
}

