using System.Threading;
using Greg.Xrm.Command.Model;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Greg.Xrm.Command.Services.Plugin
{
	public class SdkMessage : EntityWrapper
	{
		protected SdkMessage(Entity entity) : base(entity)
		{
		}

		public string name
		{
			get => Get<string>();
			set => SetValue(value);
		}

		public SdkMessageFilter[] Filters { get; private set; } = [];

		public SdkMessageFilter? GetFilter(string tableName)
		{
			return Filters.FirstOrDefault(x => string.Equals(x.primaryobjecttypecode, tableName, StringComparison.OrdinalIgnoreCase));
		}



		public class Repository : ISdkMessageRepository
		{
			public async Task<SdkMessage[]> GetAllAsync(IOrganizationServiceAsync2 crm, CancellationToken cancellationToken = default)
			{
				var query = new QueryExpression("sdkmessage");
				query.ColumnSet.AddColumns("name");

				var response = await crm.RetrieveMultipleAsync(query, cancellationToken).ConfigureAwait(false);
				return response.Entities.Select(e => new SdkMessage(e)).ToArray();
			}

			public async Task<SdkMessage?> GetByNameAsync(IOrganizationServiceAsync2 crm, string name, CancellationToken cancellationToken = default)
			{
				var query = new QueryExpression("sdkmessage");
				query.ColumnSet.AddColumns("name");
				query.Criteria.AddCondition("name", ConditionOperator.Equal, name);
				query.TopCount = 1;

				var response = await crm.RetrieveMultipleAsync(query, cancellationToken).ConfigureAwait(false);
				var sdkMessage = response.Entities.Select(e => new SdkMessage(e)).FirstOrDefault();
				if (sdkMessage == null) return null;


				var query2 = new QueryExpression("sdkmessagefilter");
				query2.ColumnSet.AddColumns("primaryobjecttypecode");
				query2.Criteria.AddCondition("sdkmessageid", ConditionOperator.Equal, sdkMessage.Id);

				var response2 = await crm.RetrieveMultipleAsync(query2, cancellationToken).ConfigureAwait(false);
				var tableNames = response2.Entities
					.Select(e => new SdkMessageFilter(e))
					.Where(x => !"none".Equals(x.primaryobjecttypecode, StringComparison.OrdinalIgnoreCase))
					.ToArray();

				sdkMessage.Filters = tableNames;
				return sdkMessage;
			}
		}
	}
}
