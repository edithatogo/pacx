using System.Text.Json;
using Greg.Xrm.Command.Services.DataverseGaps;
using Greg.Xrm.Command.Services.Output;
using System.Globalization;

namespace Greg.Xrm.Command.Commands.DataverseGaps
{
	internal static class DataverseGapOutput
	{
		private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

		public static async Task<CommandResult> WriteJsonAsync(IOutput output, JsonDocument document, string? filePath = null, CancellationToken cancellationToken = default)
		{
			using (document)
			{
				var json = JsonSerializer.Serialize(document.RootElement, JsonOptions);
				if (string.IsNullOrWhiteSpace(filePath))
				{
					output.WriteLine(json);
				}
				else
				{
					await File.WriteAllTextAsync(filePath, json, cancellationToken).ConfigureAwait(false);
				}
			}

			return CommandResult.Success();
		}
	}

	public class BusinessRuleListCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<BusinessRuleListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(BusinessRuleListCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.QueryAsync("workflow", WorkflowColumns, new Dictionary<string, object?> { ["category"] = 2, ["primaryentity"] = command.Table }, cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);

		internal static readonly string[] WorkflowColumns = ["workflowid", "name", "primaryentity", "category", "statecode", "statuscode", "createdon", "modifiedon"];
	}

	public class BusinessRuleExportCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<BusinessRuleExportCommand>
	{
		public async Task<CommandResult> ExecuteAsync(BusinessRuleExportCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.ExportWorkflowAsync(command.Id, cancellationToken).ConfigureAwait(false), command.FilePath, cancellationToken).ConfigureAwait(false);
	}

	public class BusinessRuleImportCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<BusinessRuleImportCommand>
	{
		public async Task<CommandResult> ExecuteAsync(BusinessRuleImportCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.ImportWorkflowAsync(command.FilePath, command.Table, 2, cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public class BusinessRuleActivateCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<BusinessRuleActivateCommand>
	{
		public async Task<CommandResult> ExecuteAsync(BusinessRuleActivateCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.SetStateAsync("workflow", command.Id, 1, 2, cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public class BusinessRuleDeactivateCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<BusinessRuleDeactivateCommand>
	{
		public async Task<CommandResult> ExecuteAsync(BusinessRuleDeactivateCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.SetStateAsync("workflow", command.Id, 0, 1, cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public class BpfListCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<BpfListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(BpfListCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.QueryAsync("workflow", BusinessRuleListCommandExecutor.WorkflowColumns, new Dictionary<string, object?> { ["category"] = 4 }, cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public class BpfExportCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<BpfExportCommand>
	{
		public async Task<CommandResult> ExecuteAsync(BpfExportCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.ExportWorkflowAsync(command.Id, cancellationToken).ConfigureAwait(false), command.FilePath, cancellationToken).ConfigureAwait(false);
	}

	public class BpfImportCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<BpfImportCommand>
	{
		public async Task<CommandResult> ExecuteAsync(BpfImportCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.ImportWorkflowAsync(command.FilePath, null, 4, cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public class BpfActivateCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<BpfActivateCommand>
	{
		public async Task<CommandResult> ExecuteAsync(BpfActivateCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.SetStateAsync("workflow", command.Id, 1, 2, cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public class DdrListCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<DdrListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DdrListCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.QueryAsync("duplicaterule", ["duplicateruleid", "name", "baseentityname", "matchingentityname", "statecode", "statuscode"], new Dictionary<string, object?>(), cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public class DdrRunCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<DdrRunCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DdrRunCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.ExecuteActionAsync("PublishDuplicateRule", new Dictionary<string, object?> { ["DuplicateRuleId"] = command.RuleId }, cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public class DdrEnableCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<DdrEnableCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DdrEnableCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.SetStateAsync("duplicaterule", command.RuleId, 1, 2, cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public class DdrDisableCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<DdrDisableCommand>
	{
		public async Task<CommandResult> ExecuteAsync(DdrDisableCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.SetStateAsync("duplicaterule", command.RuleId, 0, 1, cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public class AuditStatusCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<AuditStatusCommand>
	{
		public async Task<CommandResult> ExecuteAsync(AuditStatusCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.QueryAsync("organization", ["organizationid", "name", "isauditenabled", "auditretentionperiodv2"], new Dictionary<string, object?>(), cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public class AuditEnableTableCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<AuditEnableTableCommand>
	{
		public async Task<CommandResult> ExecuteAsync(AuditEnableTableCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.ExecuteActionAsync("UpdateEntity", new Dictionary<string, object?> { ["LogicalName"] = command.Table, ["IsAuditEnabled"] = true }, cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public class AuditExportCommandExecutor(IDataverseGapClient client) : ICommandExecutor<AuditExportCommand>
	{
		public async Task<CommandResult> ExecuteAsync(AuditExportCommand command, CancellationToken cancellationToken)
		{
			if (!string.Equals(command.Format, "jsonl", StringComparison.OrdinalIgnoreCase))
			{
				return CommandResult.Fail("Only jsonl audit export is currently supported.");
			}

			var document = await client.QueryAsync("audit", ["auditid", "objectid", "action", "operation", "createdon", "userid", "attributemask", "changedata"], new Dictionary<string, object?> { ["objecttypecode"] = command.Table }, cancellationToken).ConfigureAwait(false);
			using (document)
			{
				await using var writer = new StreamWriter(command.FilePath);
				foreach (var item in document.RootElement.GetProperty("value").EnumerateArray())
				{
					if (item.TryGetProperty("createdon", out var createdOn) && DateTime.TryParse(createdOn.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsed) && parsed < command.Since)
					{
						continue;
					}

					await writer.WriteLineAsync(item.GetRawText()).ConfigureAwait(false);
				}
			}

			return CommandResult.Success();
		}
	}

	public class FspListCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<FspListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FspListCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.QueryAsync("fieldsecurityprofile", ["fieldsecurityprofileid", "name", "description"], new Dictionary<string, object?>(), cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public class FspApplyCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<FspApplyCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FspApplyCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.CreateAsync("systemuserprofiles", new Dictionary<string, object?> { ["fieldsecurityprofileid"] = command.ProfileId, ["systemuserid"] = command.UserOrTeamId }, cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public class FspBulkAssignCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<FspBulkAssignCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FspBulkAssignCommand command, CancellationToken cancellationToken)
		{
			if (!File.Exists(command.FilePath))
			{
				return CommandResult.Fail($"CSV file not found: {command.FilePath}");
			}

			var assigned = 0;
			foreach (var row in await File.ReadAllLinesAsync(command.FilePath, cancellationToken).ConfigureAwait(false))
			{
				var value = row.Split(',')[0].Trim();
				if (string.Equals(value, "userOrTeamId", StringComparison.OrdinalIgnoreCase) || !Guid.TryParse(value, out var id))
				{
					continue;
				}

				using var _ = await client.CreateAsync("systemuserprofiles", new Dictionary<string, object?> { ["fieldsecurityprofileid"] = command.ProfileId, ["systemuserid"] = id }, cancellationToken).ConfigureAwait(false);
				assigned++;
			}

			output.WriteLine($"Assigned field security profile to {assigned} principal(s).");
			return CommandResult.Success();
		}
	}

	public class EndpointListCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<EndpointListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(EndpointListCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.QueryAsync("serviceendpoint", ["serviceendpointid", "name", "url", "authtype", "contract", "messageformat"], new Dictionary<string, object?>(), cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public class EndpointRegisterCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<EndpointRegisterCommand>
	{
		public async Task<CommandResult> ExecuteAsync(EndpointRegisterCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.CreateAsync("serviceendpoint", new Dictionary<string, object?> { ["name"] = command.Name ?? command.Url, ["url"] = command.Url, ["authtype"] = command.Auth }, cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public class EndpointDeleteCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<EndpointDeleteCommand>
	{
		public async Task<CommandResult> ExecuteAsync(EndpointDeleteCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.DeleteAsync("serviceendpoint", command.Id, cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public class ColumnFileUploadCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<ColumnFileUploadCommand>
	{
		public async Task<CommandResult> ExecuteAsync(ColumnFileUploadCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.UploadFileAsync(command.Table, command.RecordId, command.Column, command.FilePath, cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public class ColumnRollupRecalculateCommandExecutor(IDataverseGapClient client, IOutput output) : ICommandExecutor<ColumnRollupRecalculateCommand>
	{
		public async Task<CommandResult> ExecuteAsync(ColumnRollupRecalculateCommand command, CancellationToken cancellationToken)
			=> await DataverseGapOutput.WriteJsonAsync(output, await client.ExecuteActionAsync("CalculateRollupField", new Dictionary<string, object?> { ["Target"] = command.RecordId, ["FieldName"] = command.Column, ["EntityName"] = command.Table }, cancellationToken).ConfigureAwait(false), cancellationToken: cancellationToken).ConfigureAwait(false);
	}
}
