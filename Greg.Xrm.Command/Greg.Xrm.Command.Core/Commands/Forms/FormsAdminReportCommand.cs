using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Forms
{
	[Command("forms", "admin", "report", HelpText = "Generate a tenant-wide Microsoft Forms inventory report.")]
	public class FormsAdminReportCommand
	{
		[Option("tenant", "t", Order = 1, Required = true, HelpText = "Tenant ID or domain.")]
		public string TenantId { get; set; } = "";

		[Option("output", "o", Order = 2, DefaultValue = "forms-report.xlsx", HelpText = "Output file path for the report.")]
		public string OutputPath { get; set; } = "forms-report.xlsx";

		[Option("include-groups", "g", Order = 3, HelpText = "Include group-owned forms (requires ROPC auth).")]
		public bool IncludeGroups { get; set; }

		[Option("token", Order = 4, HelpText = "OAuth2 access token.")]
		public string? Token { get; set; }
	}
}
