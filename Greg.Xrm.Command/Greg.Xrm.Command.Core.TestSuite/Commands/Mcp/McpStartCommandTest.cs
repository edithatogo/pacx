using System.Collections.Generic;
using System.Linq;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Mcp
{
	[TestClass]
	public class McpStartCommandTest
	{
		[TestMethod]
		public void ParseWithDefaultsShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<McpStartCommand>(
				"mcp", "start");

			Assert.AreEqual(3000, command.Port);
			Assert.AreEqual("stdio", command.Transport);
			Assert.AreEqual("localhost", command.Host);
		}

		[TestMethod]
		public void ParseWithAllOptionsShouldWork()
		{
			var command = Commands.Utility.TestParseCommand<McpStartCommand>(
				"mcp", "start",
				"-p", "5000",
				"-t", "http",
				"--host", "0.0.0.0");

			Assert.AreEqual(5000, command.Port);
			Assert.AreEqual("http", command.Transport);
			Assert.AreEqual("0.0.0.0", command.Host);
		}
	}

	[TestClass]
	public class CapturedOutputTest
	{
		[TestMethod]
		public void WriteShouldCaptureText()
		{
			var output = new TestCapturedOutput();
			output.Write("Hello");
			output.WriteLine(" World");

			Assert.AreEqual(2, output.Lines.Count);
			Assert.AreEqual("Hello", output.Lines[0]);
			Assert.AreEqual(" World", output.Lines[1]);
		}

		[TestMethod]
		public void WriteLineShouldCaptureWithNewline()
		{
			var output = new TestCapturedOutput();
			output.WriteLine("Line 1");
			output.WriteLine("Line 2");

			var captured = output.GetCapturedOutput();
			Assert.AreEqual("Line 1\nLine 2", captured);
		}

		[TestMethod]
		public void WriteTableShouldSerializeHeadersAndRows()
		{
			var output = new TestCapturedOutput();
			var data = new[]
			{
				new { Name = "Alice", Age = "30" },
				new { Name = "Bob", Age = "25" },
			};

			output.WriteTable(data, () => new[] { "Name", "Age" }, row => new[] { row.Name, row.Age });

			Assert.IsTrue(output.Lines.Count >= 3);
			Assert.AreEqual("Name | Age", output.Lines[0]);
			Assert.IsTrue(output.Lines[1].StartsWith("---"));
			Assert.AreEqual("Alice | 30", output.Lines[2]);
		}
	}

	internal sealed class TestCapturedOutput : IOutput
	{
		private readonly List<string> _lines = new();

		public IReadOnlyList<string> Lines => _lines;

		public string GetCapturedOutput() => string.Join("\n", _lines);

		public IOutput Write(object? text)
		{
			_lines.Add(text?.ToString() ?? "");
			return this;
		}

		public IOutput Write(object? text, ConsoleColor color)
		{
			_lines.Add(text?.ToString() ?? "");
			return this;
		}

		public IOutput WriteLine()
		{
			_lines.Add("");
			return this;
		}

		public IOutput WriteLine(object? text)
		{
			_lines.Add(text?.ToString() ?? "");
			return this;
		}

		public IOutput WriteLine(object? text, ConsoleColor color)
		{
			_lines.Add(text?.ToString() ?? "");
			return this;
		}

		public IOutput WriteTable<TRow>(IReadOnlyList<TRow> collection, Func<string[]> rowHeaders, Func<TRow, string[]> rowData, Func<int, TRow, ConsoleColor?>? colorPicker = null)
		{
			var headers = rowHeaders();
			_lines.Add(string.Join(" | ", headers));
			_lines.Add(new string('-', headers.Sum(h => h.Length + 3) - 3));
			for (var i = 0; i < collection.Count; i++)
			{
				_lines.Add(string.Join(" | ", rowData(collection[i])));
			}

			return this;
		}

		public IOutput WriteCorrelationHeader(string correlationId)
		{
			_lines.Add($"CorrelationId: {correlationId}");
			return this;
		}
	}
}
