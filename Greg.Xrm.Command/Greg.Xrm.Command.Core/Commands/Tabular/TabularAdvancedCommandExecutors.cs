using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Tabular;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Commands.Tabular
{
	public class TabularTranslateCommandExecutor(
		IOutput output,
		ITabularClient tabularClient) : ICommandExecutor<TabularTranslateCommand>
	{
		public async Task<CommandResult> ExecuteAsync(TabularTranslateCommand command, CancellationToken cancellationToken)
		{
			try
			{
				if (!File.Exists(command.TranslationFile))
				{
					return CommandResult.Fail($"Translation file not found: {command.TranslationFile}");
				}

				var content = await File.ReadAllTextAsync(command.TranslationFile, cancellationToken).ConfigureAwait(false);

				switch (command.Mode.ToLower())
				{
					case "export":
						output.WriteLine($"Export of translations for model '{command.ModelId}' requires TOM library.", ConsoleColor.Yellow);
						output.WriteLine($"  Language: {command.LanguageCode}");
						output.WriteLine($"  Would write to: {command.TranslationFile}");
						return CommandResult.Success();

					case "diff":
						output.WriteLine($"Diff of translations for model '{command.ModelId}' requires TOM library.", ConsoleColor.Yellow);
						output.WriteLine($"  Language: {command.LanguageCode}");
						output.WriteLine($"  File: {command.TranslationFile}");
						return CommandResult.Success();

					case "deploy":
					default:
						output.WriteLine($"Deploying translations to model '{command.ModelId}'", ConsoleColor.Cyan);
						output.WriteLine($"  File: {command.TranslationFile}");
						output.WriteLine($"  Language: {command.LanguageCode}");

						if (string.IsNullOrWhiteSpace(command.WorkspaceId))
						{
							return CommandResult.Fail("Workspace ID (--workspace) is required for translation deployment.");
						}

						var datasetId = await tabularClient.GetDatasetIdByNameAsync(command.WorkspaceId, command.ModelId, cancellationToken).ConfigureAwait(false);

						if (datasetId == null)
						{
							datasetId = command.ModelId;
						}

						await tabularClient.UpdateDefinitionAsync(command.WorkspaceId, datasetId, content, cancellationToken).ConfigureAwait(false);
						output.WriteLine("Translations deployed successfully.", ConsoleColor.Green);
						return CommandResult.Success();
				}
			}
			catch (Exception ex) when (ex is IOException or InvalidOperationException)
			{
				return CommandResult.Fail($"Error during translation operation: {ex.Message}", ex);
			}
		}
	}

	public class TabularRoleAddMeasuresCommandExecutor(
		IOutput output,
		ITabularClient tabularClient) : ICommandExecutor<TabularRoleAddMeasuresCommand>
	{
		public async Task<CommandResult> ExecuteAsync(TabularRoleAddMeasuresCommand command, CancellationToken cancellationToken)
		{
			try
			{
				output.WriteLine($"Adding {command.Measures.Length} measure(s) to all roles in model '{command.ModelId}'", ConsoleColor.Cyan);
				output.WriteLine($"  Measures: {string.Join(", ", command.Measures)}");

				if (command.DryRun)
				{
					output.WriteLine("[DRY RUN] No changes made.", ConsoleColor.Yellow);
					return CommandResult.Success();
				}

				if (string.IsNullOrWhiteSpace(command.WorkspaceId))
				{
					return CommandResult.Fail("Workspace ID (--workspace) is required.");
				}

				var datasetId = await tabularClient.GetDatasetIdByNameAsync(command.WorkspaceId, command.ModelId, cancellationToken).ConfigureAwait(false);
				if (datasetId == null)
				{
					datasetId = command.ModelId;
				}

				var deployedJson = await tabularClient.GetDeployedBimAsync(command.WorkspaceId, datasetId, cancellationToken).ConfigureAwait(false);

				if (string.IsNullOrWhiteSpace(deployedJson))
				{
					return CommandResult.Fail("Failed to retrieve deployed model.");
				}

				var bim = JsonDocument.Parse(deployedJson);
				var modified = AddMeasuresToRoles(bim, command.Measures);

				await tabularClient.UpdateDefinitionAsync(command.WorkspaceId, datasetId, modified, cancellationToken).ConfigureAwait(false);
				output.WriteLine("Roles updated successfully.", ConsoleColor.Green);

				return CommandResult.Success();
			}
			catch (Exception ex) when (ex is InvalidOperationException)
			{
				return CommandResult.Fail($"Error adding measures to roles: {ex.Message}", ex);
			}
		}

		private static string AddMeasuresToRoles(JsonDocument bim, string[] measureNames)
		{
			using var stream = new MemoryStream();
			var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

			writer.WriteStartObject();

			foreach (var prop in bim.RootElement.EnumerateObject())
			{
				if (prop.Name == "roles" && prop.Value.ValueKind == JsonValueKind.Array)
				{
					writer.WritePropertyName("roles");
					writer.WriteStartArray();

					foreach (var role in prop.Value.EnumerateArray())
					{
						writer.WriteStartObject();

						foreach (var roleProp in role.EnumerateObject())
						{
							if (roleProp.Name == "members" && roleProp.Value.ValueKind == JsonValueKind.Array)
							{
								writer.WritePropertyName("members");
								writer.WriteStartArray();

								foreach (var member in roleProp.Value.EnumerateArray())
								{
									writer.WriteStartObject();

									foreach (var memberProp in member.EnumerateObject())
									{
										writer.WritePropertyName(memberProp.Name);
										memberProp.Value.WriteTo(writer);
									}

									writer.WriteEndObject();
								}

								foreach (var measure in measureNames)
								{
									writer.WriteStartObject();
									writer.WriteString("memberName", measure);
									writer.WriteEndObject();
								}

								writer.WriteEndArray();
							}
							else
							{
								writer.WritePropertyName(roleProp.Name);
								roleProp.Value.WriteTo(writer);
							}
						}

						writer.WriteEndObject();
					}

					writer.WriteEndArray();
				}
				else
				{
					writer.WritePropertyName(prop.Name);
					prop.Value.WriteTo(writer);
				}
			}

			writer.WriteEndObject();
			writer.Flush();

			return System.Text.Encoding.UTF8.GetString(stream.ToArray());
		}
	}

	public class TabularPerspectiveManageCommandExecutor(
		IOutput output,
		ITabularClient tabularClient) : ICommandExecutor<TabularPerspectiveManageCommand>
	{
		public async Task<CommandResult> ExecuteAsync(TabularPerspectiveManageCommand command, CancellationToken cancellationToken)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(command.WorkspaceId))
				{
					return CommandResult.Fail("Workspace ID (--workspace) is required.");
				}

				var datasetId = await tabularClient.GetDatasetIdByNameAsync(command.WorkspaceId, command.ModelId, cancellationToken).ConfigureAwait(false);
				if (datasetId == null)
				{
					datasetId = command.ModelId;
				}

				switch (command.Action.ToLower())
				{
					case "list":
						return await ListPerspectivesAsync(command, datasetId, cancellationToken).ConfigureAwait(false);

					case "create":
						return await CreatePerspectiveAsync(command, datasetId, cancellationToken).ConfigureAwait(false);

					case "delete":
						return await DeletePerspectiveAsync(command, datasetId, cancellationToken).ConfigureAwait(false);

					case "add-tables":
						return await AddTablesToPerspectiveAsync(command, datasetId, cancellationToken).ConfigureAwait(false);

					case "remove-tables":
						return await RemoveTablesFromPerspectiveAsync(command, datasetId, cancellationToken).ConfigureAwait(false);

					default:
						return CommandResult.Fail($"Unknown action '{command.Action}'. Valid actions: create, delete, list, add-tables, remove-tables.");
				}
			}
			catch (Exception ex) when (ex is InvalidOperationException)
			{
				return CommandResult.Fail($"Error managing perspective: {ex.Message}", ex);
			}
		}

		private async Task<CommandResult> ListPerspectivesAsync(TabularPerspectiveManageCommand command, string datasetId, CancellationToken ct)
		{
			output.WriteLine($"Listing perspectives for model '{command.ModelId}'", ConsoleColor.Cyan);

			var deployedJson = await tabularClient.GetDeployedBimAsync(command.WorkspaceId!, datasetId, ct).ConfigureAwait(false);

			if (string.IsNullOrWhiteSpace(deployedJson))
			{
				return CommandResult.Fail("Failed to retrieve deployed model.");
			}

			var bim = JsonDocument.Parse(deployedJson);
			if (bim.RootElement.TryGetProperty("perspectives", out var perspectives) && perspectives.ValueKind == JsonValueKind.Array)
			{
				if (perspectives.GetArrayLength() == 0)
				{
					output.WriteLine("No perspectives defined.", ConsoleColor.Yellow);
				}
				else
				{
					foreach (var p in perspectives.EnumerateArray())
					{
						var name = p.TryGetProperty("name", out var n) ? n.GetString() : "?";
						var tables = "?";
						if (p.TryGetProperty("tables", out var pt) && pt.ValueKind == JsonValueKind.Array)
							tables = pt.GetArrayLength().ToString();
						output.WriteLine($"  {name} ({tables} tables)");
					}
				}
			}
			else
			{
				output.WriteLine("No perspectives defined in model.", ConsoleColor.Yellow);
			}

			return CommandResult.Success();
		}

		private async Task<CommandResult> CreatePerspectiveAsync(TabularPerspectiveManageCommand command, string datasetId, CancellationToken ct)
		{
			if (string.IsNullOrEmpty(command.PerspectiveName))
				return CommandResult.Fail("Perspective name (--name) is required for 'create' action.");

			output.WriteLine($"Creating perspective '{command.PerspectiveName}' in model '{command.ModelId}'", ConsoleColor.Cyan);

			var deployedJson = await tabularClient.GetDeployedBimAsync(command.WorkspaceId!, datasetId, ct).ConfigureAwait(false);

			if (string.IsNullOrWhiteSpace(deployedJson))
			{
				return CommandResult.Fail("Failed to retrieve deployed model.");
			}

			var modified = AddPerspective(deployedJson, command.PerspectiveName);
			await tabularClient.UpdateDefinitionAsync(command.WorkspaceId!, datasetId, modified, ct).ConfigureAwait(false);
			output.WriteLine($"Perspective '{command.PerspectiveName}' created.", ConsoleColor.Green);

			return CommandResult.Success();
		}

		private async Task<CommandResult> DeletePerspectiveAsync(TabularPerspectiveManageCommand command, string datasetId, CancellationToken ct)
		{
			if (string.IsNullOrEmpty(command.PerspectiveName))
				return CommandResult.Fail("Perspective name (--name) is required for 'delete' action.");

			output.WriteLine($"Deleting perspective '{command.PerspectiveName}' from model '{command.ModelId}'", ConsoleColor.Cyan);

			var deployedJson = await tabularClient.GetDeployedBimAsync(command.WorkspaceId!, datasetId, ct).ConfigureAwait(false);

			if (string.IsNullOrWhiteSpace(deployedJson))
			{
				return CommandResult.Fail("Failed to retrieve deployed model.");
			}

			var modified = RemovePerspective(deployedJson, command.PerspectiveName);
			await tabularClient.UpdateDefinitionAsync(command.WorkspaceId!, datasetId, modified, ct).ConfigureAwait(false);
			output.WriteLine($"Perspective '{command.PerspectiveName}' deleted.", ConsoleColor.Green);

			return CommandResult.Success();
		}

		private async Task<CommandResult> AddTablesToPerspectiveAsync(TabularPerspectiveManageCommand command, string datasetId, CancellationToken ct)
		{
			if (string.IsNullOrEmpty(command.PerspectiveName) || command.Tables == null || command.Tables.Length == 0)
				return CommandResult.Fail("Perspective name (--name) and tables (--tables) are required for 'add-tables' action.");

			output.WriteLine($"Adding tables to perspective '{command.PerspectiveName}': {string.Join(", ", command.Tables)}", ConsoleColor.Cyan);

			var deployedJson = await tabularClient.GetDeployedBimAsync(command.WorkspaceId!, datasetId, ct).ConfigureAwait(false);

			if (string.IsNullOrWhiteSpace(deployedJson))
			{
				return CommandResult.Fail("Failed to retrieve deployed model.");
			}

			var modified = ModifyPerspectiveTables(deployedJson, command.PerspectiveName, command.Tables, add: true);
			await tabularClient.UpdateDefinitionAsync(command.WorkspaceId!, datasetId, modified, ct).ConfigureAwait(false);
			output.WriteLine($"Tables added to perspective '{command.PerspectiveName}'.", ConsoleColor.Green);

			return CommandResult.Success();
		}

		private async Task<CommandResult> RemoveTablesFromPerspectiveAsync(TabularPerspectiveManageCommand command, string datasetId, CancellationToken ct)
		{
			if (string.IsNullOrEmpty(command.PerspectiveName) || command.Tables == null || command.Tables.Length == 0)
				return CommandResult.Fail("Perspective name (--name) and tables (--tables) are required for 'remove-tables' action.");

			output.WriteLine($"Removing tables from perspective '{command.PerspectiveName}': {string.Join(", ", command.Tables)}", ConsoleColor.Cyan);

			var deployedJson = await tabularClient.GetDeployedBimAsync(command.WorkspaceId!, datasetId, ct).ConfigureAwait(false);

			if (string.IsNullOrWhiteSpace(deployedJson))
			{
				return CommandResult.Fail("Failed to retrieve deployed model.");
			}

			var modified = ModifyPerspectiveTables(deployedJson, command.PerspectiveName, command.Tables, add: false);
			await tabularClient.UpdateDefinitionAsync(command.WorkspaceId!, datasetId, modified, ct).ConfigureAwait(false);
			output.WriteLine($"Tables removed from perspective '{command.PerspectiveName}'.", ConsoleColor.Green);

			return CommandResult.Success();
		}

		private static string AddPerspective(string bimJson, string perspectiveName)
		{
			var doc = JsonDocument.Parse(bimJson);
			using var stream = new MemoryStream();
			var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

			writer.WriteStartObject();

			foreach (var prop in doc.RootElement.EnumerateObject())
			{
				if (prop.Name == "perspectives" && prop.Value.ValueKind == JsonValueKind.Array)
				{
					writer.WritePropertyName("perspectives");
					writer.WriteStartArray();

					foreach (var p in prop.Value.EnumerateArray())
					{
						p.WriteTo(writer);
					}

					writer.WriteStartObject();
					writer.WriteString("name", perspectiveName);
					writer.WriteStartArray("tables");
					writer.WriteEndArray();
					writer.WriteEndObject();

					writer.WriteEndArray();
				}
				else
				{
					writer.WritePropertyName(prop.Name);
					prop.Value.WriteTo(writer);
				}
			}

			writer.WriteEndObject();
			writer.Flush();

			return System.Text.Encoding.UTF8.GetString(stream.ToArray());
		}

		private static string RemovePerspective(string bimJson, string perspectiveName)
		{
			var doc = JsonDocument.Parse(bimJson);
			using var stream = new MemoryStream();
			var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

			writer.WriteStartObject();

			foreach (var prop in doc.RootElement.EnumerateObject())
			{
				if (prop.Name == "perspectives" && prop.Value.ValueKind == JsonValueKind.Array)
				{
					writer.WritePropertyName("perspectives");
					writer.WriteStartArray();

					foreach (var p in prop.Value.EnumerateArray())
					{
						var name = p.TryGetProperty("name", out var n) ? n.GetString() : "";
						if (name != perspectiveName)
						{
							p.WriteTo(writer);
						}
					}

					writer.WriteEndArray();
				}
				else
				{
					writer.WritePropertyName(prop.Name);
					prop.Value.WriteTo(writer);
				}
			}

			writer.WriteEndObject();
			writer.Flush();

			return System.Text.Encoding.UTF8.GetString(stream.ToArray());
		}

		private static string ModifyPerspectiveTables(string bimJson, string perspectiveName, string[] tables, bool add)
		{
			var doc = JsonDocument.Parse(bimJson);
			using var stream = new MemoryStream();
			var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });

			var tablesSet = new System.Collections.Generic.HashSet<string>(tables);

			writer.WriteStartObject();

			foreach (var prop in doc.RootElement.EnumerateObject())
			{
				if (prop.Name == "perspectives" && prop.Value.ValueKind == JsonValueKind.Array)
				{
					writer.WritePropertyName("perspectives");
					writer.WriteStartArray();

					foreach (var p in prop.Value.EnumerateArray())
					{
						var name = p.TryGetProperty("name", out var n) ? n.GetString() : "";

						if (name == perspectiveName)
						{
							writer.WriteStartObject();

							foreach (var pp in p.EnumerateObject())
							{
								if (pp.Name == "tables" && pp.Value.ValueKind == JsonValueKind.Array)
								{
									writer.WritePropertyName("tables");
									writer.WriteStartArray();

									var existing = new System.Collections.Generic.List<string>();
									foreach (var t in pp.Value.EnumerateArray())
									{
										var tn = t.TryGetProperty("name", out var tnp) ? tnp.GetString() : "";
										if (tn != null) existing.Add(tn);
										if (!add || !tablesSet.Contains(tn ?? ""))
										{
											t.WriteTo(writer);
										}
									}

									if (add)
									{
										foreach (var t in tables)
										{
											if (!existing.Contains(t))
											{
												writer.WriteStartObject();
												writer.WriteString("name", t);
												writer.WriteEndObject();
											}
										}
									}

									writer.WriteEndArray();
								}
								else
								{
									writer.WritePropertyName(pp.Name);
									pp.Value.WriteTo(writer);
								}
							}

							writer.WriteEndObject();
						}
						else
						{
							p.WriteTo(writer);
						}
					}

					writer.WriteEndArray();
				}
				else
				{
					writer.WritePropertyName(prop.Name);
					prop.Value.WriteTo(writer);
				}
			}

			writer.WriteEndObject();
			writer.Flush();

			return System.Text.Encoding.UTF8.GetString(stream.ToArray());
		}
	}
}
