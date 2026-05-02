using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Forms
{
	[Command("forms", "close", HelpText = "Close a Microsoft Form to stop accepting responses.")]
	public class FormsCloseCommand : FormsFormCommandBase
	{
	}

	[Command("forms", "reopen", HelpText = "Reopen a closed Microsoft Form to resume accepting responses.")]
	public class FormsReopenCommand : FormsFormCommandBase
	{
	}

	[Command("forms", "duplicate", HelpText = "Duplicate/clone a Microsoft Form.")]
	public class FormsDuplicateCommand : FormsFormCommandBase
	{
		[Option("new-title", "t", Order = 5, HelpText = "Title for the duplicated form. Defaults to original title + ' (Copy)'.")]
		public string? NewTitle { get; set; }
	}
}
