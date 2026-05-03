using System.Reflection;
using System.Text.RegularExpressions;
using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.Validate
{
	public interface ICommandReferenceParityValidator
	{
		CommandReferenceParityResult Validate(ICommandRegistry registry, string docsIndexPath);
	}

	public sealed class CommandReferenceParityResult
	{
		public CommandReferenceParityResult(IReadOnlyList<string> missingPages, IReadOnlyList<string> extraPages, IReadOnlyList<string> contentIssues, Exception? exception = null)
		{
			ArgumentNullException.ThrowIfNull(missingPages);
			ArgumentNullException.ThrowIfNull(extraPages);
			ArgumentNullException.ThrowIfNull(contentIssues);
			this.MissingPages = missingPages;
			this.ExtraPages = extraPages;
			this.ContentIssues = contentIssues;
			this.Exception = exception;
		}

		public IReadOnlyList<string> MissingPages { get; }

		public IReadOnlyList<string> ExtraPages { get; }

		public IReadOnlyList<string> ContentIssues { get; }

		public Exception? Exception { get; }

		public bool IsValid => this.Exception is null && this.MissingPages.Count == 0 && this.ExtraPages.Count == 0 && this.ContentIssues.Count == 0;
	}

	public sealed class CommandReferenceParityValidator : ICommandReferenceParityValidator
	{
		private static readonly Regex MarkdownLinkRegex = new(@"\[[^\]]+\]\((?<path>[^)]+?\.md)\)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		private static readonly Regex HeadingRegex = new(@"^#\s+(?<heading>.+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		public CommandReferenceParityResult Validate(ICommandRegistry registry, string docsIndexPath)
		{
			try
			{
				if (registry is null) throw new ArgumentNullException(nameof(registry));
				if (string.IsNullOrWhiteSpace(docsIndexPath)) throw new ArgumentException("A docs index path is required.", nameof(docsIndexPath));

				var expectedPages = registry.Commands
					.Where(command => command.PluginName is null)
					.Select(command => NormalizeSlug(command.ExpandedVerbs) + ".md")
					.ToHashSet(StringComparer.OrdinalIgnoreCase);

				var docsText = File.ReadAllText(docsIndexPath);
				var actualPages = MarkdownLinkRegex
					.Matches(docsText)
					.Select(match => NormalizeRelativePath(match.Groups["path"].Value))
					.Where(path => !string.Equals(path, "index.md", StringComparison.OrdinalIgnoreCase))
					.ToHashSet(StringComparer.OrdinalIgnoreCase);

				var missingPages = expectedPages.Except(actualPages, StringComparer.OrdinalIgnoreCase).OrderBy(path => path, StringComparer.OrdinalIgnoreCase).ToList();
				var extraPages = actualPages.Except(expectedPages, StringComparer.OrdinalIgnoreCase).OrderBy(path => path, StringComparer.OrdinalIgnoreCase).ToList();
				var contentIssues = new List<string>();

				if (missingPages.Count == 0)
				{
					var docsRoot = Path.GetDirectoryName(Path.GetFullPath(docsIndexPath)) ?? string.Empty;
					foreach (var command in registry.Commands.Where(command => command.PluginName is null))
					{
						var pagePath = Path.Combine(docsRoot, NormalizeSlug(command.ExpandedVerbs) + ".md");
						if (!File.Exists(pagePath))
						{
							continue;
						}

						contentIssues.AddRange(ValidatePage(command, pagePath));
					}
				}

				return new CommandReferenceParityResult(missingPages, extraPages, contentIssues);
			}
			catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
			{
				return new CommandReferenceParityResult(Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>(), ex);
			}
		}

		private static IReadOnlyList<string> ValidatePage(CommandDefinition command, string pagePath)
		{
			var issues = new List<string>();
			var lines = File.ReadAllLines(pagePath);
			if (lines.Length == 0)
			{
				issues.Add($"{Path.GetFileName(pagePath)} is empty.");
				return issues;
			}

			var expectedHeading = $"# {command.ExpandedVerbs.ToLowerInvariant()}";
			if (!HeadingRegex.IsMatch(lines[0]) || !string.Equals(lines[0].Trim(), expectedHeading, StringComparison.OrdinalIgnoreCase))
			{
				issues.Add($"{Path.GetFileName(pagePath)} heading should be '{expectedHeading}'.");
			}

			var optionRows = ParseOptionRows(lines);
			var expectedRows = BuildExpectedRows(command);
			ValidateSummary(command, lines, issues);
			ValidateUsage(command, lines, issues);

			if (optionRows.Count != expectedRows.Count)
			{
				issues.Add($"{Path.GetFileName(pagePath)} has {optionRows.Count} option row(s) but expected {expectedRows.Count}.");
				return issues;
			}

			for (var i = 0; i < expectedRows.Count; i++)
			{
				var expected = expectedRows[i];
				var actual = optionRows[i];
				if (!string.Equals(expected.Long, actual.Long, StringComparison.OrdinalIgnoreCase))
				{
					issues.Add($"{Path.GetFileName(pagePath)} option {i + 1} expected long name '{expected.Long}' but found '{actual.Long}'.");
				}

				if (!string.Equals(expected.Short, actual.Short, StringComparison.OrdinalIgnoreCase))
				{
					issues.Add($"{Path.GetFileName(pagePath)} option {i + 1} expected short name '{expected.Short}' but found '{actual.Short}'.");
				}

				if (!string.Equals(expected.Type, actual.Type, StringComparison.OrdinalIgnoreCase))
				{
					issues.Add($"{Path.GetFileName(pagePath)} option {i + 1} expected type '{expected.Type}' but found '{actual.Type}'.");
				}

				if (!string.Equals(expected.Required, actual.Required, StringComparison.OrdinalIgnoreCase))
				{
					issues.Add($"{Path.GetFileName(pagePath)} option {i + 1} expected required flag '{expected.Required}' but found '{actual.Required}'.");
				}

				if (!string.Equals(expected.Description, actual.Description, StringComparison.Ordinal))
				{
					issues.Add($"{Path.GetFileName(pagePath)} option {i + 1} description differs.");
				}
			}

			return issues;
		}

		private static void ValidateSummary(CommandDefinition command, IReadOnlyList<string> lines, ICollection<string> issues)
		{
			if (string.IsNullOrWhiteSpace(command.HelpText))
			{
				return;
			}

			var summary = ExtractSummary(lines);
			var expectedSummary = NormalizeDescription(command.HelpText);
			if (!string.Equals(summary, expectedSummary, StringComparison.Ordinal))
			{
				issues.Add($"{command.ExpandedVerbs}.md summary differs.");
			}
		}

		private static void ValidateUsage(CommandDefinition command, IReadOnlyList<string> lines, ICollection<string> issues)
		{
			var usage = ExtractUsage(lines);
			if (usage is null)
			{
				issues.Add($"{command.ExpandedVerbs}.md usage block is missing.");
				return;
			}

			var expectedUsage = $"pacx {command.ExpandedVerbs.ToLowerInvariant()}";
			if (!string.Equals(usage, expectedUsage, StringComparison.OrdinalIgnoreCase))
			{
				issues.Add($"{command.ExpandedVerbs}.md usage differs.");
			}
		}

		private static IReadOnlyList<OptionRow> ParseOptionRows(IReadOnlyList<string> lines)
		{
			var rows = new List<OptionRow>();
			var inOptions = false;

			for (var i = 0; i < lines.Count; i++)
			{
				var line = lines[i].Trim();
				if (!inOptions)
				{
					if (line.Equals("## Options", StringComparison.OrdinalIgnoreCase))
					{
						inOptions = true;
					}
					continue;
				}

				if (string.IsNullOrWhiteSpace(line))
				{
					if (rows.Count > 0)
					{
						break;
					}
					continue;
				}

				if (!line.StartsWith("|", StringComparison.Ordinal))
				{
					if (rows.Count > 0)
					{
						break;
					}
					continue;
				}

				if (line.Contains("---", StringComparison.Ordinal))
				{
					continue;
				}

				var columns = line.Split('|', StringSplitOptions.TrimEntries);
				var values = columns.Where(column => !string.IsNullOrWhiteSpace(column)).ToList();
				if (values.Count < 5)
				{
					continue;
				}

				rows.Add(new OptionRow(
					CleanCell(values[0]),
					CleanCell(values[1]),
					CleanCell(values[2]),
					CleanCell(values[3]),
					CleanCell(values[4])));
			}

			return rows;
		}

		private static IReadOnlyList<OptionRow> BuildExpectedRows(CommandDefinition command)
		{
			var instance = Activator.CreateInstance(command.CommandType);
			return command.Options
				.Select(option =>
				{
					var defaultValue = option.Option.DefaultValue ?? option.Property.GetValue(instance);
					return new OptionRow(
						"--" + option.Option.LongName,
						option.Option.ShortName ?? string.Empty,
						GetDocsTypeName(option.Property.PropertyType, defaultValue),
						option.IsRequired ? "True" : "False",
						NormalizeDescription(option.Option.HelpText ?? string.Empty));
				})
				.ToList();
		}

		private static string GetDocsTypeName(Type propertyType, object? defaultValue)
		{
			if (propertyType == typeof(string))
			{
				return defaultValue is null ? "string?" : "string";
			}

			if (propertyType == typeof(bool))
			{
				return "bool";
			}

			if (propertyType == typeof(int))
			{
				return "int";
			}

			if (propertyType == typeof(long))
			{
				return "long";
			}

			if (propertyType == typeof(double))
			{
				return "double";
			}

			if (propertyType == typeof(decimal))
			{
				return "decimal";
			}

			if (propertyType == typeof(Guid))
			{
				return "Guid";
			}

			var nullableUnderlyingType = Nullable.GetUnderlyingType(propertyType);
			if (nullableUnderlyingType is not null)
			{
				return GetDocsTypeName(nullableUnderlyingType, defaultValue) + "?";
			}

			if (propertyType.IsArray)
			{
				var elementType = propertyType.GetElementType() ?? typeof(object);
				var suffix = defaultValue is null ? "?" : string.Empty;
				return $"{GetDocsTypeName(elementType, null)}[]{suffix}";
			}

			if (propertyType.IsGenericType && propertyType.GetGenericArguments().Length == 1)
			{
				var genericArgument = propertyType.GetGenericArguments()[0];
				if (genericArgument == typeof(string))
				{
					return defaultValue is null ? "string[]?" : "string[]";
				}
			}

			return propertyType.Name;
		}

		private static string NormalizeDescription(string text)
		{
			return text.Replace("\r", " ").Replace("\n", " ").Trim();
		}

		private static string ExtractSummary(IReadOnlyList<string> lines)
		{
			var sawHeading = false;
			foreach (var rawLine in lines)
			{
				var line = rawLine.Trim();
				if (!sawHeading)
				{
					if (HeadingRegex.IsMatch(line))
					{
						sawHeading = true;
					}
					continue;
				}

				if (string.IsNullOrWhiteSpace(line))
				{
					continue;
				}

				if (line.StartsWith("## ", StringComparison.Ordinal))
				{
					return string.Empty;
				}

				return NormalizeDescription(line);
			}

			return string.Empty;
		}

		private static string? ExtractUsage(IReadOnlyList<string> lines)
		{
			var inUsage = false;
			var inCodeBlock = false;

			foreach (var rawLine in lines)
			{
				var line = rawLine.Trim();

				if (!inUsage)
				{
					if (line.Equals("## Usage", StringComparison.OrdinalIgnoreCase))
					{
						inUsage = true;
					}
					continue;
				}

				if (line.StartsWith("```", StringComparison.Ordinal))
				{
					inCodeBlock = !inCodeBlock;
					continue;
				}

				if (inCodeBlock && !string.IsNullOrWhiteSpace(line))
				{
					return NormalizeDescription(line);
				}

				if (!inCodeBlock && line.StartsWith("## ", StringComparison.Ordinal))
				{
					break;
				}
			}

			return null;
		}

		private static string CleanCell(string value)
		{
			value = value.Trim();
			if (value == "-")
			{
				return string.Empty;
			}

			if (value.StartsWith('`') && value.EndsWith('`') && value.Length >= 2)
			{
				value = value[1..^1];
			}

			return value.Trim();
		}

		private static string NormalizeSlug(string text)
		{
			return Regex.Replace(text.ToLowerInvariant(), @"[^a-z0-9]+", "-").Trim('-');
		}

		private static string NormalizeRelativePath(string path)
		{
			return path.Replace('\\', '/').TrimStart('/');
		}

		private sealed record OptionRow(string Long, string Short, string Type, string Required, string Description);
	}
}
