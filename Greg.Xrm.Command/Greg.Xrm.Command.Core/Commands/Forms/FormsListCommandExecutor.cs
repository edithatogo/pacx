namespace Greg.Xrm.Command.Commands.Forms
{
	public class FormsListCommandExecutor(
		IOutput output,
		Services.Forms.IFormsApiClient formsApiClient) : ICommandExecutor<FormsListCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FormsListCommand command, CancellationToken cancellationToken)
		{
			var ownerId = command.OwnerId ?? "me";
			var ownerType = command.OwnerType ?? "User";

			try
			{
				var forms = await formsApiClient.GetFormsAsync(command.TenantId, ownerId, ownerType, cancellationToken);

				if (forms.Count == 0)
				{
					output.WriteLine("No forms found.", ConsoleColor.Yellow);
					return CommandResult.Success();
				}

				if (string.Equals(command.Format, "json", StringComparison.OrdinalIgnoreCase))
				{
					output.WriteLine(System.Text.Json.JsonSerializer.Serialize(forms, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
				}
				else
				{
					output.WriteTable(
						forms,
						() => ["ID", "Title", "Status", "Responses", "Created", "Modified"],
						f => [f.Id, f.Title, f.Status, f.RowCount.ToString(), f.CreatedDate?.ToString("yyyy-MM-dd") ?? "", f.ModifiedDate?.ToString("yyyy-MM-dd") ?? ""]);
				}

				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to list forms: {ex.Message}", ex);
			}
		}
	}
}
