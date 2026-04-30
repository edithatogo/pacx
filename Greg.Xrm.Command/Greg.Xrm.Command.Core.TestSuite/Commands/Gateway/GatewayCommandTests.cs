using System.Text.Json;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.PowerBi;

namespace Greg.Xrm.Command.Commands.Gateway
{
	[TestClass]
	public class GatewayCommandTests
	{
		[TestMethod]
		public async Task GatewayList_ShouldCallListGateways()
		{
			var client = new RecordingPowerBiClient(JsonDocument.Parse("""{"value":[{"id":"gw-1","name":"My Gateway"}]}"""));
			var output = new OutputToMemory();
			var executor = new GatewayListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new GatewayListCommand(), CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			StringAssert.Contains(output.ToString(), "gw-1");
		}

		[TestMethod]
		public async Task GatewayGet_ShouldCallGetGateway()
		{
			var client = new RecordingPowerBiClient(JsonDocument.Parse("""{"id":"gw-1","name":"My Gateway"}"""));
			var output = new OutputToMemory();
			var executor = new GatewayGetCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new GatewayGetCommand { GatewayId = "gw-1" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			StringAssert.Contains(output.ToString(), "gw-1");
		}

		[TestMethod]
		public async Task GatewayClientError_ShouldReturnFailResult()
		{
			var client = new ThrowingPowerBiClient(new InvalidOperationException("API error"));
			var output = new OutputToMemory();
			var executor = new GatewayListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new GatewayListCommand(), CancellationToken.None);

			Assert.IsFalse(result.IsSuccess);
			StringAssert.Contains(result.ErrorMessage, "API error");
		}

		private sealed class RecordingPowerBiClient(JsonDocument response) : IPowerBiClient
		{
			public string? LastMethod { get; private set; }
			public string? LastPath { get; private set; }

			public Task<JsonDocument> GetAsync(string path, CancellationToken cancellationToken)
			{
				LastMethod = "GET";
				LastPath = path;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> PostAsync(string path, object payload, CancellationToken cancellationToken)
			{
				LastMethod = "POST";
				LastPath = path;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> PostFileAsync(string path, string filePath, CancellationToken cancellationToken)
			{
				LastMethod = "POST";
				LastPath = path;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> DeleteAsync(string path, CancellationToken cancellationToken)
			{
				LastMethod = "DELETE";
				LastPath = path;
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> ListGatewaysAsync(CancellationToken cancellationToken)
			{
				LastMethod = "GET";
				LastPath = "gateways";
				return Task.FromResult(Clone(response));
			}

			public Task<JsonDocument> GetGatewayAsync(string gatewayId, CancellationToken cancellationToken)
			{
				LastMethod = "GET";
				LastPath = $"gateways/{gatewayId}";
				return Task.FromResult(Clone(response));
			}

			private static JsonDocument Clone(JsonDocument document)
				=> JsonDocument.Parse(document.RootElement.GetRawText());
		}

		private sealed class ThrowingPowerBiClient(Exception exception) : IPowerBiClient
		{
			public Task<JsonDocument> GetAsync(string path, CancellationToken cancellationToken) => throw exception;
			public Task<JsonDocument> PostAsync(string path, object payload, CancellationToken cancellationToken) => throw exception;
			public Task<JsonDocument> PostFileAsync(string path, string filePath, CancellationToken cancellationToken) => throw exception;
			public Task<JsonDocument> DeleteAsync(string path, CancellationToken cancellationToken) => throw exception;
			public Task<JsonDocument> ListGatewaysAsync(CancellationToken cancellationToken) => throw exception;
			public Task<JsonDocument> GetGatewayAsync(string gatewayId, CancellationToken cancellationToken) => throw exception;
		}
	}
}
