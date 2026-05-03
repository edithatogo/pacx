using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Greg.Xrm.Command.Commands.Connector
{
	public interface IConnectorSchemaValidator
	{
		ConnectorSchemaValidationResult Validate(string fileContent);
	}

	public sealed class ConnectorSchemaValidationResult
	{
		public ConnectorSchemaValidationResult(IReadOnlyList<string> errors, IReadOnlyList<string> warnings, Exception? exception = null)
		{
			ArgumentNullException.ThrowIfNull(errors);
			ArgumentNullException.ThrowIfNull(warnings);
			this.Errors = errors;
			this.Warnings = warnings;
			this.Exception = exception;
		}

		public IReadOnlyList<string> Errors { get; }

		public IReadOnlyList<string> Warnings { get; }

		public Exception? Exception { get; }

		public bool IsValid => this.Errors.Count == 0;
	}

	public sealed class ConnectorSchemaValidator : IConnectorSchemaValidator
	{
		public ConnectorSchemaValidationResult Validate(string fileContent)
		{
			try
			{
				using var document = JsonDocument.Parse(fileContent);
				var root = document.RootElement;
				var warnings = new List<string>();
				var errors = new List<string>();

				if (root.ValueKind != JsonValueKind.Object)
				{
					errors.Add("Connector definition must be a JSON object.");
				}
				else
				{
					ValidateVersionField(root, warnings, errors);
					ValidateInfoSection(root, warnings, errors);
					ValidatePathsSection(root, errors);
				}

				return new ConnectorSchemaValidationResult(errors, warnings);
			}
			catch (JsonException ex)
			{
				return new ConnectorSchemaValidationResult(
					["Invalid JSON: " + ex.Message],
					Array.Empty<string>(),
					ex);
			}
		}

		private static void ValidateVersionField(JsonElement root, List<string> warnings, List<string> errors)
		{
			var hasOpenApiVersion = root.TryGetProperty("openapi", out var openApiVersion);
			var hasSwaggerVersion = root.TryGetProperty("swagger", out var swaggerVersion);

			if (!hasOpenApiVersion && !hasSwaggerVersion)
			{
				warnings.Add("Missing 'openapi' or 'swagger' version field.");
				return;
			}

			if (hasOpenApiVersion && openApiVersion.ValueKind != JsonValueKind.String)
			{
				errors.Add("'openapi' must be a string.");
			}

			if (hasSwaggerVersion && swaggerVersion.ValueKind != JsonValueKind.String)
			{
				errors.Add("'swagger' must be a string.");
			}
		}

		private static void ValidateInfoSection(JsonElement root, List<string> warnings, List<string> errors)
		{
			if (!root.TryGetProperty("info", out var info))
			{
				warnings.Add("Missing 'info' section.");
				return;
			}

			if (info.ValueKind != JsonValueKind.Object)
			{
				errors.Add("'info' must be a JSON object.");
				return;
			}

			if (!info.TryGetProperty("title", out var title) || title.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(title.GetString()))
			{
				errors.Add("'info.title' is required and must be a non-empty string.");
			}
		}

		private static void ValidatePathsSection(JsonElement root, List<string> errors)
		{
			if (!root.TryGetProperty("paths", out var paths))
			{
				errors.Add("Missing 'paths' section.");
				return;
			}

			if (paths.ValueKind != JsonValueKind.Object)
			{
				errors.Add("'paths' must be a JSON object.");
				return;
			}

			if (!paths.EnumerateObject().MoveNext())
			{
				errors.Add("'paths' must contain at least one operation.");
			}
		}
	}
}
