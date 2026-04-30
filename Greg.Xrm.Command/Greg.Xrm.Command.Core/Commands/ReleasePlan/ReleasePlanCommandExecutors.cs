using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.ReleasePlan;
using System.Text.Json;

namespace Greg.Xrm.Command.Commands.ReleasePlan
{
	internal static class ReleasePlanOutput
	{
		private static readonly JsonSerializerOptions JsonOptions = new()
		{
			WriteIndented = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		};

		public static CommandResult WriteJson(IOutput output, object data)
		{
			output.WriteLine(JsonSerializer.Serialize(data, JsonOptions));
			return CommandResult.Success();
		}

		public static CommandResult WriteTable(IOutput output, List<ReleasePlanItem> items)
		{
			if (items.Count == 0)
			{
				output.WriteLine("No release plan items found.", ConsoleColor.Yellow);
				return CommandResult.Success();
			}

			foreach (var item in items)
			{
				output.Write("  ");
				output.Write($"[{item.Status}]", GetStatusColor(item.Status));
				output.Write($" {item.Product} — ");
				output.WriteLine($"{item.Title}", ConsoleColor.Cyan);
				output.Write($"    ID: {item.Id}");
				if (!string.IsNullOrWhiteSpace(item.Wave))
				{
					output.Write($" | Wave: {item.Wave}");
				}
				output.WriteLine(string.Empty);
			}

			output.WriteLine($"Total: {items.Count} item(s).", ConsoleColor.Green);
			return CommandResult.Success();
		}

		private static ConsoleColor GetStatusColor(string? status)
		{
			return status?.ToLowerInvariant() switch
			{
				"launched" => ConsoleColor.Green,
				"rollingout" => ConsoleColor.Blue,
				"indevelopment" or "in development" => ConsoleColor.Yellow,
				"deprecated" => ConsoleColor.Red,
				"cancelled" => ConsoleColor.Red,
				_ => ConsoleColor.Gray,
			};
		}
	}

	public class ReleasePlanListCommandExecutor(
		IReleasePlanService service,
		IOutput output) : ICommandExecutor<ReleasePlanListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(
			ReleasePlanListCommand command,
			CancellationToken cancellationToken)
		{
			var filter = new ReleasePlanFilter
			{
				Product = command.Product,
				Status = command.Status,
				Category = command.Category,
				MaxResults = command.MaxResults > 0 ? command.MaxResults : 50,
			};

			var items = await service.GetItemsAsync(filter, cancellationToken)
				.ConfigureAwait(false);

			return ReleasePlanOutput.WriteTable(output, items);
		}
	}

	public class ReleasePlanGetCommandExecutor(
		IReleasePlanService service,
		IOutput output) : ICommandExecutor<ReleasePlanGetCommand>
	{
		public async Task<CommandResult> ExecuteAsync(
			ReleasePlanGetCommand command,
			CancellationToken cancellationToken)
		{
			var items = await service.GetItemsAsync(null, cancellationToken)
				.ConfigureAwait(false);

			var item = items.FirstOrDefault(i =>
				string.Equals(i.Id, command.Id, StringComparison.OrdinalIgnoreCase));

			if (item is null)
			{
				return CommandResult.Fail($"Release plan item with ID '{command.Id}' not found.");
			}

			return ReleasePlanOutput.WriteJson(output, item);
		}
	}

	public class ReleasePlanSearchCommandExecutor(
		IReleasePlanService service,
		IOutput output) : ICommandExecutor<ReleasePlanSearchCommand>
	{
		public async Task<CommandResult> ExecuteAsync(
			ReleasePlanSearchCommand command,
			CancellationToken cancellationToken)
		{
			var filter = new ReleasePlanFilter
			{
				Search = command.Query,
				Product = command.Product,
				MaxResults = command.MaxResults > 0 ? command.MaxResults : 20,
			};

			var items = await service.GetItemsAsync(filter, cancellationToken)
				.ConfigureAwait(false);

			return ReleasePlanOutput.WriteTable(output, items);
		}
	}

	public class ReleasePlanRefreshCommandExecutor(
		IReleasePlanService service,
		IOutput output) : ICommandExecutor<ReleasePlanRefreshCommand>
	{
		public async Task<CommandResult> ExecuteAsync(
			ReleasePlanRefreshCommand command,
			CancellationToken cancellationToken)
		{
			output.WriteLine("Refreshing release plan cache from Microsoft 365 Roadmap API...", ConsoleColor.Yellow);
			var snapshot = await service.RefreshAsync(cancellationToken).ConfigureAwait(false);
			output.WriteLine($"Cached {snapshot.Items.Count} items (fetched at {snapshot.FetchedAtUtc:O}).", ConsoleColor.Green);
			return CommandResult.Success();
		}
	}

	public class ReleasePlanProductsCommandExecutor(
		IReleasePlanService service,
		IOutput output) : ICommandExecutor<ReleasePlanProductsCommand>
	{
		public async Task<CommandResult> ExecuteAsync(
			ReleasePlanProductsCommand command,
			CancellationToken cancellationToken)
		{
			var products = await service.GetProductsAsync(cancellationToken)
				.ConfigureAwait(false);

			if (products.Count == 0)
			{
				output.WriteLine("No cached release plan data. Run 'pacx release-plan refresh' first.", ConsoleColor.Yellow);
				return CommandResult.Success();
			}

			foreach (var product in products)
			{
				output.WriteLine($"  {product}");
			}

			output.WriteLine($"\nTotal: {products.Count} product(s).", ConsoleColor.Green);
			return CommandResult.Success();
		}
	}
}
