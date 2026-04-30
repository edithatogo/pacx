using System.Text.Json;
using Greg.Xrm.Command.Services.ReleasePlan;

namespace Greg.Xrm.Command.TestSuite.Services.ReleasePlan
{
	[TestClass]
	public class ReleasePlanItemTests
	{
		[TestMethod]
		public void ReleasePlanItemShouldBeJsonSerializable()
		{
			var item = new ReleasePlanItem
			{
				Id = "123456",
				Title = "Test Feature",
				Description = "A test roadmap item.",
				Product = "Teams",
				Category = "New",
				Status = "RollingOut",
				Wave = "CY2025-Q2",
				Url = "https://www.microsoft.com/microsoft-365/roadmap?filters=&searchterms=123456",
				LastModifiedUtc = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
				Tags = ["ai", "copilot"],
			};

			var json = JsonSerializer.Serialize(item);
			var deserialized = JsonSerializer.Deserialize<ReleasePlanItem>(json);

			Assert.IsNotNull(deserialized);
			Assert.AreEqual("123456", deserialized.Id);
			Assert.AreEqual("Teams", deserialized.Product);
			Assert.AreEqual("RollingOut", deserialized.Status);
			Assert.AreEqual(2, deserialized.Tags.Count);
		}

		[TestMethod]
		public void ReleasePlanSnapshotShouldBeJsonSerializable()
		{
			var snapshot = new ReleasePlanSnapshot
			{
				Items =
				[
					new ReleasePlanItem { Id = "1", Title = "Item 1", Product = "Teams" },
					new ReleasePlanItem { Id = "2", Title = "Item 2", Product = "SharePoint" },
				],
				FetchedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
				Etag = "abc123",
			};

			var json = JsonSerializer.Serialize(snapshot);
			var deserialized = JsonSerializer.Deserialize<ReleasePlanSnapshot>(json);

			Assert.IsNotNull(deserialized);
			Assert.AreEqual(2, deserialized.Items.Count);
			Assert.AreEqual("abc123", deserialized.Etag);
		}
	}
}
