namespace Greg.Xrm.Command.Services.Telemetry
{
	public interface ITelemetryService
	{
		Task TrackCommandAsync(string commandName, int exitCode, TimeSpan duration, CancellationToken cancellationToken);
	}
}
