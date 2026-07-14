using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.CustomApi
{
	[Command("custom-api", "describe", HelpText = "Describe a Custom API and its request and response parameters.")]
	public class CustomApiDescribeCommand
	{
		[Option("name", "n", Order = 1, HelpText = "Unique name of the Custom API to describe.")]
		[Required]
		public string Name { get; set; } = "";
	}
}
