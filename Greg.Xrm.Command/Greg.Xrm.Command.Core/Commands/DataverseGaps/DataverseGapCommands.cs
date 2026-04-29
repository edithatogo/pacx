using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.DataverseGaps
{
	public abstract class WorkflowGapCommandBase
	{
		[Option("id", "i", Order = 1, Required = true, HelpText = "Workflow or process ID.")]
		public Guid Id { get; set; }
	}

	public abstract class WorkflowImportCommandBase
	{
		[Option("file", "f", Order = 1, Required = true, HelpText = "Workflow export file.")]
		public string FilePath { get; set; } = "";
	}

	[Command("business-rule", "list", HelpText = "List Dataverse business rules.")]
	public class BusinessRuleListCommand
	{
		[Option("table", "t", Order = 1, HelpText = "Optional primary table logical name.")]
		public string? Table { get; set; }
	}

	[Command("business-rule", "export", HelpText = "Export a Dataverse business rule definition.")]
	public class BusinessRuleExportCommand : WorkflowGapCommandBase
	{
		[Option("file", "f", Order = 2, Required = true, HelpText = "Output JSON file.")]
		public string FilePath { get; set; } = "";
	}

	[Command("business-rule", "import", HelpText = "Import a Dataverse business rule definition.")]
	public class BusinessRuleImportCommand : WorkflowImportCommandBase
	{
		[Option("table", "t", Order = 2, Required = true, HelpText = "Primary table logical name.")]
		public string Table { get; set; } = "";
	}

	[Command("business-rule", "activate", HelpText = "Activate a Dataverse business rule.")]
	public class BusinessRuleActivateCommand : WorkflowGapCommandBase
	{
	}

	[Command("business-rule", "deactivate", HelpText = "Deactivate a Dataverse business rule.")]
	public class BusinessRuleDeactivateCommand : WorkflowGapCommandBase
	{
	}

	[Command("bpf", "list", HelpText = "List Dataverse business process flows.")]
	public class BpfListCommand
	{
	}

	[Command("bpf", "export", HelpText = "Export a Dataverse business process flow definition.")]
	public class BpfExportCommand : WorkflowGapCommandBase
	{
		[Option("file", "f", Order = 2, Required = true, HelpText = "Output JSON file.")]
		public string FilePath { get; set; } = "";
	}

	[Command("bpf", "import", HelpText = "Import a Dataverse business process flow definition.")]
	public class BpfImportCommand : WorkflowImportCommandBase
	{
	}

	[Command("bpf", "activate", HelpText = "Activate a Dataverse business process flow.")]
	public class BpfActivateCommand : WorkflowGapCommandBase
	{
	}

	[Command("ddr", "list", HelpText = "List duplicate detection rules.")]
	public class DdrListCommand
	{
	}

	[Command("ddr", "run", HelpText = "Run a duplicate detection job for a rule.")]
	public class DdrRunCommand
	{
		[Option("rule-id", "r", Order = 1, Required = true, HelpText = "Duplicate detection rule ID.")]
		public Guid RuleId { get; set; }
	}

	[Command("ddr", "enable", HelpText = "Enable a duplicate detection rule.")]
	public class DdrEnableCommand : DdrRunCommand
	{
	}

	[Command("ddr", "disable", HelpText = "Disable a duplicate detection rule.")]
	public class DdrDisableCommand : DdrRunCommand
	{
	}

	[Command("audit", "status", HelpText = "Show organization audit settings.")]
	public class AuditStatusCommand
	{
		[Option("env", "e", Order = 1, HelpText = "Optional environment label for CI output; the current pacx connection is used.")]
		public string? EnvironmentName { get; set; }
	}

	[Command("audit", "enable-table", HelpText = "Enable auditing on a Dataverse table.")]
	public class AuditEnableTableCommand
	{
		[Option("name", "n", Order = 1, Required = true, HelpText = "Table logical name.")]
		public string Table { get; set; } = "";
	}

	[Command("audit", "export", HelpText = "Export audit records.")]
	public class AuditExportCommand
	{
		[Option("table", "t", Order = 1, Required = true, HelpText = "Audited table logical name.")]
		public string Table { get; set; } = "";

		[Option("since", "s", Order = 2, Required = true, HelpText = "UTC start date/time.")]
		public DateTime Since { get; set; }

		[Option("file", "f", Order = 3, Required = true, HelpText = "Output JSONL file.")]
		public string FilePath { get; set; } = "";

		[Option("format", Order = 4, HelpText = "Output format. Currently only jsonl is supported.")]
		public string Format { get; set; } = "jsonl";
	}

	[Command("fsp", "list", HelpText = "List field security profiles.")]
	public class FspListCommand
	{
	}

	[Command("fsp", "apply", HelpText = "Assign a field security profile to a user or team.")]
	public class FspApplyCommand
	{
		[Option("profile-id", "p", Order = 1, Required = true, HelpText = "Field security profile ID.")]
		public Guid ProfileId { get; set; }

		[Option("user-or-team-id", "u", Order = 2, Required = true, HelpText = "System user or team ID.")]
		public Guid UserOrTeamId { get; set; }
	}

	[Command("fsp", "bulk-assign", HelpText = "Bulk assign a field security profile from a CSV file.")]
	public class FspBulkAssignCommand
	{
		[Option("profile-id", "p", Order = 1, Required = true, HelpText = "Field security profile ID.")]
		public Guid ProfileId { get; set; }

		[Option("file", "f", Order = 2, Required = true, HelpText = "CSV file with one userOrTeamId column or a single GUID per row.")]
		public string FilePath { get; set; } = "";
	}

	[Command("endpoint", "list", HelpText = "List Dataverse service endpoints.")]
	public class EndpointListCommand
	{
	}

	[Command("endpoint", "register", HelpText = "Register a Dataverse service endpoint/webhook.")]
	public class EndpointRegisterCommand
	{
		[Option("url", "u", Order = 1, Required = true, HelpText = "Endpoint URL.")]
		public string Url { get; set; } = "";

		[Option("auth", "a", Order = 2, Required = true, HelpText = "Authentication type label or value.")]
		public string Auth { get; set; } = "";

		[Option("name", "n", Order = 3, HelpText = "Endpoint name.")]
		public string? Name { get; set; }
	}

	[Command("endpoint", "delete", HelpText = "Delete a Dataverse service endpoint.")]
	public class EndpointDeleteCommand
	{
		[Option("id", "i", Order = 1, Required = true, HelpText = "Service endpoint ID.")]
		public Guid Id { get; set; }
	}

	[Command("column", "file-upload", HelpText = "Upload a local file into a Dataverse file column.")]
	public class ColumnFileUploadCommand
	{
		[Option("table", "t", Order = 1, Required = true, HelpText = "Table logical name.")]
		public string Table { get; set; } = "";

		[Option("id", "i", Order = 2, Required = true, HelpText = "Record ID.")]
		public Guid RecordId { get; set; }

		[Option("column", "c", Order = 3, Required = true, HelpText = "File column logical name.")]
		public string Column { get; set; } = "";

		[Option("file", "f", Order = 4, Required = true, HelpText = "Path to the local file.")]
		public string FilePath { get; set; } = "";
	}

	[Command("column", "rollup-recalculate", HelpText = "Recalculate a Dataverse rollup column.")]
	public class ColumnRollupRecalculateCommand
	{
		[Option("table", "t", Order = 1, Required = true, HelpText = "Table logical name.")]
		public string Table { get; set; } = "";

		[Option("id", "i", Order = 2, Required = true, HelpText = "Record ID.")]
		public Guid RecordId { get; set; }

		[Option("column", "c", Order = 3, Required = true, HelpText = "Rollup column logical name.")]
		public string Column { get; set; } = "";
	}
}
