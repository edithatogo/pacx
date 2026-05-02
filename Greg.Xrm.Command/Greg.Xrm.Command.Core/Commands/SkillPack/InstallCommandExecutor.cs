using Greg.Xrm.Command.Services;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Tooling;

namespace Greg.Xrm.Command.Commands.SkillPack
{
	public sealed class InstallCommandExecutor(IOutput output, IStorage storage) : ICommandExecutor<InstallCommand>
	{
		public Task<CommandResult> ExecuteAsync(InstallCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var catalog = SkillPackCatalog.Load(command.CatalogPath);
				var pack = catalog.Packs.FirstOrDefault(p =>
					string.Equals(p.Id, command.Id, StringComparison.OrdinalIgnoreCase));

				if (pack is null)
				{
					return Task.FromResult(CommandResult.Fail($"Skill pack <{command.Id}> was not found in <{command.CatalogPath}>."));
				}

				output.WriteLine($"{pack.Name} v{pack.Version}", ConsoleColor.Green);
				output.WriteLine(pack.Description);
				output.WriteLine($"Author: {pack.Author}");
				output.WriteLine($"Tags: {string.Join(", ", pack.Tags)}");

				if (pack.Capabilities.Count > 0)
				{
					output.WriteLine("Capabilities:");
					foreach (var cap in pack.Capabilities)
					{
						output.WriteLine($"  - {cap}");
					}
				}

				if (pack.Dependencies.Count > 0)
				{
					output.WriteLine("Dependencies:");
					foreach (var dep in pack.Dependencies)
					{
						output.WriteLine($"  - {dep}");
					}
				}

				if (command.DryRun)
				{
					output.WriteLine("Dry-run mode: no changes were made.", ConsoleColor.Yellow);
					return Task.FromResult(CommandResult.Success());
				}

				var packsDir = storage.GetOrCreateStorageFolder().CreateSubdirectory("SkillPacks");
				var packDir = packsDir.CreateSubdirectory(pack.Id);
				var markerFile = Path.Combine(packDir.FullName, ".installed");
				File.WriteAllText(markerFile,
					$"Installed: {pack.Name} v{pack.Version} on {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");

				output.WriteLine($"Skill pack <{pack.Name}> installed.", ConsoleColor.Green);

				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Failed to install skill pack: {ex.Message}", ex));
			}
		}
	}
}
