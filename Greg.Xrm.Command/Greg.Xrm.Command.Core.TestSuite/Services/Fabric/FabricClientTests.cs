using System.Net;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Fabric;

namespace Greg.Xrm.Command.Services.Fabric
{
	[TestClass]
	public class FabricClientTests
	{
		[TestMethod]
		public async Task GetAsync_ShouldAttachFabricTokenAndCallApi()
		{
			var handler = new RecordingHandler("""{"value":[{"id":"workspace-1"}]}""");
			var client = new FabricClient(new StaticTokenProvider("token-1"), new StaticHttpClientFactory(new HttpClient(handler)));

			using var result = await client.GetAsync("/workspaces", CancellationToken.None);

			Assert.AreEqual("workspace-1", result.RootElement.GetProperty("value")[0].GetProperty("id").GetString());
			Assert.AreEqual(HttpMethod.Get, handler.LastRequest?.Method);
			Assert.AreEqual("https://api.fabric.microsoft.com/v1/workspaces", handler.LastRequest?.RequestUri?.ToString());
			Assert.AreEqual("Bearer", handler.LastRequest?.Headers.Authorization?.Scheme);
			Assert.AreEqual("token-1", handler.LastRequest?.Headers.Authorization?.Parameter);
		}

		private sealed class StaticTokenProvider(string token) : ITokenProvider
		{
			public Task<string?> GetTokenAsync(string resource, CancellationToken cancellationToken = default)
			{
				Assert.AreEqual(FabricClient.Resource, resource);
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
