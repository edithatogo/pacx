using Greg.Xrm.Command.Commands.Connector;

namespace Greg.Xrm.Command.Commands.Connector
{
	[TestClass]
	public class ConnectorSchemaValidationPropertyTests
	{
		[TestMethod]
		public void Validator_ShouldNotThrowForJsonObjectsWithRandomPathNames()
		{
			var validator = new ConnectorSchemaValidator();

			for (var i = 0; i < 100; i++)
			{
				var path = "/items/" + i.ToString(System.Globalization.CultureInfo.InvariantCulture);
				var json = $$"""
				{
				  "openapi": "3.0.1",
				  "info": {
				    "title": "Generated connector {{i}}"
				  },
				  "paths": {
				    "{{path}}": {
				      "get": {
				        "operationId": "getItem{{i}}",
				        "responses": {
				          "200": {
				            "description": "OK"
				          }
				        }
				      }
				    }
				  }
				}
				""";

				var result = validator.Validate(json);

				Assert.IsTrue(result.IsValid, string.Join(Environment.NewLine, result.Errors));
			}
		}
	}
}
