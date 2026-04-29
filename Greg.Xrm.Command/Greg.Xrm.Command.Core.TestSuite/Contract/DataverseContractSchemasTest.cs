using System.Text.Json;

namespace Greg.Xrm.Command.Contract
{
	[TestClass]
	public class DataverseContractSchemasTest
	{
		[DataTestMethod]
		[DataRow("customapi-list")]
		[DataRow("catalog-entry")]
		[DataRow("elastic-table")]
		public void DataverseContractSamples_ShouldMatchPinnedSchema(string contractName)
		{
			using var schema = OpenJson($"Contract/Schemas/{contractName}.schema.json");
			using var sample = OpenJson($"Contract/Samples/{contractName}.sample.json");

			var required = schema.RootElement.GetProperty("required").EnumerateArray().Select(item => item.GetString()!).ToArray();
			foreach (var propertyName in required)
			{
				Assert.IsTrue(sample.RootElement.TryGetProperty(propertyName, out _), $"Missing required property '{propertyName}'.");
			}

			foreach (var property in schema.RootElement.GetProperty("properties").EnumerateObject())
			{
				if (!sample.RootElement.TryGetProperty(property.Name, out var value))
				{
					continue;
				}

				var expectedType = property.Value.GetProperty("type").GetString();
				Assert.AreEqual(expectedType, ToSchemaType(value), $"Property '{property.Name}' has an unexpected JSON type.");
			}
		}

		private static JsonDocument OpenJson(string relativePath)
		{
			var path = Path.Combine(AppContext.BaseDirectory, relativePath.Replace('/', Path.DirectorySeparatorChar));
			return JsonDocument.Parse(File.ReadAllText(path));
		}

		private static string ToSchemaType(JsonElement value)
		{
			return value.ValueKind switch
			{
				JsonValueKind.Object => "object",
				JsonValueKind.Array => "array",
				JsonValueKind.String => "string",
				JsonValueKind.Number => "number",
				JsonValueKind.True or JsonValueKind.False => "boolean",
				JsonValueKind.Null => "null",
				_ => "unknown"
			};
		}
	}
}
