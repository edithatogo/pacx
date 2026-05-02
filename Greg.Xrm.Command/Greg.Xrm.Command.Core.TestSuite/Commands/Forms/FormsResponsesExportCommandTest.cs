namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsResponsesExportCommandTest
	{
		[TestMethod]
		public void ParseWithRequiredOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<FormsResponsesExportCommand>(
				"forms", "responses", "export",
				"--tenant", "contoso.onmicrosoft.com",
				"--form-id", "form123",
				"--output", "responses.csv");

			Assert.AreEqual("contoso.onmicrosoft.com", command.TenantId);
			Assert.AreEqual("form123", command.FormId);
			Assert.AreEqual("responses.csv", command.OutputPath);
			Assert.AreEqual("csv", command.Format);
			Assert.IsFalse(command.Incremental);
		}

		[TestMethod]
		public void ParseWithAllOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<FormsResponsesExportCommand>(
				"forms", "responses", "export",
				"--tenant", "contoso.onmicrosoft.com",
				"--form-id", "form123",
				"--output", "responses.json",
				"--owner", "user123",
				"--owner-type", "Group",
				"--format", "json",
				"--incremental");

			Assert.AreEqual("json", command.Format);
			Assert.IsTrue(command.Incremental);
			Assert.AreEqual("user123", command.OwnerId);
			Assert.AreEqual("Group", command.OwnerType);
		}
	}
}
