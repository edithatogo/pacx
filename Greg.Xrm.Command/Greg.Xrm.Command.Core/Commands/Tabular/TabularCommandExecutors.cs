using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Tabular;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace Greg.Xrm.Command.Commands.Tabular
{
	public class TabularDeployCommandExecutor(
		IOutput output,
		ITabularClient tabularClient) : ICommandExecutor<TabularDeployCommand>
	{
		public async Task<CommandResult> ExecuteAsync(TabularDeployCommand command, CancellationToken cancellationToken)
		{
			if (!File.Exists(command.BimFilePath))
			{
				return CommandResult.Fail($"BIM file not found: {command.BimFilePath}");
			}

			var bimContent = await File.ReadAllTextAsync(command.BimFilePath, cancellationToken).ConfigureAwait(false);
			var bim = JsonDocument.Parse(bimContent);

			var modelName = bim.RootElement.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : "Unknown";
			var compatLevel = bim.RootElement.TryGetProperty("compatibilityLevel", out var compatProp) ? compatProp.GetInt32() : 0;

			output.WriteLine($"Tabular Deploy: {modelName}", ConsoleColor.Cyan);
			output.WriteLine($"  BIM File: {command.BimFilePath}");
			output.WriteLine($"  Workspace: {command.Workspace}");
			output.WriteLine($"  Mode: {command.Mode}");
			output.WriteLine($"  Compatibility Level: {compatLevel}");

			var tableCount = 0;
			if (bim.RootElement.TryGetProperty("tables", out var tables) && tables.ValueKind == JsonValueKind.Array)
				tableCount = tables.GetArrayLength();
			var measureCount = CountMeasures(bim);

			output.WriteLine($"  Tables: {tableCount}");
			output.WriteLine($"  Measures: {measureCount}");

			if (command.DryRun)
			{
				output.WriteLine("[DRY RUN] Would deploy:", ConsoleColor.Yellow);
				output.WriteLine($"  Model: {modelName}");
				return CommandResult.Success();
			}

			if (string.IsNullOrWhiteSpace(command.DatasetName))
			{
				return CommandResult.Fail("Dataset name (--dataset) is required for deployment.");
			}

			try
			{
				var datasetId = await tabularClient.DeployBimAsync(command.Workspace, command.DatasetName, bimContent, cancellationToken).ConfigureAwait(false);

				if (!string.IsNullOrEmpty(datasetId))
				{
					output.WriteLine($"Deployed successfully. Import ID: {datasetId}", ConsoleColor.Green);
				}
				else
				{
					output.WriteLine("Deployment initiated.", ConsoleColor.Green);
				}
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Deployment failed: {ex.Message}", ex);
			}

			return CommandResult.Success();
		}

		private static int CountMeasures(JsonDocument bim)
		{
			var count = 0;
			if (bim.RootElement.TryGetProperty("tables", out var tables) && tables.ValueKind == JsonValueKind.Array)
			{
				foreach (var table in tables.EnumerateArray())
				{
					if (table.TryGetProperty("measures", out var measures) && measures.ValueKind == JsonValueKind.Array)
					{
						count += measures.GetArrayLength();
					}
				}
			}
			return count;
		}
	}

	public class TabularDiffCommandExecutor(
		IOutput output,
		ITabularClient tabularClient) : ICommandExecutor<TabularDiffCommand>
	{
		public async Task<CommandResult> ExecuteAsync(TabularDiffCommand command, CancellationToken cancellationToken)
		{
			if (!File.Exists(command.BimFilePath))
			{
				return CommandResult.Fail($"BIM file not found: {command.BimFilePath}");
			}

			var local = JsonDocument.Parse(await File.ReadAllTextAsync(command.BimFilePath, cancellationToken).ConfigureAwait(false));
			var localName = local.RootElement.TryGetProperty("name", out var n) ? n.GetString() : "Unknown";

			output.WriteLine($"Tabular Diff: {command.BimFilePath} vs {command.Workspace}/{command.DatasetName}", ConsoleColor.Cyan);

			if (string.IsNullOrWhiteSpace(command.DatasetName))
			{
				return CommandResult.Fail("Dataset name (--dataset) is required for comparison.");
			}

			try
			{
				var datasetId = await tabularClient.GetDatasetIdByNameAsync(command.Workspace, command.DatasetName, cancellationToken).ConfigureAwait(false);

				if (datasetId == null)
				{
					output.WriteLine($"Dataset '{command.DatasetName}' not found in workspace '{command.Workspace}'.", ConsoleColor.Yellow);
					output.WriteLine($"{localName}: {CountLocalTables(local)} tables, {CountLocalMeasures(local)} measures (local only)");
					return CommandResult.Success();
				}

				var deployedJson = await tabularClient.GetDeployedBimAsync(command.Workspace, datasetId, cancellationToken).ConfigureAwait(false);

				if (string.IsNullOrWhiteSpace(deployedJson))
				{
					return CommandResult.Fail("Failed to retrieve deployed model definition.");
				}

				var deployed = JsonDocument.Parse(deployedJson);

				var localTables = GetTableNames(local);
				var deployedTables = GetDeployedTableNames(deployed);

				var added = localTables.Except(deployedTables).ToList();
				var removed = deployedTables.Except(localTables).ToList();
				var common = localTables.Intersect(deployedTables).ToList();

				output.WriteLine($"  Local: {localTables.Count} tables, {CountLocalMeasures(local)} measures");
				output.WriteLine($"  Deployed ({command.DatasetName}): {deployedTables.Count} tables");
				output.WriteLine();

				if (added.Count == 0 && removed.Count == 0)
				{
					output.WriteLine("No structural differences detected.", ConsoleColor.Green);
				}
				else
				{
					if (added.Count > 0)
					{
						output.WriteLine($"Tables to add ({added.Count}):", ConsoleColor.Green);
						foreach (var t in added)
							output.WriteLine($"  + {t}");
					}

					if (removed.Count > 0)
					{
						output.WriteLine($"Tables to remove ({removed.Count}):", ConsoleColor.Red);
						foreach (var t in removed)
							output.WriteLine($"  - {t}");
					}

					output.WriteLine($"  Common: {common.Count} tables");
				}
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Comparison failed: {ex.Message}", ex);
			}

			return CommandResult.Success();
		}

		private static List<string> GetTableNames(JsonDocument bim)
		{
			var names = new List<string>();
			if (bim.RootElement.TryGetProperty("tables", out var tables) && tables.ValueKind == JsonValueKind.Array)
			{
				foreach (var table in tables.EnumerateArray())
				{
					if (table.TryGetProperty("name", out var name))
						names.Add(name.GetString() ?? "?");
				}
			}
			return names;
		}

		private static List<string> GetDeployedTableNames(JsonDocument deployed)
		{
			var names = new List<string>();
			if (deployed.RootElement.TryGetProperty("tables", out var tables) && tables.ValueKind == JsonValueKind.Array)
			{
				foreach (var table in tables.EnumerateArray())
				{
					if (table.TryGetProperty("name", out var name))
						names.Add(name.GetString() ?? "?");
				}
			}
			return names;
		}

		private static int CountLocalTables(JsonDocument bim)
		{
			return bim.RootElement.TryGetProperty("tables", out var tables) && tables.ValueKind == JsonValueKind.Array ? tables.GetArrayLength() : 0;
		}

		private static int CountLocalMeasures(JsonDocument bim)
		{
			var count = 0;
			if (bim.RootElement.TryGetProperty("tables", out var tables) && tables.ValueKind == JsonValueKind.Array)
			{
				foreach (var table in tables.EnumerateArray())
				{
					if (table.TryGetProperty("measures", out var measures) && measures.ValueKind == JsonValueKind.Array)
						count += measures.GetArrayLength();
				}
			}
			return count;
		}
	}

	public class TabularValidateCommandExecutor(
		IOutput output) : ICommandExecutor<TabularValidateCommand>
	{
		public async Task<CommandResult> ExecuteAsync(TabularValidateCommand command, CancellationToken cancellationToken)
		{
			if (!File.Exists(command.BimFilePath))
			{
				return CommandResult.Fail($"BIM file not found: {command.BimFilePath}");
			}

			var bimContent = await File.ReadAllTextAsync(command.BimFilePath, cancellationToken).ConfigureAwait(false);
			var bim = JsonDocument.Parse(bimContent);

			var issues = 0;

			// Check for tables without columns
			if (bim.RootElement.TryGetProperty("tables", out var tables) && tables.ValueKind == JsonValueKind.Array)
			{
				foreach (var table in tables.EnumerateArray())
				{
					var tableName = table.TryGetProperty("name", out var nt) ? nt.GetString() : "Unknown";

					if (!table.TryGetProperty("columns", out var columns) || columns.GetArrayLength() == 0)
					{
						output.WriteLine($"  WARNING: Table '{tableName}' has no columns", ConsoleColor.Yellow);
						issues++;
					}

					// Check for measures with circular references
					if (table.TryGetProperty("measures", out var measures) && measures.ValueKind == JsonValueKind.Array)
					{
						foreach (var measure in measures.EnumerateArray())
						{
							var measureName = measure.TryGetProperty("name", out var mn) ? mn.GetString() : "Unknown";
							var expression = measure.TryGetProperty("expression", out var expr) ? expr.GetString() : "";

							// Simple circular reference check (measure referencing itself)
							if (!string.IsNullOrEmpty(expression) && expression.Contains($"[{measureName}]"))
							{
								output.WriteLine($"  ERROR: Measure '{tableName}'.'{measureName}' has circular reference", ConsoleColor.Red);
								issues++;
							}
						}
					}
				}
			}

			if (issues > 0)
			{
				output.WriteLine($"\nFound {issues} issue(s).", command.Strict ? ConsoleColor.Red : ConsoleColor.Yellow);
				return command.Strict ? CommandResult.Fail($"Validation failed: {issues} issue(s)") : CommandResult.Success();
			}

			output.WriteLine("Validation passed. No issues found.", ConsoleColor.Green);
			return CommandResult.Success();
		}
	}

	public class BimCompareCommandExecutor(
		IOutput output) : ICommandExecutor<BimCompareCommand>
	{
		public async Task<CommandResult> ExecuteAsync(BimCompareCommand command, CancellationToken cancellationToken)
		{
			if (!File.Exists(command.FileA))
				return CommandResult.Fail($"File A not found: {command.FileA}");
			if (!File.Exists(command.FileB))
				return CommandResult.Fail($"File B not found: {command.FileB}");

			var bimA = JsonDocument.Parse(await File.ReadAllTextAsync(command.FileA, cancellationToken).ConfigureAwait(false));
			var bimB = JsonDocument.Parse(await File.ReadAllTextAsync(command.FileB, cancellationToken).ConfigureAwait(false));

			var nameA = bimA.RootElement.TryGetProperty("name", out var na) ? na.GetString() : "Unknown";
			var nameB = bimB.RootElement.TryGetProperty("name", out var nb) ? nb.GetString() : "Unknown";

			output.WriteLine($"BIM Compare: {nameA} vs {nameB}", ConsoleColor.Cyan);
			output.WriteLine();

			var tablesA = GetTableInfo(bimA);
			var tablesB = GetTableInfo(bimB);

			var tableNamesA = tablesA.Keys.ToHashSet();
			var tableNamesB = tablesB.Keys.ToHashSet();

			var added = tableNamesA.Except(tableNamesB).OrderBy(x => x).ToList();
			var removed = tableNamesB.Except(tableNamesA).OrderBy(x => x).ToList();
			var common = tableNamesA.Intersect(tableNamesB).OrderBy(x => x).ToList();

			var changed = new List<string>();
			foreach (var name in common)
			{
				var infoA = tablesA[name];
				var infoB = tablesB[name];
				if (infoA.columns != infoB.columns || infoA.measures != infoB.measures)
				{
					changed.Add(name);
				}
			}

			output.WriteLine($"  {Path.GetFileName(command.FileA)} ({nameA}): {tableNamesA.Count} tables");
			output.WriteLine($"  {Path.GetFileName(command.FileB)} ({nameB}): {tableNamesB.Count} tables");
			output.WriteLine($"  Total difference: {tableNamesA.Count - tableNamesB.Count} tables");
			output.WriteLine();

			if (added.Count > 0)
			{
				output.WriteLine($"Tables only in A ({added.Count}):", ConsoleColor.Green);
				foreach (var t in added)
				{
					var info = tablesA[t];
					output.WriteLine($"  + {t} ({info.columns} cols, {info.measures} measures)");
				}
			}

			if (removed.Count > 0)
			{
				output.WriteLine($"Tables only in B ({removed.Count}):", ConsoleColor.Red);
				foreach (var t in removed)
				{
					var info = tablesB[t];
					output.WriteLine($"  - {t} ({info.columns} cols, {info.measures} measures)");
				}
			}

			if (changed.Count > 0)
			{
				output.WriteLine($"Tables changed ({changed.Count}):", ConsoleColor.Yellow);
				foreach (var t in changed)
				{
					var infoA = tablesA[t];
					var infoB = tablesB[t];
					output.WriteLine($"  ~ {t}: A({infoA.columns} cols, {infoA.measures} measures) vs B({infoB.columns} cols, {infoB.measures} measures)");
				}
			}

			if (added.Count == 0 && removed.Count == 0 && changed.Count == 0)
			{
				output.WriteLine("Files are structurally identical.", ConsoleColor.Green);
			}

			return CommandResult.Success();
		}

		private static Dictionary<string, (int columns, int measures)> GetTableInfo(JsonDocument bim)
		{
			var result = new Dictionary<string, (int, int)>();
			if (bim.RootElement.TryGetProperty("tables", out var tables) && tables.ValueKind == JsonValueKind.Array)
			{
				foreach (var table in tables.EnumerateArray())
				{
					var name = table.TryGetProperty("name", out var nt) ? nt.GetString() : "?";
					var cols = table.TryGetProperty("columns", out var c) && c.ValueKind == JsonValueKind.Array ? c.GetArrayLength() : 0;
					var measures = table.TryGetProperty("measures", out var m) && m.ValueKind == JsonValueKind.Array ? m.GetArrayLength() : 0;
					result[name ?? "?"] = (cols, measures);
				}
			}
			return result;
		}
	}
}
