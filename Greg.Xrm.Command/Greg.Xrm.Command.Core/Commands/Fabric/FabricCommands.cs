using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Fabric
{
	[Command("fabric", "workspace", "list", HelpText = "List Microsoft Fabric workspaces.")]
	public class FabricWorkspaceListCommand
	{
	}

	[Command("fabric", "workspace", "create", HelpText = "Create a Microsoft Fabric workspace.")]
	public class FabricWorkspaceCreateCommand
	{
		[Option("name", "n", Order = 1, Required = true, HelpText = "Workspace display name.")]
		public string Name { get; set; } = "";

		[Option("capacity-id", Order = 2, HelpText = "Optional Fabric capacity ID.")]
		public string? CapacityId { get; set; }
	}

	[Command("fabric", "capacity", "list", HelpText = "List Microsoft Fabric capacities.")]
	public class FabricCapacityListCommand
	{
	}

	[Command("fabric", "lakehouse", "list", HelpText = "List lakehouses in a Fabric workspace.")]
	public class FabricLakehouseListCommand
	{
		[Option("workspace-id", "w", Order = 1, Required = true, HelpText = "Fabric workspace ID.")]
		public string WorkspaceId { get; set; } = "";
	}

	[Command("fabric", "lakehouse", "create", HelpText = "Create a lakehouse in a Fabric workspace.")]
	public class FabricLakehouseCreateCommand
	{
		[Option("workspace-id", "w", Order = 1, Required = true, HelpText = "Fabric workspace ID.")]
		public string WorkspaceId { get; set; } = "";

		[Option("name", "n", Order = 2, Required = true, HelpText = "Lakehouse display name.")]
		public string Name { get; set; } = "";
	}

	[Command("fabric", "semantic-model", "list", HelpText = "List semantic models in a Fabric workspace.")]
	public class FabricSemanticModelListCommand
	{
		[Option("workspace-id", "w", Order = 1, Required = true, HelpText = "Fabric workspace ID.")]
		public string WorkspaceId { get; set; } = "";
	}

	[Command("fabric", "semantic-model", "refresh", HelpText = "Trigger a semantic model refresh.")]
	public class FabricSemanticModelRefreshCommand
	{
		[Option("workspace-id", "w", Order = 1, Required = true, HelpText = "Fabric workspace ID.")]
		public string WorkspaceId { get; set; } = "";

		[Option("semantic-model-id", "s", Order = 2, Required = true, HelpText = "Semantic model ID.")]
		public string SemanticModelId { get; set; } = "";
	}

	[Command("fabric", "link", "create", HelpText = "Create a Dataverse-to-Fabric link request.")]
	public class FabricLinkCreateCommand
	{
		[Option("dataverse-env", Order = 1, Required = true, HelpText = "Dataverse environment URL or ID.")]
		public string DataverseEnvironment { get; set; } = "";

		[Option("target-workspace", "w", Order = 2, Required = true, HelpText = "Target Fabric workspace ID.")]
		public string TargetWorkspaceId { get; set; } = "";
	}

	[Command("fabric", "link", "status", HelpText = "List Fabric Dataverse link requests.")]
	public class FabricLinkStatusCommand
	{
	}
}
