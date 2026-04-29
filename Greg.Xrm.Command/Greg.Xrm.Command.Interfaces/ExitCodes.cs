namespace Greg.Xrm.Command
{
	public static class ExitCodes
	{
		public const int Success = 0;
		public const int UsageError = 1;
		public const int AuthError = 2;
		public const int ApiError = 3;
		public const int ValidationError = 4;
		public const int NetworkError = 5;
		public const int InternalError = 64;
		public const int Cancelled = 130;

		public static int FromException(Exception? exception)
		{
			if (exception == null) return ApiError;

			if (exception is ArgumentException
				|| exception is InvalidDataException
				|| exception is System.Data.DataException
				|| exception is CommandException)
			{
				return UsageError;
			}

			if (exception is System.ComponentModel.DataAnnotations.ValidationException)
			{
				return ValidationError;
			}

			if (exception is System.Net.Http.HttpRequestException
				|| exception is TimeoutException
				|| exception is TaskCanceledException)
			{
				return NetworkError;
			}

			if (exception.GetType().Name.Contains("Authentication", StringComparison.OrdinalIgnoreCase)
				|| exception.GetType().Name.Contains("Msal", StringComparison.OrdinalIgnoreCase)
				|| exception.Message.Contains("Authentication required", StringComparison.OrdinalIgnoreCase))
			{
				return AuthError;
			}

			if (exception.GetType().Name.Contains("OrganizationService", StringComparison.OrdinalIgnoreCase)
				|| exception.GetType().Name.Contains("ServiceFault", StringComparison.OrdinalIgnoreCase))
			{
				return ApiError;
			}

			return InternalError;
		}
	}
}
