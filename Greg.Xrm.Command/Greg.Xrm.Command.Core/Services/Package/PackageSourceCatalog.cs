using System.Text.Json;

namespace Greg.Xrm.Command.Services.Package
{
	public sealed class PackageSourceCatalog
	{
		public IReadOnlyList<PackageSourceCatalogEntry> Sources { get; set; } = [];

		public static PackageSourceCatalog Load(string path)
		{
			using var stream = File.OpenRead(path);
			var catalog = JsonSerializer.Deserialize<PackageSourceCatalog>(stream, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			});

			return catalog ?? throw new InvalidDataException($"Unable to read package source catalog at <{path}>.");
		}
	}

	public sealed class PackageSourceCatalogEntry
	{
		public string Name { get; set; } = string.Empty;

		public string Provider { get; set; } = string.Empty;

		public string Category { get; set; } = string.Empty;

		public string Kind { get; set; } = string.Empty;

		public string Summary { get; set; } = string.Empty;

		public string? HomePage { get; set; }

		public IReadOnlyList<string> Packages { get; set; } = [];

		public IReadOnlyList<string> Capabilities { get; set; } = [];
	}
}
