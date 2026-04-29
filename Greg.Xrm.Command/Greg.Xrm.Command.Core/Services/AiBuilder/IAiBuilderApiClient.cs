using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Greg.Xrm.Command.Services.AiBuilder
{
	public interface IAiBuilderApiClient
	{
		Task<IEnumerable<AiModelInfo>> ListModelsAsync(CancellationToken ct = default);
		Task TrainModelAsync(string modelId, bool wait, CancellationToken ct = default);
		Task TrainModelAsync(string modelId, bool wait, TimeSpan pollInterval, TimeSpan timeout, CancellationToken ct = default);
		Task PublishModelAsync(string modelId, CancellationToken ct = default);
		Task ConfigureFormProcessorAsync(string modelId, string documentType, string[]? fields, string[]? tables, CancellationToken ct = default);
	}
}
