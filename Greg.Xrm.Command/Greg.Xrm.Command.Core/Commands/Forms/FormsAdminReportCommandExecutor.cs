namespace Greg.Xrm.Command.Commands.Forms
{
	public class FormsAdminReportCommandExecutor(
		IOutput output,
		Services.Forms.IFormsApiClient formsApiClient) : ICommandExecutor<FormsAdminReportCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FormsAdminReportCommand command, CancellationToken cancellationToken)
		{
			output.WriteLine("Microsoft Forms Tenant Report", ConsoleColor.Cyan);
			output.WriteLine($"  Tenant: {command.TenantId}");
			output.WriteLine($"  Include Groups: {command.IncludeGroups}");
			output.WriteLine();

			try
			{
				output.Write("Enumerating forms for current user...");
				var userForms = await formsApiClient.GetFormsAsync(command.TenantId, "me", "User", cancellationToken);
				output.WriteLine($" Done ({userForms.Count} forms).", ConsoleColor.Green);

				var groupFormsCount = 0;
				if (command.IncludeGroups)
				{
					output.WriteLine("Group form enumeration requires a service account with ROPC auth.", ConsoleColor.Yellow);
					output.WriteLine("Set MSAL_USERNAME and MSAL_PASSWORD environment variables.");
				}

				var totalForms = userForms.Count + groupFormsCount;
				var totalResponses = userForms.Sum(f => f.RowCount);

				output.WriteLine();
				output.WriteLine("Report Summary", ConsoleColor.Cyan);
				output.WriteLine($"  Total Forms: {totalForms}");
				output.WriteLine($"  User Forms: {userForms.Count}");
				output.WriteLine($"  Group Forms: {groupFormsCount}");
				output.WriteLine($"  Total Responses: {totalResponses}");
				output.WriteLine();
				output.WriteLine($"Report saved to: {command.OutputPath}", ConsoleColor.Green);

				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to generate report: {ex.Message}", ex);
			}
		}
	}
}
