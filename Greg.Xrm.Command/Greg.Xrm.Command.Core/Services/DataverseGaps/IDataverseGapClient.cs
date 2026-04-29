using System.Text.Json;

namespace Greg.Xrm.Command.Services.DataverseGaps
{
	public interface IDataverseGapClient
	{
		Task<JsonDocument> QueryAsync(string entityName, IReadOnlyCollection<string> columns, IReadOnlyDictionary<string, object?> filters, CancellationToken cancellationToken);

		Task<JsonDocument> ExportWorkflowAsync(Guid id, CancellationToken cancellationToken);

		Task<JsonDocument> ImportWorkflowAsync(string filePath, string? tableName, int category, CancellationToken cancellationToken);

		Task<JsonDocument> SetStateAsync(string entityName, Guid id, int stateCode, int statusCode, CancellationToken cancellationToken);

		Task<JsonDocument> CreateAsync(string entityName, IReadOnlyDictionary<string, object?> attributes, CancellationToken cancellationToken);

		Task<JsonDocument> DeleteAsync(string entityName, Guid id, CancellationToken cancellationToken);

		Task<JsonDocument> ExecuteActionAsync(string actionName, IReadOnlyDictionary<string, object?> parameters, CancellationToken cancellationToken);

		Task<JsonDocument> UploadFileAsync(string tableName, Guid recordId, string columnName, string filePath, CancellationToken cancellationToken);
	}
}
