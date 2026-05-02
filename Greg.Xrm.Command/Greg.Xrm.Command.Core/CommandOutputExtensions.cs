using System.Text.Json;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command
{
	public static class CommandOutputExtensions
	{
		private static readonly JsonSerializerOptions JsonOptions = new()
		{
			WriteIndented = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase
		};

		public static string ToOutputString(this CommandResult result, string format)
		{
			return format.ToLowerInvariant() switch
			{
				"json" => JsonSerializer.Serialize((IReadOnlyDictionary<string, object>)result, JsonOptions),
				"yaml" => YamlSerializer.Serialize((IReadOnlyDictionary<string, object>)result),
				_ => result.ToString() ?? string.Empty
			};
		}
	}
}
