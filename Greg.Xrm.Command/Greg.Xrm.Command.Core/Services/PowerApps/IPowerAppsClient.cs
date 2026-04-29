using System.Text.Json;

namespace Greg.Xrm.Command.Services.PowerApps
{
	public interface IPowerAppsClient
	{
		// Power App CRUD
		Task<JsonDocument> ListAppsAsync(string? environmentName, bool asAdmin, CancellationToken cancellationToken);

		Task<JsonDocument> GetAppAsync(string appName, string? environmentName, bool asAdmin, CancellationToken cancellationToken);

		Task DeleteAppAsync(string appName, string? environmentName, bool asAdmin, CancellationToken cancellationToken);

		Task<(byte[] content, string fileName)> ExportAppAsync(string appName, string environmentName, CancellationToken cancellationToken);

		// Consent
		Task SetAppConsentAsync(string appName, string environmentName, bool bypass, CancellationToken cancellationToken);

		// Owner
		Task SetAppOwnerAsync(string appName, string environmentName, string newOwnerId, string? roleForOldOwner, CancellationToken cancellationToken);

		// Permissions
		Task<JsonDocument> ListAppPermissionsAsync(string appName, string? environmentName, bool asAdmin, CancellationToken cancellationToken);

		Task AddAppPermissionAsync(string appName, string? environmentName, bool asAdmin, string principalId, string principalType, string roleName, CancellationToken cancellationToken);

		Task RemoveAppPermissionAsync(string appName, string? environmentName, bool asAdmin, string principalId, CancellationToken cancellationToken);
	}
}
