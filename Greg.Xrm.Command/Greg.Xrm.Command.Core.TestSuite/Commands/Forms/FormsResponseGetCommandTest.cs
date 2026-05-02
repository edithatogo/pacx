namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsResponseGetCommandTest
	{
		[TestMethod]
		public void ParseWithRequiredOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<FormsResponseGetCommand>(
				"forms", "response", "get",
				"--form-id", "f1",
				"--tenant", "t",
				"--response-id", "42");

			Assert.AreEqual("f1", command.FormId);
			Assert.AreEqual("t", command.TenantId);
			Assert.AreEqual(42, command.ResponseId);
		}
	}
}
