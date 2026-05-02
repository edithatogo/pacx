namespace Greg.Xrm.Command.Commands.Diag
{
	[TestClass]
	public class DiagCommandTest
	{
		[TestMethod]
		public void ParseDiagWithDefaultsShouldWork()
		{
			var command = Utility.TestParseCommand<DiagCommand>("diag");
			Assert.IsFalse(command.Verbose);
		}

		[TestMethod]
		public void ParseDiagWithVerboseShouldWork()
		{
			var command = Utility.TestParseCommand<DiagCommand>("diag", "--verbose");
			Assert.IsTrue(command.Verbose);
		}

		[TestMethod]
		public void ParseDiagWithVerboseShortNameShouldWork()
		{
			var command = Utility.TestParseCommand<DiagCommand>("diag", "-v");
			Assert.IsTrue(command.Verbose);
		}
	}
}
