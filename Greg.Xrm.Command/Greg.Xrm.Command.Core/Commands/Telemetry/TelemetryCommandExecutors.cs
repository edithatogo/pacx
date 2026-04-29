using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Settings;

namespace Greg.Xrm.Command.Commands.Telemetry
{
	public class TelemetryEnableCommandExecutor(ISettingsRepository settingsRepository, IOutput output) : ICommandExecutor<TelemetryEnableCommand>
	{
		public async Task<CommandResult> ExecuteAsync(TelemetryEnableCommand command, CancellationToken cancellationToken)
		{
			var settings = new TelemetrySettings
			{
				Enabled = true,
				Endpoint = string.IsNullOrWhiteSpace(command.Endpoint) ? null : command.Endpoint,
			};

			await settingsRepository.SetAsync("telemetry", settings, cancellationToken).ConfigureAwait(false);
			output.WriteLine("Telemetry enabled.", ConsoleColor.Green);
			if (settings.Endpoint == null)
			{
				output.WriteLine("No OTLP endpoint configured; telemetry remains local/no-op until an endpoint is set.", ConsoleColor.Yellow);
			}

			return CommandResult.Success();
		}
	}

	public class TelemetryDisableCommandExecutor(ISettingsRepository settingsRepository, IOutput output) : ICommandExecutor<TelemetryDisableCommand>
	{
		public async Task<CommandResult> ExecuteAsync(TelemetryDisableCommand command, CancellationToken cancellationToken)
		{
			await settingsRepository.SetAsync("telemetry", new TelemetrySettings { Enabled = false }, cancellationToken).ConfigureAwait(false);
			output.WriteLine("Telemetry disabled.", ConsoleColor.Green);
			return CommandResult.Success();
		}
	}

	public class TelemetryStatusCommandExecutor(ISettingsRepository settingsRepository, IOutput output) : ICommandExecutor<TelemetryStatusCommand>
	{
		public async Task<CommandResult> ExecuteAsync(TelemetryStatusCommand command, CancellationToken cancellationToken)
		{
			var settings = await settingsRepository.GetAsync<TelemetrySettings>("telemetry", cancellationToken).ConfigureAwait(false) ?? new TelemetrySettings();
			var disabledByEnvironment = IsOptedOutByEnvironment();

			output.WriteLine($"Telemetry setting: {(settings.Enabled ? "enabled" : "disabled")}");
			output.WriteLine($"Environment opt-out: {(disabledByEnvironment ? "yes" : "no")}");
			output.WriteLine($"OTLP endpoint: {(string.IsNullOrWhiteSpace(settings.Endpoint) ? "(not configured)" : settings.Endpoint)}");
			return CommandResult.Success();
		}

		private static bool IsOptedOutByEnvironment()
		{
			return string.Equals(Environment.GetEnvironmentVariable("DOTNET_CLI_TELEMETRY_OPTOUT"), "1", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(Environment.GetEnvironmentVariable("PACX_TELEMETRY_OPTOUT"), "1", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(Environment.GetEnvironmentVariable("PACX_TELEMETRY_OPTOUT"), "true", StringComparison.OrdinalIgnoreCase);
		}
	}
}
