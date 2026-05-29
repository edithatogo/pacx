using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Forms
{
	[Command("forms", "push", HelpText = "Push/publish a local Microsoft Forms authoring manifest to the online Microsoft Forms service.")]
	public class FormsPushCommand : FormsFormCommandBase
	{
		[Option("file", "f", Order = 1, Required = true, HelpText = "Path to the local authoring manifest JSON file.")]
		public string FilePath { get; set; } = string.Empty;
	}
}
