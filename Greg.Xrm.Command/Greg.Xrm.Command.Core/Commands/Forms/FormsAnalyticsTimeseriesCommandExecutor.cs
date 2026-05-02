using System.Text.Json;

namespace Greg.Xrm.Command.Commands.Forms
{
	public class FormsAnalyticsTimeseriesCommandExecutor(
		IOutput output,
		Services.Forms.IFormsApiClient formsApiClient) : ICommandExecutor<FormsAnalyticsTimeseriesCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FormsAnalyticsTimeseriesCommand command, CancellationToken cancellationToken)
		{
			var ownerId = command.OwnerId ?? "me";
			try
			{
				var analytics = await formsApiClient.GetAnalyticsAsync(command.TenantId!, ownerId, command.OwnerType, command.FormId, cancellationToken);
				output.WriteLine($"Time series analytics (bucket: {command.Bucket}):", ConsoleColor.Cyan);
				if (analytics?.RootElement.TryGetProperty("timeseries", out var ts) == true)
				{
					output.WriteLine(JsonSerializer.Serialize(ts, new JsonSerializerOptions { WriteIndented = true }));
				}
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to get time series: {ex.Message}", ex);
			}
		}
	}
}
