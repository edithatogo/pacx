using Greg.Xrm.Command.Services.Output;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Greg.Xrm.Command.Core.TestSuite.Services.Output
{
	[TestClass]
	public class OutputToMemoryTest
	{
		[TestMethod]
		public void BearerTokensShouldBeRedacted()
		{
			var output = new OutputToMemory();

			output.WriteLine("Authorization: Bearer abc123.def-456_ghi");

			var text = output.ToString();
			Assert.IsTrue(text.Contains("Bearer [REDACTED]"));
			Assert.IsFalse(text.Contains("abc123.def-456_ghi"));
		}
	}
}
