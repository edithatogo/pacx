using System.Net;
using System.Text.Json;
using Greg.Xrm.Command.Services.Connection;

namespace Greg.Xrm.Command.Services.PowerAutomate
{
	[TestClass]
	public class PowerAutomateClientTests
	{
		[TestMethod]
		public async Task ListFlowsAsync_ShouldBuildCorrectUrl_WithoutSharingFilter()
		{
			var handler = new RecordingHandler("""{"value":[{"name":"flow-1"}]}""");
			var client = CreateClient(handler);

			using var result = await client.ListFlowsAsync("MyEnv", null, false, false, CancellationToken.None);

			Assert.AreEqual("flow-1", result.RootElement.GetProperty("value")[0].GetProperty("name").GetString());
			var uri = handler.LastRequest?.RequestUri?.ToString();
			Assert.IsNotNull(uri);
			StringAssert.Contains(uri, "/providers/Microsoft.ProcessSimple/environments/MyEnv/flows");
			StringAssert.Contains(uri, "api-version=2016-11-01");
			Assert.IsFalse(uri!.Contains("$filter"));
			Assert.IsFalse(uri.Contains("include"));
		}

		[TestMethod]
		public async Task ListFlowsAsync_ShouldBuildCorrectUrl_WithSharingFilter()
		{
			var handler = new RecordingHandler("""{"value":[]}""");
			var client = CreateClient(handler);

			using var result = await client.ListFlowsAsync("MyEnv", "personal", false, false, CancellationToken.None);

			var uri = handler.LastRequest?.RequestUri?.ToString();
			Assert.IsNotNull(uri);
			StringAssert.Contains(uri, "$filter=search('personal')");
		}

		[TestMethod]
		public async Task ListFlowsAsync_ShouldIncludeWithSolutionsParameter()
		{
			var handler = new RecordingHandler("""{"value":[]}""");
			var client = CreateClient(handler);

			using var result = await client.ListFlowsAsync("MyEnv", null, true, false, CancellationToken.None);

			var uri = handler.LastRequest?.RequestUri?.ToString();
			Assert.IsNotNull(uri);
			StringAssert.Contains(uri, "include=includeSolutionCloudFlows");
		}

		[TestMethod]
		public async Task ListFlowsAsync_AsAdmin_ShouldUseAdminScope()
		{
			var handler = new RecordingHandler("""{"value":[]}""");
			var client = CreateClient(handler);

			using var result = await client.ListFlowsAsync("MyEnv", null, false, true, CancellationToken.None);

			var uri = handler.LastRequest?.RequestUri?.ToString();
			Assert.IsNotNull(uri);
			StringAssert.Contains(uri, "/scopes/admin/environments/MyEnv/v2/flows");
		}

		[TestMethod]
		public async Task GetFlowAsync_ShouldBuildCorrectUrl()
		{
			var handler = new RecordingHandler("""{"name":"flow-1"}""");
			var client = CreateClient(handler);

			using var result = await client.GetFlowAsync("MyEnv", "flow-1", CancellationToken.None);

			var uri = handler.LastRequest?.RequestUri?.ToString();
			Assert.IsNotNull(uri);
			StringAssert.Contains(uri, "/environments/MyEnv/flows/flow-1");
			StringAssert.Contains(uri, "api-version=2016-11-01");
			Assert.AreEqual(HttpMethod.Get, handler.LastRequest?.Method);
		}

		[TestMethod]
		public async Task EnableFlowAsync_ShouldUsePostWithStartEndpoint()
		{
			var handler = new RecordingHandler("{}");
			var client = CreateClient(handler);

			using var result = await client.EnableFlowAsync("MyEnv", "flow-1", false, CancellationToken.None);

			var uri = handler.LastRequest?.RequestUri?.ToString();
			Assert.IsNotNull(uri);
			StringAssert.Contains(uri, "/environments/MyEnv/flows/flow-1/start");
			Assert.AreEqual(HttpMethod.Post, handler.LastRequest?.Method);
		}

		[TestMethod]
		public async Task DisableFlowAsync_ShouldUsePostWithStopEndpoint()
		{
			var handler = new RecordingHandler("{}");
			var client = CreateClient(handler);

			using var result = await client.DisableFlowAsync("MyEnv", "flow-1", false, CancellationToken.None);

			var uri = handler.LastRequest?.RequestUri?.ToString();
			Assert.IsNotNull(uri);
			StringAssert.Contains(uri, "/environments/MyEnv/flows/flow-1/stop");
			Assert.AreEqual(HttpMethod.Post, handler.LastRequest?.Method);
		}

		[TestMethod]
		public async Task DeleteFlowAsync_ShouldUseDeleteMethod()
		{
			var handler = new RecordingHandler("");
			var client = CreateClient(handler);

			await client.DeleteFlowAsync("MyEnv", "flow-1", false, CancellationToken.None);

			var uri = handler.LastRequest?.RequestUri?.ToString();
			Assert.IsNotNull(uri);
			StringAssert.Contains(uri, "/environments/MyEnv/flows/flow-1");
			Assert.AreEqual(HttpMethod.Delete, handler.LastRequest?.Method);
		}

		[TestMethod]
		public async Task EnableFlowAsync_AsAdmin_ShouldUseAdminScope()
		{
			var handler = new RecordingHandler("{}");
			var client = CreateClient(handler);

			using var result = await client.EnableFlowAsync("MyEnv", "flow-1", true, CancellationToken.None);

			var uri = handler.LastRequest?.RequestUri?.ToString();
			Assert.IsNotNull(uri);
			StringAssert.Contains(uri, "/scopes/admin/environments/MyEnv/flows/flow-1/start");
		}

		[TestMethod]
		public async Task DeleteFlowAsync_AsAdmin_ShouldUseAdminScope()
		{
			var handler = new RecordingHandler("");
			var client = CreateClient(handler);

			await client.DeleteFlowAsync("MyEnv", "flow-1", true, CancellationToken.None);

			var uri = handler.LastRequest?.RequestUri?.ToString();
			Assert.IsNotNull(uri);
			StringAssert.Contains(uri, "/scopes/admin/environments/MyEnv/flows/flow-1");
		}

		[TestMethod]
		public async Task ShouldAttachBearerToken()
		{
			var handler = new RecordingHandler("""{"value":[]}""");
			var client = CreateClient(handler);

			using var result = await client.ListFlowsAsync("MyEnv", null, false, false, CancellationToken.None);

			Assert.AreEqual("Bearer", handler.LastRequest?.Headers.Authorization?.Scheme);
			Assert.AreEqual("test-token", handler.LastRequest?.Headers.Authorization?.Parameter);
		}

		[TestMethod]
		public async Task WhenApiReturnsError_ShouldThrow()
		{
			var handler = new RecordingHandler("", HttpStatusCode.InternalServerError);
			var client = CreateClient(handler);

			try
			{
				await client.ListFlowsAsync("MyEnv", null, false, false, CancellationToken.None);
				Assert.Fail("Expected InvalidOperationException was not thrown.");
			}
			catch (InvalidOperationException ex)
			{
				StringAssert.Contains(ex.Message, "InternalServerError");
			}
		}

		private static PowerAutomateClient CreateClient(HttpMessageHandler handler)
		{
			return new PowerAutomateClient(
				new StaticTokenProvider("test-token"),
				new StaticHttpClientFactory(new HttpClient(handler)));
		}

		private sealed class StaticTokenProvider(string token) : ITokenProvider
		{
			public Task<string?> GetTokenAsync(string resource, CancellationToken cancellationToken = default)
			{
				Assert.AreEqual(PowerAutomateClient.Resource, resource);
				return Task.FromResult<string?>(token);
			}
		}

		private sealed class StaticHttpClientFactory(HttpClient client) : IHttpClientFactory
		{
			public HttpClient CreateClient(string name) => client;
		}

		private sealed class RecordingHandler(string responseContent, HttpStatusCode statusCode = HttpStatusCode.OK) : HttpMessageHandler
		{
			public HttpRequestMessage? LastRequest { get; private set; }

			protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				LastRequest = request;
				return Task.FromResult(new HttpResponseMessage(statusCode)
				{
					Content = new StringContent(responseContent)
				});
			}
		}
	}
}
