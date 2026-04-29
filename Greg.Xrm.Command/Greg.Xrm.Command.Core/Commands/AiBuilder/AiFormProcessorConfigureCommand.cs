using Greg.Xrm.Command.Parsing;
using System.ComponentModel.DataAnnotations;

namespace Greg.Xrm.Command.Commands.AiBuilder
{
	[Command("ai", "form-processor", "configure", HelpText = "Configure form processing model (document type, fields, tables).")]
	public class AiFormProcessorConfigureCommand
	{
		[Option("model-id", "m", Order = 1, Required = true, HelpText = "Form processing model ID.")]
		public string ModelId { get; set; } = "";

		[Option("doc-type", "d", Order = 2, Required = true, HelpText = "Document type name.")]
		public string DocumentType { get; set; } = "";

		[Option("fields", "f", Order = 3, HelpText = "Comma-separated list of field names to extract.")]
		public string[]? Fields { get; set; }

		[Option("tables", "t", Order = 4, HelpText = "Comma-separated list of table names to extract.")]
		public string[]? Tables { get; set; }
	}
}
