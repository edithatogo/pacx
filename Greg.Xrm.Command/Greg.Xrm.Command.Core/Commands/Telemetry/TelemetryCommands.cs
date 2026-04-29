using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Telemetry
{
	[Command("telemetry", "enable", HelpText = "Enable opt-in PACX telemetry.")]
	public class TelemetryEnableCommand
	{
		[Option("endpoint", "e", HelpText = "Optional OTLP endpoint. If omitted, PACX records consent but does not export telemetry.")]
		public string? Endpoint { get; set; }
	}

	[Command("telemetry", "disable", HelpText = "Disable PACX telemetry.")]
	public class TelemetryDisableCommand
	{
	}

	[Command("telemetry", "status", HelpText = "Show PACX telemetry consent and endpoint status.")]
	public class TelemetryStatusCommand
	{
	}
}
