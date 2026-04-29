using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Auth
{
	[Command("auth", "ping", HelpText = "Tests the connection to the Dataverse environment currently selected")]
	[Alias("ping")]
	public class PingCommand
	{
	}

	[Command("auth", "whoami", HelpText = "Shows the current Dataverse user for the selected authentication profile.")]
	public class WhoAmICommand
	{
	}
}
