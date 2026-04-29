using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services.Output;
using Microsoft.Extensions.Logging.Abstractions;

namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsAdvancedCommandsTest
	{
		[TestMethod]
		public void Parser_ShouldResolveBranchingExport()
		{
			var parser = CreateParser();

			var (command, _) = parser.Parse("forms", "branching", "export", "--form-id", "form-1");

			Assert.IsInstanceOfType<FormsBranchingExportCommand>(command);
		}

		[TestMethod]
		public async Task AnalyticsSummary_ShouldPrintApiPlan()
		{
			var output = new OutputToMemory();
			var executor = new FormsAnalyticsSummaryCommandExecutor(output);

			var result = await executor.ExecuteAsync(new FormsAnalyticsSummaryCommand { FormId = "form-1" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			StringAssert.Contains(output.ToString(), "Forms Analytics Summary");
			StringAssert.Contains(output.ToString(), "completion rate");
		}

		private static CommandParser CreateParser()
		{
			var output = new OutputToMemory();
			var registry = new CommandRegistry(NullLogger<CommandRegistry>.Instance, output, new Greg.Xrm.Command.Services.Storage());
			registry.InitializeFromAssembly(typeof(FormsListCommand).Assembly);
			return new CommandParser(output, registry);
		}
	}
}
