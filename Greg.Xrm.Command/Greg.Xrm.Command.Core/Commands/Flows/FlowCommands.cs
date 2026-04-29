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

	// Phase 2: Flow owner management
	[Command("flow", "owner", "list", HelpText = "List owners/permissions of a Power Automate cloud flow.")]
	public class FlowOwnerListCommand
	{
		[Option("environment", "env", Order = 1, Required = true, HelpText = "The environment name or ID.")]
		public string EnvironmentName { get; set; } = string.Empty;

		[Option("name", "n", Order = 2, Required = true, HelpText = "The name (ID) of the flow.")]
		public string FlowName { get; set; } = string.Empty;

		[Option("as-admin", "a", Order = 3, HelpText = "Run as admin.")]
		public bool AsAdmin { get; set; }
	}

	[Command("flow", "owner", "ensure", HelpText = "Add or update a permission (owner) on a Power Automate cloud flow.")]
	public class FlowOwnerEnsureCommand
	{
		[Option("environment", "env", Order = 1, Required = true, HelpText = "The environment name or ID.")]
		public string EnvironmentName { get; set; } = string.Empty;

		[Option("name", "n", Order = 2, Required = true, HelpText = "The name (ID) of the flow.")]
		public string FlowName { get; set; } = string.Empty;

		[Option("principal-id", "p", Order = 3, Required = true, HelpText = "The AAD object ID of the user or group.")]
		public string PrincipalId { get; set; } = string.Empty;

		[Option("principal-type", "t", Order = 4, DefaultValue = "User", HelpText = "Principal type: User or Group.")]
		public string PrincipalType { get; set; } = "User";

		[Option("role", "r", Order = 5, DefaultValue = "CanEdit", HelpText = "Role to assign: CanView or CanEdit.")]
		public string Role { get; set; } = "CanEdit";

		[Option("as-admin", "a", Order = 6, HelpText = "Run as admin.")]
		public bool AsAdmin { get; set; }
	}

	[Command("flow", "owner", "remove", HelpText = "Remove a permission (owner) from a Power Automate cloud flow.")]
	public class FlowOwnerRemoveCommand
	{
		[Option("environment", "env", Order = 1, Required = true, HelpText = "The environment name or ID.")]
		public string EnvironmentName { get; set; } = string.Empty;

		[Option("name", "n", Order = 2, Required = true, HelpText = "The name (ID) of the flow.")]
		public string FlowName { get; set; } = string.Empty;

		[Option("principal-id", "p", Order = 3, Required = true, HelpText = "The AAD object ID of the user or group to remove.")]
		public string PrincipalId { get; set; } = string.Empty;

		[Option("as-admin", "a", Order = 4, HelpText = "Run as admin.")]
		public bool AsAdmin { get; set; }
	}

	// Phase 2: Flow environment
	[Command("flow", "environment", "list", HelpText = "List Power Automate environments.")]
	public class FlowEnvironmentListCommand
	{
	}

	[Command("flow", "environment", "get", HelpText = "Get details of a Power Automate environment.")]
	public class FlowEnvironmentGetCommand
	{
		[Option("environment", "env", Order = 1, Required = true, HelpText = "The environment name or ID.")]
		public string EnvironmentName { get; set; } = string.Empty;
	}

	// Phase 2: Flow recycle bin
	[Command("flow", "recyclebin", "list", HelpText = "List soft-deleted (recycle bin) Power Automate cloud flows.")]
	public class FlowRecycleBinListCommand
	{
		[Option("environment", "env", Order = 1, Required = true, HelpText = "The environment name or ID.")]
		public string EnvironmentName { get; set; } = string.Empty;
	}

	[Command("flow", "recyclebin", "restore", HelpText = "Restore a soft-deleted Power Automate cloud flow from the recycle bin.")]
	public class FlowRecycleBinRestoreCommand
	{
		[Option("environment", "env", Order = 1, Required = true, HelpText = "The environment name or ID.")]
		public string EnvironmentName { get; set; } = string.Empty;

		[Option("name", "n", Order = 2, Required = true, HelpText = "The name (ID) of the flow to restore.")]
		public string FlowName { get; set; } = string.Empty;
	}
}
