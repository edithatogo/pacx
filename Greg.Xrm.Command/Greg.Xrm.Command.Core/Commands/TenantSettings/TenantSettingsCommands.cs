using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.TenantSettings
{
	[Command("tenant", "settings", "list", HelpText = "List Power Platform tenant settings.")]
	public class TenantSettingsListCommand
	{
	}

	[Command("tenant", "settings", "set", HelpText = "Update Power Platform tenant settings.")]
	public class TenantSettingsSetCommand
	{
	}
}
