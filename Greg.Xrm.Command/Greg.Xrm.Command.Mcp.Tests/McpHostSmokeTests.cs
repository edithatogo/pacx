using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Greg.Xrm.Command.Mcp.Tests;

[TestClass]
public class McpHostSmokeTests
{
	[TestMethod]
	[Timeout(30000)]
	public async Task McpHostShouldListAndExecuteToolsOverStdio()
	{
		var host = new McpHostProcess();
		try
		{
			await host.StartAsync();

			var initializeResponse = await host.RequestAsync(new
			{
				jsonrpc = "2.0",
				id = 1,
				method = "initialize",
				@params = new
				{
					protocolVersion = "2024-11-05",
					capabilities = new { },
					clientInfo = new { name = "pacx-test-suite", version = "1.0.0" },
				},
			});

			Assert.AreEqual("2.0", initializeResponse.RootElement.GetProperty("jsonrpc").GetString());
			Assert.AreEqual(1, initializeResponse.RootElement.GetProperty("id").GetInt32());
			Assert.IsTrue(initializeResponse.RootElement.TryGetProperty("result", out _));

			await host.NotifyAsync(new
			{
				jsonrpc = "2.0",
				method = "notifications/initialized",
			});

			var listResponse = await host.RequestAsync(new
			{
				jsonrpc = "2.0",
				id = 2,
				method = "tools/list",
			});

			var tools = listResponse.RootElement.GetProperty("result").GetProperty("tools");
			Assert.IsTrue(tools.EnumerateArray().Any(tool => string.Equals(tool.GetProperty("name").GetString(), "nop", StringComparison.OrdinalIgnoreCase)));

			var callResponse = await host.RequestAsync(new
			{
				jsonrpc = "2.0",
				id = 3,
				method = "tools/call",
				@params = new
				{
					name = "nop",
					arguments = new { },
				},
			});

			var callResult = callResponse.RootElement.GetProperty("result");
			Assert.IsFalse(callResult.GetProperty("isError").GetBoolean());

			var text = string.Join(Environment.NewLine,
				callResult.GetProperty("content").EnumerateArray()
					.Select(block => block.GetProperty("text").GetString())
					.Where(textValue => !string.IsNullOrWhiteSpace(textValue)));

			Assert.IsTrue(string.IsNullOrWhiteSpace(text));
		}
		finally
		{
			await host.DisposeAsync();
		}
	}
}

internal sealed class McpHostProcess : IAsyncDisposable
{
	private readonly string _hostDllPath;
	private readonly string _dotnetExePath;
	private readonly Process _process;
	private StreamWriter? _stdin;
	private StreamReader? _stdout;
	private readonly CancellationTokenSource _stderrCts = new();
	private Task _stderrPump = Task.CompletedTask;

	public McpHostProcess()
	{
		_hostDllPath = ResolveHostDllPath();
		var dotnetRoot = Environment.GetEnvironmentVariable("DOTNET_ROOT");
		_dotnetExePath = !string.IsNullOrWhiteSpace(dotnetRoot)
			? Path.Combine(dotnetRoot, "dotnet.exe")
			: "dotnet";

		var startInfo = new ProcessStartInfo
		{
			FileName = _dotnetExePath,
			Arguments = $"\"{_hostDllPath}\"",
			WorkingDirectory = Path.GetDirectoryName(_hostDllPath)!,
			UseShellExecute = false,
			RedirectStandardInput = true,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			CreateNoWindow = true,
			StandardOutputEncoding = Encoding.UTF8,
			StandardErrorEncoding = Encoding.UTF8,
		};

		_process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
	}

	private static string ResolveHostDllPath()
	{
		var outputRoots = new[] { "Debug", "Release" };
		foreach (var configuration in outputRoots)
		{
			var candidate = Path.GetFullPath(Path.Combine(
				AppContext.BaseDirectory,
				"..",
				"..",
				"..",
				"..",
				"Greg.Xrm.Command.Mcp",
				"bin",
				configuration,
				"net10.0",
				"Greg.Xrm.Command.Mcp.dll"));

			if (File.Exists(candidate))
			{
				return candidate;
			}
		}

		throw new FileNotFoundException("Could not locate the MCP host DLL in Debug or Release output.");
	}

	public async Task StartAsync()
	{
		if (_dotnetExePath != "dotnet")
		{
			Assert.IsTrue(File.Exists(_dotnetExePath), $"Expected dotnet at {_dotnetExePath}");
		}
		Assert.IsTrue(File.Exists(_hostDllPath), $"Expected MCP host DLL at {_hostDllPath}");

		if (!_process.Start())
		{
			throw new InvalidOperationException("Failed to start MCP host process.");
		}

		_stdin = _process.StandardInput;
		_stdout = _process.StandardOutput;
		_stdin.AutoFlush = true;
		_stderrPump = Task.Run(async () =>
		{
			try
			{
				while (!_stderrCts.Token.IsCancellationRequested && !_process.HasExited)
				{
					var line = await _process.StandardError.ReadLineAsync();
					if (line == null)
					{
						break;
					}
				}
			}
			catch
			{
				// Best-effort drain only.
			}
		});

		await Task.Delay(250);
	}

	public async Task<JsonDocument> RequestAsync(object payload)
	{
		if (_stdin is null || _stdout is null)
		{
			throw new InvalidOperationException("MCP host has not been started.");
		}

		var json = JsonSerializer.Serialize(payload);
		await _stdin.WriteLineAsync(json);
		await _stdin.FlushAsync();

		var response = await ReadResponseAsync();
		return JsonDocument.Parse(response);
	}

	public async Task NotifyAsync(object payload)
	{
		if (_stdin is null)
		{
			throw new InvalidOperationException("MCP host has not been started.");
		}

		var json = JsonSerializer.Serialize(payload);
		await _stdin.WriteLineAsync(json);
		await _stdin.FlushAsync();
	}

	private async Task<string> ReadResponseAsync()
	{
		if (_stdout is null)
		{
			throw new InvalidOperationException("MCP host has not been started.");
		}

		var readTask = _stdout.ReadLineAsync();
		var completed = await Task.WhenAny(readTask, Task.Delay(10000));
		if (completed != readTask)
		{
			throw new TimeoutException("Timed out waiting for MCP host response.");
		}

		var line = await readTask;
		if (string.IsNullOrWhiteSpace(line))
		{
			throw new InvalidOperationException("MCP host returned an empty response.");
		}

		return line;
	}

	public async ValueTask DisposeAsync()
	{
		try
		{
			await _stderrCts.CancelAsync();

			if (!_process.HasExited)
			{
				_process.Kill(entireProcessTree: true);
			}
		}
		catch (Exception ex) when (
			ex is InvalidOperationException
			or ObjectDisposedException
			or System.ComponentModel.Win32Exception)
		{
			Debug.WriteLine($"MCP host cleanup ignored: {ex.Message}");
		}

		await _stderrPump;
		_process.Dispose();
		_stderrCts.Dispose();
	}
}
