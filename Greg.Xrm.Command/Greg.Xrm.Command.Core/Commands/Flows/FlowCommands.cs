using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Flows
{
	[Command("flow", "list", HelpText = "List Power Automate cloud flows in an environment.")]
	[Alias("flows", "list")]
	public class FlowListCommand
	{
		[Option("environment", "env", Order = 1, Required = true, HelpText = "The environment name or ID.")]
		public string EnvironmentName { get; set; } = string.Empty;

		[Option("sharing-status", "s", Order = 2, HelpText = "Filter by sharing status: 'personal', 'sharedWithMe', or 'all'. Defaults to all.")]
		public string? SharingStatus { get; set; }

		[Option("with-solutions", "w", Order = 3, HelpText = "Include solution cloud flows.")]
		public bool WithSolutions { get; set; }

		[Option("as-admin", "a", Order = 4, HelpText = "Run as admin against all flows in the environment.")]
		public bool AsAdmin { get; set; }
	}

	[Command("flow", "get", HelpText = "Get a specific Power Automate cloud flow definition.")]
	[Alias("flows", "get")]
	public class FlowGetCommand
	{
		[Option("environment", "env", Order = 1, Required = true, HelpText = "The environment name or ID.")]
		public string EnvironmentName { get; set; } = string.Empty;

		[Option("name", "n", Order = 2, Required = true, HelpText = "The name (ID) of the flow to get.")]
		public string FlowName { get; set; } = string.Empty;
	}

	[Command("flow", "enable", HelpText = "Enable/start a Power Automate cloud flow.")]
	[Alias("flows", "enable")]
	public class FlowEnableCommand
	{
		[Option("environment", "env", Order = 1, Required = true, HelpText = "The environment name or ID.")]
		public string EnvironmentName { get; set; } = string.Empty;

		[Option("name", "n", Order = 2, Required = true, HelpText = "The name (ID) of the flow to enable.")]
		public string FlowName { get; set; } = string.Empty;

		[Option("as-admin", "a", Order = 3, HelpText = "Run as admin.")]
		public bool AsAdmin { get; set; }
	}

	[Command("flow", "disable", HelpText = "Disable/stop a Power Automate cloud flow.")]
	[Alias("flows", "disable")]
	public class FlowDisableCommand
	{
		[Option("environment", "env", Order = 1, Required = true, HelpText = "The environment name or ID.")]
		public string EnvironmentName { get; set; } = string.Empty;

		[Option("name", "n", Order = 2, Required = true, HelpText = "The name (ID) of the flow to disable.")]
		public string FlowName { get; set; } = string.Empty;

		[Option("as-admin", "a", Order = 3, HelpText = "Run as admin.")]
		public bool AsAdmin { get; set; }
	}

	[Command("flow", "remove", HelpText = "Delete a Power Automate cloud flow.")]
	[Alias("flows", "remove")]
	public class FlowRemoveCommand
	{
		[Option("environment", "env", Order = 1, Required = true, HelpText = "The environment name or ID.")]
		public string EnvironmentName { get; set; } = string.Empty;

		[Option("name", "n", Order = 2, Required = true, HelpText = "The name (ID) of the flow to remove.")]
		public string FlowName { get; set; } = string.Empty;

		[Option("as-admin", "a", Order = 3, HelpText = "Run as admin.")]
		public bool AsAdmin { get; set; }

		[Option("confirm", "c", Order = 4, HelpText = "Skip the confirmation prompt.")]
		public bool Confirm { get; set; }
	}

	[Command("flow", "export", HelpText = "Export a Power Automate cloud flow as a package or ARM template.")]
	[Alias("flows", "export")]
	public class FlowExportCommand
	{
		[Option("environment", "env", Order = 1, Required = true, HelpText = "The environment name or ID.")]
		public string EnvironmentName { get; set; } = string.Empty;

		[Option("name", "n", Order = 2, Required = true, HelpText = "The name (ID) of the flow to export.")]
		public string FlowName { get; set; } = string.Empty;

		[Option("format", "f", Order = 3, DefaultValue = "zip", HelpText = "Export format: 'zip' (package) or 'json' (ARM template). Default is 'zip'.")]
		public string Format { get; set; } = "zip";

		[Option("output", "o", Order = 4, HelpText = "Output file path. Defaults to current directory with the flow display name.")]
		public string? OutputPath { get; set; }
	}

	[Command("flows", "help", HelpText = "Shows help for flow commands")]
	public class FlowHelpCommand
	{
	}
}
