using System.Text;
using System.Text.Json;
using Greg.Xrm.Command.Services.Connection;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Greg.Xrm.Command.Services.DataverseGaps
{
	public class DataverseGapClient(IOrganizationServiceRepository organizationServiceRepository) : IDataverseGapClient
	{
		private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
		{
			WriteIndented = true
		};

		private readonly IOrganizationServiceRepository organizationServiceRepository = organizationServiceRepository ?? throw new ArgumentNullException(nameof(organizationServiceRepository));

		public async Task<JsonDocument> QueryAsync(string entityName, IReadOnlyCollection<string> columns, IReadOnlyDictionary<string, object?> filters, CancellationToken cancellationToken)
		{
			var crm = await this.organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			var query = new QueryExpression(entityName)
			{
				ColumnSet = new ColumnSet(columns.Count == 0)
			};

			foreach (var column in columns)
			{
				query.ColumnSet.AddColumn(column);
			}

			foreach (var filter in filters.Where(x => x.Value is not null && !string.IsNullOrWhiteSpace(Convert.ToString(x.Value))))
			{
				query.Criteria.AddCondition(filter.Key, ConditionOperator.Equal, NormalizeValue(filter.Value));
			}

			var result = await crm.RetrieveMultipleAsync(query, cancellationToken).ConfigureAwait(false);
			return ToJsonDocument(new
			{
				value = result.Entities.Select(ToDictionary).ToArray(),
				count = result.Entities.Count
			});
		}

		public async Task<JsonDocument> ExportWorkflowAsync(Guid id, CancellationToken cancellationToken)
		{
			var crm = await this.organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			var entity = await crm.RetrieveAsync("workflow", id, new ColumnSet(true), cancellationToken).ConfigureAwait(false);
			return ToJsonDocument(ToDictionary(entity));
		}

		public async Task<JsonDocument> ImportWorkflowAsync(string filePath, string? tableName, int category, CancellationToken cancellationToken)
		{
			var crm = await this.organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			var payload = await File.ReadAllTextAsync(filePath, cancellationToken).ConfigureAwait(false);
			var entity = new Entity("workflow")
			{
				["name"] = Path.GetFileNameWithoutExtension(filePath),
				["category"] = new OptionSetValue(category),
				["xaml"] = payload
			};

			if (!string.IsNullOrWhiteSpace(tableName))
			{
				entity["primaryentity"] = tableName;
			}

			var id = await crm.CreateAsync(entity, cancellationToken).ConfigureAwait(false);
			return ToJsonDocument(new { id, entity = "workflow" });
		}

		public async Task<JsonDocument> SetStateAsync(string entityName, Guid id, int stateCode, int statusCode, CancellationToken cancellationToken)
		{
			var crm = await this.organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			await crm.ExecuteAsync(new SetStateRequest
			{
				EntityMoniker = new EntityReference(entityName, id),
				State = new OptionSetValue(stateCode),
				Status = new OptionSetValue(statusCode)
			}, cancellationToken).ConfigureAwait(false);

			return ToJsonDocument(new { id, entity = entityName, stateCode, statusCode });
		}

		public async Task<JsonDocument> CreateAsync(string entityName, IReadOnlyDictionary<string, object?> attributes, CancellationToken cancellationToken)
		{
			var crm = await this.organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			var entity = new Entity(entityName);
			foreach (var attribute in attributes.Where(x => x.Value is not null && !string.IsNullOrWhiteSpace(Convert.ToString(x.Value))))
			{
				entity[attribute.Key] = NormalizeValue(attribute.Value);
			}

			var id = await crm.CreateAsync(entity, cancellationToken).ConfigureAwait(false);
			return ToJsonDocument(new { id, entity = entityName });
		}

		public async Task<JsonDocument> DeleteAsync(string entityName, Guid id, CancellationToken cancellationToken)
		{
			var crm = await this.organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			await crm.DeleteAsync(entityName, id, cancellationToken).ConfigureAwait(false);
			return ToJsonDocument(new { id, entity = entityName, deleted = true });
		}

		public async Task<JsonDocument> ExecuteActionAsync(string actionName, IReadOnlyDictionary<string, object?> parameters, CancellationToken cancellationToken)
		{
			var crm = await this.organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			var request = new OrganizationRequest(actionName);
			foreach (var parameter in parameters.Where(x => x.Value is not null && !string.IsNullOrWhiteSpace(Convert.ToString(x.Value))))
			{
				request.Parameters[parameter.Key] = NormalizeValue(parameter.Value);
			}

			var response = await crm.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
			return ToJsonDocument(new
			{
				action = actionName,
				results = response.Results.ToDictionary(x => x.Key, x => SerializeValue(x.Value))
			});
		}

		public async Task<JsonDocument> UploadFileAsync(string tableName, Guid recordId, string columnName, string filePath, CancellationToken cancellationToken)
		{
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException("File column upload source not found.", filePath);
			}

			var crm = await this.organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			var entity = new Entity(tableName, recordId)
			{
				[columnName] = Convert.ToBase64String(await File.ReadAllBytesAsync(filePath, cancellationToken).ConfigureAwait(false))
			};

			await crm.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
			return ToJsonDocument(new { table = tableName, recordId, column = columnName, file = Path.GetFileName(filePath) });
		}

		private static object? NormalizeValue(object? value)
		{
			if (value is null)
			{
				return null;
			}

			if (value is Guid guid)
			{
				return guid;
			}

			var text = Convert.ToString(value);
			if (Guid.TryParse(text, out var parsedGuid))
			{
				return parsedGuid;
			}

			if (int.TryParse(text, out var parsedInt))
			{
				return parsedInt;
			}

			return value;
		}

		private static Dictionary<string, object?> ToDictionary(Entity entity)
		{
			var result = new Dictionary<string, object?>
			{
				["id"] = entity.Id,
				["logicalName"] = entity.LogicalName
			};

			foreach (var attribute in entity.Attributes)
			{
				result[attribute.Key] = SerializeValue(attribute.Value);
			}

			return result;
		}

		private static object? SerializeValue(object? value)
		{
			return value switch
			{
				null => null,
				EntityReference entityReference => new { entityReference.Id, entityReference.LogicalName, entityReference.Name },
				OptionSetValue option => option.Value,
				Money money => money.Value,
				DateTime dateTime => dateTime.ToString("o"),
				byte[] bytes => Encoding.UTF8.GetString(bytes),
				_ => value
			};
		}

		private static JsonDocument ToJsonDocument(object value)
			=> JsonDocument.Parse(JsonSerializer.Serialize(value, JsonOptions));
	}
}
