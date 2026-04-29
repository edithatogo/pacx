using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Forms
{
	[Command("forms", "create", HelpText = "Create a Microsoft Forms authoring manifest.")]
	public class FormsCreateCommand
	{
		[Option("output", "o", Order = 1, Required = true, HelpText = "Output path for the authoring manifest JSON.")]
		public string OutputPath { get; set; } = string.Empty;

		[Option("title", "t", Order = 2, Required = true, HelpText = "Form title.")]
		public string Title { get; set; } = string.Empty;

		[Option("description", "d", Order = 3, HelpText = "Optional form description.")]
		public string? Description { get; set; }

		[Option("published", "p", Order = 4, HelpText = "Set the initial published state.", DefaultValue = false)]
		public bool Published { get; set; }
	}

	[Command("forms", "update", HelpText = "Update a Microsoft Forms authoring manifest.")]
	public class FormsUpdateCommand
	{
		[Option("file", "f", Order = 1, Required = true, HelpText = "Path to the authoring manifest JSON.")]
		public string FilePath { get; set; } = string.Empty;

		[Option("title", "t", Order = 2, HelpText = "Updated form title.")]
		public string? Title { get; set; }

		[Option("description", "d", Order = 3, HelpText = "Updated form description.")]
		public string? Description { get; set; }

		[Option("published", "p", Order = 4, HelpText = "Updated published state.")]
		public bool? Published { get; set; }
	}

	[Command("forms", "delete", HelpText = "Delete a Microsoft Forms authoring manifest.")]
	public class FormsDeleteCommand
	{
		[Option("file", "f", Order = 1, Required = true, HelpText = "Path to the authoring manifest JSON.")]
		public string FilePath { get; set; } = string.Empty;

		[Option("force", Order = 2, HelpText = "Delete without an interactive confirmation prompt.")]
		public bool Force { get; set; }
	}

	[Command("forms", "publish", HelpText = "Publish or unpublish a Microsoft Forms authoring manifest.")]
	public class FormsPublishCommand
	{
		[Option("file", "f", Order = 1, Required = true, HelpText = "Path to the authoring manifest JSON.")]
		public string FilePath { get; set; } = string.Empty;

		[Option("published", "p", Order = 2, HelpText = "Set published to true or false.", DefaultValue = true)]
		public bool Published { get; set; } = true;
	}

	[Command("forms", "inspect", HelpText = "Inspect a Microsoft Forms authoring manifest.")]
	public class FormsInspectCommand
	{
		[Option("file", "f", Order = 1, Required = true, HelpText = "Path to the authoring manifest JSON.")]
		public string FilePath { get; set; } = string.Empty;
	}

	[Command("forms", "question", "add", HelpText = "Add a question to a Microsoft Forms authoring manifest.")]
	public class FormsQuestionAddCommand
	{
		[Option("file", "f", Order = 1, Required = true, HelpText = "Path to the authoring manifest JSON.")]
		public string FilePath { get; set; } = string.Empty;

		[Option("text", "t", Order = 2, Required = true, HelpText = "Question text.")]
		public string QuestionText { get; set; } = string.Empty;

		[Option("type", Order = 3, DefaultValue = "text", HelpText = "Question type.")]
		public string Type { get; set; } = "text";

		[Option("required", "r", Order = 4, HelpText = "Mark the question as required.")]
		public bool Required { get; set; }

		[Option("options", "o", Order = 5, HelpText = "Comma-separated answer options for choice questions.")]
		public string? OptionsCsv { get; set; }
	}

	[Command("forms", "question", "update", HelpText = "Update a question in a Microsoft Forms authoring manifest.")]
	public class FormsQuestionUpdateCommand
	{
		[Option("file", "f", Order = 1, Required = true, HelpText = "Path to the authoring manifest JSON.")]
		public string FilePath { get; set; } = string.Empty;

		[Option("id", Order = 2, Required = true, HelpText = "Question identifier.")]
		public string QuestionId { get; set; } = string.Empty;

		[Option("text", "t", Order = 3, HelpText = "Updated question text.")]
		public string? QuestionText { get; set; }

		[Option("type", Order = 4, HelpText = "Updated question type.")]
		public string? Type { get; set; }

		[Option("required", "r", Order = 5, HelpText = "Updated required flag.")]
		public bool? Required { get; set; }

		[Option("options", "o", Order = 6, HelpText = "Comma-separated answer options for choice questions.")]
		public string? OptionsCsv { get; set; }
	}

	[Command("forms", "question", "list", HelpText = "List questions in a Microsoft Forms authoring manifest.")]
	public class FormsQuestionListCommand
	{
		[Option("file", "f", Order = 1, Required = true, HelpText = "Path to the authoring manifest JSON.")]
		public string FilePath { get; set; } = string.Empty;
	}

	[Command("forms", "question", "delete", HelpText = "Delete a question from a Microsoft Forms authoring manifest.")]
	public class FormsQuestionDeleteCommand
	{
		[Option("file", "f", Order = 1, Required = true, HelpText = "Path to the authoring manifest JSON.")]
		public string FilePath { get; set; } = string.Empty;

		[Option("id", Order = 2, Required = true, HelpText = "Question identifier.")]
		public string QuestionId { get; set; } = string.Empty;

		[Option("force", Order = 3, HelpText = "Delete without confirmation.")]
		public bool Force { get; set; }
	}

	[Command("forms", "section", "add", HelpText = "Add a section to a Microsoft Forms authoring manifest.")]
	public class FormsSectionAddCommand
	{
		[Option("file", "f", Order = 1, Required = true, HelpText = "Path to the authoring manifest JSON.")]
		public string FilePath { get; set; } = string.Empty;

		[Option("title", "t", Order = 2, Required = true, HelpText = "Section title.")]
		public string Title { get; set; } = string.Empty;

		[Option("description", "d", Order = 3, HelpText = "Optional section description.")]
		public string? Description { get; set; }

		[Option("order", "n", Order = 4, HelpText = "Display order for the section.")]
		public int? Order { get; set; }
	}

	[Command("forms", "section", "list", HelpText = "List sections in a Microsoft Forms authoring manifest.")]
	public class FormsSectionListCommand
	{
		[Option("file", "f", Order = 1, Required = true, HelpText = "Path to the authoring manifest JSON.")]
		public string FilePath { get; set; } = string.Empty;
	}

	[Command("forms", "section", "delete", HelpText = "Delete a section from a Microsoft Forms authoring manifest.")]
	public class FormsSectionDeleteCommand
	{
		[Option("file", "f", Order = 1, Required = true, HelpText = "Path to the authoring manifest JSON.")]
		public string FilePath { get; set; } = string.Empty;

		[Option("id", Order = 2, Required = true, HelpText = "Section identifier.")]
		public string SectionId { get; set; } = string.Empty;

		[Option("force", Order = 3, HelpText = "Delete without confirmation.")]
		public bool Force { get; set; }
	}

	[Command("forms", "section", "update", HelpText = "Update a section in a Microsoft Forms authoring manifest.")]
	public class FormsSectionUpdateCommand
	{
		[Option("file", "f", Order = 1, Required = true, HelpText = "Path to the authoring manifest JSON.")]
		public string FilePath { get; set; } = string.Empty;

		[Option("id", Order = 2, Required = true, HelpText = "Section identifier.")]
		public string SectionId { get; set; } = string.Empty;

		[Option("title", "t", Order = 3, HelpText = "Updated section title.")]
		public string? Title { get; set; }

		[Option("description", "d", Order = 4, HelpText = "Updated section description.")]
		public string? Description { get; set; }

		[Option("order", "n", Order = 5, HelpText = "Updated display order.")]
		public int? Order { get; set; }
	}
}
