using System.Net.Http.Headers;
using System.Text.Json;
using Greg.Xrm.Command.Services.Connection;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Services.AsyncJobPoller
{
    public class AsyncJobPoller(
        ITokenProvider tokenProvider,
        IHttpClientFactory httpClientFactory) : IAsyncJobPoller
    {
        private const string Resource = "https://management.azure.com";
        private const string BaseUrl = "https://management.azure.com";

        private readonly ITokenProvider tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        private readonly IHttpClientFactory httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

        public async Task<bool> WaitForOperationAsync(string operationLocation, TimeSpan timeout, TimeSpan pollingInterval, IOutput output, CancellationToken ct)
        {
            var startTime = DateTime.UtcNow;

            while (DateTime.UtcNow - startTime < timeout)
            {
                var result = await GetAsync(operationLocation, ct).ConfigureAwait(false);
                var status = GetOperationStatus(result);

                if (status == "Succeeded")
                    return true;
                if (status == "Failed" || status == "Canceled")
                {
                    output.WriteLine($"  Operation failed with status: {status}", ConsoleColor.Red);
                    return false;
                }

                var elapsed = DateTime.UtcNow - startTime;
                output.WriteLine($"  Operation status: {status} ({elapsed.Minutes}m {elapsed.Seconds}s elapsed)", ConsoleColor.Yellow);

                await Task.Delay(pollingInterval, ct).ConfigureAwait(false);
            }

            output.WriteLine("  Timeout reached while waiting for operation to complete.", ConsoleColor.Red);
            return false;
        }

        public async Task<bool> WaitForEnvironmentProvisioningAsync(string environmentId, TimeSpan timeout, TimeSpan pollingInterval, IOutput output, CancellationToken ct)
        {
            var startTime = DateTime.UtcNow;
            var path = $"/providers/Microsoft.BusinessAppPlatform/scopes/admin/environments/{environmentId}?api-version=2020-10-01";

            while (DateTime.UtcNow - startTime < timeout)
            {
                var result = await GetAsync(path, ct).ConfigureAwait(false);
                var state = GetProvisioningState(result);

                if (string.IsNullOrEmpty(state))
                {
                    output.WriteLine("  Unable to determine provisioning state from response.", ConsoleColor.Yellow);
                    return false;
                }

                if (state == "Succeeded")
                    return true;
                if (state == "Failed" || state == "Canceled")
                {
                    output.WriteLine($"  Environment provisioning failed with state: {state}", ConsoleColor.Red);
                    return false;
                }

                var elapsed = DateTime.UtcNow - startTime;
                output.WriteLine($"  Provisioning state: {state} ({elapsed.Minutes}m {elapsed.Seconds}s elapsed)", ConsoleColor.Yellow);

                await Task.Delay(pollingInterval, ct).ConfigureAwait(false);
            }

            output.WriteLine("  Timeout reached while waiting for environment provisioning.", ConsoleColor.Red);
            return false;
        }

        private static string? GetOperationStatus(JsonDocument doc)
        {
            if (doc.RootElement.TryGetProperty("status", out var status))
                return status.GetString();

            if (doc.RootElement.TryGetProperty("properties", out var props) && props.TryGetProperty("provisioningState", out var state))
                return state.GetString();

            return null;
        }

        private static string? GetProvisioningState(JsonDocument doc)
        {
            if (doc.RootElement.TryGetProperty("properties", out var props) && props.TryGetProperty("provisioningState", out var state))
                return state.GetString();

            return null;
        }

        private async Task<JsonDocument> GetAsync(string pathOrUrl, CancellationToken ct)
        {
            var token = await tokenProvider.GetTokenAsync(Resource, ct).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidOperationException("Unable to acquire a Power Platform Admin access token from the current connection.");
            }

            var requestUrl = pathOrUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? pathOrUrl
                : BaseUrl + (pathOrUrl.StartsWith("/", StringComparison.Ordinal) ? pathOrUrl : "/" + pathOrUrl);

            var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var client = httpClientFactory.CreateClient();
            using var response = await client.SendAsync(request, ct).ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Power Platform Admin API error ({response.StatusCode}): {content}");
            }

            return JsonDocument.Parse(string.IsNullOrWhiteSpace(content) ? "{}" : content);
        }
    }
}
