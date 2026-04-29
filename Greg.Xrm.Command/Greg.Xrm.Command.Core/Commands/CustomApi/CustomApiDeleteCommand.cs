using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.CustomApi
{
	[Command("custom-api", "delete", HelpText = "Delete a Custom API from Dataverse.")]
	public class CustomApiDeleteCommand
	{
		[Option("name", "n", Order = 1, HelpText = "Unique name of the Custom API to delete.")]
		[Required]
		public string Name { get; set; } = "";
	}
}
