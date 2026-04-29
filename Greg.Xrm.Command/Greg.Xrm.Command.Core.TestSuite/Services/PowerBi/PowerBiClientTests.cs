using System.Net;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.PowerBi;

namespace Greg.Xrm.Command.Services.PowerBi
{
	[TestClass]
	public class PowerBiClientTests
	{
		[TestMethod]
		public async Task GetAsync_ShouldAttachPowerBiTokenAndCallApi()
		{
			var handler = new RecordingHandler("""{"value":[{"id":"dataset-1"}]}""");
			var client = new PowerBiClient(new StaticTokenProvider("token-1"), new StaticHttpClientFactory(new HttpClient(handler)));

			using var result = await client.GetAsync("/groups/workspace-1/datasets", CancellationToken.None);

			Assert.AreEqual("dataset-1", result.RootElement.GetProperty("value")[0].GetProperty("id").GetString());
			Assert.AreEqual(HttpMethod.Get, handler.LastRequest?.Method);
			Assert.AreEqual("https://api.powerbi.com/v1.0/myorg/groups/workspace-1/datasets", handler.LastRequest?.RequestUri?.ToString());
			Assert.AreEqual("Bearer", handler.LastRequest?.Headers.Authorization?.Scheme);
			Assert.AreEqual("token-1", handler.LastRequest?.Headers.Authorization?.Parameter);
		}

		private sealed class StaticTokenProvider(string token) : ITokenProvider
		{
			public Task<string?> GetTokenAsync(string resource, CancellationToken cancellationToken = default)
			{
				Assert.AreEqual(PowerBiClient.Resource, resource);
				return Task.FromResult<string?>(token);
			}
		}

		private sealed class StaticHttpClientFactory(HttpClient client) : IHttpClientFactory
		{
			public HttpClient CreateClient(string name) => client;
		}

		private sealed class RecordingHandler(string responseContent) : HttpMessageHandler
		{
			public HttpRequestMessage? LastRequest { get; private set; }

			protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				this.LastRequest = request;
				return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
				{
					Content = new StringContent(responseContent)
				});
			}
		}
	}
}
