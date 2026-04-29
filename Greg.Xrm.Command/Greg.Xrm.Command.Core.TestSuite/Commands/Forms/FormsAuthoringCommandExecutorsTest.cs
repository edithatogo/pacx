using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.TestSuite;

namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsAuthoringCommandExecutorsTest
	{
		[TestMethod]
		public void FormsCreate_ShouldWriteManifest()
		{
			var output = new OutputToMemory();
			var tempDir = TestTempPath.CreateDirectory("forms_authoring");
			var manifestPath = Path.Combine(tempDir, "forms.json");

			try
			{
				var executor = new FormsCreateCommandExecutor(output);
				var result = executor.ExecuteAsync(new FormsCreateCommand
				{
					OutputPath = manifestPath,
					Title = "Quarterly survey",
					Description = "Customer feedback",
					Published = true
				}, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				Assert.IsTrue(File.Exists(manifestPath));
				var json = File.ReadAllText(manifestPath);
				StringAssert.Contains(json, "\"title\": \"Quarterly survey\"");
				StringAssert.Contains(json, "\"published\": true");
				StringAssert.Contains(output.ToString(), "Created Microsoft Forms authoring manifest.");
			}
			finally
			{
				Directory.Delete(tempDir, recursive: true);
			}
		}

		[TestMethod]
		public void FormsUpdate_ShouldModifyManifest()
		{
			var output = new OutputToMemory();
			var tempDir = TestTempPath.CreateDirectory("forms_authoring");
			var manifestPath = Path.Combine(tempDir, "forms.json");
			File.WriteAllText(manifestPath, """
{
  "id": "form_123",
  "title": "Quarterly survey",
  "description": "Customer feedback",
  "published": false,
  "createdAt": "2026-04-28T00:00:00+00:00",
  "updatedAt": "2026-04-28T00:00:00+00:00",
  "questions": [],
  "sections": []
}
""");

			try
			{
				var executor = new FormsUpdateCommandExecutor(output);
				var result = executor.ExecuteAsync(new FormsUpdateCommand
				{
					FilePath = manifestPath,
					Title = "Updated title",
					Published = true
				}, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				var json = File.ReadAllText(manifestPath);
				StringAssert.Contains(json, "\"title\": \"Updated title\"");
				StringAssert.Contains(json, "\"published\": true");
			}
			finally
			{
				Directory.Delete(tempDir, recursive: true);
			}
		}

		[TestMethod]
		public void FormsDelete_ShouldRequireForce()
		{
			var output = new OutputToMemory();
			var tempDir = TestTempPath.CreateDirectory("forms_authoring");
			var manifestPath = Path.Combine(tempDir, "forms.json");
			File.WriteAllText(manifestPath, "{}");

			try
			{
				var executor = new FormsDeleteCommandExecutor(output);
				var result = executor.ExecuteAsync(new FormsDeleteCommand
				{
					FilePath = manifestPath,
					Force = false
				}, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsFalse(result.IsSuccess);
				Assert.IsTrue(File.Exists(manifestPath));
				StringAssert.Contains(result.ErrorMessage, "--force");
			}
			finally
			{
				Directory.Delete(tempDir, recursive: true);
			}
		}

		[TestMethod]
		public void FormsPublish_ShouldTogglePublishedState()
		{
			var output = new OutputToMemory();
			var tempDir = TestTempPath.CreateDirectory("forms_authoring");
			var manifestPath = Path.Combine(tempDir, "forms.json");
			File.WriteAllText(manifestPath, """
{
  "id": "form_123",
  "title": "Quarterly survey",
  "published": false,
  "createdAt": "2026-04-28T00:00:00+00:00",
  "updatedAt": "2026-04-28T00:00:00+00:00",
  "questions": [],
  "sections": []
}
""");

			try
			{
				var executor = new FormsPublishCommandExecutor(output);
				var result = executor.ExecuteAsync(new FormsPublishCommand
				{
					FilePath = manifestPath,
					Published = true
				}, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				var json = File.ReadAllText(manifestPath);
				StringAssert.Contains(json, "\"published\": true");
				StringAssert.Contains(output.ToString(), "Published Microsoft Forms authoring manifest.");
			}
			finally
			{
				Directory.Delete(tempDir, recursive: true);
			}
		}

		[TestMethod]
		public void FormsQuestionAdd_ShouldAppendQuestion()
		{
			var output = new OutputToMemory();
			var tempDir = TestTempPath.CreateDirectory("forms_authoring");
			var manifestPath = Path.Combine(tempDir, "forms.json");
			WriteManifest(manifestPath);

			try
			{
				var executor = new FormsQuestionAddCommandExecutor(output);
				var result = executor.ExecuteAsync(new FormsQuestionAddCommand
				{
					FilePath = manifestPath,
					QuestionText = "What is your name?",
					Type = "text",
					Required = true
				}, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				var json = File.ReadAllText(manifestPath);
				StringAssert.Contains(json, "\"text\": \"What is your name?\"");
				StringAssert.Contains(json, "\"required\": true");
				StringAssert.Contains(output.ToString(), "Added Microsoft Forms question.");
			}
			finally
			{
				Directory.Delete(tempDir, recursive: true);
			}
		}

		[TestMethod]
		public void FormsQuestionUpdate_ShouldModifyQuestion()
		{
			var output = new OutputToMemory();
			var tempDir = TestTempPath.CreateDirectory("forms_authoring");
			var manifestPath = Path.Combine(tempDir, "forms.json");
			WriteManifest(manifestPath, """
{
  "id": "form_123",
  "title": "Quarterly survey",
  "published": false,
  "createdAt": "2026-04-28T00:00:00+00:00",
  "updatedAt": "2026-04-28T00:00:00+00:00",
  "questions": [
    {
      "id": "question_1",
      "type": "text",
      "text": "Original question",
      "required": false,
      "options": []
    }
  ],
  "sections": []
}
""");

			try
			{
				var executor = new FormsQuestionUpdateCommandExecutor(output);
				var result = executor.ExecuteAsync(new FormsQuestionUpdateCommand
				{
					FilePath = manifestPath,
					QuestionId = "question_1",
					QuestionText = "Updated question",
					Type = "choice",
					Required = true,
					OptionsCsv = "One,Two"
				}, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				var json = File.ReadAllText(manifestPath);
				StringAssert.Contains(json, "\"text\": \"Updated question\"");
				StringAssert.Contains(json, "\"type\": \"choice\"");
				StringAssert.Contains(json, "\"required\": true");
				StringAssert.Contains(json, "\"options\": [");
			}
			finally
			{
				Directory.Delete(tempDir, recursive: true);
			}
		}

		[TestMethod]
		public void FormsQuestionDelete_ShouldRequireForce()
		{
			var output = new OutputToMemory();
			var tempDir = TestTempPath.CreateDirectory("forms_authoring");
			var manifestPath = Path.Combine(tempDir, "forms.json");
			WriteManifest(manifestPath);

			try
			{
				var executor = new FormsQuestionDeleteCommandExecutor(output);
				var result = executor.ExecuteAsync(new FormsQuestionDeleteCommand
				{
					FilePath = manifestPath,
					QuestionId = "question_1",
					Force = false
				}, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsFalse(result.IsSuccess);
				Assert.IsTrue(File.Exists(manifestPath));
				StringAssert.Contains(result.ErrorMessage, "--force");
			}
			finally
			{
				Directory.Delete(tempDir, recursive: true);
			}
		}

		[TestMethod]
		public void FormsSectionAdd_ShouldAssignOrder()
		{
			var output = new OutputToMemory();
			var tempDir = TestTempPath.CreateDirectory("forms_authoring");
			var manifestPath = Path.Combine(tempDir, "forms.json");
			WriteManifest(manifestPath);

			try
			{
				var executor = new FormsSectionAddCommandExecutor(output);
				var result = executor.ExecuteAsync(new FormsSectionAddCommand
				{
					FilePath = manifestPath,
					Title = "Section 1"
				}, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				var json = File.ReadAllText(manifestPath);
				StringAssert.Contains(json, "\"title\": \"Section 1\"");
				StringAssert.Contains(json, "\"order\": 1");
			}
			finally
			{
				Directory.Delete(tempDir, recursive: true);
			}
		}

		[TestMethod]
		public void FormsSectionUpdate_ShouldModifySection()
		{
			var output = new OutputToMemory();
			var tempDir = TestTempPath.CreateDirectory("forms_authoring");
			var manifestPath = Path.Combine(tempDir, "forms.json");
			WriteManifest(manifestPath, """
{
  "id": "form_123",
  "title": "Quarterly survey",
  "published": false,
  "createdAt": "2026-04-28T00:00:00+00:00",
  "updatedAt": "2026-04-28T00:00:00+00:00",
  "questions": [],
  "sections": [
    {
      "id": "section_1",
      "title": "Original section",
      "description": "Original description",
      "order": 2
    }
  ]
}
""");

			try
			{
				var executor = new FormsSectionUpdateCommandExecutor(output);
				var result = executor.ExecuteAsync(new FormsSectionUpdateCommand
				{
					FilePath = manifestPath,
					SectionId = "section_1",
					Title = "Updated section",
					Order = 1
				}, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				var json = File.ReadAllText(manifestPath);
				StringAssert.Contains(json, "\"title\": \"Updated section\"");
				StringAssert.Contains(json, "\"order\": 1");
			}
			finally
			{
				Directory.Delete(tempDir, recursive: true);
			}
		}

		[TestMethod]
		public void FormsSectionDelete_ShouldRequireForce()
		{
			var output = new OutputToMemory();
			var tempDir = TestTempPath.CreateDirectory("forms_authoring");
			var manifestPath = Path.Combine(tempDir, "forms.json");
			WriteManifest(manifestPath, """
{
  "id": "form_123",
  "title": "Quarterly survey",
  "published": false,
  "createdAt": "2026-04-28T00:00:00+00:00",
  "updatedAt": "2026-04-28T00:00:00+00:00",
  "questions": [],
  "sections": [
    {
      "id": "section_1",
      "title": "Section 1",
      "order": 1
    }
  ]
}
""");

			try
			{
				var executor = new FormsSectionDeleteCommandExecutor(output);
				var result = executor.ExecuteAsync(new FormsSectionDeleteCommand
				{
					FilePath = manifestPath,
					SectionId = "section_1",
					Force = false
				}, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsFalse(result.IsSuccess);
				Assert.IsTrue(File.Exists(manifestPath));
				StringAssert.Contains(result.ErrorMessage, "--force");
			}
			finally
			{
				Directory.Delete(tempDir, recursive: true);
			}
		}

		[TestMethod]
		public void FormsSectionDelete_ShouldRemoveSection()
		{
			var output = new OutputToMemory();
			var tempDir = TestTempPath.CreateDirectory("forms_authoring");
			var manifestPath = Path.Combine(tempDir, "forms.json");
			WriteManifest(manifestPath, """
{
  "id": "form_123",
  "title": "Quarterly survey",
  "published": false,
  "createdAt": "2026-04-28T00:00:00+00:00",
  "updatedAt": "2026-04-28T00:00:00+00:00",
  "questions": [],
  "sections": [
    {
      "id": "section_1",
      "title": "Section 1",
      "order": 1
    }
  ]
}
""");

			try
			{
				var executor = new FormsSectionDeleteCommandExecutor(output);
				var result = executor.ExecuteAsync(new FormsSectionDeleteCommand
				{
					FilePath = manifestPath,
					SectionId = "section_1",
					Force = true
				}, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				var json = File.ReadAllText(manifestPath);
				StringAssert.Contains(json, "\"sections\": []");
				StringAssert.Contains(output.ToString(), "Deleted Microsoft Forms section.");
			}
			finally
			{
				Directory.Delete(tempDir, recursive: true);
			}
		}

		[TestMethod]
		public void FormsInspect_ShouldPrintSummary()
		{
			var output = new OutputToMemory();
			var tempDir = TestTempPath.CreateDirectory("forms_authoring");
			var manifestPath = Path.Combine(tempDir, "forms.json");
			WriteManifest(manifestPath, """
{
  "id": "form_123",
  "title": "Quarterly survey",
  "published": true,
  "createdAt": "2026-04-28T00:00:00+00:00",
  "updatedAt": "2026-04-28T00:00:00+00:00",
  "questions": [
    {
      "id": "question_1",
      "type": "text",
      "text": "Question 1",
      "required": false,
      "options": []
    }
  ],
  "sections": [
    {
      "id": "section_1",
      "title": "Section 1",
      "description": "Intro",
      "order": 1
    }
  ]
}
""");

			try
			{
				var executor = new FormsInspectCommandExecutor(output);
				var result = executor.ExecuteAsync(new FormsInspectCommand
				{
					FilePath = manifestPath
				}, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "Microsoft Forms authoring manifest.");
				StringAssert.Contains(output.ToString(), "Questions: 1");
				StringAssert.Contains(output.ToString(), "Sections: 1");
			}
			finally
			{
				Directory.Delete(tempDir, recursive: true);
			}
		}

		[TestMethod]
		public void FormsQuestionList_ShouldPrintQuestions()
		{
			var output = new OutputToMemory();
			var tempDir = TestTempPath.CreateDirectory("forms_authoring");
			var manifestPath = Path.Combine(tempDir, "forms.json");
			WriteManifest(manifestPath, """
{
  "id": "form_123",
  "title": "Quarterly survey",
  "published": false,
  "createdAt": "2026-04-28T00:00:00+00:00",
  "updatedAt": "2026-04-28T00:00:00+00:00",
  "questions": [
    {
      "id": "question_1",
      "type": "choice",
      "text": "Pick one",
      "required": true,
      "options": ["A", "B"]
    }
  ],
  "sections": []
}
""");

			try
			{
				var executor = new FormsQuestionListCommandExecutor(output);
				var result = executor.ExecuteAsync(new FormsQuestionListCommand
				{
					FilePath = manifestPath
				}, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "question_1");
				StringAssert.Contains(output.ToString(), "required=True");
				StringAssert.Contains(output.ToString(), "options=A, B");
			}
			finally
			{
				Directory.Delete(tempDir, recursive: true);
			}
		}

		[TestMethod]
		public void FormsSectionList_ShouldPrintSections()
		{
			var output = new OutputToMemory();
			var tempDir = TestTempPath.CreateDirectory("forms_authoring");
			var manifestPath = Path.Combine(tempDir, "forms.json");
			WriteManifest(manifestPath, """
{
  "id": "form_123",
  "title": "Quarterly survey",
  "published": false,
  "createdAt": "2026-04-28T00:00:00+00:00",
  "updatedAt": "2026-04-28T00:00:00+00:00",
  "questions": [],
  "sections": [
    {
      "id": "section_1",
      "title": "Section 1",
      "description": "Intro",
      "order": 1
    }
  ]
}
""");

			try
			{
				var executor = new FormsSectionListCommandExecutor(output);
				var result = executor.ExecuteAsync(new FormsSectionListCommand
				{
					FilePath = manifestPath
				}, CancellationToken.None).GetAwaiter().GetResult();

				Assert.IsTrue(result.IsSuccess);
				StringAssert.Contains(output.ToString(), "section_1");
				StringAssert.Contains(output.ToString(), "order=1");
				StringAssert.Contains(output.ToString(), "Section 1");
			}
			finally
			{
				Directory.Delete(tempDir, recursive: true);
			}
		}

		private static void WriteManifest(string path, string? content = null)
		{
			File.WriteAllText(path, content ?? """
{
  "id": "form_123",
  "title": "Quarterly survey",
  "published": false,
  "createdAt": "2026-04-28T00:00:00+00:00",
  "updatedAt": "2026-04-28T00:00:00+00:00",
  "questions": [],
  "sections": []
}
""");
		}
	}
}
