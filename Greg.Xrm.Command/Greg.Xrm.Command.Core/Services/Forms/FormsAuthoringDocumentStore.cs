using System.Text.Json;
using System.Text.Json.Serialization;

namespace Greg.Xrm.Command.Services.Forms
{
	public sealed class FormsAuthoringDocumentStore
	{
		private static readonly JsonSerializerOptions SerializerOptions = new()
		{
			WriteIndented = true,
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

		public FormsAuthoringDocument Create(string title, string? description, bool published)
		{
			var now = DateTimeOffset.UtcNow;
			return new FormsAuthoringDocument
			{
				Id = $"form_{Guid.NewGuid():N}",
				Title = title,
				Description = description,
				Published = published,
				CreatedAt = now,
				UpdatedAt = now
			};
		}

		public FormsAuthoringDocument Load(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentException("A forms document path is required.", nameof(path));
			}

			var fullPath = Path.GetFullPath(path);
			if (!File.Exists(fullPath))
			{
				throw new FileNotFoundException($"Forms authoring document not found: {fullPath}", fullPath);
			}

			using var stream = File.OpenRead(fullPath);
			var document = JsonSerializer.Deserialize<FormsAuthoringDocument>(stream, SerializerOptions)
				?? throw new InvalidDataException($"Forms authoring document could not be parsed: {fullPath}");

			if (string.IsNullOrWhiteSpace(document.Id))
			{
				throw new InvalidDataException($"Forms authoring document is missing required 'id' field: {fullPath}");
			}

			if (string.IsNullOrWhiteSpace(document.Title))
			{
				throw new InvalidDataException($"Forms authoring document is missing required 'title' field: {fullPath}");
			}

			return document;
		}

		public void Save(string path, FormsAuthoringDocument document)
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				throw new ArgumentException("A forms document path is required.", nameof(path));
			}

			var fullPath = Path.GetFullPath(path);
			var directory = Path.GetDirectoryName(fullPath);
			if (!string.IsNullOrWhiteSpace(directory))
			{
				Directory.CreateDirectory(directory);
			}

			if (document.CreatedAt == default)
			{
				document.CreatedAt = DateTimeOffset.UtcNow;
			}

			document.UpdatedAt = DateTimeOffset.UtcNow;

			File.WriteAllText(fullPath, JsonSerializer.Serialize(document, SerializerOptions));
		}

		public FormsAuthoringQuestion AddQuestion(
			FormsAuthoringDocument document,
			string text,
			string type,
			bool required,
			IEnumerable<string>? options = null)
		{
			var question = new FormsAuthoringQuestion
			{
				Id = $"question_{Guid.NewGuid():N}",
				Text = text,
				Type = type,
				Required = required,
				Options = NormalizeValues(options)
			};

			document.Questions.Add(question);
			return question;
		}

		public FormsAuthoringQuestion UpdateQuestion(
			FormsAuthoringDocument document,
			string questionId,
			string? text = null,
			string? type = null,
			bool? required = null,
			IEnumerable<string>? options = null)
		{
			var question = FindQuestion(document, questionId);

			if (text is not null)
			{
				question.Text = text;
			}

			if (type is not null)
			{
				question.Type = type;
			}

			if (required.HasValue)
			{
				question.Required = required.Value;
			}

			if (options is not null)
			{
				question.Options = NormalizeValues(options);
			}

			return question;
		}

		public bool DeleteQuestion(FormsAuthoringDocument document, string questionId)
		{
			var question = FindQuestion(document, questionId);
			return document.Questions.Remove(question);
		}

		public FormsAuthoringSection AddSection(
			FormsAuthoringDocument document,
			string title,
			string? description = null,
			int? order = null)
		{
			var section = new FormsAuthoringSection
			{
				Id = $"section_{Guid.NewGuid():N}",
				Title = title,
				Description = description,
				Order = order ?? GetNextSectionOrder(document)
			};

			document.Sections.Add(section);
			SortSections(document);
			return section;
		}

		public FormsAuthoringSection UpdateSection(
			FormsAuthoringDocument document,
			string sectionId,
			string? title = null,
			string? description = null,
			int? order = null)
		{
			var section = FindSection(document, sectionId);

			if (title is not null)
			{
				section.Title = title;
			}

			if (description is not null)
			{
				section.Description = description;
			}

			if (order.HasValue)
			{
				section.Order = order.Value;
			}

			SortSections(document);
			return section;
		}

		public bool DeleteSection(FormsAuthoringDocument document, string sectionId)
		{
			var section = FindSection(document, sectionId);
			var removed = document.Sections.Remove(section);
			if (removed)
			{
				SortSections(document);
			}

			return removed;
		}

		private static FormsAuthoringQuestion FindQuestion(FormsAuthoringDocument document, string questionId)
		{
			var question = document.Questions.FirstOrDefault(x => string.Equals(x.Id, questionId, StringComparison.OrdinalIgnoreCase));
			return question ?? throw new KeyNotFoundException($"Question not found: {questionId}");
		}

		private static FormsAuthoringSection FindSection(FormsAuthoringDocument document, string sectionId)
		{
			var section = document.Sections.FirstOrDefault(x => string.Equals(x.Id, sectionId, StringComparison.OrdinalIgnoreCase));
			return section ?? throw new KeyNotFoundException($"Section not found: {sectionId}");
		}

		private static int GetNextSectionOrder(FormsAuthoringDocument document)
		{
			return document.Sections.Count == 0
				? 1
				: document.Sections.Max(section => section.Order) + 1;
		}

		private static void SortSections(FormsAuthoringDocument document)
		{
			document.Sections = document.Sections
				.OrderBy(section => section.Order)
				.ThenBy(section => section.Title, StringComparer.OrdinalIgnoreCase)
				.ToList();
		}

		private static List<string> NormalizeValues(IEnumerable<string>? values)
		{
			var normalized = new List<string>();
			if (values is null)
			{
				return normalized;
			}

			foreach (var value in values)
			{
				var trimmed = value?.Trim();
				if (string.IsNullOrWhiteSpace(trimmed))
				{
					continue;
				}

				if (normalized.Any(existing => string.Equals(existing, trimmed, StringComparison.OrdinalIgnoreCase)))
				{
					continue;
				}

				normalized.Add(trimmed);
			}

			return normalized;
		}
	}

	public sealed class FormsAuthoringDocument
	{
		public string Id { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public bool Published { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }
		public List<FormsAuthoringQuestion> Questions { get; set; } = [];
		public List<FormsAuthoringSection> Sections { get; set; } = [];
	}

	public sealed class FormsAuthoringQuestion
	{
		public string Id { get; set; } = string.Empty;
		public string Type { get; set; } = "text";
		public string Text { get; set; } = string.Empty;
		public bool Required { get; set; }
		public List<string> Options { get; set; } = [];
	}

	public sealed class FormsAuthoringSection
	{
		public string Id { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public int Order { get; set; }
	}
}
