using Microsoft.PowerPlatform.Dataverse.Client;

namespace Greg.Xrm.Command.Commands.Settings.Model
{
	public interface ISettingDefinitionRepository
	{
		Task<IReadOnlyList<SettingDefinition>> GetAllAsync(IOrganizationServiceAsync2 crm, Guid? solutionId, bool onlyVisible, CancellationToken cancellationToken = default);
		Task<SettingDefinition?> GetByUniqueNameAsync(IOrganizationServiceAsync2 crm, string uniqueName, CancellationToken cancellationToken = default);
	}
}
