using Microsoft.PowerPlatform.Dataverse.Client;

namespace Greg.Xrm.Command.Model
{
	public interface IUserQueryRepository
	{
		Task<IReadOnlyList<UserQuery>> GetByIdAsync(IOrganizationServiceAsync2 crm, IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
		Task<IReadOnlyList<UserQuery>> GetContainingAsync(IOrganizationServiceAsync2 crm, string tableName, string columnName, CancellationToken cancellationToken = default);
		Task<IReadOnlyList<UserQuery>> GetByTableNameAsync(IOrganizationServiceAsync2 crm, string tableName, CancellationToken cancellationToken = default);
		Task<IReadOnlyList<UserQuery>> GetByNameAsync(IOrganizationServiceAsync2 crm, string viewName, CancellationToken cancellationToken = default);
	}
}
