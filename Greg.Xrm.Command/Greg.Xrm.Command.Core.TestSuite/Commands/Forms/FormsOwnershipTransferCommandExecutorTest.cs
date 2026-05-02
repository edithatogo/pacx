using Greg.Xrm.Command.Services.Forms;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsOwnershipTransferCommandExecutorTest
	{
		[TestMethod]
		public void ExecuteShouldTransferOwnership()
		{
			var apiMock = new Mock<IFormsApiClient>();
			apiMock.Setup(a => a.TransferOwnershipAsync("contoso.onmicrosoft.com", "me", "User", "f1", "newowner@contoso.com", It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			var output = new OutputToMemory();
			var executor = new FormsOwnershipTransferCommandExecutor(output, apiMock.Object);
			var result = executor.ExecuteAsync(
				new FormsOwnershipTransferCommand { TenantId = "contoso.onmicrosoft.com", FormId = "f1", TargetUserPrincipalName = "newowner@contoso.com" },
				CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsTrue(result.IsSuccess);
			StringAssert.Contains(output.ToString(), "transferred");
		}
	}
}
