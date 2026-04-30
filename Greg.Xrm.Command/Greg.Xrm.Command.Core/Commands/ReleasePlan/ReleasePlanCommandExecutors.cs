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

	public class ReleasePlanAnalyzeCommandExecutor(
		IReleasePlanService service,
		ReleasePlanComponentMap componentMap,
		IOutput output) : ICommandExecutor<ReleasePlanAnalyzeCommand>
	{
		public async Task<CommandResult> ExecuteAsync(
			ReleasePlanAnalyzeCommand command,
			CancellationToken cancellationToken)
		{
			var filter = new ReleasePlanFilter
			{
				Product = command.Product,
				Status = command.Status,
			};

			var items = await service.GetItemsAsync(filter, cancellationToken)
				.ConfigureAwait(false);

			var impactedItems = new List<ImpactedItem>();

			foreach (var item in items)
			{
				var componentTypes = componentMap.GetComponentTypes(item.Product);

				if (componentTypes.Count == 0)
				{
					continue;
				}

				var impactCategory = string.IsNullOrWhiteSpace(item.Category)
					? "Updated"
					: item.Category;

				var relevance = item.Status?.ToLowerInvariant() switch
				{
					"deprecated" or "cancelled" => "High",
					"rollingout" => "High",
					"indevelopment" or "in development" => "Medium",
					"launched" => "Low",
					_ => "Medium",
				};

				var rationale = BuildRationale(item, componentTypes);

				impactedItems.Add(new ImpactedItem
				{
					ReleaseItem = item,
					ImpactCategory = impactCategory,
					Relevance = relevance,
					Rationale = rationale,
					AffectedComponentTypes = [.. componentTypes],
				});
			}

			if (impactedItems.Count == 0)
			{
				output.WriteLine("No impacted items found for the given criteria.", ConsoleColor.Yellow);
				return CommandResult.Success();
			}

			output.WriteLine($"Impact Analysis for Environment: {command.EnvironmentName}", ConsoleColor.White);
			output.WriteLine($"Generated at: {DateTime.UtcNow:O}", ConsoleColor.Gray);
			output.WriteLine($"Total impacted items: {impactedItems.Count}", ConsoleColor.Green);
			output.WriteLine(string.Empty);

			foreach (var impactedItem in impactedItems.OrderByDescending(i => i.Relevance).ThenBy(i => i.ImpactCategory))
			{
				var relevanceColor = impactedItem.Relevance switch
				{
					"High" => ConsoleColor.Red,
					"Medium" => ConsoleColor.Yellow,
					_ => ConsoleColor.Gray,
				};

				output.Write($"  [{impactedItem.Relevance}] ", relevanceColor);
				output.WriteLine(impactedItem.ReleaseItem?.Title ?? "(untitled)", ConsoleColor.Cyan);

				if (impactedItem.ReleaseItem is not null)
				{
					output.Write($"    Product: {impactedItem.ReleaseItem.Product}");
					output.Write($" | Status: {impactedItem.ReleaseItem.Status}");
					output.WriteLine($" | Impact: {impactedItem.ImpactCategory}");
				}

				if (!string.IsNullOrWhiteSpace(impactedItem.Rationale))
				{
					output.WriteLine($"    {impactedItem.Rationale}", ConsoleColor.Gray);
				}

				output.WriteLine($"    Affected components: {string.Join(", ", impactedItem.AffectedComponentTypes)}", ConsoleColor.Gray);
				output.WriteLine(string.Empty);
			}

			return CommandResult.Success();
		}

		private static string? BuildRationale(ReleasePlanItem item, IReadOnlyList<string> componentTypes)
		{
			if (item.Status?.Equals("Deprecated", StringComparison.OrdinalIgnoreCase) == true)
			{
				return $"This {string.Join("/", componentTypes.Take(2))} feature is being deprecated.";
			}

			if (item.Status?.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) == true)
			{
				return $"Planned changes for {string.Join("/", componentTypes.Take(2))} have been cancelled.";
			}

			if (item.Status?.Equals("RollingOut", StringComparison.OrdinalIgnoreCase) == true)
			{
				return $"New capabilities for {string.Join("/", componentTypes.Take(2))} are rolling out now.";
			}

			if (item.Status?.Equals("InDevelopment", StringComparison.OrdinalIgnoreCase) == true
				|| item.Status?.Equals("In Development", StringComparison.OrdinalIgnoreCase) == true)
			{
				return $"Upcoming changes to {string.Join("/", componentTypes.Take(2))} are in development.";
			}

			if (!string.IsNullOrWhiteSpace(item.Description))
			{
				var trimmed = item.Description.Length > 120
					? item.Description[..117] + "..."
					: item.Description;
				return trimmed;
			}

			return null;
		}
	}

	public class ReleasePlanReportCommandExecutor(
		IReleasePlanService service,
		ReleasePlanComponentMap componentMap,
		IOutput output) : ICommandExecutor<ReleasePlanReportCommand>
	{
		public async Task<CommandResult> ExecuteAsync(
			ReleasePlanReportCommand command,
			CancellationToken cancellationToken)
		{
			var filter = new ReleasePlanFilter
			{
				Product = command.Product,
				Status = command.Status,
			};

			var items = await service.GetItemsAsync(filter, cancellationToken)
				.ConfigureAwait(false);

			if (items.Count == 0)
			{
				output.WriteLine("No release plan items found for the given criteria.", ConsoleColor.Yellow);
				return CommandResult.Success();
			}

			var report = GenerateReport(items, command.Format, componentMap);

			if (!string.IsNullOrWhiteSpace(command.OutputPath))
			{
				var directory = Path.GetDirectoryName(command.OutputPath);
				if (!string.IsNullOrWhiteSpace(directory))
				{
					Directory.CreateDirectory(directory);
				}
				await File.WriteAllTextAsync(command.OutputPath, report, cancellationToken)
					.ConfigureAwait(false);
				output.WriteLine($"Report written to: {command.OutputPath}", ConsoleColor.Green);
			}
			else
			{
				output.WriteLine(report);
			}

			return CommandResult.Success();
		}

		private static string GenerateReport(List<ReleasePlanItem> items, string format, ReleasePlanComponentMap componentMap)
		{
			var isHtml = format.Equals("html", StringComparison.OrdinalIgnoreCase);

			if (isHtml)
			{
				return GenerateHtmlReport(items, componentMap);
			}

			return GenerateMarkdownReport(items, componentMap);
		}

		private static string GenerateMarkdownReport(List<ReleasePlanItem> items, ReleasePlanComponentMap componentMap)
		{
			var sb = new System.Text.StringBuilder();

			sb.AppendLine("# Release Plan Report");
			sb.AppendLine();
			sb.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
			sb.AppendLine();
			sb.AppendLine($"## Summary");
			sb.AppendLine();
			sb.AppendLine($"- **Total items**: {items.Count}");
			sb.AppendLine($"- **Status breakdown**:");
			foreach (var group in items.GroupBy(i => i.Status ?? "Unknown").OrderBy(g => g.Key))
			{
				sb.AppendLine($"  - **{group.Key}**: {group.Count()}");
			}
			sb.AppendLine();

			var productGroup = items.GroupBy(i => i.Product).OrderBy(g => g.Key);
			sb.AppendLine($"- **Products covered**: {productGroup.Count()}");
			foreach (var group in productGroup)
			{
				sb.AppendLine($"  - **{group.Key}**: {group.Count()} item(s)");
			}
			sb.AppendLine();

			sb.AppendLine("## Items");
			sb.AppendLine();

			foreach (var item in items.OrderBy(i => i.Product).ThenBy(i => i.Status))
			{
				var componentTypes = componentMap.GetComponentTypes(item.Product);
				sb.AppendLine($"### {item.Title}");
				sb.AppendLine();
				sb.AppendLine($"- **ID**: {item.Id}");
				sb.AppendLine($"- **Product**: {item.Product}");
				sb.AppendLine($"- **Status**: {item.Status}");
				sb.AppendLine($"- **Category**: {item.Category}");
				if (!string.IsNullOrWhiteSpace(item.Wave))
					sb.AppendLine($"- **Wave**: {item.Wave}");
				if (!string.IsNullOrWhiteSpace(item.Description))
					sb.AppendLine($"- **Description**: {item.Description}");
				if (!string.IsNullOrWhiteSpace(item.Url))
					sb.AppendLine($"- **URL**: {item.Url}");
				if (componentTypes.Count > 0)
					sb.AppendLine($"- **Affected Component Types**: {string.Join(", ", componentTypes)}");
				sb.AppendLine();
			}

			return sb.ToString();
		}

		private static string GenerateHtmlReport(List<ReleasePlanItem> items, ReleasePlanComponentMap componentMap)
		{
			var sb = new System.Text.StringBuilder();

			sb.AppendLine("<!DOCTYPE html>");
			sb.AppendLine("<html lang=\"en\">");
			sb.AppendLine("<head><meta charset=\"utf-8\">");
			sb.AppendLine("<title>Release Plan Report</title>");
			sb.AppendLine("<style>");
			sb.AppendLine("body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; max-width: 960px; margin: 0 auto; padding: 20px; }");
			sb.AppendLine("h1 { color: #1a1a2e; border-bottom: 2px solid #e0e0e0; padding-bottom: 8px; }");
			sb.AppendLine("h2 { color: #16213e; margin-top: 24px; }");
			sb.AppendLine("h3 { color: #0f3460; }");
			sb.AppendLine(".metadata { color: #666; font-size: 0.9em; }");
			sb.AppendLine(".item { border: 1px solid #e0e0e0; border-radius: 6px; padding: 12px 16px; margin: 12px 0; background: #fafafa; }");
			sb.AppendLine(".item h3 { margin: 0 0 8px 0; }");
			sb.AppendLine(".status { display: inline-block; padding: 2px 8px; border-radius: 4px; font-size: 0.85em; font-weight: 600; }");
			sb.AppendLine(".status-launched { background: #d4edda; color: #155724; }");
			sb.AppendLine(".status-rollingout { background: #cce5ff; color: #004085; }");
			sb.AppendLine(".status-indevelopment { background: #fff3cd; color: #856404; }");
			sb.AppendLine(".status-deprecated { background: #f8d7da; color: #721c24; }");
			sb.AppendLine("table { width: 100%; border-collapse: collapse; margin: 16px 0; }");
			sb.AppendLine("th, td { border: 1px solid #ddd; padding: 8px 12px; text-align: left; }");
			sb.AppendLine("th { background: #f5f5f5; font-weight: 600; }");
			sb.AppendLine("</style></head><body>");

			sb.AppendLine("<h1>Release Plan Report</h1>");
			sb.AppendLine($"<p class=\"metadata\">Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC | Items: {items.Count}</p>");

			// Summary table
			sb.AppendLine("<h2>Summary</h2>");
			sb.AppendLine("<table><tr><th>Status</th><th>Count</th></tr>");
			foreach (var group in items.GroupBy(i => i.Status ?? "Unknown").OrderBy(g => g.Key))
			{
				var cssClass = group.Key?.ToLowerInvariant().Replace(" ", "") ?? "unknown";
				sb.AppendLine($"<tr><td><span class=\"status status-{cssClass}\">{group.Key}</span></td><td>{group.Count()}</td></tr>");
			}
			sb.AppendLine("</table>");

			sb.AppendLine("<h2>Items</h2>");
			foreach (var item in items.OrderBy(i => i.Product).ThenBy(i => i.Status))
			{
				var cssClass = (item.Status ?? "Unknown").ToLowerInvariant().Replace(" ", "");
				var componentTypes = componentMap.GetComponentTypes(item.Product);
				sb.AppendLine("<div class=\"item\">");
				sb.AppendLine($"<h3>{item.Title}</h3>");
				sb.AppendLine($"<p><span class=\"status status-{cssClass}\">{item.Status}</span>");
				sb.AppendLine($"<strong>Product:</strong> {item.Product} | <strong>Category:</strong> {item.Category}</p>");
				if (!string.IsNullOrWhiteSpace(item.Description))
					sb.AppendLine($"<p>{item.Description}</p>");
				if (!string.IsNullOrWhiteSpace(item.Url))
					sb.AppendLine($"<p><a href=\"{item.Url}\">{item.Url}</a></p>");
				if (componentTypes.Count > 0)
					sb.AppendLine($"<p><strong>Affected Component Types:</strong> {string.Join(", ", componentTypes)}</p>");
				sb.AppendLine("</div>");
			}

			sb.AppendLine("</body></html>");
			return sb.ToString();
		}
	}
}
