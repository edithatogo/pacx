using System.ComponentModel.DataAnnotations;
using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Tool
{
	[Command("tool", "source", "add", HelpText = "Registers a new tool source feed.")]
	[Alias("tool", "source", "register")]
	public class SourceAddCommand : IValidatableObject
	{
		[Option("name", "n", Order = 1, Required = true, HelpText = "A friendly name for the source.")]
		[Required]
		public string Name { get; set; } = string.Empty;

		[Option("url", "u", Order = 2, Required = true, HelpText = "The URL of the source feed.")]
		[Required]
		public string Url { get; set; } = string.Empty;

		[Option("type", "t", Order = 3, DefaultValue = "nuget", HelpText = "Source type (nuget, mcp, npm).")]
		public string Type { get; set; } = "nuget";

		[Option("pat", Order = 4, HelpText = "Personal Access Token for private feeds.")]
		public string? PersonalAccessToken { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (!Uri.TryCreate(Url, UriKind.Absolute, out var uri) || (uri.Scheme != "http" && uri.Scheme != "https"))
			{
				yield return new ValidationResult($"The URL <{Url}> is not a valid HTTP/HTTPS URL.", [nameof(Url)]);
			}

			var validTypes = new[] { "nuget", "mcp", "npm" };
			if (!validTypes.Contains(Type, StringComparer.OrdinalIgnoreCase))
			{
				yield return new ValidationResult($"Source type must be one of: {string.Join(", ", validTypes)}.", [nameof(Type)]);
			}
		}
	}
}
