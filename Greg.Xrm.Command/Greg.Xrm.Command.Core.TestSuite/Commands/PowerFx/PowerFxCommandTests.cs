using Greg.Xrm.Command.Services.PowerFx;

namespace Greg.Xrm.Command.Commands.PowerFx
{
	[TestClass]
	public class PowerFxCommandTests
	{
		[TestMethod]
		public async Task Validate_ShouldReportInvalidExpression()
		{
			var output = new OutputToMemory();
			var executor = new PowerFxValidateCommandExecutor(new PowerFxValidationService(), output);

			var result = await executor.ExecuteAsync(new PowerFxValidateCommand { Expression = "If(true, 1" }, CancellationToken.None);

			Assert.IsFalse(result.IsSuccess);
			StringAssert.Contains(output.ToString(), "invalid");
		}

		[TestMethod]
		public async Task Format_ShouldNormalizeWhitespace()
		{
			var output = new OutputToMemory();
			var executor = new PowerFxFormatCommandExecutor(new PowerFxValidationService(), output);

			var result = await executor.ExecuteAsync(new PowerFxFormatCommand { Expression = "If(\n true,   1 )" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			StringAssert.Contains(output.ToString(), "If( true, 1 )");
		}
	}
}
