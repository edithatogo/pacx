namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsAdminReportCommandTest
	{
		[TestMethod]
		public void ParseWithRequiredOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<FormsAdminReportCommand>("forms", "admin", "report", "--tenant", "contoso.onmicrosoft.com");
			Assert.AreEqual("contoso.onmicrosoft.com", command.TenantId);
			Assert.AreEqual("forms-report.xlsx", command.OutputPath);
			Assert.IsFalse(command.IncludeGroups);
		}

		[TestMethod]
		public void ParseWithAllOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<FormsAdminReportCommand>(
				"forms", "admin", "report",
				"--tenant", "contoso.onmicrosoft.com",
				"--output", "report.xlsx",
				"--include-groups");

			Assert.AreEqual("report.xlsx", command.OutputPath);
			Assert.IsTrue(command.IncludeGroups);
		}
	}
}
