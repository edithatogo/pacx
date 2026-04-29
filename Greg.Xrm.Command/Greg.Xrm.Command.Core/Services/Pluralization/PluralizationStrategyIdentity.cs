namespace Greg.Xrm.Command.Services.Pluralization
{
	public class PluralizationStrategyIdentity : IPluralizationStrategy
	{
		public Task<string> GetPluralForAsync(string word, CancellationToken cancellationToken = default)
		{
			return Task.FromResult(word);
		}
	}
}
