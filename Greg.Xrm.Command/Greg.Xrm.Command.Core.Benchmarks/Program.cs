using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Greg.Xrm.Command.Commands.Auth;
using Greg.Xrm.Command.Parsing;
using Greg.Xrm.Command.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace Greg.Xrm.Command.Benchmarks
{
	/// <summary>
	/// Benchmark suite for PACX hot paths: command parsing and output formatting.
	/// Run with: dotnet run --configuration Release --project Greg.Xrm.Command.Core.Benchmarks
	/// </summary>
	public static class Program
	{
		public static void Main(string[] args)
		{
			BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
		}
	}

	[MemoryDiagnoser]
	public class CommandLineParserBenchmarks
	{
		private CommandParser? _parser;
		[GlobalSetup]
		public void Setup()
		{
			var log = NullLogger<CommandRegistry>.Instance;
			var output = new OutputToMemory();
			var storage = new Storage();
			var registry = new CommandRegistry(log, output, storage);
			registry.InitializeFromAssembly(typeof(ListCommand).Assembly);
			_parser = new CommandParser(new OutputToMemory(), registry);
		}

		[Benchmark]
		public void ParseSimpleCommand()
		{
			_parser!.Parse("auth", "list");
		}

		[Benchmark]
		public void ParseCommandWithOptions()
		{
			_parser!.Parse("auth", "create", "--name", "test", "--url", "https://test.crm.dynamics.com");
		}

		[Benchmark]
		public void ParseInvalidCommand()
		{
			_parser!.Parse("nonexistent", "command", "with", "many", "args");
		}

		[Benchmark]
		public void ParseWithSpecialCharacters()
		{
			_parser!.Parse("auth", "create", "--name", "test&special|chars", "--url", "https://test.crm.dynamics.com?param=value");
		}
	}

	[MemoryDiagnoser]
	public class OutputFormattingBenchmarks
	{
		private OutputToMemory _output = null!;

		[GlobalSetup]
		public void Setup()
		{
			_output = new OutputToMemory();
		}

		[Benchmark]
		public void WriteLine_Simple()
		{
			_output.WriteLine("Simple output line");
		}

		[Benchmark]
		public void WriteLine_WithData()
		{
			for (int i = 0; i < 100; i++)
			{
				_output.WriteLine($"Item {i}: Name={Guid.NewGuid()}, Value={i * 3.14}");
			}
		}

		[Benchmark]
		public void Write_TableFormat()
		{
			for (int i = 0; i < 50; i++)
			{
				_output.WriteLine($"| Column{i} | Data{i} | Value{i} |");
			}
		}
	}
}
