using Microsoft.Extensions.Logging;

namespace Greg.Xrm.Command;

abstract partial class CommandRunnerBase
{
	[LoggerMessage(LogLevel.Error, "Invalid command options")]
	private static partial void LogInvalidCommandOptions(ILogger logger);

	[LoggerMessage(LogLevel.Information, "Command {CommandType} has been executed. Result is null.")]
	private static partial void LogCommandExecutedResultIsNull(ILogger logger, Type commandType);

	[LoggerMessage(LogLevel.Information, "Command {CommandType} has been executed. Result is {CommandResult}.")]
	private static partial void LogCommandExecutedResult(ILogger logger, Type commandType, bool commandResult);

	[LoggerMessage(LogLevel.Error, "Unhandled error: {ErrorMessage}")]
	private static partial void LogUnhandledError(ILogger logger, Exception exception, string errorMessage);

	[LoggerMessage(LogLevel.Error, "No command executor found for command {CommandType}.")]
	private static partial void LogNoCommandExecutorFound(ILogger logger, Type commandType);

	[LoggerMessage(LogLevel.Error, "No ExecuteAsync method found for command executor {CommandExecutorType}.")]
	private static partial void LogNoExecuteAsyncMethodFound(ILogger logger, Type commandExecutorType);

	[LoggerMessage(LogLevel.Error, "Command {CommandType} has error, Fault type: {FaultType}, {ErrorMessage}.")]
	private static partial void LogCommandHasError(ILogger logger, Type commandType, Type faultType, string errorMessage, Exception exception);

	[LoggerMessage(LogLevel.Error, "Inner exception: {ErrorMessage}.")]
	private static partial void LogInnerException(ILogger logger, Exception exception, string errorMessage);

	[LoggerMessage(LogLevel.Error, "Invalid result from command executor ExecuteAsync: {CommandType}.")]
	private static partial void LogInvalidResultFromExecutor(ILogger logger, Type commandType);

	[LoggerMessage(LogLevel.Error, "Error while executing command {CommandType}.")]
	private static partial void LogErrorWhileExecutingCommand(ILogger logger, Type commandType, Exception? exception);

	[LoggerMessage(LogLevel.Error, "Command {CommandType} has been cancelled.")]
	private static partial void LogCommandCancelled(ILogger logger, Type commandType);
}
