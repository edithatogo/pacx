using Greg.Xrm.Command.Services.Forms;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsAdminReportCommandExecutorTest
	{
		[TestMethod]
		public void ExecuteShouldShowSummary()
		{
			var apiMock = new Mock<IFormsApiClient>();
			apiMock.Setup(a => a.GetFormsAsync("contoso.onmicrosoft.com", "me", "User", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<FormsForm>
				{
					new() { Id = "f1", Title = "S1", RowCount = 10 },
					new() { Id = "f2", Title = "S2", RowCount = 25 }
				});

			var output = new OutputToMemory();
			var executor = new FormsAdminReportCommandExecutor(output, apiMock.Object);
			var result = executor.ExecuteAsync(
				new FormsAdminReportCommand { TenantId = "contoso.onmicrosoft.com" },
				CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsTrue(result.IsSuccess);
			StringAssert.Contains(output.ToString(), "Total Forms");
			StringAssert.Contains(output.ToString(), "Total Responses");
		}
	}
}
