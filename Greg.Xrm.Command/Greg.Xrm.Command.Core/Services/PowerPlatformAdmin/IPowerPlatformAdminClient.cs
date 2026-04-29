using System.Text.Json;

namespace Greg.Xrm.Command.Services.PowerPlatformAdmin
{
	public interface IPowerPlatformAdminClient
	{
		// Management apps
		Task<JsonDocument> ListManagementAppsAsync(CancellationToken cancellationToken);

		// Tenant settings
		Task<JsonDocument> GetTenantSettingsAsync(CancellationToken cancellationToken);

		Task SetTenantSettingsAsync(object settings, CancellationToken cancellationToken);
	}
}
