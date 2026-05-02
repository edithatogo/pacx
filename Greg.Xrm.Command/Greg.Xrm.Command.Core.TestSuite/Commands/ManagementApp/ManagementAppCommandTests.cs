using System.Text.Json;
using Greg.Xrm.Command.Services.Output;
using Greg.Xrm.Command.Services.PowerPlatformAdmin;

namespace Greg.Xrm.Command.Commands.ManagementApp
{
	[TestClass]
	public class ManagementAppCommandTests
	{
		[TestMethod]
		public async Task ManagementAppList_ShouldCallListManagementApps()
		{
			var client = new RecordingAdminClient(JsonDocument.Parse("""{"value":[]}"""));
			var output = new OutputToMemory();
			var executor = new ManagementAppListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new ManagementAppListCommand(), CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
		}

		[TestMethod]
		public async Task ManagementAppList_Error_ShouldReturnFailResult()
		{
			var client = new ThrowingAdminClient(new InvalidOperationException("API error"));
			var output = new OutputToMemory();
			var executor = new ManagementAppListCommandExecutor(client, output);

			var result = await executor.ExecuteAsync(new ManagementAppListCommand(), CancellationToken.None);

			Assert.IsFalse(result.IsSuccess);
			StringAssert.Contains(result.ErrorMessage, "API error");
		}

		private sealed class RecordingAdminClient(JsonDocument response) : IPowerPlatformAdminClient
		{
			public Task<JsonDocument> ListManagementAppsAsync(CancellationToken ct) => Task.FromResult(JsonDocument.Parse(response.RootElement.GetRawText()));
			public Task<JsonDocument> GetTenantSettingsAsync(CancellationToken ct) => Task.FromResult(JsonDocument.Parse(response.RootElement.GetRawText()));
			public Task SetTenantSettingsAsync(object settings, CancellationToken ct) => Task.CompletedTask;
			public Task<JsonDocument> CreateEnvironmentAsync(string n, string t, string r, string c, string l, CancellationToken ct) => Task.FromResult(response);
			public Task<JsonDocument> GetEnvironmentAsync(string id, CancellationToken ct) => Task.FromResult(response);
			public Task<JsonDocument> ListEnvironmentsAsync(CancellationToken ct) => Task.FromResult(response);
			public Task<JsonDocument> ResetEnvironmentAsync(string id, string rt, CancellationToken ct) => Task.FromResult(response);
			public Task<JsonDocument> CopyEnvironmentAsync(string s, string tn, string m, CancellationToken ct) => Task.FromResult(response);
			public Task<JsonDocument> BackupEnvironmentAsync(string id, string l, CancellationToken ct) => Task.FromResult(response);
			public Task<JsonDocument> RestoreEnvironmentAsync(string id, string b, CancellationToken ct) => Task.FromResult(response);
			public Task<JsonDocument> GetEnvironmentCapacityAsync(string id, CancellationToken ct) => Task.FromResult(response);
		}

		private sealed class ThrowingAdminClient(Exception exception) : IPowerPlatformAdminClient
		{
			public Task<JsonDocument> ListManagementAppsAsync(CancellationToken ct) => throw exception;
			public Task<JsonDocument> GetTenantSettingsAsync(CancellationToken ct) => throw exception;
			public Task SetTenantSettingsAsync(object settings, CancellationToken ct) => throw exception;
			public Task<JsonDocument> CreateEnvironmentAsync(string n, string t, string r, string c, string l, CancellationToken ct) => throw exception;
			public Task<JsonDocument> GetEnvironmentAsync(string id, CancellationToken ct) => throw exception;
			public Task<JsonDocument> ListEnvironmentsAsync(CancellationToken ct) => throw exception;
			public Task<JsonDocument> ResetEnvironmentAsync(string id, string rt, CancellationToken ct) => throw exception;
			public Task<JsonDocument> CopyEnvironmentAsync(string s, string tn, string m, CancellationToken ct) => throw exception;
			public Task<JsonDocument> BackupEnvironmentAsync(string id, string l, CancellationToken ct) => throw exception;
			public Task<JsonDocument> RestoreEnvironmentAsync(string id, string b, CancellationToken ct) => throw exception;
			public Task<JsonDocument> GetEnvironmentCapacityAsync(string id, CancellationToken ct) => throw exception;
		}
	}
}
