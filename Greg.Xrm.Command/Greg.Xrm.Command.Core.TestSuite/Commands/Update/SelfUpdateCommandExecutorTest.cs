using System.Net;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Update
{
	[TestClass]
	public class SelfUpdateCommandExecutorTest
	{
		[TestMethod]
		public void ParseVersionShouldHandleVPrefix()
		{
			var version = SelfUpdateCommandExecutor.ParseVersion("v1.2.3");
			Assert.IsNotNull(version);
			Assert.AreEqual(1, version.Major);
			Assert.AreEqual(2, version.Minor);
			Assert.AreEqual(3, version.Build);
		}

		[TestMethod]
		public void ParseVersionShouldHandleNoPrefix()
		{
			var version = SelfUpdateCommandExecutor.ParseVersion("2.0.1");
			Assert.IsNotNull(version);
			Assert.AreEqual(2, version.Major);
			Assert.AreEqual(0, version.Minor);
			Assert.AreEqual(1, version.Build);
		}

		[TestMethod]
		public void ParseVersionShouldHandleNull()
		{
			Assert.IsNull(SelfUpdateCommandExecutor.ParseVersion(null));
		}

		[TestMethod]
		public void ParseVersionShouldHandleEmptyString()
		{
			Assert.IsNull(SelfUpdateCommandExecutor.ParseVersion(string.Empty));
		}

		[TestMethod]
		public void ParseVersionShouldHandleInvalidString()
		{
			Assert.IsNull(SelfUpdateCommandExecutor.ParseVersion("not-a-version"));
		}

		[TestMethod]
		public void ExecuteShouldReturnSuccessWhenUpToDate()
		{
			var json = """
			{
				"tag_name": "v0.0.1",
				"assets": []
			}
			""";

			var handler = new MockHttpMessageHandler(json, HttpStatusCode.OK);
			var httpClient = new HttpClient(handler);
			var httpClientFactory = new MockHttpClientFactory(httpClient);
			var output = new OutputToMemory();

			var executor = new SelfUpdateCommandExecutor(output, httpClientFactory);
			var command = new SelfUpdateCommand { CheckOnly = true };
			var result = executor.ExecuteAsync(command, CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsTrue(result.IsSuccess);
		}

		[TestMethod]
		public void ExecuteShouldWarnWhenNewerVersionAvailableInCheckMode()
		{
			var json = """
			{
				"tag_name": "v99.99.99",
				"assets": []
			}
			""";

			var handler = new MockHttpMessageHandler(json, HttpStatusCode.OK);
			var httpClient = new HttpClient(handler);
			var httpClientFactory = new MockHttpClientFactory(httpClient);
			var output = new OutputToMemory();

			var executor = new SelfUpdateCommandExecutor(output, httpClientFactory);
			var command = new SelfUpdateCommand { CheckOnly = true };
			var result = executor.ExecuteAsync(command, CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsTrue(result.IsSuccess);
		}

		[TestMethod]
		public void ExecuteShouldHandleHttpError()
		{
			var handler = new MockHttpMessageHandler("Not Found", HttpStatusCode.NotFound);
			var httpClient = new HttpClient(handler);
			var httpClientFactory = new MockHttpClientFactory(httpClient);
			var output = new OutputToMemory();

			var executor = new SelfUpdateCommandExecutor(output, httpClientFactory);
			var command = new SelfUpdateCommand();
			var result = executor.ExecuteAsync(command, CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsFalse(result.IsSuccess);
		}
	}

	public class MockHttpMessageHandler : HttpMessageHandler
	{
		private readonly string responseContent;
		private readonly HttpStatusCode statusCode;

		public MockHttpMessageHandler(string responseContent, HttpStatusCode statusCode)
		{
			this.responseContent = responseContent;
			this.statusCode = statusCode;
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return Task.FromResult(new HttpResponseMessage
			{
				StatusCode = statusCode,
				Content = new StringContent(responseContent)
			});
		}
	}

	public class MockHttpClientFactory : IHttpClientFactory
	{
		private readonly HttpClient httpClient;

		public MockHttpClientFactory(HttpClient httpClient)
		{
			this.httpClient = httpClient;
		}

		public HttpClient CreateClient(string name)
		{
			return httpClient;
		}
	}
}
