using System.ComponentModel.DataAnnotations;
using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.SkillPack
{
	[Command("skill", "pack", "install", HelpText = "Installs a skill pack from the catalog.")]
	[Alias("skill-pack", "install")]
	public class InstallCommand : IValidatableObject
	{
		[Option("id", "i", Order = 1, Required = true, HelpText = "The ID of the skill pack to install.")]
		[Required]
		public string Id { get; set; } = string.Empty;

		[Option("catalog", "c", Order = 2, DefaultValue = "conductor/skill-pack-catalog/packs.json", HelpText = "Path to the skill pack catalog JSON file.")]
		public string CatalogPath { get; set; } = "conductor/skill-pack-catalog/packs.json";

		[Option("dry-run", "d", Order = 3, HelpText = "Preview what would be installed without making changes.")]
		public bool DryRun { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (string.IsNullOrWhiteSpace(Id))
			{
				yield return new ValidationResult("The skill pack ID is required.", [nameof(Id)]);
			}
		}
	}
}
