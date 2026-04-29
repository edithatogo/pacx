using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.PowerBi
{
	[Command("dataset", "list", HelpText = "List Power BI datasets in a workspace.")]
	public class DatasetListCommand
	{
		[Option("workspace-id", "w", Order = 1, Required = true, HelpText = "Power BI workspace ID.")]
		public string WorkspaceId { get; set; } = "";
	}

	[Command("dataset", "publish", HelpText = "Publish a PBIX file to a Power BI workspace.")]
	public class DatasetPublishCommand
	{
		[Option("workspace-id", "w", Order = 1, Required = true, HelpText = "Power BI workspace ID.")]
		public string WorkspaceId { get; set; } = "";

		[Option("pbix", "p", Order = 2, Required = true, HelpText = "Path to the PBIX file.")]
		public string PbixPath { get; set; } = "";

		[Option("name", "n", Order = 3, HelpText = "Dataset display name. Defaults to the PBIX file name.")]
		public string? Name { get; set; }
	}

	[Command("dataset", "clone", HelpText = "Clone a Power BI dataset into another workspace.")]
	public class DatasetCloneCommand
	{
		[Option("source-id", "s", Order = 1, Required = true, HelpText = "Source dataset ID.")]
		public string SourceId { get; set; } = "";

		[Option("target-workspace-id", "w", Order = 2, Required = true, HelpText = "Target workspace ID.")]
		public string TargetWorkspaceId { get; set; } = "";
	}

	[Command("dataset", "delete", HelpText = "Delete a Power BI dataset.")]
	public class DatasetDeleteCommand
	{
		[Option("workspace-id", "w", Order = 1, Required = true, HelpText = "Power BI workspace ID.")]
		public string WorkspaceId { get; set; } = "";

		[Option("id", "i", Order = 2, Required = true, HelpText = "Dataset ID.")]
		public string DatasetId { get; set; } = "";
	}

	[Command("dataset", "rls", "list", HelpText = "List Power BI dataset RLS user assignments.")]
	public class DatasetRlsListCommand
	{
		[Option("workspace-id", "w", Order = 1, Required = true, HelpText = "Power BI workspace ID.")]
		public string WorkspaceId { get; set; } = "";

		[Option("dataset-id", "d", Order = 2, Required = true, HelpText = "Dataset ID.")]
		public string DatasetId { get; set; } = "";
	}

	[Command("dataset", "rls", "apply", HelpText = "Apply a Power BI dataset RLS user assignment.")]
	public class DatasetRlsApplyCommand
	{
		[Option("workspace-id", "w", Order = 1, Required = true, HelpText = "Power BI workspace ID.")]
		public string WorkspaceId { get; set; } = "";

		[Option("dataset-id", "d", Order = 2, Required = true, HelpText = "Dataset ID.")]
		public string DatasetId { get; set; } = "";

		[Option("user", "u", Order = 3, Required = true, HelpText = "User principal name.")]
		public string User { get; set; } = "";

		[Option("role", "r", Order = 4, Required = true, HelpText = "Dataset role name.")]
		public string Role { get; set; } = "";
	}

	[Command("dataset", "refresh", "trigger", HelpText = "Trigger a Power BI dataset refresh.")]
	public class DatasetRefreshTriggerCommand : DatasetCommandBase
	{
	}

	[Command("dataset", "refresh", "status", HelpText = "List Power BI dataset refresh history.")]
	public class DatasetRefreshStatusCommand : DatasetCommandBase
	{
	}

	[Command("dataset", "refresh", "schedule", HelpText = "Set a simple Power BI dataset refresh schedule note.")]
	public class DatasetRefreshScheduleCommand : DatasetCommandBase
	{
		[Option("cron", "c", Order = 3, Required = true, HelpText = "Cron expression to store as schedule metadata.")]
		public string Cron { get; set; } = "";
	}

	public abstract class DatasetCommandBase
	{
		[Option("workspace-id", "w", Order = 1, Required = true, HelpText = "Power BI workspace ID.")]
		public string WorkspaceId { get; set; } = "";

		[Option("id", "i", Order = 2, Required = true, HelpText = "Dataset ID.")]
		public string DatasetId { get; set; } = "";
	}

	[Command("pipeline", "list", HelpText = "List Power BI deployment pipelines.")]
	public class PipelineListCommand
	{
	}

	[Command("pipeline", "stage-assign", HelpText = "Assign a Power BI workspace to a deployment pipeline stage.")]
	public class PipelineStageAssignCommand
	{
		[Option("pipeline-id", "p", Order = 1, Required = true, HelpText = "Deployment pipeline ID.")]
		public string PipelineId { get; set; } = "";

		[Option("stage", "s", Order = 2, Required = true, HelpText = "Stage name: dev, test, or prod.")]
		public string Stage { get; set; } = "";

		[Option("workspace-id", "w", Order = 3, Required = true, HelpText = "Power BI workspace ID.")]
		public string WorkspaceId { get; set; } = "";
	}

	[Command("pipeline", "deploy", HelpText = "Deploy content between Power BI deployment pipeline stages.")]
	public class PipelineDeployCommand
	{
		[Option("pipeline-id", "p", Order = 1, Required = true, HelpText = "Deployment pipeline ID.")]
		public string PipelineId { get; set; } = "";

		[Option("source-stage", Order = 2, Required = true, HelpText = "Source stage name.")]
		public string SourceStage { get; set; } = "";

		[Option("target-stage", Order = 3, Required = true, HelpText = "Target stage name.")]
		public string TargetStage { get; set; } = "";
	}

	[Command("capacity", "list", HelpText = "List Power BI capacities.")]
	public class CapacityListCommand
	{
	}

	[Command("capacity", "workspace-assign", HelpText = "Assign a Power BI workspace to a capacity.")]
	public class CapacityWorkspaceAssignCommand
	{
		[Option("capacity-id", "c", Order = 1, Required = true, HelpText = "Capacity ID.")]
		public string CapacityId { get; set; } = "";

		[Option("workspace-id", "w", Order = 2, Required = true, HelpText = "Power BI workspace ID.")]
		public string WorkspaceId { get; set; } = "";
	}

	[Command("capacity", "metrics", HelpText = "Get Power BI capacity metrics.")]
	public class CapacityMetricsCommand
	{
		[Option("capacity-id", "c", Order = 1, Required = true, HelpText = "Capacity ID.")]
		public string CapacityId { get; set; } = "";
	}
}
