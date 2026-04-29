using Greg.Xrm.Command.Commands.AiBuilder;
using Greg.Xrm.Command.Commands.Auth;
using Greg.Xrm.Command.Commands.Completions;
using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services.Output;
using Microsoft.Extensions.Logging.Abstractions;

namespace Greg.Xrm.Command.Commands.CliUx
{
	[TestClass]
	public class CliUxCommandsTest
	{
		[TestMethod]
		public void Parser_ShouldResolveShellCompletionCommand()
		{
			var parser = CreateParser(out _);

			var (command, _) = parser.Parse("completions", "pwsh");

			Assert.IsInstanceOfType<CompletionsPwshCommand>(command);
		}

		[TestMethod]
		public void CompletionExecutor_ShouldPrintGlobalOptionsAndCommandVerbs()
		{
			var output = new OutputToMemory();
			var registry = CreateRegistry(output);
			var executor = new CompletionsBashCommandExecutor(registry, output);

			var result = executor.ExecuteAsync(new CompletionsBashCommand(), CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsTrue(result.IsSuccess);
			var text = output.ToString();
			StringAssert.Contains(text, "--format");
			StringAssert.Contains(text, "solution");
		}

		[TestMethod]
		public void Parser_ShouldAcceptGlobalFormatBeforeCommand()
		{
			var parser = CreateParser(out _);

			var (command, _) = parser.Parse("--format", "json", "ai", "model", "list");

			var aiCommand = (AiModelListCommand)command;
			Assert.AreEqual("json", aiCommand.Format);
		}

		[TestMethod]
		public void Parser_ShouldIgnoreGlobalNoColorForCommandsWithoutMatchingProperty()
		{
			var parser = CreateParser(out _);

			var (command, _) = parser.Parse("auth", "list", "--no-color");

			Assert.IsInstanceOfType<ListCommand>(command);
		}

		[TestMethod]
		public void Parser_ShouldResolveAuthWhoAmI()
		{
			var parser = CreateParser(out _);

			var (command, _) = parser.Parse("auth", "whoami");

			Assert.IsInstanceOfType<WhoAmICommand>(command);
		}

		[TestMethod]
		public void ExitCodes_ShouldMapCommonExceptionCategories()
		{
			Assert.AreEqual(ExitCodes.UsageError, ExitCodes.FromException(new ArgumentException("bad option")));
			Assert.AreEqual(ExitCodes.NetworkError, ExitCodes.FromException(new HttpRequestException("network")));
			Assert.AreEqual(ExitCodes.InternalError, ExitCodes.FromException(new InvalidOperationException("unexpected")));
		}

		private static CommandParser CreateParser(out CommandRegistry registry)
		{
			var output = new OutputToMemory();
			registry = CreateRegistry(output);
			return new CommandParser(output, registry);
		}

		private static CommandRegistry CreateRegistry(IOutput output)
		{
			var registry = new CommandRegistry(NullLogger<CommandRegistry>.Instance, output, new Greg.Xrm.Command.Services.Storage());
			registry.InitializeFromAssembly(typeof(ListCommand).Assembly);
			return registry;
		}
	}
}
