using Greg.Xrm.Command.Services.Settings;

namespace Greg.Xrm.Command.Services.CommandHistory
{
	public class HistoryTracker : IHistoryTracker
	{
		private readonly ISettingsRepository settings;
		const string CommandHistoryKey = "commandHistory";


		public HistoryTracker(ISettingsRepository settings)
		{
			this.settings = settings;
		}

		public async Task AddAsync(CancellationToken cancellationToken = default, params string[] command)
		{
			var history = await this.settings.GetAsync<CommandHistory>(CommandHistoryKey, cancellationToken).ConfigureAwait(false);
			history ??= new CommandHistory();
			history.Add(command);
			await this.settings.SetAsync(CommandHistoryKey, history, cancellationToken).ConfigureAwait(false);
		}

		public async Task<IReadOnlyList<string>> GetLastAsync(int? count, CancellationToken cancellationToken = default)
		{
			var history = await this.settings.GetAsync<CommandHistory>(CommandHistoryKey, cancellationToken).ConfigureAwait(false);
			if (history == null)
			{
				return Array.Empty<string>();
			}

			if (count == null)
			{
				return history.Commands;
			}

			if (count >= history.Commands.Count)
			{
				return history.Commands.ToArray();
			}

			return history.Commands.Skip(history.Commands.Count - count.Value).ToArray();
		}

		public async Task SetMaxLengthAsync(int maxLength, CancellationToken cancellationToken = default)
		{
			var history = await this.settings.GetAsync<CommandHistory>(CommandHistoryKey, cancellationToken).ConfigureAwait(false);
			history ??= new CommandHistory();
			history.MaxSize = maxLength;

			if (history.Commands.Count > maxLength)
			{
				history.Commands = history.Commands.Skip(history.Commands.Count - maxLength).ToList();
			}

			await this.settings.SetAsync(CommandHistoryKey, history, cancellationToken).ConfigureAwait(false);
		}

		public async Task ClearAsync(CancellationToken cancellationToken = default)
		{
			var history = await this.settings.GetAsync<CommandHistory>(CommandHistoryKey, cancellationToken).ConfigureAwait(false);
			if (history == null) return;

			history.Commands.Clear();
			await this.settings.SetAsync(CommandHistoryKey, history, cancellationToken).ConfigureAwait(false);
		}
	}

}
