using System;

namespace Greg.Xrm.Command.Services.AiBuilder
{
	public sealed class AiBuilderOperationResult
	{
		private AiBuilderOperationResult(bool isSuccess, string? errorMessage, Exception? exception)
		{
			this.IsSuccess = isSuccess;
			this.ErrorMessage = errorMessage;
			this.Exception = exception;
		}

		public bool IsSuccess { get; }

		public string? ErrorMessage { get; }

		public Exception? Exception { get; }

		public static AiBuilderOperationResult Success()
		{
			return new AiBuilderOperationResult(true, null, null);
		}

		public static AiBuilderOperationResult Fail(string errorMessage, Exception? exception = null)
		{
			return new AiBuilderOperationResult(false, errorMessage, exception);
		}
	}

	public sealed class AiBuilderOperationResult<T>
	{
		public AiBuilderOperationResult(bool isSuccess, T? value, string? errorMessage, Exception? exception)
		{
			this.IsSuccess = isSuccess;
			this.Value = value;
			this.ErrorMessage = errorMessage;
			this.Exception = exception;
		}

		public bool IsSuccess { get; }

		public T? Value { get; }

		public string? ErrorMessage { get; }

		public Exception? Exception { get; }
	}
}
