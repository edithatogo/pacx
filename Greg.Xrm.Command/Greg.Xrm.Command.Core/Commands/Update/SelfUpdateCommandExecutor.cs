using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Update
{
	public class SelfUpdateCommandExecutor(
		IOutput output,
		IHttpClientFactory httpClientFactory) : ICommandExecutor<SelfUpdateCommand>
	{
		private readonly IOutput output = output ?? throw new ArgumentNullException(nameof(output));
		private readonly IHttpClientFactory httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
		private const string GitHubApiUrl = "https://api.github.com/repos/edithatogo/Greg.Xrm.Command/releases";

		public async Task<CommandResult> ExecuteAsync(SelfUpdateCommand command, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var currentVersion = Assembly.GetEntryAssembly()?.GetName()?.Version;
			output.Write("Current version: ").WriteLine(currentVersion?.ToString() ?? "unknown", ConsoleColor.Cyan);

			try
			{
				var client = httpClientFactory.CreateClient(string.Empty);
				client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("PACX", currentVersion?.ToString() ?? "0.0.0.0"));

				var latestVersion = await FetchLatestVersionAsync(client, command.IncludePrerelease, cancellationToken);
				if (latestVersion == null)
				{
					output.WriteLine("Could not determine the latest version.", ConsoleColor.Yellow);
					return CommandResult.Success();
				}

				output.Write("Latest remote version: ").WriteLine(latestVersion.ToString(), ConsoleColor.Cyan);

				if (currentVersion != null && latestVersion <= currentVersion)
				{
					output.WriteLine("You are already using the latest version.", ConsoleColor.Green);
					return CommandResult.Success();
				}

				output.WriteLine($"A new version is available: {latestVersion}", ConsoleColor.Yellow);

				if (command.CheckOnly)
				{
					output.WriteLine("Run without --check to install the update.", ConsoleColor.Cyan);
					return CommandResult.Success();
				}

				return await DownloadAndInstallAsync(client, latestVersion, cancellationToken);
			}
			catch (OperationCanceledException)
			{
				return CommandResult.Fail("Update was cancelled.");
			}
			catch (HttpRequestException ex)
			{
				return CommandResult.Fail($"Failed to reach GitHub releases: {ex.Message}", ex);
			}
			catch (JsonException ex)
			{
				return CommandResult.Fail($"Failed to parse GitHub response: {ex.Message}", ex);
			}
			catch (Exception ex)
			{
				return CommandResult.Fail($"Update failed: {ex.Message}", ex);
			}
		}

		private static async Task<Version?> FetchLatestVersionAsync(HttpClient client, bool includePrerelease, CancellationToken cancellationToken)
		{
			var url = includePrerelease
				? $"{GitHubApiUrl}?per_page=20"
				: $"{GitHubApiUrl}/latest";

			var json = await client.GetStringAsync(url, cancellationToken);
			using var doc = JsonDocument.Parse(json);

			if (includePrerelease)
			{
				var root = doc.RootElement;
				if (root.ValueKind != JsonValueKind.Array)
					return null;

				foreach (var release in root.EnumerateArray())
				{
					if (release.TryGetProperty("prerelease", out var prerelease) && prerelease.GetBoolean())
						continue;

					// skip drafts
					if (release.TryGetProperty("draft", out var draft) && draft.GetBoolean())
						continue;

					var tagName = release.GetProperty("tag_name").GetString();
					var version = ParseVersion(tagName);
					if (version != null)
						return version;
				}

				return null;
			}

			var tag = doc.RootElement.GetProperty("tag_name").GetString();
			return ParseVersion(tag);
		}

		private async Task<CommandResult> DownloadAndInstallAsync(HttpClient client, Version targetVersion, CancellationToken cancellationToken)
		{
			var releaseUrl = $"{GitHubApiUrl}/tags/v{targetVersion}";
			string json;
			try
			{
				json = await client.GetStringAsync(releaseUrl, cancellationToken);
			}
			catch (HttpRequestException)
			{
				releaseUrl = $"{GitHubApiUrl}/tags/{targetVersion}";
				json = await client.GetStringAsync(releaseUrl, cancellationToken);
			}

			using var doc = JsonDocument.Parse(json);
			var assets = doc.RootElement.GetProperty("assets");

			string? downloadUrl = null;
			foreach (var asset in assets.EnumerateArray())
			{
				var name = asset.GetProperty("name").GetString();
				if (string.IsNullOrEmpty(name)) continue;

				if (name.EndsWith(".nupkg", StringComparison.OrdinalIgnoreCase))
				{
					downloadUrl = asset.GetProperty("browser_download_url").GetString();
					break;
				}
			}

			if (string.IsNullOrEmpty(downloadUrl))
			{
				return CommandResult.Fail("No .nupkg asset found in the release.");
			}

			output.Write("Downloading update...");

			var downloadBytes = await client.GetByteArrayAsync(downloadUrl, cancellationToken);
			var tempPath = Path.Combine(Path.GetTempPath(), $"pacx_update_{Guid.NewGuid():N}.nupkg");
			await File.WriteAllBytesAsync(tempPath, downloadBytes, cancellationToken);

			output.WriteLine(" done.").WriteLine($"Downloaded to: {tempPath}");

			var currentProcessPath = Assembly.GetEntryAssembly()?.Location;
			if (string.IsNullOrEmpty(currentProcessPath))
			{
				output.WriteLine("Update package saved. Install manually with: dotnet tool update --global greg.xrm.command --add-source", ConsoleColor.Yellow);
				return CommandResult.Success();
			}

			output.WriteLine("Installing update...");

			var psi = new System.Diagnostics.ProcessStartInfo
			{
				FileName = "dotnet",
				Arguments = $"tool update --global Greg.Xrm.Command --add-source \"{Path.GetDirectoryName(tempPath)}\" --ignore-failed-sources",
				UseShellExecute = false,
				CreateNoWindow = true
			};

			using var process = System.Diagnostics.Process.Start(psi);
			if (process == null)
			{
				return CommandResult.Fail("Failed to start dotnet tool update process.");
			}

			await process.WaitForExitAsync(cancellationToken);
			output.WriteLine("Update completed successfully.", ConsoleColor.Green);

			try { File.Delete(tempPath); } catch { /* cleanup best-effort */ }

			return CommandResult.Success();
		}

		internal static Version? ParseVersion(string? tagName)
		{
			if (string.IsNullOrEmpty(tagName)) return null;
			var versionStr = tagName.TrimStart('v', 'V');
			if (Version.TryParse(versionStr, out var version))
				return version;
			return null;
		}
	}
}
