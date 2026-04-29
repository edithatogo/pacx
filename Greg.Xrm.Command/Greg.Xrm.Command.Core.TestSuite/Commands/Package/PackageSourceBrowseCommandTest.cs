namespace Greg.Xrm.Command.Commands.Package
{
	[TestClass]
	public class PackageSourceBrowseCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Utility.TestParseCommand<PackageSourceBrowseCommand>("package", "source", "browse");

			Assert.AreEqual("conductor/source-catalog/sources.json", command.CatalogPath);
		}
	}
}
