using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.ServiceModel;
using Greg.Xrm.Command.Commands.Help;
using Greg.Xrm.Command.Commands.History;
using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services.CommandHistory;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.Telemetry;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;

namespace Greg.Xrm.Command
{
abstract partial class CommandRunnerBase(
		IOutput output,
		ILogger log,
		ICommandExecutorFactory commandExecutorFactory,
		IHistoryTracker historyTracker,
		ICommandLineArguments args,
		ITelemetryService telemetryService,
		Greg.Xrm.Command.Diagnostics.ICorrelationIdProvider correlationIdProvider)
	{
		private readonly Greg.Xrm.Command.Diagnostics.ICorrelationIdProvider correlationIdProvider = correlationIdProvider ?? throw new ArgumentNullException(nameof(correlationIdProvider));
		private static readonly Type[] commandsNotToTrack =
		[
			typeof(HelpCommand),
			typeof(GetCommand),
			typeof(SetLengthCommand),
			typeof(ClearCommand),
		];


		protected virtual bool IsValidCommand(object command)
		{
			var validationContext = new ValidationContext(command);
			var validationResults = new List<ValidationResult>();
			if (!Validator.TryValidateObject(command, validationContext, validationResults, true))
			{
				output.WriteLine("Invalid command options:", ConsoleColor.Red).WriteLine();
				foreach (var validationResult in validationResults)
				{
					output.Write("    ");
					output.WriteLine(validationResult.ErrorMessage, ConsoleColor.Red);
				}

				LogInvalidCommandOptions(log);
				return false;
			}

			return true;
		}



		protected async Task TrackCommandIntoHistoryAsync(object command)
		{
			if (commandsNotToTrack.Contains(command.GetType()))
				return;

			await historyTracker.AddAsync(command: [.. args]);
		}

		protected async Task<int> ExecuteCommand(object command, CancellationToken cancellationToken)
		{
			var startedAt = DateTimeOffset.UtcNow;
			var exitCode = ExitCodes.InternalError;
			try
			{
				if (!GetCommandExecutor(command, out MethodInfo? method, out object? commandExecutor) || method == null || commandExecutor == null)
				{
					exitCode = ExitCodes.InternalError;
					return exitCode;
				}

				var result = await ExecuteCommandAsync(command, commandExecutor, method, cancellationToken);

				if (result == null)
				{
					LogCommandExecutedResultIsNull(log, command.GetType());
					exitCode = ExitCodes.InternalError;
					return exitCode;
				}

				if (!result.IsSuccess)
				{
					LogCommandExecutedResult(log, command.GetType(), result.IsSuccess);
					PrintFailure(command, result);

					exitCode = ExitCodes.FromException(result.Exception);
					return exitCode;
				}

				if (result.IsSuccess && result.Count > 0)
				{
					PrintSuccess(result);
				}

				LogCommandExecutedResult(log, command.GetType(), result.IsSuccess);
				exitCode = ExitCodes.Success;
				return exitCode;
			}
			catch (Exception ex)
			{
				var message = ex.Message;

				if (ex.InnerException != null)
				{
					message += Environment.NewLine + " Inner exception: " + ex.InnerException.Message;
				}

				output.WriteLine(message, ConsoleColor.Red).WriteLine();
				output.WriteCorrelationHeader(correlationIdProvider.Current);
				LogUnhandledError(log, ex, ex.Message);

				exitCode = ExitCodes.FromException(ex);
				return exitCode;
			}
			finally
			{
				await telemetryService.TrackCommandAsync(command.GetType().Name, exitCode, DateTimeOffset.UtcNow - startedAt, cancellationToken).ConfigureAwait(false);
			}
		}

		private bool GetCommandExecutor(object command, out MethodInfo? method, out object? commandExecutor)
		{
			method = null;
			commandExecutor = commandExecutorFactory.CreateFor(command.GetType());
			if (commandExecutor == null)
			{
				output.WriteLine("Internal error, see logs for more info.", ConsoleColor.Red).WriteLine();
				LogNoCommandExecutorFound(log, command.GetType());

				return false;
			}

			var specificCommandExecutorType = typeof(ICommandExecutor<>).MakeGenericType(command.GetType());

			method = specificCommandExecutorType.GetMethod("ExecuteAsync");
			if (method == null)
			{
				output.WriteLine("Internal error, see logs for more info.", ConsoleColor.Red).WriteLine();
				LogNoExecuteAsyncMethodFound(log, specificCommandExecutorType);

				return false;
			}

			return true;
		}







		private void PrintFailure(object command, CommandResult result)
		{
			output.Write(result.ErrorMessage, ConsoleColor.Red).WriteLine();
			output.WriteCorrelationHeader(correlationIdProvider.Current);

			var ex = result.Exception;
			if (ex == null) return;

			LogCommandHasError(log, command.GetType(), ex.GetType(), ex.Message, ex);
			if (ex.InnerException != null)
			{
				LogInnerException(log, ex.InnerException, ex.InnerException.Message);
			}

			if (ex.GetType() != typeof(FaultException<OrganizationServiceFault>))
			{
				output
					.Write("  Exception of type '", ConsoleColor.Red)
					.Write(ex.GetType().FullName, ConsoleColor.Red)
					.Write("'. ", ConsoleColor.Red);
			}

			if (ex.InnerException != null)
			{
				output.Write("  ").WriteLine(ex.InnerException.Message, ConsoleColor.Red);
			}
		}





		private void PrintSuccess(CommandResult result)
		{
			var padding = result.Max(_ => _.Key.Length);
			output.WriteLine("Result: ");
			foreach (var kvp in result)
			{
				output.Write("  ").Write(kvp.Key.PadRight(padding)).Write(": ").WriteLine(kvp.Value, ConsoleColor.Yellow);
			}
		}


		protected async Task<CommandResult?> ExecuteCommandAsync(object command, object commandExecutor, MethodInfo method, CancellationToken cancellationToken)
		{
			var task = (Task<CommandResult>?)method.Invoke(commandExecutor, [command, cancellationToken]);
			if (task == null)
			{
				output.WriteLine("Internal error, see logs for more info.", ConsoleColor.Red).WriteLine();
				LogInvalidResultFromExecutor(log, command.GetType());

				return null;
			}

			var result = await task;
			output.WriteLine();

			if (task.IsFaulted)
			{
				output.WriteLine("Internal error, see logs for more info.", ConsoleColor.Red).WriteLine();
				LogErrorWhileExecutingCommand(log, command.GetType(), task.Exception);
				return null;
			}

			if (task.IsCanceled)
			{
				output.WriteLine("Internal error, see logs for more info.", ConsoleColor.Red).WriteLine();
				LogCommandCancelled(log, command.GetType());
				return null;
			}

			return result;
		}
	}
}
