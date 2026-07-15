using System.Text.Json;

namespace Greg.Xrm.Command.Services.ReleasePlan
{
	/// <summary>
	/// File-based local cache for release plan data.
	/// Stores the snapshot as JSON in the user's local app data directory.
	/// </summary>
	public class ReleasePlanCache : IReleasePlanCache
	{
		private readonly string? cacheFilePath;

		private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
		{
			WriteIndented = true
		};

		private static string DefaultCacheFilePath
		{
			get
			{
				var dir = Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
					"PACX",
					"release-plan-cache");
				Directory.CreateDirectory(dir);
				return Path.Combine(dir, "snapshot.json");
			}
		}

		public ReleasePlanCache(string? cacheFilePath = null)
		{
			this.cacheFilePath = cacheFilePath;
		}

		private string CacheFilePath => this.cacheFilePath ?? DefaultCacheFilePath;

		public async Task<ReleasePlanSnapshot?> GetAsync(CancellationToken cancellationToken)
		{
			var path = CacheFilePath;
			if (!File.Exists(path))
			{
				return null;
			}

			try
			{
				var json = await File.ReadAllTextAsync(path, cancellationToken)
					.ConfigureAwait(false);
				return JsonSerializer.Deserialize<ReleasePlanSnapshot>(json, JsonOptions);
			}
			catch (JsonException)
			{
				return null;
			}
		}

		public Task SetAsync(ReleasePlanSnapshot snapshot, CancellationToken cancellationToken)
		{
			var path = CacheFilePath;
			var directory = Path.GetDirectoryName(path);
			if (!string.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}
			var json = JsonSerializer.Serialize(snapshot, JsonOptions);
			return File.WriteAllTextAsync(path, json, cancellationToken);
		}

		public async Task<TimeSpan?> GetAgeAsync(CancellationToken cancellationToken)
		{
			var snapshot = await GetAsync(cancellationToken).ConfigureAwait(false);
			if (snapshot is null)
			{
				return null;
			}

			return DateTime.UtcNow - snapshot.FetchedAtUtc;
		}
	}
}
