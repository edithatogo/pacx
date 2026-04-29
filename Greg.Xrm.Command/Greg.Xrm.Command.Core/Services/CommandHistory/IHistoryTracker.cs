namespace Greg.Xrm.Command.Services.CommandHistory
{
	public interface IHistoryTracker
	{
		Task SetMaxLengthAsync(int maxLength, CancellationToken cancellationToken = default);


		Task AddAsync(CancellationToken cancellationToken = default, params string[] command);

		Task<IReadOnlyList<string>> GetLastAsync(int? count, CancellationToken cancellationToken = default);

		Task ClearAsync(CancellationToken cancellationToken = default);
	}

}
