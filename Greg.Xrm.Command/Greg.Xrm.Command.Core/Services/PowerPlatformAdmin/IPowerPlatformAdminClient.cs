using System.Text.Json;

namespace Greg.Xrm.Command.Services.PowerPlatformAdmin
{
	public interface IPowerPlatformAdminClient
	{
		Task<JsonDocument> ListManagementAppsAsync(CancellationToken cancellationToken);
		Task<JsonDocument> GetTenantSettingsAsync(CancellationToken cancellationToken);
		Task SetTenantSettingsAsync(object settings, CancellationToken cancellationToken);

		Task<JsonDocument> CreateEnvironmentAsync(string name, string type, string region, string currency, string language, CancellationToken ct);
		Task<JsonDocument> GetEnvironmentAsync(string environmentId, CancellationToken ct);
		Task<JsonDocument> ListEnvironmentsAsync(CancellationToken ct);
		Task<JsonDocument> ResetEnvironmentAsync(string environmentId, string resetType, CancellationToken ct);
		Task<JsonDocument> CopyEnvironmentAsync(string sourceEnvId, string targetName, string mode, CancellationToken ct);
		Task<JsonDocument> BackupEnvironmentAsync(string environmentId, string label, CancellationToken ct);
		Task<JsonDocument> RestoreEnvironmentAsync(string environmentId, string backupId, CancellationToken ct);
		Task<JsonDocument> GetEnvironmentCapacityAsync(string environmentId, CancellationToken ct);
	}
}
