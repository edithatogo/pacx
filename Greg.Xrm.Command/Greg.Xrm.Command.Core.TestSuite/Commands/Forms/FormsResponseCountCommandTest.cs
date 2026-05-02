namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsResponseCountCommandTest
	{
		[TestMethod]
		public void ParseWithRequiredOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<FormsResponseCountCommand>(
				"forms", "response", "count",
				"--tenant", "contoso.onmicrosoft.com",
				"--form-id", "form123");

			Assert.AreEqual("contoso.onmicrosoft.com", command.TenantId);
			Assert.AreEqual("form123", command.FormId);
			Assert.AreEqual("User", command.OwnerType);
		}
	}
}
