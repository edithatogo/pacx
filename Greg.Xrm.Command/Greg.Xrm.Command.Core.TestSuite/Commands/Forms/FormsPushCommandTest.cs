namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsPushCommandTest
	{
		[TestMethod]
		public void ParseWithRequiredOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<FormsPushCommand>("forms", "push", "--file", "manifest.json");
			Assert.AreEqual("manifest.json", command.FilePath);
			Assert.IsNull(command.TenantId);
			Assert.IsNull(command.OwnerId);
			Assert.AreEqual("User", command.OwnerType);
		}

		[TestMethod]
		public void ParseWithAllOptionsShouldWork()
		{
			var command = Utility.TestParseCommand<FormsPushCommand>(
				"forms", "push",
				"--file", "manifest.json",
				"--tenant", "contoso.onmicrosoft.com",
				"--owner", "user123",
				"--owner-type", "Group");

			Assert.AreEqual("manifest.json", command.FilePath);
			Assert.AreEqual("contoso.onmicrosoft.com", command.TenantId);
			Assert.AreEqual("user123", command.OwnerId);
			Assert.AreEqual("Group", command.OwnerType);
		}
	}
}
