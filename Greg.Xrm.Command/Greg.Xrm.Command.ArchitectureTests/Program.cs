using System;

namespace Greg.Xrm.Command.ArchitectureTests
{
	internal static class Program
	{
		private static int Main()
		{
			var tests = new ArchitectureRulesTests();

			try
			{
				Console.WriteLine("stage=1");
				Console.Out.Flush();
				tests.CommandsShouldNotReferenceHttpClientDirectly();
				Console.WriteLine("stage=2");
				Console.Out.Flush();
				tests.ExecutorsShouldNotDependOnOtherExecutors();
				Console.WriteLine("stage=3");
				Console.Out.Flush();
				tests.ExecutorsShouldImplementICommandExecutor();
				Console.WriteLine("Architecture checks passed.");
				Console.Out.Flush();
				return 0;
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
				return 1;
			}
		}
	}
}
