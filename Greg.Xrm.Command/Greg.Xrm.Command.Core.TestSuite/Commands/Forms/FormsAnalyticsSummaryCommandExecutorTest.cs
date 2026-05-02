using System.Text.Json;
using Greg.Xrm.Command.Services.Forms;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsAnalyticsSummaryCommandExecutorTest
	{
		[TestMethod]
		public void ExecuteShouldRenderAnalytics()
		{
			var apiMock = new Mock<IFormsApiClient>();
			var analytics = JsonDocument.Parse("""{"totalSubmissions":100,"completionRate":0.85}""");
			apiMock.Setup(a => a.GetAnalyticsAsync("contoso.onmicrosoft.com", "me", "User", "f1", It.IsAny<CancellationToken>()))
				.ReturnsAsync(analytics);

			var output = new OutputToMemory();
			var executor = new FormsAnalyticsSummaryCommandExecutor(output, apiMock.Object);
			var result = executor.ExecuteAsync(
				new FormsAnalyticsSummaryCommand { TenantId = "contoso.onmicrosoft.com", FormId = "f1" },
				CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsTrue(result.IsSuccess);
			StringAssert.Contains(output.ToString(), "totalSubmissions");
		}
	}
}
