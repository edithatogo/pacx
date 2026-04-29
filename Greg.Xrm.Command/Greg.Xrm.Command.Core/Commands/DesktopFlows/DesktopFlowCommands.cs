using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.DesktopFlows
{
	[Command("desktop-flow", "list", HelpText = "List Power Automate Desktop flows.")]
	public class DesktopFlowListCommand
	{
		[Option("env", "e", Order = 1, HelpText = "Environment ID for API context.")]
		public string? EnvironmentId { get; set; }
	}

	[Command("desktop-flow", "get", HelpText = "Get a desktop flow definition preview.")]
	public class DesktopFlowGetCommand
	{
		[Option("id", "i", Order = 1, Required = true, HelpText = "Desktop flow workflow ID.")]
		public Guid Id { get; set; }
	}

	[Command("desktop-flow", "trigger", HelpText = "Trigger a desktop flow run.")]
	public class DesktopFlowTriggerCommand
	{
		[Option("env", "e", Order = 1, Required = true, HelpText = "Environment ID.")]
		public string EnvironmentId { get; set; } = "";

		[Option("id", "i", Order = 2, Required = true, HelpText = "Desktop flow workflow ID.")]
		public Guid Id { get; set; }

		[Option("machine-group", "g", Order = 3, Required = true, HelpText = "Target machine group name or ID.")]
		public string MachineGroup { get; set; } = "";

		[Option("input", Order = 4, HelpText = "Input key=value pair. Repeat for multiple inputs.")]
		public string? Input { get; set; }
	}

	[Command("desktop-flow", "run", "list", HelpText = "List desktop flow runs.")]
	public class DesktopFlowRunListCommand
	{
		[Option("env", "e", Order = 1, Required = true, HelpText = "Environment ID.")]
		public string EnvironmentId { get; set; } = "";

		[Option("id", "i", Order = 2, Required = true, HelpText = "Desktop flow workflow ID.")]
		public Guid Id { get; set; }
	}

	[Command("desktop-flow", "run", "get", HelpText = "Get a desktop flow run.")]
	public class DesktopFlowRunGetCommand
	{
		[Option("env", "e", Order = 1, Required = true, HelpText = "Environment ID.")]
		public string EnvironmentId { get; set; } = "";

		[Option("run-id", "r", Order = 2, Required = true, HelpText = "Run ID.")]
		public string RunId { get; set; } = "";
	}

	[Command("desktop-flow", "machine", "list", HelpText = "List desktop flow machines.")]
	public class DesktopFlowMachineListCommand : DesktopFlowEnvironmentCommand
	{
	}

	[Command("desktop-flow", "machine-group", "list", HelpText = "List desktop flow machine groups.")]
	public class DesktopFlowMachineGroupListCommand : DesktopFlowEnvironmentCommand
	{
	}

	[Command("desktop-flow", "machine-group", "assign", HelpText = "Assign a machine to a desktop flow machine group.")]
	public class DesktopFlowMachineGroupAssignCommand : DesktopFlowEnvironmentCommand
	{
		[Option("machine-id", "m", Order = 2, Required = true, HelpText = "Machine ID.")]
		public string MachineId { get; set; } = "";

		[Option("group-id", "g", Order = 3, Required = true, HelpText = "Machine group ID.")]
		public string GroupId { get; set; } = "";
	}

	[Command("desktop-flow", "scaffold", HelpText = "Create a Power Automate Desktop script skeleton.")]
	public class DesktopFlowScaffoldCommand
	{
		[Option("name", "n", Order = 1, Required = true, HelpText = "Flow name.")]
		public string Name { get; set; } = "";

		[Option("output", "o", Order = 2, HelpText = "Output .txt path.")]
		public string? OutputPath { get; set; }
	}

	[Command("approval", "list", HelpText = "List approvals in an environment.")]
	public class ApprovalListCommand : DesktopFlowEnvironmentCommand
	{
	}

	[Command("approval", "respond", HelpText = "Respond to an approval.")]
	public class ApprovalRespondCommand : DesktopFlowEnvironmentCommand
	{
		[Option("id", "i", Order = 2, Required = true, HelpText = "Approval ID.")]
		public string ApprovalId { get; set; } = "";

		[Option("decision", "d", Order = 3, Required = true, HelpText = "Decision: approve or reject.")]
		public string Decision { get; set; } = "";

		[Option("comment", "c", Order = 4, HelpText = "Optional response comment.")]
		public string? Comment { get; set; }
	}

	public abstract class DesktopFlowEnvironmentCommand
	{
		[Option("env", "e", Order = 1, Required = true, HelpText = "Environment ID.")]
		public string EnvironmentId { get; set; } = "";
	}
}
