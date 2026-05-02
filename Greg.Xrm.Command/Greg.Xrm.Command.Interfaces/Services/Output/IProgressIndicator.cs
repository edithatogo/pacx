namespace Greg.Xrm.Command.Services.Output
{
	public interface IProgressIndicator : IDisposable
	{
		void Report(double progress, string message);
		void SetMessage(string message);
		void Complete(string? completionMessage = null);
		void Fail(string? errorMessage = null);
	}
}
