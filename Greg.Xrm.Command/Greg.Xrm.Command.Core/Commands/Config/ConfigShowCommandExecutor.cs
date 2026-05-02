using Greg.Xrm.Command.Services.Config;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Config
{
	public class ConfigShowCommandExecutor(IOutput output) : ICommandExecutor<ConfigShowCommand>
	{
		private readonly ConfigLoader configLoader = new();

		public Task<CommandResult> ExecuteAsync(ConfigShowCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var configPath = configLoader.FindConfigPath();
			var config = configLoader.Load();

			if (configPath == null)
			{
				output.WriteLine("No PACX configuration file found.", ConsoleColor.Yellow);
				output.WriteLine("Create one at the current directory or ~/.pacx/pacx.config.json", ConsoleColor.Cyan);
				return Task.FromResult(CommandResult.Success());
			}

			output.Write("Config file: ").WriteLine(configPath, ConsoleColor.Cyan);
			output.WriteLine();

			if (config == null)
			{
				output.WriteLine("Failed to parse configuration file.", ConsoleColor.Red);
				return Task.FromResult(CommandResult.Success());
			}

			var items = new List<ConfigItem>
			{
				new("DefaultTenantId", config.DefaultTenantId ?? "(not set)"),
				new("DefaultEnvironmentUrl", config.DefaultEnvironmentUrl ?? "(not set)"),
				new("OutputFormat", config.OutputFormat ?? "table"),
				new("Connections", config.Connections.Count > 0
					? string.Join(", ", config.Connections.Keys.Select(k => $"{RedactKey(k)}"))
					: "(none)"),
				new("Options", config.Options.Count > 0
					? string.Join(", ", config.Options.Select(kvp => $"{kvp.Key}={kvp.Value}"))
					: "(none)")
			};

			output.WriteTable(
				items,
				() => ["Setting", "Value"],
				item => [item.Name, item.Value]);

			return Task.FromResult(CommandResult.Success());
		}

		private static string RedactKey(string key)
		{
			if (key.Contains("secret", StringComparison.OrdinalIgnoreCase)
				|| key.Contains("token", StringComparison.OrdinalIgnoreCase)
				|| key.Contains("password", StringComparison.OrdinalIgnoreCase))
			{
				return "***redacted***";
			}
			return key;
		}

		private sealed class ConfigItem
		{
			public string Name { get; }
			public string Value { get; }

			public ConfigItem(string name, string value)
			{
				Name = name;
				Value = value;
			}
		}
	}
}
