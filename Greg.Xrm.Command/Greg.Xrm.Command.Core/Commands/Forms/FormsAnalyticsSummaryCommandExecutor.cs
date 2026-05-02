using System.Text.Json;

namespace Greg.Xrm.Command.Commands.Forms
{
	public class FormsAnalyticsSummaryCommandExecutor(
		IOutput output,
		Services.Forms.IFormsApiClient formsApiClient) : ICommandExecutor<FormsAnalyticsSummaryCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FormsAnalyticsSummaryCommand command, CancellationToken cancellationToken)
		{
			var ownerId = command.OwnerId ?? "me";
			try
			{
				var analytics = await formsApiClient.GetAnalyticsAsync(command.TenantId!, ownerId, command.OwnerType, command.FormId, cancellationToken);
				if (analytics?.RootElement is JsonElement root)
				{
					output.WriteLine($"Analytics for form <{command.FormId}>:", ConsoleColor.Cyan);
					foreach (var prop in root.EnumerateObject())
					{
						output.WriteLine($"  {prop.Name}: {prop.Value}");
					}
				}
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to get analytics: {ex.Message}", ex);
			}
		}
	}
}
