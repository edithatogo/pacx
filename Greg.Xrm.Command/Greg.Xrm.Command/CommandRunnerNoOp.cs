namespace Greg.Xrm.Command
{
	sealed class CommandRunnerNoOp(int result) : ICommandRunner
	{
		public Task<int> RunCommandAsync(CancellationToken cancellationToken)
		{
			return Task.FromResult(result);
		}
	}
}
