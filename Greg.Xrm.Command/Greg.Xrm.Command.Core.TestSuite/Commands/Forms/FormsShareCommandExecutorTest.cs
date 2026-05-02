using Greg.Xrm.Command.Services.Forms;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsShareCommandExecutorTest
	{
		[TestMethod]
		public void ExecuteShouldShareForm()
		{
			var apiMock = new Mock<IFormsApiClient>();
			apiMock.Setup(a => a.ShareFormAsync("contoso.onmicrosoft.com", "me", "User", "f1", "g1", "respond", It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var output = new OutputToMemory();
			var executor = new FormsShareCommandExecutor(output, apiMock.Object);
			var result = executor.ExecuteAsync(
				new FormsShareCommand { TenantId = "contoso.onmicrosoft.com", FormId = "f1", GroupId = "g1", Role = "respond" },
				CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsTrue(result.IsSuccess);
			StringAssert.Contains(output.ToString(), "shared");
		}
	}
}
