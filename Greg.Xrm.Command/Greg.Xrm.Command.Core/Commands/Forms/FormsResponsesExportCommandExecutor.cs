using System.Text.Json;

namespace Greg.Xrm.Command.Commands.Forms
{
	public class FormsResponsesExportCommandExecutor(
		IOutput output,
		Services.Forms.IFormsApiClient formsApiClient) : ICommandExecutor<FormsResponsesExportCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FormsResponsesExportCommand command, CancellationToken cancellationToken)
		{
			var ownerId = command.OwnerId ?? "me";
			var ownerType = command.OwnerType ?? "User";
			var pageSize = 100;
			var skip = 0;

			if (command.Incremental)
			{
				var lastExportPath = command.OutputPath + ".last";
				if (File.Exists(lastExportPath))
				{
					var lastPos = await File.ReadAllTextAsync(lastExportPath, cancellationToken);
					int.TryParse(lastPos, out skip);
				}
			}

			try
			{
				output.Write($"Fetching responses for form <{command.FormId}>...");

				var allResponses = new List<Services.Forms.FormsResponse>();
				while (true)
				{
					var batch = await formsApiClient.GetResponsesAsync(
						command.TenantId, ownerId, ownerType, command.FormId,
						pageSize, skip, cancellationToken);

					if (batch.Count == 0) break;
					allResponses.AddRange(batch);
					skip += batch.Count;
					output.Write(".");
				}

				output.WriteLine($" Done ({allResponses.Count} total).", ConsoleColor.Green);

				var format = (command.Format ?? "csv").ToLowerInvariant();
				switch (format)
				{
					case "json":
						await ExportJsonAsync(command.OutputPath, allResponses, cancellationToken);
						break;
					case "sql":
						await ExportSqlAsync(command.OutputPath, allResponses, cancellationToken);
						break;
					default:
						await ExportCsvAsync(command.OutputPath, allResponses, cancellationToken);
						break;
				}

				if (command.Incremental)
				{
					await File.WriteAllTextAsync(command.OutputPath + ".last", skip.ToString(), cancellationToken);
				}

				output.WriteLine($"Exported to {command.OutputPath}", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to export responses: {ex.Message}", ex);
			}
		}

		private static async Task ExportCsvAsync(string path, List<Services.Forms.FormsResponse> responses, CancellationToken ct)
		{
			using var writer = new StreamWriter(path);
			await writer.WriteLineAsync("ResponseId,SubmittedAt,Answers");
			foreach (var r in responses)
			{
				var answers = (r.Answers ?? "").Replace("\"", "\"\"");
				await writer.WriteLineAsync($"{r.Id},{r.SubmittedAt:O},\"{answers}\"");
			}
		}

		private static async Task ExportJsonAsync(string path, List<Services.Forms.FormsResponse> responses, CancellationToken ct)
		{
			var json = JsonSerializer.Serialize(responses, new JsonSerializerOptions { WriteIndented = true });
			await File.WriteAllTextAsync(path, json, ct);
		}

		private static async Task ExportSqlAsync(string path, List<Services.Forms.FormsResponse> responses, CancellationToken ct)
		{
			using var writer = new StreamWriter(path);
			await writer.WriteLineAsync("CREATE TABLE IF NOT EXISTS form_responses (ResponseId INT, SubmittedAt TEXT, Answers TEXT);");
			foreach (var r in responses)
			{
				var answers = (r.Answers ?? "").Replace("'", "''");
				await writer.WriteLineAsync($"INSERT INTO form_responses VALUES ({r.Id}, '{r.SubmittedAt:O}', '{answers}');");
			}
		}
	}
}
