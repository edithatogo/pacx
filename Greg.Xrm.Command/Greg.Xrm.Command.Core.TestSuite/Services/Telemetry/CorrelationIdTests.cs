using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Greg.Xrm.Command.Diagnostics;
using Greg.Xrm.Command.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Greg.Xrm.Command.Core.TestSuite.Services.Telemetry
{
	[TestClass]
	public class CorrelationIdTests
	{
		[TestMethod]
		public void AmbientCorrelationIdProviderShouldGenerateAndPreserveValue()
		{
			var provider = new AmbientCorrelationIdProvider();

			try
			{
				var correlationId = provider.Current;

				Assert.IsFalse(string.IsNullOrWhiteSpace(correlationId));
				Assert.AreEqual(correlationId, provider.Current);

				var explicitCorrelationId = Guid.NewGuid().ToString("N");
				provider.Current = explicitCorrelationId;

				Assert.AreEqual(explicitCorrelationId, provider.Current);
			}
			finally
			{
				provider.Current = null!;
			}
		}

		[TestMethod]
		public async Task CorrelationIdHandlerShouldAddHeadersWhenMissing()
		{
			var provider = new AmbientCorrelationIdProvider();
			var correlationId = Guid.NewGuid().ToString("N");
			provider.Current = correlationId;

			var handler = new CorrelationIdHandler(provider)
			{
				InnerHandler = new RecordingHandler()
			};
			using var client = new HttpClient(handler);

			await client.GetAsync("https://example.org/test").ConfigureAwait(false);

			var request = ((RecordingHandler)handler.InnerHandler!).CapturedRequest;
			Assert.IsNotNull(request);
			Assert.IsTrue(request!.Headers.TryGetValues("x-ms-client-request-id", out var clientRequestIds));
			Assert.IsTrue(request.Headers.TryGetValues("x-correlation-id", out var correlationIds));
			Assert.AreEqual(correlationId, clientRequestIds.Single());
			Assert.AreEqual(correlationId, correlationIds.Single());
		}

		[TestMethod]
		public async Task CorrelationIdHandlerShouldPreserveExistingHeaders()
		{
			var provider = new AmbientCorrelationIdProvider();
			provider.Current = Guid.NewGuid().ToString("N");

			var handler = new CorrelationIdHandler(provider)
			{
				InnerHandler = new RecordingHandler()
			};
			using var client = new HttpClient(handler);

			var request = new HttpRequestMessage(HttpMethod.Get, "https://example.org/test");
			request.Headers.Add("x-ms-client-request-id", "existing-client");
			request.Headers.Add("x-correlation-id", "existing-correlation");

			await client.SendAsync(request).ConfigureAwait(false);

			var captured = ((RecordingHandler)handler.InnerHandler!).CapturedRequest;
			Assert.IsNotNull(captured);
			Assert.IsTrue(captured!.Headers.TryGetValues("x-ms-client-request-id", out var clientRequestIds));
			Assert.IsTrue(captured.Headers.TryGetValues("x-correlation-id", out var correlationIds));
			Assert.AreEqual("existing-client", clientRequestIds.Single());
			Assert.AreEqual("existing-correlation", correlationIds.Single());
		}

		private sealed class RecordingHandler : HttpMessageHandler
		{
			public HttpRequestMessage? CapturedRequest { get; private set; }

			protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				CapturedRequest = request;
				return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
			}
		}
	}
}
