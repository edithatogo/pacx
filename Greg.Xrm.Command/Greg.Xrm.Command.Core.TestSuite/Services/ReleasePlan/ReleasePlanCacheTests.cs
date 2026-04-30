using Greg.Xrm.Command.Services.ReleasePlan;

namespace Greg.Xrm.Command.TestSuite.Services.ReleasePlan
{
	[TestClass]
	public class ReleasePlanCacheTests
	{
		[TestMethod]
		public async Task CacheShouldReturnNullWhenNoData()
		{
			// Use a unique temp path for the cache
			var cache = new ReleasePlanCache();

			var result = await cache.GetAsync(CancellationToken.None);

			// No cache file exists initially in the default path
			// The cache file is at %LOCALAPPDATA%/PACX/release-plan-cache/snapshot.json
			// which won't exist in CI — so this should return null
			Assert.IsNull(result, "Expected null when no cache exists.");
		}

		[TestMethod]
		public async Task CacheShouldRoundTripSnapshot()
		{
			var cache = new ReleasePlanCache();

			var snapshot = new ReleasePlanSnapshot
			{
				Items =
				[
					new ReleasePlanItem { Id = "1", Title = "Test" },
				],
				FetchedAtUtc = DateTime.UtcNow,
				Etag = "roundtrip-test",
			};

			await cache.SetAsync(snapshot, CancellationToken.None);
			var result = await cache.GetAsync(CancellationToken.None);

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Items.Count);
			Assert.AreEqual("roundtrip-test", result.Etag);
		}

		[TestMethod]
		public async Task GetAgeShouldReturnCorrectAge()
		{
			var cache = new ReleasePlanCache();

			// Write a snapshot with a known time
			var snapshot = new ReleasePlanSnapshot
			{
				Items = [],
				FetchedAtUtc = DateTime.UtcNow.AddHours(-2),
				Etag = "age-test",
			};

			await cache.SetAsync(snapshot, CancellationToken.None);
			var age = await cache.GetAgeAsync(CancellationToken.None);

			Assert.IsNotNull(age);
			Assert.IsTrue(age.Value.TotalHours >= 1.5, $"Expected age ~2h, got {age.Value.TotalHours:F1}h");
			Assert.IsTrue(age.Value.TotalHours <= 3, $"Expected age ~2h, got {age.Value.TotalHours:F1}h");
		}

		[TestCleanup]
		public void Cleanup()
		{
			// Clean up test cache file
			var cacheDir = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"PACX",
				"release-plan-cache");
			var cacheFile = Path.Combine(cacheDir, "snapshot.json");
			if (File.Exists(cacheFile))
			{
				File.Delete(cacheFile);
			}
		}
	}
}
