namespace Greg.Xrm.Command.Services.ReleasePlan
{
	public class ReleasePlanService(
		IReleasePlanClient client,
		IReleasePlanCache cache) : IReleasePlanService
	{
		/// <summary>
		/// Maximum age of cached data before a refresh is triggered (24 hours).
		/// </summary>
		private static readonly TimeSpan MaxCacheAge = TimeSpan.FromHours(24);

		public async Task<List<ReleasePlanItem>> GetItemsAsync(
			ReleasePlanFilter? filter,
			CancellationToken cancellationToken)
		{
			var snapshot = await cache.GetAsync(cancellationToken).ConfigureAwait(false);

			if (snapshot is null || (DateTime.UtcNow - snapshot.FetchedAtUtc) > MaxCacheAge)
			{
				snapshot = await RefreshAsync(cancellationToken).ConfigureAwait(false);
			}

			var items = snapshot.Items.AsEnumerable();

			if (filter is not null)
			{
				if (!string.IsNullOrWhiteSpace(filter.Product))
				{
					items = items.Where(i =>
						i.Product.Contains(filter.Product, StringComparison.OrdinalIgnoreCase));
				}

				if (!string.IsNullOrWhiteSpace(filter.Status))
				{
					items = items.Where(i =>
						i.Status.Contains(filter.Status, StringComparison.OrdinalIgnoreCase));
				}

				if (!string.IsNullOrWhiteSpace(filter.Category))
				{
					items = items.Where(i =>
						i.Category.Contains(filter.Category, StringComparison.OrdinalIgnoreCase));
				}

				if (!string.IsNullOrWhiteSpace(filter.Search))
				{
					var search = filter.Search;
					items = items.Where(i =>
						i.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
						i.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
						i.Tags.Any(t => t.Contains(search, StringComparison.OrdinalIgnoreCase)));
				}

				if (filter.MaxResults.HasValue && filter.MaxResults.Value > 0)
				{
					items = items.Take(filter.MaxResults.Value);
				}
			}

			return items.ToList();
		}

		public async Task<ReleasePlanSnapshot> RefreshAsync(CancellationToken cancellationToken)
		{
			var snapshot = await client.FetchAsync(cancellationToken).ConfigureAwait(false);
			await cache.SetAsync(snapshot, cancellationToken).ConfigureAwait(false);
			return snapshot;
		}

		public Task<TimeSpan?> GetCacheAgeAsync(CancellationToken cancellationToken)
		{
			return cache.GetAgeAsync(cancellationToken);
		}

		public async Task<List<string>> GetProductsAsync(CancellationToken cancellationToken)
		{
			var snapshot = await cache.GetAsync(cancellationToken).ConfigureAwait(false);
			return snapshot?.Items
				.Select(i => i.Product)
				.Where(p => !string.IsNullOrWhiteSpace(p))
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.OrderBy(p => p)
				.ToList() ?? [];
		}
	}
}
