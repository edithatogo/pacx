using System.Text.Json;
using Greg.Xrm.Command.Services;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Tool
{
	public sealed class SourceListCommandExecutor(IOutput output, IStorage storage) : ICommandExecutor<SourceListCommand>
	{
		public Task<CommandResult> ExecuteAsync(SourceListCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var sourcesDir = storage.GetOrCreateStorageFolder().CreateSubdirectory("Sources");
				if (!sourcesDir.Exists)
				{
					output.WriteLine("No sources registered.", ConsoleColor.Yellow);
					return Task.FromResult(CommandResult.Success());
				}

				var sourceFiles = sourcesDir.GetFiles("*.json").OrderBy(f => f.Name).ToList();
				if (sourceFiles.Count == 0)
				{
					output.WriteLine("No sources registered.", ConsoleColor.Yellow);
					return Task.FromResult(CommandResult.Success());
				}

				var sources = new List<SourceDisplay>();
				foreach (var file in sourceFiles)
				{
					try
					{
						var json = File.ReadAllText(file.FullName);
						var entry = JsonSerializer.Deserialize<SourceEntry>(json);
						if (entry != null)
						{
							sources.Add(new SourceDisplay
							{
								Name = entry.Name,
								Url = entry.Url,
								Type = entry.Type,
								RegisteredAt = entry.RegisteredAt
							});
						}
					}
					catch { }
				}

				if (sources.Count == 0)
				{
					output.WriteLine("No sources registered.", ConsoleColor.Yellow);
					return Task.FromResult(CommandResult.Success());
				}

				output.WriteTable(
					sources,
					() => ["Name", "URL", "Type", "Registered"],
					s => [s.Name, s.Url, s.Type, s.RegisteredAt.ToString("yyyy-MM-dd HH:mm") + " UTC"]);

				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Failed to list sources: {ex.Message}", ex));
			}
		}

		private sealed class SourceDisplay
		{
			public string Name { get; set; } = string.Empty;
			public string Url { get; set; } = string.Empty;
			public string Type { get; set; } = string.Empty;
			public DateTimeOffset RegisteredAt { get; set; }
		}
	}
}
