using System.Text.Json;
using Greg.Xrm.Command.Services;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Tool
{
	public sealed class SourceAddCommandExecutor(IOutput output, IStorage storage) : ICommandExecutor<SourceAddCommand>
	{
		public Task<CommandResult> ExecuteAsync(SourceAddCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var sourcesDir = storage.GetOrCreateStorageFolder().CreateSubdirectory("Sources");
				var filePath = Path.Combine(sourcesDir.FullName, $"{SanitizeFileName(command.Name)}.json");

				if (File.Exists(filePath))
				{
					return Task.FromResult(CommandResult.Fail($"A source named <{command.Name}> is already registered. Use 'tool source remove' first if you want to replace it."));
				}

				var sourceEntry = new SourceEntry
				{
					Name = command.Name,
					Url = command.Url,
					Type = command.Type,
					PersonalAccessToken = command.PersonalAccessToken,
					RegisteredAt = DateTimeOffset.UtcNow
				};

				var json = JsonSerializer.Serialize(sourceEntry, new JsonSerializerOptions { WriteIndented = true });
				File.WriteAllText(filePath, json);

				output.WriteLine($"Source <{command.Name}> registered.", ConsoleColor.Green);
				output.WriteLine($"  URL: {command.Url}");
				output.WriteLine($"  Type: {command.Type}");

				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Failed to register source: {ex.Message}", ex));
			}
		}

		private static string SanitizeFileName(string name)
		{
			var invalid = Path.GetInvalidFileNameChars();
			return string.Join("_", name.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
		}
	}

	internal sealed class SourceEntry
	{
		public string Name { get; set; } = string.Empty;
		public string Url { get; set; } = string.Empty;
		public string Type { get; set; } = "nuget";
		public string? PersonalAccessToken { get; set; }
		public DateTimeOffset RegisteredAt { get; set; }
	}
}
