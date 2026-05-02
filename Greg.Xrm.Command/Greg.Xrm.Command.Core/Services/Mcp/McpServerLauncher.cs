using Autofac;
using Greg.Xrm.Command;
using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services;
using Greg.Xrm.Command.Services.Output;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System.Reflection;
using System.Text.Json;

namespace Greg.Xrm.Command.Services.Mcp;

public sealed class McpServerLauncher : IMcpServerLauncher
{
	public Task RunAsync(IReadOnlyCommandRegistry registry, IStorage storage, CancellationToken cancellationToken = default)
	{
		return McpServerHost.RunAsync(registry, storage, cancellationToken);
	}
}

public static class McpServerHost
{
	public static async Task RunAsync(IReadOnlyCommandRegistry registry, IStorage storage, CancellationToken cancellationToken = default)
	{
		var builder = Host.CreateApplicationBuilder(Array.Empty<string>());

		// Stdio servers must keep stdout reserved for JSON-RPC traffic.
		builder.Logging.ClearProviders();
		builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);

		builder.Services.AddSingleton(registry);
		builder.Services.AddSingleton<IReadOnlyCommandRegistry>(registry);
		builder.Services.AddSingleton<IStorage>(storage);
		builder.Services.AddSingleton<IOutput, OutputToMemory>();
		builder.Services.AddMcpServer()
			.WithStdioServerTransport()
			.WithListToolsHandler((_, ct) => new ValueTask<ListToolsResult>(BuildListToolsResult(registry)))
			.WithCallToolHandler((context, ct) => new ValueTask<CallToolResult>(ExecuteToolAsync(registry, storage, context.Params, ct)));

		await builder.Build().RunAsync(cancellationToken).ConfigureAwait(false);
	}

	private static ListToolsResult BuildListToolsResult(IReadOnlyCommandRegistry registry)
	{
		return new ListToolsResult
		{
			Tools = registry.Commands.Select(BuildTool).ToList(),
		};
	}

	private static Tool BuildTool(CommandDefinition command)
	{
		using var schema = JsonDocument.Parse(BuildInputSchema(command));

		return new Tool
		{
			Name = string.Join("_", command.Verbs),
			Title = command.ExpandedVerbs,
			Description = command.HelpText,
			InputSchema = schema.RootElement.Clone(),
		};
	}

	private static string BuildInputSchema(CommandDefinition command)
	{
		var properties = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
		var required = new List<string>();

		foreach (var option in command.Options)
		{
			var propName = option.Option.LongName.TrimStart('-').Replace("-", "_");
			var schema = new Dictionary<string, object?>
			{
				["type"] = option.Property.PropertyType == typeof(bool) || option.Property.PropertyType == typeof(bool?)
					? "boolean"
					: "string",
			};

			if (!string.IsNullOrWhiteSpace(option.Option.HelpText))
			{
				schema["description"] = option.Option.HelpText;
			}

			if (option.Option.DefaultValue != null)
			{
				schema["default"] = option.Option.DefaultValue;
			}

			properties[propName] = schema;

			if (option.IsRequired)
			{
				required.Add(propName);
			}
		}

		var schemaObject = new Dictionary<string, object?>
		{
			["type"] = "object",
			["properties"] = properties,
		};

		if (required.Count > 0)
		{
			schemaObject["required"] = required;
		}

		return JsonSerializer.Serialize(schemaObject);
	}

	private static async Task<CallToolResult> ExecuteToolAsync(
		IReadOnlyCommandRegistry registry,
		IStorage storage,
		CallToolRequestParams request,
		CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(request.Name))
		{
			return Error("Missing tool name.");
		}

		var verbs = request.Name.Split('_', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		var command = registry.Commands.FirstOrDefault(c => c.Verbs.SequenceEqual(verbs, StringComparer.OrdinalIgnoreCase));
		if (command is null)
		{
			return Error($"Unknown tool: {request.Name}");
		}

		var output = new OutputToMemory();
		var commandInstance = CreateCommandInstance(command, request.Arguments);

		var executorType = command.CommandExecutorType;
		if (executorType == null)
		{
			return Error($"No executor found for command {command.ExpandedVerbs}.");
		}

		var container = BuildContainer(registry, storage, output, command.CommandType.Assembly, executorType.Assembly);
		using var scope = container.BeginLifetimeScope(builder =>
		{
			builder.RegisterInstance(commandInstance).As(command.CommandType);
		});

		var executor = scope.ResolveOptional(executorType);
		if (executor == null)
		{
			return Error($"Unable to resolve executor {executorType.FullName}.");
		}

		var method = executorType.GetMethod("ExecuteAsync", new[] { command.CommandType, typeof(CancellationToken) });
		if (method == null)
		{
			return Error($"Executor {executorType.FullName} does not expose ExecuteAsync.");
		}

		try
		{
			var task = (Task<CommandResult>?)method.Invoke(executor, new object[] { commandInstance, cancellationToken });
			if (task == null)
			{
				return Error($"Executor {executorType.FullName} returned no task.");
			}

			var result = await task.ConfigureAwait(false);
			return result.IsSuccess
				? Success(BuildSuccessText(output, result))
				: Error(BuildFailureText(output, result));
		}
		catch (TargetInvocationException ex) when (ex.InnerException != null)
		{
			Console.Error.WriteLine($"MCP command failed: {ex.InnerException}");
			return Error($"Command failed: {ex.InnerException.GetType().Name}");
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"MCP command failed: {ex}");
			return Error($"Command failed: {ex.GetType().Name}");
		}
	}

	private static IContainer BuildContainer(
		IReadOnlyCommandRegistry registry,
		IStorage storage,
		IOutput output,
		params Assembly[] assemblies)
	{
		var builder = new ContainerBuilder();
		builder.RegisterModule(new IoCModule());
		foreach (var module in registry.Modules)
		{
			builder.RegisterModule(module);
		}
		builder.RegisterInstance(registry).As<IReadOnlyCommandRegistry>();
		builder.RegisterInstance(storage).As<IStorage>();
		builder.RegisterInstance(output).As<IOutput>();
		foreach (var assembly in assemblies.Distinct())
		{
			builder.RegisterAssemblyTypes(assembly)
				.Where(t => t.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICommandExecutor<>)))
				.AsSelf()
				.AsImplementedInterfaces();
		}
		return builder.Build();
	}

	private static object CreateCommandInstance(CommandDefinition command, IDictionary<string, JsonElement>? arguments)
	{
		var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		if (arguments != null)
		{
			foreach (var (key, value) in arguments)
			{
				options["--" + key.Replace('_', '-')] = value.ToString();
			}
		}

		return command.CreateCommand(options);
	}

	private static string BuildSuccessText(OutputToMemory output, CommandResult result)
	{
		var parts = new List<string>();
		var captured = output.ToString();
		if (!string.IsNullOrWhiteSpace(captured))
		{
			parts.Add(captured.TrimEnd());
		}

		if (result.Count > 0)
		{
			var padding = result.Max(x => x.Key.Length);
			var lines = new List<string> { "Result:" };
			foreach (var kvp in result)
			{
				lines.Add($"  {kvp.Key.PadRight(padding)}: {kvp.Value}");
			}

			parts.Add(string.Join(Environment.NewLine, lines));
		}

		return string.Join(Environment.NewLine + Environment.NewLine, parts);
	}

	private static string BuildFailureText(OutputToMemory output, CommandResult result)
	{
		var parts = new List<string>();
		var captured = output.ToString();
		if (!string.IsNullOrWhiteSpace(captured))
		{
			parts.Add(captured.TrimEnd());
		}

		parts.Add(result.ErrorMessage);
		if (result.Exception != null)
		{
			Console.Error.WriteLine($"MCP command failure detail: {result.Exception}");
			parts.Add($"Exception: {result.Exception.GetType().Name}");
		}

		return string.Join(Environment.NewLine + Environment.NewLine, parts.Where(x => !string.IsNullOrWhiteSpace(x)));
	}

	private static CallToolResult Success(string text)
	{
		return new CallToolResult
		{
			IsError = false,
			Content = string.IsNullOrWhiteSpace(text)
				? []
				: [new TextContentBlock { Text = text }],
		};
	}

	private static CallToolResult Error(string text)
	{
		return new CallToolResult
		{
			IsError = true,
			Content = [new TextContentBlock { Text = text }],
		};
	}
}
