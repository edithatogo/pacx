using Greg.Xrm.Command.Services.Forms;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsResponseCountCommandExecutorTest
	{
		[TestMethod]
		public void ExecuteShouldShowResponseCount()
		{
			var apiMock = new Mock<IFormsApiClient>();
			apiMock.Setup(a => a.GetResponseCountAsync("contoso.onmicrosoft.com", "me", "User", "form123", It.IsAny<CancellationToken>()))
				.ReturnsAsync(42);

			var output = new OutputToMemory();
			var executor = new FormsResponseCountCommandExecutor(output, apiMock.Object);
			var result = executor.ExecuteAsync(
				new FormsResponseCountCommand { TenantId = "contoso.onmicrosoft.com", FormId = "form123" },
				CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsTrue(result.IsSuccess);
			StringAssert.Contains(output.ToString(), "42");
		}
	}
}
