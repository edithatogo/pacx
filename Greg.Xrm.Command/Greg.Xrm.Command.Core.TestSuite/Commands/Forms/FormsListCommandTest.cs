namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsListCommandTest
	{
		[TestMethod]
		public void ParseWithRequiredOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<FormsListCommand>("forms", "list", "--tenant", "contoso.onmicrosoft.com");
			Assert.AreEqual("contoso.onmicrosoft.com", command.TenantId);
			Assert.IsNull(command.OwnerId);
			Assert.AreEqual("table", command.Format);
			Assert.AreEqual("User", command.OwnerType);
		}

		[TestMethod]
		public void ParseWithAllOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<FormsListCommand>(
				"forms", "list",
				"--tenant", "contoso.onmicrosoft.com",
				"--owner", "user123",
				"--owner-type", "Group",
				"--format", "json");

			Assert.AreEqual("contoso.onmicrosoft.com", command.TenantId);
			Assert.AreEqual("user123", command.OwnerId);
			Assert.AreEqual("Group", command.OwnerType);
			Assert.AreEqual("json", command.Format);
		}
	}
}
