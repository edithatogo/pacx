using Newtonsoft.Json;

namespace Greg.Xrm.Command.Services.Settings
{
	public class SettingsRepository : ISettingsRepository
	{
		private string? settingsFolder = null;
		private readonly IStorage storage;

		public SettingsRepository(IStorage storage)
		{
			this.storage = storage;
		}


		private void InitializeSettings()
		{
			if (this.settingsFolder != null) return;
			this.settingsFolder = this.storage.GetOrCreateStorageFolder().FullName;
		}



		public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
		{
			this.InitializeSettings();
			if (this.settingsFolder == null)
				throw new InvalidOperationException("Settings folder is not initialized.");

			var fileName = Path.Combine(this.settingsFolder, $"{key}.json");

			if (File.Exists(fileName))
			{
				var json = await File.ReadAllTextAsync(fileName, cancellationToken).ConfigureAwait(false);

				if (typeof(T) == typeof(string))
					return (T)(object)json;


				return JsonConvert.DeserializeObject<T>(json);
			}

			return default;
		}

		public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
		{
			this.InitializeSettings();
			if (this.settingsFolder == null)
				throw new InvalidOperationException("Settings folder is not initialized.");

			var fileName = Path.Combine(this.settingsFolder, $"{key}.json");

			string? json;
			if (typeof(T) == typeof(string))
			{
				json = value?.ToString();
			}
			else
			{
				json = JsonConvert.SerializeObject(value, Formatting.Indented);
			}

			await File.WriteAllTextAsync(fileName, json ?? string.Empty, System.Text.Encoding.UTF8, cancellationToken).ConfigureAwait(false);
		}
	}
}
