using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Services.AiBuilder
{
	public interface IAiBuilderService
	{
		Task<AiBuilderOperationResult<IReadOnlyList<AiModelInfo>>> ListModelsAsync(CancellationToken cancellationToken = default);

		Task<AiBuilderOperationResult> TrainModelWithRetryAsync(
			string modelId,
			bool wait,
			TimeSpan pollInterval,
			TimeSpan timeout,
			CancellationToken cancellationToken = default);

		Task<AiBuilderOperationResult> PublishModelAsync(string modelId, CancellationToken cancellationToken = default);

		Task<AiBuilderOperationResult> ConfigureFormProcessorAsync(
			string modelId,
			string documentType,
			string[]? fields,
			string[]? tables,
			CancellationToken cancellationToken = default);
	}
}
