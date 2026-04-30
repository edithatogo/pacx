using System.Reflection;
using Greg.Xrm.Command.Parsing;

namespace Greg.Xrm.Command.Commands.ReleasePlan
{
	[TestClass]
	public class ReleasePlanReportCommandTest
	{
		[TestMethod]
		public void CommandAttributeShouldBePresent()
		{
			var attr = typeof(ReleasePlanReportCommand).GetCustomAttribute<CommandAttribute>();
			Assert.IsNotNull(attr);
			Assert.AreEqual("release-plan", attr.Verbs[0]);
			Assert.AreEqual("report", attr.Verbs[1]);
		}

		[TestMethod]
		public void ProductOptionShouldBeOptional()
		{
			var prop = typeof(ReleasePlanReportCommand).GetProperty(nameof(ReleasePlanReportCommand.Product));
			Assert.IsNotNull(prop);

			var option = prop.GetCustomAttribute<OptionAttribute>();
			Assert.IsNotNull(option);
			Assert.AreEqual("product", option.LongName);
			Assert.AreEqual("p", option.ShortName);
		}

		[TestMethod]
		public void StatusOptionShouldBeOptional()
		{
			var prop = typeof(ReleasePlanReportCommand).GetProperty(nameof(ReleasePlanReportCommand.Status));
			Assert.IsNotNull(prop);

			var option = prop.GetCustomAttribute<OptionAttribute>();
			Assert.IsNotNull(option);
			Assert.AreEqual("status", option.LongName);
			Assert.AreEqual("s", option.ShortName);
		}

		[TestMethod]
		public void OutputOptionShouldBeOptional()
		{
			var prop = typeof(ReleasePlanReportCommand).GetProperty(nameof(ReleasePlanReportCommand.OutputPath));
			Assert.IsNotNull(prop);

			var option = prop.GetCustomAttribute<OptionAttribute>();
			Assert.IsNotNull(option);
			Assert.AreEqual("output", option.LongName);
			Assert.AreEqual("o", option.ShortName);
		}

		[TestMethod]
		public void FormatOptionShouldDefaultToMarkdown()
		{
			var cmd = new ReleasePlanReportCommand();
			Assert.AreEqual("markdown", cmd.Format);
		}
	}
}
