using System.Diagnostics;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Tooling;

namespace Greg.Xrm.Command.Commands.Tool
{
	public sealed class RunCommandExecutor(IOutput output) : ICommandExecutor<RunCommand>
	{
		public Task<CommandResult> ExecuteAsync(RunCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			try
			{
				var catalog = ToolCatalog.Load(command.CatalogPath);
				var tool = catalog.Tools.FirstOrDefault(entry =>
					string.Equals(entry.Id, command.Name, StringComparison.OrdinalIgnoreCase) ||
					string.Equals(entry.Name, command.Name, StringComparison.OrdinalIgnoreCase));

				if (tool is null)
				{
					return Task.FromResult(CommandResult.Fail($"Tool <{command.Name}> was not found in <{command.CatalogPath}>."));
				}

				output.WriteLine($"{tool.Name} ({tool.Category})", ConsoleColor.Green);
				output.WriteLine(tool.Summary);

				if (!string.IsNullOrWhiteSpace(tool.HomePage))
				{
					output.WriteLine($"Home page: {tool.HomePage}");

					if (command.OpenHomePage)
					{
						Process.Start(new ProcessStartInfo(tool.HomePage)
						{
							UseShellExecute = true
						});
						output.WriteLine("Opened homepage.", ConsoleColor.Green);
					}
				}
				else if (command.OpenHomePage)
				{
					return Task.FromResult(CommandResult.Fail($"Tool <{tool.Name}> does not declare a homepage to open."));
				}

				if (tool.Capabilities.Count > 0)
				{
					output.WriteLine("Capabilities:");
					foreach (var capability in tool.Capabilities)
					{
						output.WriteLine($"- {capability}");
					}
				}

				return Task.FromResult(CommandResult.Success());
			}
			catch (Exception ex)
			{
				return Task.FromResult(CommandResult.Fail($"Unable to run tool from <{command.CatalogPath}>: {ex.Message}", ex));
			}
		}
	}
}
