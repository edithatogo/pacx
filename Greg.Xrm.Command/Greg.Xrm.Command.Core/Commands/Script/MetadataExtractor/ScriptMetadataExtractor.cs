using Greg.Xrm.Command.Commands.Script.Models;
using Greg.Xrm.Command.Commands.Script.Service;
using Greg.Xrm.Command.Services.Connection;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System.Threading;

namespace Greg.Xrm.Command.Commands.Script.MetadataExtractor
{
	public class ScriptMetadataExtractor(
		IOrganizationServiceRepository organizationServiceRepository,
		IScriptBuilder scriptBuilder) : IScriptMetadataExtractor
	{
		private RetrieveAllEntitiesResponse? cachedAllEntitiesResponse;
		private readonly SemaphoreSlim cachedAllEntitiesGate = new(1, 1);

		private async Task<RetrieveAllEntitiesResponse> GetAllEntitiesResponseAsync(CancellationToken cancellationToken = default)
		{
			if (cachedAllEntitiesResponse != null)
				return cachedAllEntitiesResponse;

			await cachedAllEntitiesGate.WaitAsync(cancellationToken).ConfigureAwait(false);
			try
			{
				if (cachedAllEntitiesResponse != null)
					return cachedAllEntitiesResponse;

				cachedAllEntitiesResponse = await LoadAllEntitiesResponseAsync(cancellationToken).ConfigureAwait(false);
				return cachedAllEntitiesResponse;
			}
			finally
			{
				cachedAllEntitiesGate.Release();
			}
		}

		private async Task<RetrieveAllEntitiesResponse> LoadAllEntitiesResponseAsync(CancellationToken cancellationToken)
		{
			try
			{
				var crm = await organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
				var request = new RetrieveAllEntitiesRequest
				{
					EntityFilters = EntityFilters.All,
					RetrieveAsIfPublished = false
				};
				return (RetrieveAllEntitiesResponse)await crm.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
			}
			catch
			{
				cachedAllEntitiesResponse = null;
				throw;
			}
		}



		public async Task<List<Extractor_EntityMetadata>> GetEntitiesByPrefixAsync(List<string> prefixes, CancellationToken cancellationToken = default)
		{
			var response = await GetAllEntitiesResponseAsync(cancellationToken).ConfigureAwait(false);
			return Extractor_EntityMetadata.ExtractEntitiesByPrefix(response.EntityMetadata, prefixes);
		}




		public async Task<List<Extractor_EntityMetadata>> GetEntitiesBySolutionAsync(string solutionName, List<string> prefixes, CancellationToken cancellationToken = default)
		{
			var crm = await organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			var query = new QueryExpression("solutioncomponent")
			{
				ColumnSet = new ColumnSet("objectid", "componenttype"),
				Criteria = new FilterExpression()
			};
			query.Criteria.AddCondition("solutionid", ConditionOperator.Equal, await GetSolutionIdAsync(crm, solutionName, cancellationToken).ConfigureAwait(false));
			query.Criteria.AddCondition("componenttype", ConditionOperator.Equal, 1); // 1 = Entity
			var solutionComponents = await crm.RetrieveMultipleAsync(query, cancellationToken).ConfigureAwait(false);
			var entityIds = solutionComponents.Entities.Select(e => (Guid)e["objectid"]).ToList();
			var response = await GetAllEntitiesResponseAsync(cancellationToken).ConfigureAwait(false);
			return Extractor_EntityMetadata.ExtractEntitiesBySolution(response.EntityMetadata, entityIds, prefixes);
		}




		private async Task<Guid> GetSolutionIdAsync(IOrganizationServiceAsync2 crm, string solutionName, CancellationToken cancellationToken)
		{
			var query = new QueryExpression("solution")
			{
				ColumnSet = new ColumnSet("solutionid"),
				Criteria = new FilterExpression()
			};
			query.Criteria.AddCondition("uniquename", ConditionOperator.Equal, solutionName);
			var solutions = await crm.RetrieveMultipleAsync(query, cancellationToken).ConfigureAwait(false);
			var solution = solutions.Entities.FirstOrDefault();
			if (solution == null) throw new InvalidOperationException($"Solution not found: {solutionName}");
			return (Guid)solution["solutionid"];
		}



		public async Task<Extractor_EntityMetadata?> GetTableAsync(string tableName, List<string> prefixes, CancellationToken cancellationToken = default)
		{
			var response = await GetAllEntitiesResponseAsync(cancellationToken).ConfigureAwait(false);
			var e = response.EntityMetadata.FirstOrDefault(x => x.LogicalName == tableName);
			if (e == null) return null;
			return Extractor_EntityMetadata.ExtractEntityByName([ e ], tableName, prefixes);
		}

		public async Task<List<Extractor_RelationshipMetadata>> GetRelationshipsAsync(List<string> prefixes, List<Extractor_EntityMetadata>? includedEntities = null, CancellationToken cancellationToken = default)
		{
			var response = await GetAllEntitiesResponseAsync(cancellationToken).ConfigureAwait(false);
			return Extractor_RelationshipMetadata.ExtractRelationships(response.EntityMetadata, prefixes, includedEntities);
		}

		public async Task<List<Extractor_OptionSetMetadata>> GetOptionSetsAsync(List<string>? entityFilter = null, CancellationToken cancellationToken = default)
		{
			var crm = await organizationServiceRepository.GetCurrentConnectionAsync(cancellationToken).ConfigureAwait(false);
			var globalRequest = new RetrieveAllOptionSetsRequest();
			var globalResponse = (RetrieveAllOptionSetsResponse)await crm.ExecuteAsync(globalRequest, cancellationToken).ConfigureAwait(false);
			var response = await GetAllEntitiesResponseAsync(cancellationToken).ConfigureAwait(false);
			var globalOptionSets = globalResponse.OptionSetMetadata.OfType<OptionSetMetadata>();
			var result = Extractor_OptionSetMetadata.ExtractOptionSets(response.EntityMetadata, globalOptionSets);
			if (entityFilter != null)
			{
				return result.Where(os => entityFilter.Contains(os.EntityName)).ToList();
			}
			return result;
		}

		public Task GenerateStateFieldsCSV(List<Extractor_OptionSetMetadata> optionSets, string outputFilePath)
		{
			scriptBuilder.GenerateOptionSetCsv(optionSets, outputFilePath);
			return Task.CompletedTask;
		}

		public string GeneratePacxScript(List<Extractor_EntityMetadata> entities, List<Extractor_RelationshipMetadata> relationships, List<string> prefixes)
		{
			return scriptBuilder.GeneratePacxScript(entities, relationships, prefixes);
		}
	}
}
