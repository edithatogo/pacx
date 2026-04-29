using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsAuthoringCommandsTest
	{
		[TestMethod]
		public void FormsCreate_ParseWithDefaultsShouldWork()
		{
			var command = Utility.TestParseCommand<FormsCreateCommand>(
				"forms", "create",
				"-o", "forms.json",
				"-t", "Customer feedback");

			Assert.AreEqual("forms.json", command.OutputPath);
			Assert.AreEqual("Customer feedback", command.Title);
			Assert.IsNull(command.Description);
			Assert.IsFalse(command.Published);
		}

		[TestMethod]
		public void FormsCreate_ParseWithAllOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<FormsCreateCommand>(
				"forms", "create",
				"-o", "forms.json",
				"-t", "Customer feedback",
				"-d", "Quarterly survey",
				"-p");

			Assert.AreEqual("forms.json", command.OutputPath);
			Assert.AreEqual("Customer feedback", command.Title);
			Assert.AreEqual("Quarterly survey", command.Description);
			Assert.IsTrue(command.Published);
		}

		[TestMethod]
		public void FormsUpdate_ParseWithAllOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<FormsUpdateCommand>(
				"forms", "update",
				"-f", "forms.json",
				"-t", "Updated title",
				"-d", "Updated description",
				"--published", "true");

			Assert.AreEqual("forms.json", command.FilePath);
			Assert.AreEqual("Updated title", command.Title);
			Assert.AreEqual("Updated description", command.Description);
			Assert.AreEqual(true, command.Published);
		}

		[TestMethod]
		public void FormsDelete_ParseWithRequiredShouldWork()
		{
			var command = Utility.TestParseCommand<FormsDeleteCommand>(
				"forms", "delete",
				"-f", "forms.json");

			Assert.AreEqual("forms.json", command.FilePath);
			Assert.IsFalse(command.Force);
		}

		[TestMethod]
		public void FormsPublish_ParseWithAllOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<FormsPublishCommand>(
				"forms", "publish",
				"-f", "forms.json",
				"--published", "false");

			Assert.AreEqual("forms.json", command.FilePath);
			Assert.IsFalse(command.Published);
		}

		[TestMethod]
		public void FormsInspect_ParseWithRequiredShouldWork()
		{
			var command = Utility.TestParseCommand<FormsInspectCommand>(
				"forms", "inspect",
				"-f", "forms.json");

			Assert.AreEqual("forms.json", command.FilePath);
		}

		[TestMethod]
		public void FormsQuestionAdd_ParseWithRequiredShouldWork()
		{
			var command = Utility.TestParseCommand<FormsQuestionAddCommand>(
				"forms", "question", "add",
				"-f", "forms.json",
				"-t", "What is your name?");

			Assert.AreEqual("forms.json", command.FilePath);
			Assert.AreEqual("What is your name?", command.QuestionText);
			Assert.AreEqual("text", command.Type);
			Assert.IsFalse(command.Required);
			Assert.IsNull(command.OptionsCsv);
		}

		[TestMethod]
		public void FormsQuestionUpdate_ParseWithRequiredShouldWork()
		{
			var command = Utility.TestParseCommand<FormsQuestionUpdateCommand>(
				"forms", "question", "update",
				"-f", "forms.json",
				"--id", "question_1");

			Assert.AreEqual("forms.json", command.FilePath);
			Assert.AreEqual("question_1", command.QuestionId);
			Assert.IsNull(command.QuestionText);
			Assert.IsNull(command.Type);
			Assert.IsNull(command.Required);
		}

		[TestMethod]
		public void FormsQuestionDelete_ParseWithRequiredShouldWork()
		{
			var command = Utility.TestParseCommand<FormsQuestionDeleteCommand>(
				"forms", "question", "delete",
				"-f", "forms.json",
				"--id", "question_1",
				"--force");

			Assert.AreEqual("forms.json", command.FilePath);
			Assert.AreEqual("question_1", command.QuestionId);
			Assert.IsTrue(command.Force);
		}

		[TestMethod]
		public void FormsQuestionList_ParseWithRequiredShouldWork()
		{
			var command = Utility.TestParseCommand<FormsQuestionListCommand>(
				"forms", "question", "list",
				"-f", "forms.json");

			Assert.AreEqual("forms.json", command.FilePath);
		}

		[TestMethod]
		public void FormsSectionAdd_ParseWithRequiredShouldWork()
		{
			var command = Utility.TestParseCommand<FormsSectionAddCommand>(
				"forms", "section", "add",
				"-f", "forms.json",
				"-t", "Section 1");

			Assert.AreEqual("forms.json", command.FilePath);
			Assert.AreEqual("Section 1", command.Title);
			Assert.IsNull(command.Description);
			Assert.IsNull(command.Order);
		}

		[TestMethod]
		public void FormsSectionList_ParseWithRequiredShouldWork()
		{
			var command = Utility.TestParseCommand<FormsSectionListCommand>(
				"forms", "section", "list",
				"-f", "forms.json");

			Assert.AreEqual("forms.json", command.FilePath);
		}

		[TestMethod]
		public void FormsSectionDelete_ParseWithRequiredShouldWork()
		{
			var command = Utility.TestParseCommand<FormsSectionDeleteCommand>(
				"forms", "section", "delete",
				"-f", "forms.json",
				"--id", "section_1",
				"--force");

			Assert.AreEqual("forms.json", command.FilePath);
			Assert.AreEqual("section_1", command.SectionId);
			Assert.IsTrue(command.Force);
		}

		[TestMethod]
		public void FormsSectionUpdate_ParseWithRequiredShouldWork()
		{
			var command = Utility.TestParseCommand<FormsSectionUpdateCommand>(
				"forms", "section", "update",
				"-f", "forms.json",
				"--id", "section_1");

			Assert.AreEqual("forms.json", command.FilePath);
			Assert.AreEqual("section_1", command.SectionId);
			Assert.IsNull(command.Title);
			Assert.IsNull(command.Description);
			Assert.IsNull(command.Order);
		}
	}
}
