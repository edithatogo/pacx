using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Greg.Xrm.Command.Diagnostics;

namespace Greg.Xrm.Command.Services
{
	/// <summary>
	/// A <see cref="DelegatingHandler"/> that adds correlation ID headers to every outgoing request.
	/// </summary>
	public class CorrelationIdHandler(ICorrelationIdProvider correlationIdProvider) : DelegatingHandler
	{
		private readonly ICorrelationIdProvider _correlationIdProvider = correlationIdProvider ?? throw new ArgumentNullException(nameof(correlationIdProvider));

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var correlationId = _correlationIdProvider.Current;

			// Add standard correlation headers
			if (!request.Headers.Contains("x-ms-client-request-id"))
			{
				request.Headers.Add("x-ms-client-request-id", correlationId);
			}

			if (!request.Headers.Contains("x-correlation-id"))
			{
				request.Headers.Add("x-correlation-id", correlationId);
			}

			return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
		}
	}
}
