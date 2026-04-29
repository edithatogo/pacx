using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Gateway
{
	[Command("gateway", "list", HelpText = "List on-premises data gateways accessible to the current user.")]
	public class GatewayListCommand
	{
	}

	[Command("gateway", "get", HelpText = "Get details of a specific on-premises data gateway.")]
	public class GatewayGetCommand
	{
		[Option("id", "i", Order = 1, Required = true, HelpText = "The ID of the gateway to retrieve.")]
		public string GatewayId { get; set; } = string.Empty;
	}
}
