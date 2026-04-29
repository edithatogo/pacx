namespace Greg.Xrm.Command.Diagnostics
{
	/// <summary>
	/// Provides access to the current correlation ID for the CLI invocation.
	/// </summary>
	public interface ICorrelationIdProvider
	{
		/// <summary>
		/// Gets the current correlation ID.
		/// </summary>
		string Current { get; }
	}
}
