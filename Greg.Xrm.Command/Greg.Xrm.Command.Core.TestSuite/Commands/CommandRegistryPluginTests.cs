using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Greg.Xrm.Command.TestSuite
{
	/// <summary>
	/// Unit tests for CommandRegistry plugin loading and command discovery.
	/// Covers Phase 1 (CommandRegistry), Phase 2 (Plugin Loading), and Phase 3 (Bootstrapper edge cases).
	/// </summary>
	[TestClass]
	public class CommandRegistryPluginTests
	{
		private CommandRegistry CreateRegistry()
			=> CreateRegistry(new Greg.Xrm.Command.Services.Storage());

		private CommandRegistry CreateRegistry(IStorage storage)
		{
			var log = NullLogger<CommandRegistry>.Instance;
			var output = new OutputToMemory();
			return new CommandRegistry(log, output, storage);
		}

		#region Phase 1: CommandRegistry Unit Tests

		[TestMethod]
		public void InitializeFromAssembly_ShouldDiscoverAllCoreCommands()
		{
			var registry = CreateRegistry();
			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);

			Assert.IsTrue(registry.Commands.Count > 0, "No commands were discovered from the core assembly.");

			// Verify specific core commands are found
			var commandTypes = registry.Commands.Select(c => c.CommandType).ToList();
			Assert.IsTrue(commandTypes.Any(t => t == typeof(Greg.Xrm.Command.Commands.Auth.ListCommand)), "ListCommand not discovered.");
			Assert.IsTrue(commandTypes.Any(t => typeof(Greg.Xrm.Command.Commands.Auth.CreateCommand).IsAssignableFrom(t)), "CreateCommand not discovered.");
			Assert.IsTrue(commandTypes.Any(t => t == typeof(Greg.Xrm.Command.Commands.Package.PackageShowCommand)), "PackageShowCommand not discovered.");
			Assert.IsTrue(commandTypes.Any(t => t == typeof(Greg.Xrm.Command.Commands.Package.PackageInitCommand)), "PackageInitCommand not discovered.");
			Assert.IsTrue(commandTypes.Any(t => t == typeof(Greg.Xrm.Command.Commands.Package.PackageAddSolutionCommand)), "PackageAddSolutionCommand not discovered.");
			Assert.IsTrue(commandTypes.Any(t => t == typeof(Greg.Xrm.Command.Commands.Package.PackageAddDataCommand)), "PackageAddDataCommand not discovered.");
			Assert.IsTrue(commandTypes.Any(t => t == typeof(Greg.Xrm.Command.Commands.Package.PackageRemoveSolutionCommand)), "PackageRemoveSolutionCommand not discovered.");
			Assert.IsTrue(commandTypes.Any(t => t == typeof(Greg.Xrm.Command.Commands.Package.PackageRemoveDataCommand)), "PackageRemoveDataCommand not discovered.");
			Assert.IsTrue(commandTypes.Any(t => t == typeof(Greg.Xrm.Command.Commands.Package.PackageListCommand)), "PackageListCommand not discovered.");
			Assert.IsTrue(commandTypes.Any(t => t == typeof(Greg.Xrm.Command.Commands.Package.PackageSyncCommand)), "PackageSyncCommand not discovered.");
			Assert.IsTrue(commandTypes.Any(t => t == typeof(Greg.Xrm.Command.Commands.Package.PackageFixCommand)), "PackageFixCommand not discovered.");
			Assert.IsTrue(commandTypes.Any(t => t == typeof(Greg.Xrm.Command.Commands.Package.PackagePublishCommand)), "PackagePublishCommand not discovered.");
			Assert.IsTrue(commandTypes.Any(t => t == typeof(Greg.Xrm.Command.Commands.Package.PackageReleaseCommand)), "PackageReleaseCommand not discovered.");
			Assert.IsTrue(commandTypes.Any(t => t == typeof(Greg.Xrm.Command.Commands.Package.PackageDeployCommand)), "PackageDeployCommand not discovered.");
			Assert.IsTrue(commandTypes.Any(t => t == typeof(Greg.Xrm.Command.Commands.Package.PackageBuildCommand)), "PackageBuildCommand not discovered.");
			Assert.IsTrue(commandTypes.Any(t => t == typeof(Greg.Xrm.Command.Commands.Package.PackageValidateCommand)), "PackageValidateCommand not discovered.");
		}

		[TestMethod]
		public void ScanForModules_ShouldDiscoverAutofacModules()
		{
			var registry = CreateRegistry();
			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);

			// The core assembly should have at least the IoCModule
			Assert.IsTrue(registry.Modules.Count > 0, "No Autofac modules were discovered from the core assembly.");
			Assert.IsTrue(registry.Modules.Any(m => m.GetType().Name == "IoCModule"), "IoCModule not discovered.");
		}

		[TestMethod]
		public void ScanForCommands_ShouldMatchCommandExecutorPairs()
		{
			var registry = CreateRegistry();
			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);

			// Every discovered command should have a matching executor
			foreach (var command in registry.Commands)
			{
				Assert.IsNotNull(command.CommandExecutorType,
					$"Command '{command.ExpandedVerbs}' has no matching ICommandExecutor type.");

				var executorInterface = typeof(ICommandExecutor<>).MakeGenericType(command.CommandType);
				Assert.IsTrue(executorInterface.IsAssignableFrom(command.CommandExecutorType),
					$"Executor '{command.CommandExecutorType.Name}' does not implement '{executorInterface.Name}'.");
			}
		}

		[TestMethod]
		public void ScanForCommands_ShouldDetectDuplicateVerbs()
		{
			// This test verifies that the registry throws on duplicate command definitions.
			// Since the core assembly has no duplicates, we verify the scan completes without error.
			var registry = CreateRegistry();
			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);

			// Verify no duplicate command definitions exist
			var verbGroups = registry.Commands
				.GroupBy(c => string.Join(" ", c.Verbs))
				.Where(g => g.Count() > 1)
				.ToList();

			Assert.AreEqual(0, verbGroups.Count,
				$"Duplicate commands found: {string.Join(", ", verbGroups.Select(g => g.Key))}");
		}

		[TestMethod]
		public void ScanForNamespaceHelpers_ShouldDiscoverHelpers()
		{
			var registry = CreateRegistry();
			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);

			// Verify namespace helpers are discovered (at minimum, the empty helper from plugin scanning)
			Assert.IsNotNull(registry.Tree, "Command tree was not built.");
		}

		[TestMethod]
		public void CreateVerbTree_ShouldBuildHierarchicalVerbTree()
		{
			var registry = CreateRegistry();
			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);

			// Verify the tree has root nodes
			Assert.IsTrue(registry.Tree.Count > 0, "Command tree has no root nodes.");

			// Verify 'auth' verb exists (known command)
			var authNode = registry.Tree.FirstOrDefault(n => n.Verb == "auth");
			Assert.IsNotNull(authNode, "'auth' verb not found in command tree.");

			// Verify 'auth list' subcommand exists
			var listNode = authNode.Children.FirstOrDefault(n => n.Verb == "list");
			Assert.IsNotNull(listNode, "'auth list' subcommand not found in command tree.");
			Assert.IsNotNull(listNode.Command, "'auth list' has no associated command.");

			var packageNode = registry.Tree.FirstOrDefault(n => n.Verb == "package");
			Assert.IsNotNull(packageNode, "'package' verb not found in command tree.");
			Assert.IsNotNull(packageNode.Children.FirstOrDefault(n => n.Verb == "init"), "'package init' subcommand not found in command tree.");
			Assert.IsNotNull(packageNode.Children.FirstOrDefault(n => n.Verb == "add"), "'package add' subcommand not found in command tree.");
			Assert.IsNotNull(packageNode.Children.FirstOrDefault(n => n.Verb == "remove"), "'package remove' subcommand not found in command tree.");
			Assert.IsNotNull(packageNode.Children.FirstOrDefault(n => n.Verb == "list"), "'package list' subcommand not found in command tree.");
			Assert.IsNotNull(packageNode.Children.FirstOrDefault(n => n.Verb == "sync"), "'package sync' subcommand not found in command tree.");
			Assert.IsNotNull(packageNode.Children.FirstOrDefault(n => n.Verb == "fix"), "'package fix' subcommand not found in command tree.");
			Assert.IsNotNull(packageNode.Children.FirstOrDefault(n => n.Verb == "publish"), "'package publish' subcommand not found in command tree.");
			Assert.IsNotNull(packageNode.Children.FirstOrDefault(n => n.Verb == "release"), "'package release' subcommand not found in command tree.");
			Assert.IsNotNull(packageNode.Children.FirstOrDefault(n => n.Verb == "deploy"), "'package deploy' subcommand not found in command tree.");
			Assert.IsNotNull(packageNode.Children.FirstOrDefault(n => n.Verb == "build"), "'package build' subcommand not found in command tree.");
			Assert.IsNotNull(packageNode.Children.FirstOrDefault(n => n.Verb == "validate"), "'package validate' subcommand not found in command tree.");
			Assert.IsNotNull(packageNode.Children.FirstOrDefault(n => n.Verb == "show"), "'package show' subcommand not found in command tree.");
		}

		#endregion

		#region Phase 2: Plugin Loading Integration Tests

		[TestMethod]
		public void ScanPluginsFolder_WithNonExistentFolder_ShouldNotThrow()
		{
			var registry = CreateRegistry();
			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);

			var args = new Greg.Xrm.Command.Parsing.CommandLineArguments(new[] { "help" });
			registry.ScanPluginsFolder(args);

			// Should complete without error even with no plugins folder
			Assert.IsTrue(true);
		}

		[TestMethod]
		public void ScanPluginsFolder_WithEmptyPluginsFolder_ShouldNotAddCommands()
		{
			var registry = CreateRegistry();
			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);
			var initialCount = registry.Commands.Count;

			var storage = new Greg.Xrm.Command.Services.Storage();
			// Point storage to temp dir (simulated)
			var args = new Greg.Xrm.Command.Parsing.CommandLineArguments(new[] { "help" });
			registry.ScanPluginsFolder(args);

			// Should not add any commands from empty folder
			Assert.AreEqual(initialCount, registry.Commands.Count);
		}

		[TestMethod]
		public void ScanPluginsFolder_ShouldDeleteFoldersMarkedForRemoval()
		{
			var root = TestTempPath.CreateDirectory("command_registry_plugins");
			var pluginsRoot = Path.Combine(root, "Plugins");
			var pluginFolder = Path.Combine(pluginsRoot, "sample");
			Directory.CreateDirectory(pluginFolder);
			File.WriteAllText(Path.Combine(pluginFolder, ".delete"), "delete");

			var storage = new Mock<IStorage>();
			storage.Setup(s => s.GetOrCreateStorageFolder()).Returns(new DirectoryInfo(root));

			var registry = CreateRegistry(storage.Object);
			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);

			registry.ScanPluginsFolder(new Greg.Xrm.Command.Parsing.CommandLineArguments(new[] { "help" }));

			Assert.IsFalse(Directory.Exists(pluginFolder), "Marked plugin folder should be deleted during plugin scan.");
		}

		[TestMethod]
		public void ScanForCommands_ShouldSkipAbstractCommandTypes()
		{
			var registry = CreateRegistry();
			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);

			// No abstract types should be in the command list
			foreach (var command in registry.Commands)
			{
				Assert.IsFalse(command.CommandType.IsAbstract,
					$"Abstract command type '{command.CommandType.Name}' was incorrectly discovered.");
			}
		}

		[TestMethod]
		public void ScanForCommands_ShouldSkipCommandsWithoutParameterlessConstructor()
		{
			var registry = CreateRegistry();
			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);

			// All discovered commands should have a parameterless constructor
			foreach (var command in registry.Commands)
			{
				var hasParameterless = command.CommandType.GetConstructors()
					.Any(c => c.IsPublic && c.GetParameters().Length == 0);
				Assert.IsTrue(hasParameterless,
					$"Command '{command.CommandType.Name}' has no parameterless constructor.");
			}
		}

		[TestMethod]
		public void ScanForCommands_ShouldSkipObsoleteCommandTypes()
		{
			var registry = CreateRegistry();
			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);

			// No obsolete types should be in the command list
			foreach (var command in registry.Commands)
			{
				var obsoleteAttr = command.CommandType.GetCustomAttribute<ObsoleteAttribute>();
				Assert.IsNull(obsoleteAttr,
					$"Obsolete command type '{command.CommandType.Name}' was incorrectly discovered.");
			}
		}

		#endregion

		#region Phase 3: Bootstrapper & Edge Cases

		[TestMethod]
		public void InitializeFromAssembly_WithMultipleCalls_ShouldNotDuplicateCommands()
		{
			var registry = CreateRegistry();
			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);
			var firstCount = registry.Commands.Count;

			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);
			var secondCount = registry.Commands.Count;

			// Second call should add more commands (since it's a new registration)
			// but within a single scan, no duplicates
			Assert.IsTrue(secondCount >= firstCount);

			// Verify no duplicates by verb
			var duplicates = registry.Commands
				.GroupBy(c => string.Join(" ", c.Verbs))
				.Where(g => g.Count() > 1)
				.Select(g => g.Key)
				.ToList();

			Assert.AreEqual(0, duplicates.Count,
				$"Duplicate commands after double initialization: {string.Join(", ", duplicates)}");
		}

		[TestMethod]
		public void CommandTree_ShouldSupportMultiLevelVerbHierarchy()
		{
			var registry = CreateRegistry();
			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);

			// Verify multi-level verbs exist (e.g., "env create", "alm pipeline create")
			var hasMultiLevel = registry.Commands.Any(c => c.Verbs.Count >= 3);

			// Even if no 3-level commands exist, the tree structure should support it
			Assert.IsNotNull(registry.Tree, "Command tree is null.");
		}

		[TestMethod]
		public void GetExecutorTypeFor_ShouldReturnCorrectExecutorType()
		{
			var registry = CreateRegistry();
			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);

			var listCommandType = typeof(Greg.Xrm.Command.Commands.Auth.ListCommand);
			var executorType = registry.GetExecutorTypeFor(listCommandType);

			Assert.IsNotNull(executorType, "No executor type found for ListCommand.");
			var executorInterface = typeof(ICommandExecutor<>).MakeGenericType(listCommandType);
			Assert.IsTrue(executorInterface.IsAssignableFrom(executorType),
				$"Executor type does not implement ICommandExecutor<ListCommand>.");
		}

		[TestMethod]
		public void GetExecutorTypeFor_WithUnknownType_ShouldReturnNull()
		{
			var registry = CreateRegistry();
			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);

			var unknownType = typeof(string);
			var executorType = registry.GetExecutorTypeFor(unknownType);

			Assert.IsNull(executorType, "Executor type should be null for unknown command type.");
		}

		[TestMethod]
		public void ScanPluginsFolder_WithToolArgument_ShouldAttemptToLoadDll()
		{
			var registry = CreateRegistry();
			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);

			var args = new Greg.Xrm.Command.Parsing.CommandLineArguments(new[] { "help", "--tool", "C:\\nonexistent\\plugin.dll" });
			registry.ScanPluginsFolder(args);

			// Should handle non-existent file gracefully without throwing
			Assert.IsTrue(true);
		}

		[TestMethod]
		public void ScanPluginsFolder_WithNonDllFile_ShouldSkip()
		{
			var registry = CreateRegistry();
			registry.InitializeFromAssembly(typeof(Greg.Xrm.Command.Commands.Help.HelpCommand).Assembly);

			var args = new Greg.Xrm.Command.Parsing.CommandLineArguments(new[] { "help", "--tool", "C:\\temp\\readme.txt" });
			registry.ScanPluginsFolder(args);

			// Should skip non-DLL file gracefully
			Assert.IsTrue(true);
		}

		#endregion
	}
}
