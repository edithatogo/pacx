namespace Greg.Xrm.Command.Services.ReleasePlan
{
	/// <summary>
	/// Client for fetching Microsoft 365 / Power Platform release plan data.
	/// </summary>
	public interface IReleasePlanClient
	{
		/// <summary>
		/// Fetches the latest release plan items from the public Roadmap API.
		/// </summary>
		Task<ReleasePlanSnapshot> FetchAsync(CancellationToken cancellationToken);
	}
}
