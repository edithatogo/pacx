using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Forms
{
	[Command("forms", "response", "get", HelpText = "Get a single response from a Microsoft Form.")]
	public class FormsResponseGetCommand : FormsFormCommandBase
	{
		[Option("response-id", "r", Order = 5, Required = true, HelpText = "The response ID to retrieve.")]
		public int ResponseId { get; set; }
	}
}
