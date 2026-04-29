using System.Net.Http.Json;
using Greg.Xrm.Command.Commands.Telemetry;
using Greg.Xrm.Command.Diagnostics;
using Greg.Xrm.Command.Services.Settings;

namespace Greg.Xrm.Command.Services.Telemetry
{
	public class TelemetryService(
		ISettingsRepository settingsRepository,
		IHttpClientFactory httpClientFactory,
		ICorrelationIdProvider correlationIdProvider) : ITelemetryService
	{
		public async Task TrackCommandAsync(string commandName, int exitCode, TimeSpan duration, CancellationToken cancellationToken)
		{
			if (IsOptedOutByEnvironment())
			{
				return;
			}

			var settings = await settingsRepository.GetAsync<TelemetrySettings>("telemetry", cancellationToken).ConfigureAwait(false);
			if (settings?.Enabled != true || string.IsNullOrWhiteSpace(settings.Endpoint))
			{
				return;
			}

			var payload = new
			{
				resource = "pacx",
				eventName = "command.executed",
				command = commandName,
				exitCode,
				durationMs = Math.Round(duration.TotalMilliseconds, 2),
				correlationId = correlationIdProvider.Current,
				timestamp = DateTimeOffset.UtcNow,
			};

			try
			{
				var client = httpClientFactory.CreateClient(string.Empty);
				await client.PostAsJsonAsync(settings.Endpoint, payload, cancellationToken).ConfigureAwait(false);
			}
			catch
			{
				// Telemetry must never change command behavior.
			}
		}

		private static bool IsOptedOutByEnvironment()
		{
			return string.Equals(Environment.GetEnvironmentVariable("DOTNET_CLI_TELEMETRY_OPTOUT"), "1", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(Environment.GetEnvironmentVariable("PACX_TELEMETRY_OPTOUT"), "1", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(Environment.GetEnvironmentVariable("PACX_TELEMETRY_OPTOUT"), "true", StringComparison.OrdinalIgnoreCase);
		}
	}
}
