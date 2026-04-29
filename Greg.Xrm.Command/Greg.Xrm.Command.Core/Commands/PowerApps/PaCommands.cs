using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.PowerApps
{
	[Command("pa", "app", "list", HelpText = "List Power Apps in an environment.")]
	public class PaAppListCommand
	{
		[Option("environment", "env", Order = 1, HelpText = "The environment name or ID. Required when using --as-admin.")]
		public string? EnvironmentName { get; set; }

		[Option("as-admin", "a", Order = 2, HelpText = "Run as admin to list apps across the environment.")]
		public bool AsAdmin { get; set; }
	}

	[Command("pa", "app", "get", HelpText = "Get details of a specific Power App.")]
	public class PaAppGetCommand
	{
		[Option("name", "n", Order = 1, Required = true, HelpText = "The name (ID) of the Power App.")]
		public string AppName { get; set; } = string.Empty;

		[Option("environment", "env", Order = 2, HelpText = "The environment name or ID. Required when using --as-admin.")]
		public string? EnvironmentName { get; set; }

		[Option("as-admin", "a", Order = 3, HelpText = "Run as admin.")]
		public bool AsAdmin { get; set; }
	}

	[Command("pa", "app", "remove", HelpText = "Delete a Power App.")]
	public class PaAppRemoveCommand
	{
		[Option("name", "n", Order = 1, Required = true, HelpText = "The name (ID) of the Power App to delete.")]
		public string AppName { get; set; } = string.Empty;

		[Option("environment", "env", Order = 2, HelpText = "The environment name or ID. Required when using --as-admin.")]
		public string? EnvironmentName { get; set; }

		[Option("as-admin", "a", Order = 3, HelpText = "Run as admin.")]
		public bool AsAdmin { get; set; }

		[Option("confirm", "c", Order = 4, HelpText = "Skip the confirmation prompt.")]
		public bool Confirm { get; set; }
	}

	[Command("pa", "app", "export", HelpText = "Export a Power App as a package (ZIP).")]
	public class PaAppExportCommand
	{
		[Option("name", "n", Order = 1, Required = true, HelpText = "The name (ID) of the Power App to export.")]
		public string AppName { get; set; } = string.Empty;

		[Option("environment", "env", Order = 2, Required = true, HelpText = "The environment name or ID.")]
		public string EnvironmentName { get; set; } = string.Empty;

		[Option("output", "o", Order = 3, HelpText = "Output file path. Defaults to current directory with the app name.")]
		public string? OutputPath { get; set; }
	}

	// Consent
	[Command("pa", "app", "consent", "set", HelpText = "Set consent bypass for a Power App.")]
	public class PaAppConsentSetCommand
	{
		[Option("name", "n", Order = 1, Required = true, HelpText = "The name (ID) of the Power App.")]
		public string AppName { get; set; } = string.Empty;

		[Option("environment", "env", Order = 2, Required = true, HelpText = "The environment name or ID.")]
		public string EnvironmentName { get; set; } = string.Empty;

		[Option("bypass", "b", Order = 3, DefaultValue = true, HelpText = "Set whether consent is bypassed. Default is true.")]
		public bool BypassConsent { get; set; } = true;
	}

	// Owner
	[Command("pa", "app", "owner", "set", HelpText = "Transfer ownership of a Power App to another user.")]
	public class PaAppOwnerSetCommand
	{
		[Option("name", "n", Order = 1, Required = true, HelpText = "The name (ID) of the Power App.")]
		public string AppName { get; set; } = string.Empty;

		[Option("environment", "env", Order = 2, Required = true, HelpText = "The environment name or ID.")]
		public string EnvironmentName { get; set; } = string.Empty;

		[Option("new-owner-id", "o", Order = 3, Required = true, HelpText = "The AAD object ID of the new owner.")]
		public string NewOwnerId { get; set; } = string.Empty;

		[Option("role-for-old-owner", "r", Order = 4, HelpText = "Role to assign to the previous owner after transfer: CanView or CanEdit.")]
		public string? RoleForOldOwner { get; set; }
	}

	// Permissions
	[Command("pa", "app", "permission", "list", HelpText = "List permissions of a Power App.")]
	public class PaAppPermissionListCommand
	{
		[Option("name", "n", Order = 1, Required = true, HelpText = "The name (ID) of the Power App.")]
		public string AppName { get; set; } = string.Empty;

		[Option("environment", "env", Order = 2, HelpText = "The environment name or ID. Required when using --as-admin.")]
		public string? EnvironmentName { get; set; }

		[Option("as-admin", "a", Order = 3, HelpText = "Run as admin.")]
		public bool AsAdmin { get; set; }

		[Option("role-name", "r", Order = 4, HelpText = "Filter by role name (client-side filter).")]
		public string? RoleName { get; set; }
	}

	[Command("pa", "app", "permission", "add", HelpText = "Add a permission to a Power App.")]
	public class PaAppPermissionAddCommand
	{
		[Option("name", "n", Order = 1, Required = true, HelpText = "The name (ID) of the Power App.")]
		public string AppName { get; set; } = string.Empty;

		[Option("environment", "env", Order = 2, HelpText = "The environment name or ID. Required when using --as-admin.")]
		public string? EnvironmentName { get; set; }

		[Option("as-admin", "a", Order = 3, HelpText = "Run as admin.")]
		public bool AsAdmin { get; set; }

		[Option("principal-id", "p", Order = 4, Required = true, HelpText = "The AAD object ID of the user or group.")]
		public string PrincipalId { get; set; } = string.Empty;

		[Option("principal-type", "t", Order = 5, DefaultValue = "User", HelpText = "Principal type: User, Group, or Tenant.")]
		public string PrincipalType { get; set; } = "User";

		[Option("role-name", "r", Order = 6, DefaultValue = "CanEdit", HelpText = "Role to assign: CanView or CanEdit.")]
		public string RoleName { get; set; } = "CanEdit";
	}

	[Command("pa", "app", "permission", "remove", HelpText = "Remove a permission from a Power App.")]
	public class PaAppPermissionRemoveCommand
	{
		[Option("name", "n", Order = 1, Required = true, HelpText = "The name (ID) of the Power App.")]
		public string AppName { get; set; } = string.Empty;

		[Option("environment", "env", Order = 2, HelpText = "The environment name or ID. Required when using --as-admin.")]
		public string? EnvironmentName { get; set; }

		[Option("as-admin", "a", Order = 3, HelpText = "Run as admin.")]
		public bool AsAdmin { get; set; }

		[Option("principal-id", "p", Order = 4, Required = true, HelpText = "The AAD object ID of the user or group to remove.")]
		public string PrincipalId { get; set; } = string.Empty;
	}
}
