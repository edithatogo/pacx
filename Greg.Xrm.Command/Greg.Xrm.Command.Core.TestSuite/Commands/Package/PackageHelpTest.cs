namespace Greg.Xrm.Command.Commands.Package
{
	[TestClass]
	public class PackageHelpTest
	{
		[TestMethod]
		public void HelpShouldDescribePackageFlow()
		{
			var help = new Help();

			Assert.AreEqual(1, help.Verbs.Length);
			Assert.AreEqual("package", help.Verbs[0]);
			StringAssert.Contains(help.GetHelp(), "authoring, validation, build, deploy, and release flows");
			StringAssert.Contains(help.GetHelp(), "Kinds are bundle, solution, and data");
		}
	}
}
