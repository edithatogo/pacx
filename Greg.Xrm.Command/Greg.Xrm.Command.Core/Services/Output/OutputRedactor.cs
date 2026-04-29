using System.Text.RegularExpressions;

namespace Greg.Xrm.Command.Services.Output
{
	internal static class OutputRedactor
	{
		private static readonly Regex BearerTokenPattern = new(
			@"Bearer [A-Za-z0-9\-_.]+",
			RegexOptions.Compiled | RegexOptions.CultureInvariant);

		public static string? Redact(object? text)
		{
			if (text is null)
			{
				return null;
			}

			var value = text.ToString();
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}

			return BearerTokenPattern.Replace(value, "Bearer [REDACTED]");
		}
	}
}
