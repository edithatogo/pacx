using Greg.Xrm.Command;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Forms
{
	internal static class FormsAdvancedOutput
	{
		public static CommandResult Planned(IOutput output, string title, string endpoint, params string[] details)
		{
			output.WriteLine(title, ConsoleColor.Cyan);
			foreach (var detail in details)
			{
				output.WriteLine($"  {detail}");
			}

			output.WriteLine();
			output.WriteLine("Note: This command requires Microsoft Forms or Customer Voice API authentication.", ConsoleColor.Yellow);
			output.WriteLine($"API: {endpoint}");
			return CommandResult.Success();
		}
	}

	public class FormsBranchingExportCommandExecutor(IOutput output) : ICommandExecutor<FormsBranchingExportCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsBranchingExportCommand command, CancellationToken cancellationToken)
			=> Task.FromResult(FormsAdvancedOutput.Planned(output, $"Forms Branching Export — {command.FormId}", "/formapi/api/{tenantId}/users/{ownerId}/forms('{formId}')/branching", "Outputs JSON branching rules plus a human-readable summary."));
	}

	public class FormsAnalyticsSummaryCommandExecutor(IOutput output) : ICommandExecutor<FormsAnalyticsSummaryCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsAnalyticsSummaryCommand command, CancellationToken cancellationToken)
			=> Task.FromResult(FormsAdvancedOutput.Planned(output, $"Forms Analytics Summary — {command.FormId}", "/formapi/api/{tenantId}/users/{ownerId}/forms('{formId}')/analytics", "Includes submissions, completion rate, median duration, and question dropoff."));
	}

	public class FormsAnalyticsTimeseriesCommandExecutor(IOutput output) : ICommandExecutor<FormsAnalyticsTimeseriesCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsAnalyticsTimeseriesCommand command, CancellationToken cancellationToken)
			=> Task.FromResult(FormsAdvancedOutput.Planned(output, $"Forms Analytics Timeseries — {command.FormId}", "/formapi/api/{tenantId}/users/{ownerId}/forms('{formId}')/responses", $"Bucket: {command.Bucket}"));
	}

	public class FormsTemplateListCommandExecutor(IOutput output) : ICommandExecutor<FormsTemplateListCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsTemplateListCommand command, CancellationToken cancellationToken)
			=> Task.FromResult(FormsAdvancedOutput.Planned(output, "Forms Template List", "/formapi/api/{tenantId}/templates", $"Scope: {(command.Organization ? "organization" : "current user")}"));
	}

	public class FormsTemplateCreateCommandExecutor(IOutput output) : ICommandExecutor<FormsTemplateCreateCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsTemplateCreateCommand command, CancellationToken cancellationToken)
			=> Task.FromResult(FormsAdvancedOutput.Planned(output, $"Forms Template Create — {command.FormId}", "/formapi/api/{tenantId}/templates", $"Scope: {command.Scope}"));
	}

	public class FormsTemplateShareCommandExecutor(IOutput output) : ICommandExecutor<FormsTemplateShareCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsTemplateShareCommand command, CancellationToken cancellationToken)
			=> Task.FromResult(FormsAdvancedOutput.Planned(output, $"Forms Template Share — {command.TemplateId}", "/formapi/api/{tenantId}/templates/{templateId}/shares", $"Group: {command.GroupId}"));
	}

	public class FormsShareCommandExecutor(IOutput output) : ICommandExecutor<FormsShareCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsShareCommand command, CancellationToken cancellationToken)
			=> Task.FromResult(FormsAdvancedOutput.Planned(output, $"Forms Share — {command.FormId}", "/formapi/api/{tenantId}/users/{ownerId}/forms('{formId}')/permissions", $"Group: {command.GroupId}", $"Role: {command.Role}"));
	}

	public class FormsOwnershipTransferCommandExecutor(IOutput output) : ICommandExecutor<FormsOwnershipTransferCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsOwnershipTransferCommand command, CancellationToken cancellationToken)
			=> Task.FromResult(FormsAdvancedOutput.Planned(output, $"Forms Ownership Transfer — {command.FormId}", "/formapi/api/{tenantId}/users/{ownerId}/forms('{formId}')/owner", $"Target owner: {command.TargetUserPrincipalName}"));
	}

	public class FormsCustomerVoiceListCommandExecutor(IOutput output) : ICommandExecutor<FormsCustomerVoiceListCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsCustomerVoiceListCommand command, CancellationToken cancellationToken)
			=> Task.FromResult(FormsAdvancedOutput.Planned(output, "Customer Voice Survey List", "/customerVoice/surveys"));
	}

	public class FormsCustomerVoiceSendCommandExecutor(IOutput output) : ICommandExecutor<FormsCustomerVoiceSendCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsCustomerVoiceSendCommand command, CancellationToken cancellationToken)
			=> Task.FromResult(FormsAdvancedOutput.Planned(output, $"Customer Voice Send — {command.SurveyId}", "/customerVoice/surveys/{surveyId}/send", $"Recipients: {command.Recipients}"));
	}

	public class FormsCustomerVoiceResponsesExportCommandExecutor(IOutput output) : ICommandExecutor<FormsCustomerVoiceResponsesExportCommand>
	{
		public Task<CommandResult> ExecuteAsync(FormsCustomerVoiceResponsesExportCommand command, CancellationToken cancellationToken)
			=> Task.FromResult(FormsAdvancedOutput.Planned(output, $"Customer Voice Responses Export — {command.SurveyId}", "/customerVoice/surveys/{surveyId}/responses", $"Output: {command.OutputPath ?? "(stdout)"}"));
	}
}
