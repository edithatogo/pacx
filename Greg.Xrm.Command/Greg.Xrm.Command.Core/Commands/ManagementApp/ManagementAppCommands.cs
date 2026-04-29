using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.ManagementApp
{
	[Command("managementapp", "list", HelpText = "List Power Platform management applications.")]
	public class ManagementAppListCommand
	{
	}

	[Command("managementapp", "add", HelpText = "Add a Power Platform management application.")]
	public class ManagementAppAddCommand
	{
		[Option("application-id", "i", Order = 1, Required = true, HelpText = "The application (client) ID of the Azure AD app registration.")]
		public string ApplicationId { get; set; } = string.Empty;
	}
}
