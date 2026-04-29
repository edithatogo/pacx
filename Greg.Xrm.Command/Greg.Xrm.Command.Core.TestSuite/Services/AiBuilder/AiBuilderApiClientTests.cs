using Greg.Xrm.Command.Services.AiBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Core.TestSuite.Services.AiBuilder
{
	[TestClass]
	public class AiBuilderApiClientTests
	{
		[TestMethod]
		public void BuildAiBuilderUrl_ShouldUsePowerPlatformRouteForDynamicsOrg()
		{
			var result = AiBuilderApiClient.BuildAiBuilderUrl("https://contoso.crm.dynamics.com");

			Assert.AreEqual("https://contoso.api.crm.powerplatform.com/aiBuilder", result);
		}

		[TestMethod]
		public void BuildAiBuilderUrl_ShouldUseFallbackRouteForNonDynamicsHost()
		{
			var result = AiBuilderApiClient.BuildAiBuilderUrl("https://example.org/base");

			Assert.AreEqual("https://example.org/base/api/aiBuilder", result);
		}

		[TestMethod]
		public async Task SendWithRetryAsync_ShouldRetryTransientResponses()
		{
			var delays = new List<TimeSpan>();
			var handler = new SequenceHandler(
				new HttpResponseMessage((HttpStatusCode)429),
				new HttpResponseMessage(HttpStatusCode.OK)
				{
					Content = new StringContent("ok")
				});

			using var client = new HttpClient(handler);

			var response = await AiBuilderApiClient.SendWithRetryAsync(
				client,
				HttpMethod.Post,
				"https://example.org/api",
				"token",
				null,
				(delay, ct) =>
				{
					delays.Add(delay);
					return Task.CompletedTask;
				},
				CancellationToken.None);

			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
			Assert.AreEqual(2, handler.CallCount);
			Assert.AreEqual(1, delays.Count);
			Assert.IsTrue(delays[0] >= TimeSpan.FromSeconds(1));
		}

		[TestMethod]
		public void GetRetryDelay_ShouldUseRetryAfterHeader()
		{
			var response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
			response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromSeconds(17));

			var delay = AiBuilderApiClient.GetRetryDelay(response, 1);

			Assert.AreEqual(TimeSpan.FromSeconds(17), delay);
		}

		private sealed class SequenceHandler : HttpMessageHandler
		{
			private readonly Queue<HttpResponseMessage> responses;

			public SequenceHandler(params HttpResponseMessage[] responses)
			{
				this.responses = new Queue<HttpResponseMessage>(responses);
			}

			public int CallCount { get; private set; }

			protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				CallCount++;

				if (responses.Count == 0)
				{
					return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
				}

				return Task.FromResult(responses.Dequeue());
			}
		}
	}
}
