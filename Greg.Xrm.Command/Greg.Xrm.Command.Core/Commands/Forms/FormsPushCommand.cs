using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Forms
{
	[Command("forms", "push", HelpText = "Push/publish a local Microsoft Forms authoring manifest to the online Microsoft Forms service.")]
	public class FormsPushCommand
	{
		[Option("file", "f", Order = 1, Required = true, HelpText = "Path to the local authoring manifest JSON file.")]
		public string FilePath { get; set; } = string.Empty;

		[Option("tenant", "t", Order = 2, HelpText = "Tenant ID or domain.")]
		public string? TenantId { get; set; }

		[Option("owner", "o", Order = 3, HelpText = "Owner user ID.")]
		public string? OwnerId { get; set; }

		[Option("owner-type", Order = 4, DefaultValue = "User", HelpText = "Owner type: User or Group.")]
		public string OwnerType { get; set; } = "User";
	}
}
