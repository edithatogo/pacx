using Greg.Xrm.Command.Services.ReleasePlan;

namespace Greg.Xrm.Command.TestSuite.Services.ReleasePlan
{
	[TestClass]
	public class ReleasePlanImpactReportTests
	{
		[TestMethod]
		public void ImpactReportShouldHaveDefaultValues()
		{
			var report = new ReleasePlanImpactReport();

			Assert.IsNotNull(report.Items);
			Assert.AreEqual(0, report.Items.Count);
			Assert.AreEqual(default, report.GeneratedAt);
			Assert.IsNull(report.EnvironmentName);
		}

		[TestMethod]
		public void ImpactReportShouldStoreEnvironmentName()
		{
			var report = new ReleasePlanImpactReport
			{
				EnvironmentName = "contoso-dev",
				GeneratedAt = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc),
			};

			Assert.AreEqual("contoso-dev", report.EnvironmentName);
			Assert.AreEqual(new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc), report.GeneratedAt);
		}

		[TestMethod]
		public void ImpactedItemShouldHaveDefaultValues()
		{
			var item = new ImpactedItem();

			Assert.IsNull(item.ReleaseItem);
			Assert.AreEqual(string.Empty, item.ImpactCategory);
			Assert.AreEqual(string.Empty, item.Relevance);
			Assert.IsNull(item.Rationale);
			Assert.IsNotNull(item.AffectedComponentTypes);
			Assert.AreEqual(0, item.AffectedComponentTypes.Count);
		}

		[TestMethod]
		public void ImpactedItemShouldStoreAllProperties()
		{
			var releaseItem = new ReleasePlanItem
			{
				Id = "123",
				Title = "New Copilot features",
				Status = "InDevelopment",
				Product = "Power Platform",
			};

			var item = new ImpactedItem
			{
				ReleaseItem = releaseItem,
				ImpactCategory = "Breaking",
				Relevance = "High",
				Rationale = "Deprecates legacy connector",
				AffectedComponentTypes = ["Custom Connector", "Flow"],
			};

			Assert.AreSame(releaseItem, item.ReleaseItem);
			Assert.AreEqual("Breaking", item.ImpactCategory);
			Assert.AreEqual("High", item.Relevance);
			Assert.AreEqual("Deprecates legacy connector", item.Rationale);
			Assert.AreEqual(2, item.AffectedComponentTypes.Count);
			Assert.AreEqual("Custom Connector", item.AffectedComponentTypes[0]);
			Assert.AreEqual("Flow", item.AffectedComponentTypes[1]);
		}
	}

	[TestClass]
	public class ReleasePlanComponentMapTests
	{
		[TestMethod]
		public void ComponentMapShouldContainKnownProductMappings()
		{
			var map = new ReleasePlanComponentMap();

			Assert.IsTrue(map.Mappings.Count > 0);
		}

		[TestMethod]
		public void ComponentMapShouldMapPowerPlatformToKnownTypes()
		{
			var map = new ReleasePlanComponentMap();

			var types = map.GetComponentTypes("Power Platform");

			Assert.IsNotNull(types);
			Assert.IsTrue(types.Count > 0);
			Assert.IsTrue(types.Contains("Canvas App"));
		}

		[TestMethod]
		public void ComponentMapShouldReturnEmptyForUnknownProduct()
		{
			var map = new ReleasePlanComponentMap();

			var types = map.GetComponentTypes("Nonexistent Product XYZ");

			Assert.IsNotNull(types);
			Assert.AreEqual(0, types.Count);
		}

		[TestMethod]
		public void ComponentMapShouldBeCaseInsensitive()
		{
			var map = new ReleasePlanComponentMap();

			var lower = map.GetComponentTypes("power platform");
			var upper = map.GetComponentTypes("POWER PLATFORM");
			var mixed = map.GetComponentTypes("Power Platform");

			Assert.IsTrue(lower.Count > 0);
			Assert.AreEqual(lower.Count, upper.Count);
			Assert.AreEqual(lower.Count, mixed.Count);
		}

		[TestMethod]
		public void ComponentMapShouldMapDynamics365ToKnownTypes()
		{
			var map = new ReleasePlanComponentMap();

			var types = map.GetComponentTypes("Dynamics 365");

			Assert.IsNotNull(types);
			Assert.IsTrue(types.Count > 0);
			Assert.IsTrue(types.Contains("Model-driven App"));
		}

		[TestMethod]
		public void ComponentMapShouldMapPowerBIToKnownTypes()
		{
			var map = new ReleasePlanComponentMap();

			var types = map.GetComponentTypes("Power BI");

			Assert.IsNotNull(types);
			Assert.IsTrue(types.Count > 0);
			Assert.IsTrue(types.Contains("Semantic Model"));
		}
	}
}
