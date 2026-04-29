using System.Text.Json;

namespace Greg.Xrm.Command.Services.Mcp
{
	public sealed class FlowMcpCatalog
	{
		public IReadOnlyList<FlowMcpCatalogEntry> Flows { get; set; } = [];

		public static FlowMcpCatalog Load(string path)
		{
			using var stream = File.OpenRead(path);
			var catalog = JsonSerializer.Deserialize<FlowMcpCatalog>(stream, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			});

			return catalog ?? throw new InvalidDataException($"Unable to read flow MCP catalog at <{path}>.");
		}
	}

	public sealed class FlowMcpCatalogEntry
	{
		public string Name { get; set; } = string.Empty;
		public string Provider { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public string Kind { get; set; } = string.Empty;
		public string Summary { get; set; } = string.Empty;
		public string? HomePage { get; set; }
		public IReadOnlyList<string> Operations { get; set; } = [];
	}
}
