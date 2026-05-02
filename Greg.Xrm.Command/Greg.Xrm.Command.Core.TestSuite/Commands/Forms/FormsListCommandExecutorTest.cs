using Greg.Xrm.Command.Services.Forms;
using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Commands.Forms
{
	[TestClass]
	public class FormsListCommandExecutorTest
	{
		[TestMethod]
		public void ExecuteShouldRenderFormList()
		{
			var apiMock = new Mock<IFormsApiClient>();
			apiMock.Setup(a => a.GetFormsAsync("contoso.onmicrosoft.com", "me", "User", It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<FormsForm>
				{
					new() { Id = "f1", Title = "Survey 1", Status = "Active", RowCount = 10, CreatedDate = new DateTime(2026, 1, 1) }
				});

			var output = new OutputToMemory();
			var executor = new FormsListCommandExecutor(output, apiMock.Object);
			var result = executor.ExecuteAsync(
				new FormsListCommand { TenantId = "contoso.onmicrosoft.com" },
				CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsTrue(result.IsSuccess);
			StringAssert.Contains(output.ToString(), "Survey 1");
		}

		[TestMethod]
		public void ExecuteWithNoFormsShouldShowWarning()
		{
			var apiMock = new Mock<IFormsApiClient>();
			apiMock.Setup(a => a.GetFormsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new List<FormsForm>());

			var output = new OutputToMemory();
			var executor = new FormsListCommandExecutor(output, apiMock.Object);
			var result = executor.ExecuteAsync(
				new FormsListCommand { TenantId = "contoso.onmicrosoft.com" },
				CancellationToken.None).GetAwaiter().GetResult();

			Assert.IsTrue(result.IsSuccess);
		}
	}
}
