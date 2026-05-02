using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Forms
{
	[Command("forms", "branching", "export", HelpText = "Export Microsoft Forms branching logic.")]
	public class FormsBranchingExportCommand : FormsFormCommandBase
	{
	}

	[Command("forms", "analytics", "summary", HelpText = "Summarize Microsoft Forms analytics.")]
	public class FormsAnalyticsSummaryCommand : FormsFormCommandBase
	{
	}

	[Command("forms", "analytics", "timeseries", HelpText = "Export Microsoft Forms response analytics as a time series.")]
	public class FormsAnalyticsTimeseriesCommand : FormsFormCommandBase
	{
		[Option("bucket", "b", Order = 4, DefaultValue = "day", HelpText = "Time bucket: day or week.")]
		public string Bucket { get; set; } = "day";
	}

	[Command("forms", "template", "list", HelpText = "List Microsoft Forms templates.")]
	public class FormsTemplateListCommand
	{
		[Option("org", Order = 1, HelpText = "List organization templates.")]
		public bool Organization { get; set; }
	}

	[Command("forms", "template", "create", HelpText = "Create a Microsoft Forms template from a form.")]
	public class FormsTemplateCreateCommand
	{
		[Option("from-form", "f", Order = 1, Required = true, HelpText = "Source form ID.")]
		public string FormId { get; set; } = "";

		[Option("scope", "s", Order = 2, DefaultValue = "team", HelpText = "Template scope: org or team.")]
		public string Scope { get; set; } = "team";
	}

	[Command("forms", "template", "share", HelpText = "Share a Microsoft Forms template with an Entra group.")]
	public class FormsTemplateShareCommand
	{
		[Option("id", "i", Order = 1, Required = true, HelpText = "Template ID.")]
		public string TemplateId { get; set; } = "";

		[Option("group", "g", Order = 2, Required = true, HelpText = "Entra group ID.")]
		public string GroupId { get; set; } = "";
	}

	[Command("forms", "share", HelpText = "Share a Microsoft Form with a group.")]
	public class FormsShareCommand : FormsFormCommandBase
	{
		[Option("with-group", "g", Order = 4, Required = true, HelpText = "Entra group ID.")]
		public string GroupId { get; set; } = "";

		[Option("role", "r", Order = 5, DefaultValue = "respond", HelpText = "Role: respond or collaborate.")]
		public string Role { get; set; } = "respond";
	}

	[Command("forms", "ownership", "transfer", HelpText = "Transfer Microsoft Form ownership.")]
	public class FormsOwnershipTransferCommand : FormsFormCommandBase
	{
		[Option("to", Order = 4, Required = true, HelpText = "Target owner UPN.")]
		public string TargetUserPrincipalName { get; set; } = "";
	}

	[Command("forms", "cv", "list", HelpText = "List Customer Voice surveys.")]
	public class FormsCustomerVoiceListCommand
	{
	}

	[Command("forms", "cv", "send", HelpText = "Send a Customer Voice survey.")]
	public class FormsCustomerVoiceSendCommand
	{
		[Option("survey-id", "s", Order = 1, Required = true, HelpText = "Customer Voice survey ID.")]
		public string SurveyId { get; set; } = "";

		[Option("to", Order = 2, Required = true, HelpText = "Comma-separated recipient UPN list.")]
		public string Recipients { get; set; } = "";
	}

	[Command("forms", "cv", "responses", "export", HelpText = "Export Customer Voice survey responses.")]
	public class FormsCustomerVoiceResponsesExportCommand
	{
		[Option("survey-id", "s", Order = 1, Required = true, HelpText = "Customer Voice survey ID.")]
		public string SurveyId { get; set; } = "";

		[Option("output", "o", Order = 2, HelpText = "Output file path.")]
		public string? OutputPath { get; set; }
	}

	public abstract class FormsFormCommandBase
	{
		[Option("form-id", "f", Order = 1, Required = true, HelpText = "Microsoft Form ID.")]
		public string FormId { get; set; } = "";

		[Option("tenant", "t", Order = 2, HelpText = "Tenant ID or domain.")]
		public string? TenantId { get; set; }

		[Option("owner", "o", Order = 3, HelpText = "Owner user ID.")]
		public string? OwnerId { get; set; }

		[Option("owner-type", Order = 4, DefaultValue = "User", HelpText = "Owner type: User or Group.")]
		public string OwnerType { get; set; } = "User";
	}
}
