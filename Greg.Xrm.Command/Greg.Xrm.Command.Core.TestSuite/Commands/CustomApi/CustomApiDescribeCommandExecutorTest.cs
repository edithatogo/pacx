using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Greg.Xrm.Command.Commands.CustomApi
{
	[TestClass]
	public class CustomApiDescribeCommandExecutorTest : CommandExecutorTestBase
	{
		[TestMethod]
		public async Task ExecuteAsync_WithApiAndParameters_ShouldDescribeSignature()
		{
			var apiId = Guid.NewGuid();
			this.OrganizationServiceMock
				.SetupSequence(x => x.RetrieveMultipleAsync(It.IsAny<QueryExpression>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new EntityCollection(new List<Entity>
				{
					new Entity("customapi")
					{
						Id = apiId,
						["uniquename"] = "new_TestApi",
						["displayname"] = "Test API",
						["description"] = "Example",
						["isfunction"] = false
					}
				}))
				.ReturnsAsync(new EntityCollection(new List<Entity>
				{
					new Entity("customapirequestparameter")
					{
						["uniquename"] = "Input",
						["type"] = new OptionSetValue(10),
						["isoptional"] = false
					}
				}))
				.ReturnsAsync(new EntityCollection());

			var executor = new CustomApiDescribeCommandExecutor(this.Output, this.OrganizationServiceRepositoryMock.Object);
			var result = await executor.ExecuteAsync(new CustomApiDescribeCommand { Name = "new_TestApi" }, CancellationToken.None);

			Assert.IsTrue(result.IsSuccess);
			StringAssert.Contains(this.Output.ToString(), "new_TestApi");
			StringAssert.Contains(this.Output.ToString(), "Input");
			Assert.AreEqual(1, result["RequestParameterCount"]);
			Assert.AreEqual(0, result["ResponsePropertyCount"]);
		}

		[TestMethod]
		public async Task ExecuteAsync_WhenApiIsMissing_ShouldFail()
		{
			this.OrganizationServiceMock
				.Setup(x => x.RetrieveMultipleAsync(It.IsAny<QueryExpression>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(new EntityCollection());

			var executor = new CustomApiDescribeCommandExecutor(this.Output, this.OrganizationServiceRepositoryMock.Object);
			var result = await executor.ExecuteAsync(new CustomApiDescribeCommand { Name = "missing" }, CancellationToken.None);

			Assert.IsFalse(result.IsSuccess);
			StringAssert.Contains(result.ErrorMessage, "was not found");
		}
	}
}
