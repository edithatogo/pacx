using System.Text.Json;

namespace Greg.Xrm.Command.Services.Tooling
{
	public sealed class SkillPackCatalog
	{
		public IReadOnlyList<SkillPackEntry> Packs { get; set; } = [];

		public static SkillPackCatalog Load(string path)
		{
			using var stream = File.OpenRead(path);
			var catalog = JsonSerializer.Deserialize<SkillPackCatalog>(stream, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			});

			return catalog ?? throw new InvalidDataException($"Unable to read skill pack catalog at <{path}>.");
		}
	}

	public sealed class SkillPackEntry
	{
		public string Id { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Version { get; set; } = string.Empty;
		public string Author { get; set; } = string.Empty;
		public IReadOnlyList<string> Capabilities { get; set; } = [];
		public IReadOnlyList<string> Dependencies { get; set; } = [];
		public IReadOnlyList<string> Tags { get; set; } = [];
	}
}
