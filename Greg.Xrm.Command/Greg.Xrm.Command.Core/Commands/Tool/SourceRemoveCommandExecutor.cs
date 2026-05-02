using Greg.Xrm.Command.Services;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Tool
{
	public sealed class SourceRemoveCommandExecutor(IOutput output, IStorage storage) : ICommandExecutor<SourceRemoveCommand>
	{
		public Task<CommandResult> ExecuteAsync(SourceRemoveCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var sourcesDir = storage.GetOrCreateStorageFolder().CreateSubdirectory("Sources");
				var filePath = Path.Combine(sourcesDir.FullName, $"{SanitizeFileName(command.Name)}.json");

				if (!File.Exists(filePath))
				{
					return Task.FromResult(CommandResult.Fail($"Source <{command.Name}> is not registered."));
				}

				File.Delete(filePath);
				output.WriteLine($"Source <{command.Name}> removed.", ConsoleColor.Green);

				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Failed to remove source: {ex.Message}", ex));
			}
		}

		private static string SanitizeFileName(string name)
		{
			var invalid = Path.GetInvalidFileNameChars();
			return string.Join("_", name.Split(invalid, StringSplitOptions.RemoveEmptyEntries));
		}
	}
}
