using System.Text.Json;
using Greg.Xrm.Command.Services.Connection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Greg.Xrm.Command.Services.DesktopFlows
{
	public class DesktopFlowClient(
		IOrganizationServiceRepository organizationServiceRepository,
		ITokenProvider tokenProvider,
		IHttpClientFactory httpClientFactory) : IDesktopFlowClient
	{
		private const string FlowResource = "https://api.flow.microsoft.com/";
		private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
		{
			WriteIndented = true
		};

		private readonly IOrganizationServiceRepository organizationServiceRepository = organizationServiceRepository ?? throw new ArgumentNullException(nameof(organizationServiceRepository));
		private readonly ITokenProvider tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
		private readonly IHttpClientFactory httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

		public async Task<JsonDocument> ListDesktopFlowsAsync(string? environmentId, CancellationToken cancellationToken)
		{
			var crm = await this.organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			var query = new QueryExpression("workflow")
			{
				ColumnSet = new ColumnSet("workflowid", "name", "category", "statecode", "statuscode", "modifiedon")
			};
			query.Criteria.AddCondition("category", ConditionOperator.Equal, 6);

			var result = await crm.RetrieveMultipleAsync(query, cancellationToken).ConfigureAwait(false);
			var rows = result.Entities.Select(entity => new
			{
				id = entity.Id,
				name = entity.GetAttributeValue<string>("name"),
				category = entity.GetAttributeValue<OptionSetValue>("category")?.Value,
				state = entity.GetAttributeValue<OptionSetValue>("statecode")?.Value,
				status = entity.GetAttributeValue<OptionSetValue>("statuscode")?.Value,
				modifiedOn = entity.GetAttributeValue<DateTime?>("modifiedon"),
				environmentId
			});

			return JsonDocument.Parse(JsonSerializer.Serialize(new { value = rows }, SerializerOptions));
		}

		public async Task<JsonDocument> GetDesktopFlowAsync(Guid id, CancellationToken cancellationToken)
		{
			var crm = await this.organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			var entity = await crm.RetrieveAsync("workflow", id, new ColumnSet("workflowid", "name", "category", "clientdata", "statecode", "statuscode", "modifiedon"), cancellationToken).ConfigureAwait(false);
			return JsonDocument.Parse(JsonSerializer.Serialize(new
			{
				id = entity.Id,
				name = entity.GetAttributeValue<string>("name"),
				category = entity.GetAttributeValue<OptionSetValue>("category")?.Value,
				state = entity.GetAttributeValue<OptionSetValue>("statecode")?.Value,
				status = entity.GetAttributeValue<OptionSetValue>("statuscode")?.Value,
				modifiedOn = entity.GetAttributeValue<DateTime?>("modifiedon"),
				scriptPreview = Preview(entity.GetAttributeValue<string>("clientdata"))
			}, SerializerOptions));
		}

		public Task<JsonDocument> TriggerDesktopFlowAsync(string environmentId, Guid id, string machineGroup, IReadOnlyDictionary<string, string> inputs, CancellationToken cancellationToken)
			=> PostFlowAsync(environmentId, $"/flows/{id}/run", new { machineGroup, inputs }, cancellationToken);

		public Task<JsonDocument> ListRunsAsync(string environmentId, Guid id, CancellationToken cancellationToken)
			=> GetFlowAsync(environmentId, $"/flows/{id}/runs", cancellationToken);

		public Task<JsonDocument> GetRunAsync(string environmentId, string runId, CancellationToken cancellationToken)
			=> GetFlowAsync(environmentId, $"/runs/{Uri.EscapeDataString(runId)}", cancellationToken);

		public Task<JsonDocument> ListMachinesAsync(string environmentId, CancellationToken cancellationToken)
			=> GetFlowAsync(environmentId, "/machines", cancellationToken);

		public Task<JsonDocument> ListMachineGroupsAsync(string environmentId, CancellationToken cancellationToken)
			=> GetFlowAsync(environmentId, "/machineGroups", cancellationToken);

		public Task<JsonDocument> AssignMachineToGroupAsync(string environmentId, string machineId, string groupId, CancellationToken cancellationToken)
			=> PostFlowAsync(environmentId, $"/machineGroups/{Uri.EscapeDataString(groupId)}/machines", new { machineId }, cancellationToken);

		public Task<JsonDocument> ListApprovalsAsync(string environmentId, CancellationToken cancellationToken)
			=> GetFlowAsync(environmentId, "/approvals", cancellationToken);

		public Task<JsonDocument> RespondToApprovalAsync(string environmentId, string approvalId, string decision, string? comment, CancellationToken cancellationToken)
			=> PostFlowAsync(environmentId, $"/approvals/{Uri.EscapeDataString(approvalId)}/responses", new { decision, comment }, cancellationToken);

		private Task<JsonDocument> GetFlowAsync(string environmentId, string path, CancellationToken cancellationToken)
			=> SendFlowAsync(HttpMethod.Get, environmentId, path, null, cancellationToken);

		private Task<JsonDocument> PostFlowAsync(string environmentId, string path, object payload, CancellationToken cancellationToken)
			=> SendFlowAsync(HttpMethod.Post, environmentId, path, payload, cancellationToken);

		private async Task<JsonDocument> SendFlowAsync(HttpMethod method, string environmentId, string path, object? payload, CancellationToken cancellationToken)
		{
			var token = await this.tokenProvider.GetTokenAsync(FlowResource, cancellationToken).ConfigureAwait(false);
			if (string.IsNullOrWhiteSpace(token))
			{
				throw new InvalidOperationException("Unable to acquire a Power Automate access token from the current connection.");
			}

			var normalizedPath = path.StartsWith("/", StringComparison.Ordinal) ? path : "/" + path;
			using var request = new HttpRequestMessage(method, $"https://api.flow.microsoft.com/providers/Microsoft.ProcessSimple/environments/{Uri.EscapeDataString(environmentId)}{normalizedPath}?api-version=2016-11-01");
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
			if (payload != null)
			{
				request.Content = new StringContent(JsonSerializer.Serialize(payload, SerializerOptions), System.Text.Encoding.UTF8, "application/json");
			}

			var client = this.httpClientFactory.CreateClient();
			using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
			var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
			if (!response.IsSuccessStatusCode)
			{
				throw new InvalidOperationException($"Power Automate API error ({response.StatusCode}): {content}");
			}

			return JsonDocument.Parse(string.IsNullOrWhiteSpace(content) ? "{}" : content);
		}

		private static string Preview(string? value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return string.Empty;
			}

			return value.Length <= 400 ? value : value[..400];
		}
	}
}
