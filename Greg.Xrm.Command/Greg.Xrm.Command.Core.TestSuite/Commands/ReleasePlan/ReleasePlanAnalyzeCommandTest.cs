using System.Reflection;
using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.ReleasePlan
{
	[TestClass]
	public class ReleasePlanAnalyzeCommandTest
	{
		[TestMethod]
		public void CommandAttributeShouldBePresent()
		{
			var attr = typeof(ReleasePlanAnalyzeCommand).GetCustomAttribute<CommandAttribute>();
			Assert.IsNotNull(attr);
			Assert.AreEqual("release-plan", attr.Verbs[0]);
			Assert.AreEqual("analyze", attr.Verbs[1]);
		}

		[TestMethod]
		public void EnvironmentOptionShouldBeRequired()
		{
			var prop = typeof(ReleasePlanAnalyzeCommand).GetProperty(nameof(ReleasePlanAnalyzeCommand.EnvironmentName));
			Assert.IsNotNull(prop);

			var option = prop.GetCustomAttribute<OptionAttribute>();
			Assert.IsNotNull(option);
			Assert.AreEqual("environment", option.LongName);
			Assert.AreEqual("env", option.ShortName);
			Assert.IsTrue(option.Required);
		}

		[TestMethod]
		public void ProductOptionShouldBeOptional()
		{
			var prop = typeof(ReleasePlanAnalyzeCommand).GetProperty(nameof(ReleasePlanAnalyzeCommand.Product));
			Assert.IsNotNull(prop);

			var option = prop.GetCustomAttribute<OptionAttribute>();
			Assert.IsNotNull(option);
			Assert.AreEqual("product", option.LongName);
			Assert.AreEqual("p", option.ShortName);
		}

		[TestMethod]
		public void StatusOptionShouldBeOptional()
		{
			var prop = typeof(ReleasePlanAnalyzeCommand).GetProperty(nameof(ReleasePlanAnalyzeCommand.Status));
			Assert.IsNotNull(prop);

			var option = prop.GetCustomAttribute<OptionAttribute>();
			Assert.IsNotNull(option);
			Assert.AreEqual("status", option.LongName);
			Assert.AreEqual("s", option.ShortName);
		}

		[TestMethod]
		public void DefaultEnvironmentShouldBeEmpty()
		{
			var cmd = new ReleasePlanAnalyzeCommand();
			Assert.AreEqual(string.Empty, cmd.EnvironmentName);
		}
	}
}
