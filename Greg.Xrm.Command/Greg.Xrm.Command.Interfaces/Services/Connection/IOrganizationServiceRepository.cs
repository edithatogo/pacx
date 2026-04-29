using Microsoft.PowerPlatform.Dataverse.Client;

namespace Greg.Xrm.Command.Services.Connection
{
	public interface IOrganizationServiceRepository
	{
		string GetTokenCachePath();

		Task<ConnectionSetting> GetAllConnectionDefinitionsAsync(CancellationToken cancellationToken = default);

		Task<IOrganizationServiceAsync2> GetCurrentConnectionAsync(CancellationToken cancellationToken = default);
		Task<IOrganizationServiceAsync2> GetConnectionByNameAsync(string connectionName, CancellationToken cancellationToken = default);

		Task<string?> GetEnvironmentFromConnectioStringAsync(string connectionName, CancellationToken cancellationToken = default);

		Task<string?> GetConnectionStringAsync(string? connectionName = null, CancellationToken cancellationToken = default);

		Task SetConnectionAsync(string name, string connectionString, CancellationToken cancellationToken = default);
		Task DeleteConnectionAsync(string name, CancellationToken cancellationToken = default);

		Task SetDefaultAsync(string name, CancellationToken cancellationToken = default);
		Task SetDefaultSolutionAsync(string uniqueName, CancellationToken cancellationToken = default);
		Task<string?> GetCurrentDefaultSolutionAsync(CancellationToken cancellationToken = default);
		Task RenameConnectionAsync(string oldName, string newName, CancellationToken cancellationToken = default);
	}
}
