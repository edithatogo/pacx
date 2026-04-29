using System.Text.Encodings.Web;
using System.Text.Json;
using Greg.Xrm.Command.Commands.Completions;
using Greg.Xrm.Command.Parsing;
using Microsoft.Extensions.Logging.Abstractions;

namespace Greg.Xrm.Command.Snapshots
{
	[TestClass]
	public class CliUxSnapshotTests
	{
		private static readonly JsonSerializerOptions SnapshotJsonOptions = new()
		{
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			WriteIndented = true
		};

		[TestMethod]
		public void GlobalOptionsParseSnapshot()
		{
			var output = new OutputToMemory();
			var parsed = CommandRunArgs.TryParse(
				["--format", "json", "auth", "whoami", "--profile", "sandbox", "--verbose", "--no-color"],
				output,
				out var result);

			var snapshot = JsonSerializer.Serialize(
				new
				{
					parsed,
					verbs = result?.Verbs,
					options = result?.Options.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value),
					output = output.ToString()
				},
				SnapshotJsonOptions);

			SnapshotAssert.Matches(snapshot);
		}

		[TestMethod]
		public void CompletionFishHeaderSnapshot()
		{
			var output = new OutputToMemory();
			var registry = CreateRegistry(output);
			var executor = new CompletionsFishCommandExecutor(registry, output);

			var result = executor.ExecuteAsync(new CompletionsFishCommand(), CancellationToken.None).GetAwaiter().GetResult();
			var lines = output.ToString()
				.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
				.Where(line => line.Contains("--format", StringComparison.Ordinal) || line.Contains("'auth'", StringComparison.Ordinal) || line.Contains("'auth whoami'", StringComparison.Ordinal))
				.OrderBy(line => line.Count(ch => ch == ' '))
				.ThenBy(line => line, StringComparer.Ordinal)
				.ToArray();

			SnapshotAssert.Matches(JsonSerializer.Serialize(new { result.IsSuccess, lines }, SnapshotJsonOptions));
		}

		private static CommandRegistry CreateRegistry(IOutput output)
		{
			var registry = new CommandRegistry(NullLogger<CommandRegistry>.Instance, output, new Greg.Xrm.Command.Services.Storage());
			registry.InitializeFromAssembly(typeof(CompletionsCommand).Assembly);
			return registry;
		}
	}
}
