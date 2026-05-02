using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.SkillPack
{
	[Command("skill", "pack", "list", HelpText = "Lists available skill packs from the catalog.")]
	[Alias("skill-pack", "list")]
	public class ListCommand
	{
		[Option("catalog", "c", Order = 1, DefaultValue = "conductor/skill-pack-catalog/packs.json", HelpText = "Path to the skill pack catalog JSON file.")]
		public string CatalogPath { get; set; } = "conductor/skill-pack-catalog/packs.json";

		[Option("query", "q", Order = 2, HelpText = "Filter packs by name, description, or tags.")]
		public string? Query { get; set; }

		[Option("tag", Order = 3, HelpText = "Filter packs by tag.")]
		public string? Tag { get; set; }
	}
}
