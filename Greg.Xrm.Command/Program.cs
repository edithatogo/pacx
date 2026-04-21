using System.Diagnostics;
using System.Threading;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Greg.Xrm.Command;
using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services;
using Greg.Xrm.Command.Services.CommandHistory;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Mcp;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Pluralization;
using Greg.Xrm.Command.Services.Project;
using Greg.Xrm.Command.Services.Settings;
using Greg.Xrm.Command.Updates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;

internal class Program
{
	private static async Task<int> Main(string[] args)
	{
		var serviceCollection = new ServiceCollection();
		serviceCollection.AddSingleton<IStorage>(new Storage());
		serviceCollection.AddSingleton<ICommandLineArguments>(new CommandLineArguments(args));
		serviceCollection.AddSingleton<ICommandRegistry, CommandRegistry>();
		serviceCollection.AddSingleton<ICommandParser, CommandParser>();
		serviceCollection.RegisterCommandExecutors(typeof(CommandAttribute).Assembly);
		serviceCollection.AddTransient<ICommandRunnerFactory, CommandRunnerFactory>();
		serviceCollection.AddTransient<ICommandExecutorFactory, CommandExecutorFactory>();
		serviceCollection.AddTransient<IPluralizationFactory, PluralizationFactory>();
		serviceCollection.AddTransient<ISettingsRepository, SettingsRepository>();
		serviceCollection.AddSingleton<IAutoUpdater, AutoUpdater>();
		serviceCollection.AddTransient<IPacxProjectRepository, PacxProjectRepository>();
		serviceCollection.AddSingleton<IOrganizationServiceRepository, OrganizationServiceRepository>();
		serviceCollection.AddSingleton<ITokenProvider, TokenProvider>();
		serviceCollection.AddSingleton<IOutput, OutputToConsole>();
		serviceCollection.AddSingleton<Greg.Xrm.Command.Diagnostics.ICorrelationIdProvider, Greg.Xrm.Command.Diagnostics.AmbientCorrelationIdProvider>();
		serviceCollection.AddSingleton<IMcpServerLauncher, Greg.Xrm.Command.Mcp.McpServerLauncher>();
		serviceCollection.AddTransient<IHistoryTracker, HistoryTracker>();
		serviceCollection.AddTransient<Bootstrapper>();
		serviceCollection.AddSingleton(AnsiConsole.Console);

		serviceCollection.AddTransient<Greg.Xrm.Command.Services.CorrelationIdHandler>();
		serviceCollection.AddHttpClient(string.Empty)
			.AddHttpMessageHandler<Greg.Xrm.Command.Services.CorrelationIdHandler>();

		serviceCollection.AddAutofac();
		serviceCollection.AddLogging(logging =>
		{
			logging.ClearProviders();
			logging.AddDebug();
		});


		var containerBuilder = new ContainerBuilder();
		containerBuilder.Populate(serviceCollection);

		var container = containerBuilder.Build();

		var correlationIdProvider = container.Resolve<Greg.Xrm.Command.Diagnostics.ICorrelationIdProvider>();
		if (correlationIdProvider is Greg.Xrm.Command.Diagnostics.AmbientCorrelationIdProvider ambientProvider)
		{
			var argsList = container.Resolve<ICommandLineArguments>();
			var index = argsList.IndexOf("--correlation-id");
			if (index >= 0 && index < argsList.Count - 1)
			{
				ambientProvider.Current = argsList[index + 1];
				argsList.RemoveAt(index + 1);
				argsList.RemoveAt(index);
			}
		}

		using var scope = container.BeginLifetimeScope("activation");
		using var cancellationTokenSource = new CancellationTokenSource();
		ConsoleCancelEventHandler? cancelHandler = null;
		cancelHandler = (_, e) =>
		{
			e.Cancel = true;
			cancellationTokenSource.Cancel();
		};
		Console.CancelKeyPress += cancelHandler;
		try
		{
			var hostedService = scope.Resolve<Bootstrapper>();
			return await hostedService.StartAsync(cancellationTokenSource.Token).ConfigureAwait(false);
		}
		catch (OperationCanceledException)
		{
			return 130;
		}
		catch (AggregateException ex)
		{
			foreach (var inner in ex.InnerExceptions)
			{
				Console.WriteLine(inner.Message);
			}
			return -1;
		}
		catch (DependencyResolutionException ex)
		{
			Console.WriteLine(ex);
			return -1;
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			return -1;
		}

		finally
		{
			Console.CancelKeyPress -= cancelHandler;
#if DEBUG
			if (Debugger.IsAttached)
			{
				Console.WriteLine("Press any key to exit...");
				Console.ReadKey();
			}
#endif
		}
	}
}
