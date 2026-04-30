using System.Net.Http.Json;
using System.Text.Json;

namespace Greg.Xrm.Command.Services.ReleasePlan
{
	/// <summary>
	/// Fetches release plan data from the public Microsoft 365 Roadmap API.
	/// The public API returns a JSON array of roadmap items.
	/// </summary>
	public class ReleasePlanClient(
		IHttpClientFactory httpClientFactory) : IReleasePlanClient
	{
		/// <summary>
		/// Public Microsoft 365 Roadmap API endpoint.
		/// Returns a JSON array of all roadmap items.
		/// </summary>
		public const string RoadmapApiUrl = "https://learn.microsoft.com/api/roadmap/filters";

		private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
		{
			WriteIndented = true
		};

		public async Task<ReleasePlanSnapshot> FetchAsync(CancellationToken cancellationToken)
		{
			var client = httpClientFactory.CreateClient();
			var response = await client.GetAsync(RoadmapApiUrl, cancellationToken)
				.ConfigureAwait(false);

			response.EnsureSuccessStatusCode();

			var items = await response.Content
				.ReadFromJsonAsync<List<ReleasePlanItem>>(JsonOptions, cancellationToken)
				.ConfigureAwait(false);

			var etag = response.Headers.TryGetValues("ETag", out var etagValues)
				? etagValues.FirstOrDefault()
				: null;

			return new ReleasePlanSnapshot
			{
				Items = items ?? [],
				FetchedAtUtc = DateTime.UtcNow,
				Etag = etag,
			};
		}
	}
}
