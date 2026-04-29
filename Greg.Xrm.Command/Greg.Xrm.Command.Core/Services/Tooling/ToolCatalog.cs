using System.Text.Json;

namespace Greg.Xrm.Command.Services.Tooling
{
	public sealed class ToolCatalog
	{
		public IReadOnlyList<ToolCatalogEntry> Tools { get; set; } = [];

		public static ToolCatalog Load(string path)
		{
			using var stream = File.OpenRead(path);
			var catalog = JsonSerializer.Deserialize<ToolCatalog>(stream, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			});

			return catalog ?? throw new InvalidDataException($"Unable to read tool catalog at <{path}>.");
		}
	}

	public sealed class ToolCatalogEntry
	{
		public string Id { get; set; } = string.Empty;

		public string Name { get; set; } = string.Empty;

		public string Provider { get; set; } = string.Empty;

		public string Category { get; set; } = string.Empty;

		public string Kind { get; set; } = string.Empty;

		public string Summary { get; set; } = string.Empty;

		public string? HomePage { get; set; }

		public IReadOnlyList<string> Capabilities { get; set; } = [];
	}
}
