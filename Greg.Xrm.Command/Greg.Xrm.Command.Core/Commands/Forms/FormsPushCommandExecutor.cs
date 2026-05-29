using Greg.Xrm.Command.Services.Forms;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Forms
{
	public sealed class FormsPushCommandExecutor(
		IOutput output,
		IFormsApiClient formsApiClient) : ICommandExecutor<FormsPushCommand>
	{
		public async Task<CommandResult> ExecuteAsync(FormsPushCommand command, CancellationToken cancellationToken)
		{
			var tenantId = command.TenantId ?? throw new ArgumentException("Tenant ID is required for network operations.");
			var ownerId = command.OwnerId ?? "me";
			var ownerType = command.OwnerType ?? "User";

			try
			{
				var store = new FormsAuthoringDocumentStore();
				var document = store.Load(command.FilePath);

				output.WriteLine($"Pushing form '{document.Title}' to Microsoft Forms...", ConsoleColor.Blue);

				// 1. Create the Form
				var onlineForm = await formsApiClient.CreateFormAsync(
					tenantId, ownerId, ownerType, document.Title, document.Description, cancellationToken);

				output.WriteLine($"Created online form: {onlineForm.Id}", ConsoleColor.Green);

				// 2. Create the Questions
				foreach (var question in document.Questions)
				{
					output.WriteLine($"  Adding question: '{question.Text}' ({question.Type})...");
					await formsApiClient.CreateQuestionAsync(
						tenantId, ownerId, ownerType, onlineForm.Id, question.Text, question.Type, question.Required, question.Options, cancellationToken);
				}

				output.WriteLine("Form pushed successfully!", ConsoleColor.Green);
				return CommandResult.Success();
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Failed to push form: {ex.Message}", ex);
			}
		}
	}
}
