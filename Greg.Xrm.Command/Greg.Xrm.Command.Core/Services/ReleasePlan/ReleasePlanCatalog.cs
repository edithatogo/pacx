using System.Text.Json;

namespace Greg.Xrm.Command.Services.ReleasePlan
{
	public sealed class ReleasePlanCatalog
	{
		public IReadOnlyList<ReleasePlanFamily> Families { get; set; } = [];

		public static ReleasePlanCatalog Load(string path)
		{
			using var stream = File.OpenRead(path);
			var catalog = JsonSerializer.Deserialize<ReleasePlanCatalog>(stream, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			});

			return catalog ?? throw new InvalidDataException($"Unable to read release-plan catalog at <{path}>.");
		}
	}

	public sealed class ReleasePlanFamily
	{
		public string Id { get; set; } = string.Empty;

		public string Name { get; set; } = string.Empty;

		public string Url { get; set; } = string.Empty;

		public string Category { get; set; } = string.Empty;

		public string Summary { get; set; } = string.Empty;
	}
}
