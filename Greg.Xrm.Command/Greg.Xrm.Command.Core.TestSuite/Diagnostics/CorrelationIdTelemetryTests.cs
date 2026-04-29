using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Greg.Xrm.Command.Diagnostics;
using Greg.Xrm.Command.Services;

namespace Greg.Xrm.Command.Core.TestSuite.Diagnostics
{
	[TestClass]
	public class CorrelationIdTelemetryTests
	{
		[TestMethod]
		public void AmbientCorrelationIdProviderShouldReturnANonEmptyCurrentValue()
		{
			var provider = new AmbientCorrelationIdProvider();

			Assert.IsFalse(string.IsNullOrWhiteSpace(provider.Current));
		}

		[TestMethod]
		public void AmbientCorrelationIdProviderShouldPreserveASetValue()
		{
			var provider = new AmbientCorrelationIdProvider();
			var correlationId = $"correlation-{Guid.NewGuid():N}";

			provider.Current = correlationId;

			Assert.AreEqual(correlationId, provider.Current);
		}

		[TestMethod]
		public async Task CorrelationIdHandlerShouldAddCorrelationHeadersToOutgoingRequests()
		{
			var correlationId = $"correlation-{Guid.NewGuid():N}";
			var provider = new AmbientCorrelationIdProvider
			{
				Current = correlationId
			};
			var recordingHandler = new RecordingHandler();
			using var handler = new TestableCorrelationIdHandler(provider, recordingHandler);
			using var client = new HttpMessageInvoker(handler);
			using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/api");

			await client.SendAsync(request, CancellationToken.None).ConfigureAwait(false);

			Assert.IsNotNull(recordingHandler.Request);
			Assert.AreEqual(correlationId, recordingHandler.Request!.Headers.GetValues("x-ms-client-request-id").Single());
			Assert.AreEqual(correlationId, recordingHandler.Request.Headers.GetValues("x-correlation-id").Single());
		}

		[TestMethod]
		public async Task CorrelationIdHandlerShouldPreservePreexistingCorrelationHeaders()
		{
			var correlationId = $"correlation-{Guid.NewGuid():N}";
			var existingClientRequestId = $"existing-ms-{Guid.NewGuid():N}";
			var existingCorrelationId = $"existing-correlation-{Guid.NewGuid():N}";
			var provider = new AmbientCorrelationIdProvider
			{
				Current = correlationId
			};
			var recordingHandler = new RecordingHandler();
			using var handler = new TestableCorrelationIdHandler(provider, recordingHandler);
			using var client = new HttpMessageInvoker(handler);
			using var request = new HttpRequestMessage(HttpMethod.Post, "https://example.com/api");

			request.Headers.Add("x-ms-client-request-id", existingClientRequestId);
			request.Headers.Add("x-correlation-id", existingCorrelationId);

			await client.SendAsync(request, CancellationToken.None).ConfigureAwait(false);

			Assert.IsNotNull(recordingHandler.Request);
			Assert.AreEqual(existingClientRequestId, recordingHandler.Request!.Headers.GetValues("x-ms-client-request-id").Single());
			Assert.AreEqual(existingCorrelationId, recordingHandler.Request.Headers.GetValues("x-correlation-id").Single());
		}

		private sealed class RecordingHandler : HttpMessageHandler
		{
			public HttpRequestMessage? Request { get; private set; }

			protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				this.Request = request;
				return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
			}
		}

		private sealed class TestableCorrelationIdHandler : CorrelationIdHandler
		{
			public TestableCorrelationIdHandler(ICorrelationIdProvider correlationIdProvider, HttpMessageHandler innerHandler)
				: base(correlationIdProvider)
			{
				this.InnerHandler = innerHandler;
			}
		}
	}
}
