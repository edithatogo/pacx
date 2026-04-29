namespace Greg.Xrm.Command.Commands.Package
{
	[TestClass]
	public class PackageValidateCommandTest
	{
		[TestMethod]
		public void PackageValidateParseShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<PackageValidateCommand>(
				"package", "validate",
				"-p", "./package");

			Assert.AreEqual("./package", command.Path);
		}
	}
}
