using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Fabric
{
	[Command("onelake", "shortcut", "list", HelpText = "List OneLake shortcuts for a Fabric item.")]
	public class OneLakeShortcutListCommand
	{
		[Option("workspace-id", "w", Order = 1, Required = true, HelpText = "Fabric workspace ID.")]
		public string WorkspaceId { get; set; } = "";

		[Option("item-id", "i", Order = 2, Required = true, HelpText = "Fabric item ID.")]
		public string ItemId { get; set; } = "";
	}

	[Command("onelake", "shortcut", "create", HelpText = "Create a OneLake shortcut for a Fabric item.")]
	public class OneLakeShortcutCreateCommand
	{
		[Option("workspace-id", "w", Order = 1, Required = true, HelpText = "Fabric workspace ID.")]
		public string WorkspaceId { get; set; } = "";

		[Option("item-id", "i", Order = 2, Required = true, HelpText = "Fabric item ID.")]
		public string ItemId { get; set; } = "";

		[Option("source-path", Order = 3, Required = true, HelpText = "External source path.")]
		public string SourcePath { get; set; } = "";

		[Option("target-path", Order = 4, Required = true, HelpText = "Target OneLake path.")]
		public string TargetPath { get; set; } = "";

		[Option("source-type", Order = 5, DefaultValue = "adlsGen2", HelpText = "Shortcut source type, such as adlsGen2, s3, gcs, or dataverse.")]
		public string SourceType { get; set; } = "adlsGen2";
	}

	[Command("onelake", "shortcut", "delete", HelpText = "Delete a OneLake shortcut for a Fabric item.")]
	public class OneLakeShortcutDeleteCommand
	{
		[Option("workspace-id", "w", Order = 1, Required = true, HelpText = "Fabric workspace ID.")]
		public string WorkspaceId { get; set; } = "";

		[Option("item-id", "i", Order = 2, Required = true, HelpText = "Fabric item ID.")]
		public string ItemId { get; set; } = "";

		[Option("shortcut-path", "p", Order = 3, Required = true, HelpText = "Shortcut path to delete.")]
		public string ShortcutPath { get; set; } = "";
	}
}
