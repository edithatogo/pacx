using System.Reflection;
using Greg.Xrm.Command.Parsing;
using Moq;

namespace Greg.Xrm.Command.Commands.Validate
{
	[TestClass]
	public class CommandReferenceParityValidatorTests
	{
		private static ICommandRegistry CreateRegistry(params CommandDefinition[] commandDefinitions)
		{
			var mock = new Mock<ICommandRegistry>();
			mock.SetupGet(r => r.Commands).Returns(commandDefinitions);
			return mock.Object;
		}

		private static CommandDefinition CreateDefinition<TCommand, TExecutor>()
		{
			var commandType = typeof(TCommand);
			var commandAttribute = commandType.GetCustomAttributes(typeof(CommandAttribute), inherit: true)
				.OfType<CommandAttribute>()
				.Single();

			return new CommandDefinition(commandAttribute, commandType, typeof(TExecutor), Array.Empty<AliasAttribute>());
		}

		private static CommandDefinition[] CreateValidatedCommandSet()
		{
			return
			[
				CreateDefinition<Greg.Xrm.Command.Commands.Auth.ListCommand, Greg.Xrm.Command.Commands.Auth.ListCommandExecutor>(),
				CreateDefinition<ValidateAllCommand, ValidateAllCommandExecutor>()
			];
		}

		[TestMethod]
		public void Validate_ShouldPassAgainstSyntheticIndex()
		{
			var registry = CreateRegistry(CreateValidatedCommandSet());
			var validator = new CommandReferenceParityValidator();
			var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(tempRoot);
			var tempFile = Path.Combine(tempRoot, "index.md");
			File.WriteAllText(tempFile, """
# Generated Command Reference

- [auth list](auth-list.md)
- [validate all](validate-all.md)
""");
			File.WriteAllText(Path.Combine(tempRoot, "auth-list.md"), """
# auth list

Lists all the authentication profiles stored on this computer

## Usage

```powershell
pacx auth list
```
""");
			File.WriteAllText(Path.Combine(tempRoot, "validate-all.md"), """
# validate all

Validate the generated command reference and catalog contracts against the live command registry.

## Usage

```powershell
pacx validate all
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--docs-index` | d | string | False | Path to the generated command reference index. |
| `--catalog-root` | c | string | False | Repository root used to validate catalog JSON files. |
""");

			try
			{
				var result = validator.Validate(registry, tempFile);

				Assert.IsTrue(result.IsValid, $"Missing={result.MissingPages.Count}; Extra={result.ExtraPages.Count}{Environment.NewLine}{string.Join(Environment.NewLine, result.MissingPages.Concat(result.ExtraPages))}");
			}
			finally
			{
				Directory.Delete(tempRoot, recursive: true);
			}
		}

		[TestMethod]
		public void Validate_ShouldReportMissingAndExtraPages()
		{
			var registry = CreateRegistry(CreateValidatedCommandSet());
			var validator = new CommandReferenceParityValidator();
			var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(tempRoot);
			var tempFile = Path.Combine(tempRoot, "index.md");
			File.WriteAllText(tempFile, """
# Generated Command Reference

- [validate all](validate-all.md)
- [orphaned command](orphaned-command.md)
""");

			try
			{
				var result = validator.Validate(registry, tempFile);

				Assert.IsFalse(result.IsValid);
				Assert.IsTrue(result.MissingPages.Contains("auth-list.md"));
				Assert.IsTrue(result.ExtraPages.Contains("orphaned-command.md"));
			}
			finally
			{
				Directory.Delete(tempRoot, recursive: true);
			}
		}

		[TestMethod]
		public void Validate_ShouldReportContentDifferences()
		{
			var registry = CreateRegistry(CreateValidatedCommandSet());
			var validator = new CommandReferenceParityValidator();
			var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(tempRoot);
			var tempFile = Path.Combine(tempRoot, "index.md");
			File.WriteAllText(tempFile, """
# Generated Command Reference

- [auth list](auth-list.md)
- [validate all](validate-all.md)
""");
			File.WriteAllText(Path.Combine(tempRoot, "auth-list.md"), """
# auth list

Lists all the authentication profiles stored on this computer
""");
			File.WriteAllText(Path.Combine(tempRoot, "validate-all.md"), """
# validate all

Validate the generated command reference and catalog contracts against the live command registry.

## Usage

```powershell
pacx validate all
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--docs-index` | d | string | False | Path to the generated command reference index, with drift. |
| `--catalog-root` | c | string | False | Repository root used to validate catalog JSON files. |
""");

			try
			{
				var result = validator.Validate(registry, tempFile);

				Assert.IsFalse(result.IsValid);
				Assert.IsTrue(result.ContentIssues.Any(issue => issue.Contains("description differs", StringComparison.OrdinalIgnoreCase)));
			}
			finally
			{
				Directory.Delete(tempRoot, recursive: true);
			}
		}

		[TestMethod]
		public void Validate_ShouldReportSummaryAndUsageDifferences()
		{
			var registry = CreateRegistry(CreateValidatedCommandSet());
			var validator = new CommandReferenceParityValidator();
			var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(tempRoot);
			var tempFile = Path.Combine(tempRoot, "index.md");
			File.WriteAllText(tempFile, """
# Generated Command Reference

- [auth list](auth-list.md)
- [validate all](validate-all.md)
""");
			File.WriteAllText(Path.Combine(tempRoot, "auth-list.md"), """
# auth list

List all the authentication profiles stored on this computer

## Usage

```powershell
pacx auth list
```
""");
			File.WriteAllText(Path.Combine(tempRoot, "validate-all.md"), """
# validate all

Validate the generated command reference and catalog contracts against the live command registry, with drift.

## Usage

```powershell
pacx validate something-else
```

## Options

| Option | Short | Type | Required | Description |
| --- | --- | --- | --- | --- |
| `--docs-index` | d | string | False | Path to the generated command reference index. |
| `--catalog-root` | c | string | Repository root used to validate catalog JSON files. |
""");

			try
			{
				var result = validator.Validate(registry, tempFile);

				Assert.IsFalse(result.IsValid);
				Assert.IsTrue(result.ContentIssues.Any(issue => issue.Contains("summary differs", StringComparison.OrdinalIgnoreCase)));
				Assert.IsTrue(result.ContentIssues.Any(issue => issue.Contains("usage differs", StringComparison.OrdinalIgnoreCase)));
			}
			finally
			{
				Directory.Delete(tempRoot, recursive: true);
			}
		}
	}
}
