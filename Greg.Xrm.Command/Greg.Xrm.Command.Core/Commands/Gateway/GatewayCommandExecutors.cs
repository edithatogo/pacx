using System.Text.Json;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.PowerBi;

namespace Greg.Xrm.Command.Commands.Gateway
{
	internal static class GatewayCommandOutput
	{
		public static CommandResult WriteJson(IOutput output, JsonDocument document)
		{
			using (document)
			{
				output.WriteLine(JsonSerializer.Serialize(document.RootElement, new JsonSerializerOptions { WriteIndented = true }));
			}
			return CommandResult.Success();
		}
	}

	public class GatewayListCommandExecutor(
		IPowerBiClient client,
		IOutput output) : ICommandExecutor<GatewayListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(GatewayListCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await client.ListGatewaysAsync(cancellationToken).ConfigureAwait(false);
				return GatewayCommandOutput.WriteJson(output, result);
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to list gateways: {ex.Message}", ex);
			}
		}
	}

	public class GatewayGetCommandExecutor(
		IPowerBiClient client,
		IOutput output) : ICommandExecutor<GatewayGetCommand>
	{
		public async Task<CommandResult> ExecuteAsync(GatewayGetCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var result = await client.GetGatewayAsync(command.GatewayId, cancellationToken).ConfigureAwait(false);
				return GatewayCommandOutput.WriteJson(output, result);
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to get gateway: {ex.Message}", ex);
			}
		}
	}
}
