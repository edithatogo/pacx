using Microsoft.PowerPlatform.Dataverse.Client;

namespace Greg.Xrm.Command.Model
{
	public interface ISavedQueryRepository
	{
		Task<IReadOnlyList<SavedQuery>> GetByIdAsync(IOrganizationServiceAsync2 crm, IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
		Task<IReadOnlyList<SavedQuery>> GetContainingAsync(IOrganizationServiceAsync2 crm, string tableName, string columnName, CancellationToken cancellationToken = default);
		Task<IReadOnlyList<SavedQuery>> GetByTableNameAsync(IOrganizationServiceAsync2 crm, string tableName, CancellationToken cancellationToken = default);
		Task<IReadOnlyList<SavedQuery>> GetByNameAsync(IOrganizationServiceAsync2 crm, string viewName, CancellationToken cancellationToken = default);
	}
}
