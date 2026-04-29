using System.Text.Json;

namespace Greg.Xrm.Command.Commands.Validate
{
	public interface ICatalogContractValidator
	{
		CatalogContractValidationResult Validate(string repoRoot);
	}

	public sealed class CatalogContractValidationResult
	{
		public CatalogContractValidationResult(IReadOnlyList<string> errors, IReadOnlyList<string> warnings, Exception? exception = null)
		{
			Errors = errors ?? throw new ArgumentNullException(nameof(errors));
			Warnings = warnings ?? throw new ArgumentNullException(nameof(warnings));
			Exception = exception;
		}

		public IReadOnlyList<string> Errors { get; }
		public IReadOnlyList<string> Warnings { get; }
		public Exception? Exception { get; }
		public bool IsValid => Errors.Count == 0;
	}

	public sealed class CatalogContractValidator : ICatalogContractValidator
	{
		private static readonly IReadOnlyList<CatalogShape> Shapes =
		[
			new CatalogShape(
				"conductor/tool-catalog/tools.json",
				"tools",
				["id", "name", "provider", "category", "kind", "summary"]),
			new CatalogShape(
				"conductor/source-catalog/sources.json",
				"sources",
				["name", "provider", "category", "kind", "summary"]),
			new CatalogShape(
				"conductor/skill-pack-catalog/skill-packs.json",
				"skillPacks",
				["id", "title", "summary"]),
			new CatalogShape(
				"conductor/flow-mcp-catalog/flows.json",
				"flows",
				["name", "provider", "category", "kind", "summary"])
		];

		public CatalogContractValidationResult Validate(string repoRoot)
		{
			try
			{
				var errors = new List<string>();
				var warnings = new List<string>();

				foreach (var shape in Shapes)
				{
					var path = Path.Combine(repoRoot, shape.RelativePath);
					if (!File.Exists(path))
					{
						errors.Add($"Missing catalog file: {shape.RelativePath}");
						continue;
					}

					using var document = JsonDocument.Parse(File.ReadAllText(path));
					ValidateShape(shape, document.RootElement, errors, warnings);
				}

				return new CatalogContractValidationResult(errors, warnings);
			}
			catch (JsonException ex)
			{
				return new CatalogContractValidationResult([$"Invalid catalog JSON: {ex.Message}"], Array.Empty<string>(), ex);
			}
			catch (Exception ex)
			{
				return new CatalogContractValidationResult([$"Catalog validation error: {ex.Message}"], Array.Empty<string>(), ex);
			}
		}

		private static void ValidateShape(CatalogShape shape, JsonElement root, List<string> errors, List<string> warnings)
		{
			if (root.ValueKind != JsonValueKind.Object)
			{
				errors.Add($"{shape.RelativePath} must be a JSON object.");
				return;
			}

			if (!root.TryGetProperty(shape.CollectionPropertyName, out var collection) || collection.ValueKind != JsonValueKind.Array)
			{
				errors.Add($"{shape.RelativePath} must define an array property named '{shape.CollectionPropertyName}'.");
				return;
			}

			if (!collection.EnumerateArray().Any())
			{
				errors.Add($"{shape.RelativePath} must contain at least one entry in '{shape.CollectionPropertyName}'.");
				return;
			}

			foreach (var entry in collection.EnumerateArray())
			{
				if (entry.ValueKind != JsonValueKind.Object)
				{
					errors.Add($"{shape.RelativePath} entries must be JSON objects.");
					continue;
				}

				foreach (var requiredProperty in shape.RequiredProperties)
				{
					if (!entry.TryGetProperty(requiredProperty, out var value) || value.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(value.GetString()))
					{
						errors.Add($"{shape.RelativePath} entry is missing required string property '{requiredProperty}'.");
					}
				}

				if (shape.CollectionPropertyName == "skillPacks")
				{
					if (!entry.TryGetProperty("docs", out var docs) || docs.ValueKind != JsonValueKind.Object ||
						!docs.TryGetProperty("guide", out var guide) || guide.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(guide.GetString()) ||
						!docs.TryGetProperty("recipe", out var recipe) || recipe.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(recipe.GetString()))
					{
						errors.Add($"{shape.RelativePath} entry must define docs.guide and docs.recipe.");
					}
					if (!entry.TryGetProperty("commands", out var commands) || commands.ValueKind != JsonValueKind.Array || !commands.EnumerateArray().Any())
					{
						errors.Add($"{shape.RelativePath} entry must define at least one command.");
					}
				}

				if (shape.CollectionPropertyName is "tools" or "sources")
				{
					var arrayProperty = shape.CollectionPropertyName == "tools" ? "capabilities" : "packages";
					if (!entry.TryGetProperty(arrayProperty, out var array) || array.ValueKind != JsonValueKind.Array || !array.EnumerateArray().Any())
					{
						errors.Add($"{shape.RelativePath} entry must define at least one '{arrayProperty}' value.");
					}
				}
			}
		}

		private sealed record CatalogShape(string RelativePath, string CollectionPropertyName, IReadOnlyList<string> RequiredProperties);
	}
}
