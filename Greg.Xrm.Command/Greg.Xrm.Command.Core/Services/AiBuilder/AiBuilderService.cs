using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Services.AiBuilder
{
	public sealed class AiBuilderService(IAiBuilderApiClientFactory aiBuilderApiClientFactory) : IAiBuilderService
	{
		private readonly IAiBuilderApiClientFactory aiBuilderApiClientFactory = aiBuilderApiClientFactory ?? throw new ArgumentNullException(nameof(aiBuilderApiClientFactory));

		public async Task<AiBuilderOperationResult<IReadOnlyList<AiModelInfo>>> ListModelsAsync(CancellationToken cancellationToken = default)
		{
			try
			{
				var client = await this.aiBuilderApiClientFactory.CreateAsync(cancellationToken).ConfigureAwait(false);
				var models = (await client.ListModelsAsync(cancellationToken).ConfigureAwait(false)).ToList();
				return new AiBuilderOperationResult<IReadOnlyList<AiModelInfo>>(true, models, null, null);
			}
			catch (Exception ex)
			{
				return new AiBuilderOperationResult<IReadOnlyList<AiModelInfo>>(false, default, $"AI model list error: {ex.Message}", ex);
			}
		}

		public async Task<AiBuilderOperationResult> TrainModelWithRetryAsync(
			string modelId,
			bool wait,
			TimeSpan pollInterval,
			TimeSpan timeout,
			CancellationToken cancellationToken = default)
		{
			try
			{
				var client = await this.aiBuilderApiClientFactory.CreateAsync(cancellationToken).ConfigureAwait(false);
				await client.TrainModelAsync(modelId, wait, pollInterval, timeout, cancellationToken).ConfigureAwait(false);
				return AiBuilderOperationResult.Success();
			}
			catch (Exception ex)
			{
				return AiBuilderOperationResult.Fail($"AI model train error: {ex.Message}", ex);
			}
		}

		public async Task<AiBuilderOperationResult> PublishModelAsync(string modelId, CancellationToken cancellationToken = default)
		{
			try
			{
				var client = await this.aiBuilderApiClientFactory.CreateAsync(cancellationToken).ConfigureAwait(false);
				await client.PublishModelAsync(modelId, cancellationToken).ConfigureAwait(false);
				return AiBuilderOperationResult.Success();
			}
			catch (Exception ex)
			{
				return AiBuilderOperationResult.Fail($"AI model publish error: {ex.Message}", ex);
			}
		}

		public async Task<AiBuilderOperationResult> ConfigureFormProcessorAsync(
			string modelId,
			string documentType,
			string[]? fields,
			string[]? tables,
			CancellationToken cancellationToken = default)
		{
			try
			{
				var client = await this.aiBuilderApiClientFactory.CreateAsync(cancellationToken).ConfigureAwait(false);
				await client.ConfigureFormProcessorAsync(modelId, documentType, fields, tables, cancellationToken).ConfigureAwait(false);
				return AiBuilderOperationResult.Success();
			}
			catch (Exception ex)
			{
				return AiBuilderOperationResult.Fail($"Form processor configuration error: {ex.Message}", ex);
			}
		}
	}
}
