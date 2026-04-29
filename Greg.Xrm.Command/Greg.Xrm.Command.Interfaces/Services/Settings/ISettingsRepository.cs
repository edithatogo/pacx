namespace Greg.Xrm.Command.Services.Settings
{
	public interface ISettingsRepository
	{
		Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
		Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);
	}
}
