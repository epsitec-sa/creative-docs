using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Schema
{


	[TestClass]
	public sealed class UnitTestEntityEngine
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DbInfrastructureHelper.ResetTestDatabase ();
		}


		[TestMethod]
		public void CreateArgumentCheck()
		{
			var access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();
			var entityTypeIds = EntityEngineHelper.GetEntityTypeIds ();

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => EntityEngine.Create (DbAccess.Empty, entityTypeIds)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => EntityEngine.Create (access, null)
			);
		}


		[TestMethod]
		public void CheckArgumentCheck()
		{
			var access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();
			var entityTypeIds = EntityEngineHelper.GetEntityTypeIds ();

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => EntityEngine.Check (DbAccess.Empty, entityTypeIds)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => EntityEngine.Check (access, null)
			);
		}


		[TestMethod]
		public void UpdateArgumentCheck()
		{
			var access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();
			var entityTypeIds = EntityEngineHelper.GetEntityTypeIds ();

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => EntityEngine.Update (DbAccess.Empty, entityTypeIds)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => EntityEngine.Update (access, null)
			);
		}


		[TestMethod]
		public void ConnectArgumentCheck()
		{
			var access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();
			var entityTypeIds = EntityEngineHelper.GetEntityTypeIds ();

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => EntityEngine.Connect (DbAccess.Empty, entityTypeIds)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => EntityEngine.Connect (access, null)
			);
		}


		[TestMethod]
		public void CreateAndCheckTest()
		{
			var access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();
			var partialEntityTypeIds = this.GetPartialEntityTypeIds ().ToList ();
			var completeEntityTypeIds = EntityEngineHelper.GetEntityTypeIds ().ToList ();

			Assert.IsFalse (EntityEngine.Check (access, completeEntityTypeIds));

			EntityEngine.Create (access, partialEntityTypeIds);

			Assert.IsTrue (EntityEngine.Check (access, completeEntityTypeIds));
		}


		[TestMethod]
		public void UpdateAndCheckTest()
		{
			var access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();
			
			var entityTypeIds1 = this.GetSubGraphOfEntityTypeIds ();
			var partialEntityTypeIds1 = entityTypeIds1.Item1;
			var completeEntityTypeIds1 = entityTypeIds1.Item2;

			var partialEntityTypeIds2 = this.GetPartialEntityTypeIds ().ToList ();
			var completeEntityTypeIds2 = EntityEngineHelper.GetEntityTypeIds ().ToList ();

			Assert.IsFalse (EntityEngine.Check (access, completeEntityTypeIds1));
			Assert.IsFalse (EntityEngine.Check (access, completeEntityTypeIds2));

			EntityEngine.Create (access, partialEntityTypeIds1);

			Assert.IsTrue (EntityEngine.Check (access, completeEntityTypeIds1));
			Assert.IsFalse (EntityEngine.Check (access, completeEntityTypeIds2));

			EntityEngine.Update (access, partialEntityTypeIds2);

			Assert.IsTrue (EntityEngine.Check (access, completeEntityTypeIds1));
			Assert.IsTrue (EntityEngine.Check (access, completeEntityTypeIds2));
		}


		[TestMethod]
		public void ConnectTest()
		{
			var access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();
			var partialEntityTypeIds = this.GetPartialEntityTypeIds ().ToList ();
			var completeEntityTypeIds = EntityEngineHelper.GetEntityTypeIds ().ToList ();

			EntityEngine.Create (access, partialEntityTypeIds);

			EntityEngine engine = EntityEngine.Connect (access, partialEntityTypeIds);

			Assert.IsNotNull (engine);
			Assert.IsNotNull (engine.TypeEngine);
			Assert.IsNotNull (engine.SchemaEngine);

			var expectedTypeIds = completeEntityTypeIds.OrderBy (id => id.ToLong ()).ToList ();
			var actualTypeIds = engine.TypeEngine.GetEntityTypes ().Select (t => t.CaptionId).OrderBy (id => id.ToLong ()).ToList ();

			CollectionAssert.AreEqual (expectedTypeIds, actualTypeIds);

			var expectedTableIds = completeEntityTypeIds.OrderBy (id => id.ToLong ()).ToList ();
			var actualTableIds = engine.SchemaEngine.GetEntityTables ().Select (t => t.CaptionId).OrderBy (id => id.ToLong ()).ToList ();

			CollectionAssert.AreEqual (expectedTableIds, actualTableIds);
		}


		private IEnumerable<Druid> GetPartialEntityTypeIds()
		{
			yield return new Druid ("[J1AE1]");
			yield return new Druid ("[J1AJ1]");
			yield return new Druid ("[J1AT1]");
			yield return new Druid ("[J1A02]");
			yield return new Druid ("[J1A42]");
			yield return new Druid ("[J1A72]");
		}


		private System.Tuple<List<Druid>, List<Druid>> GetSubGraphOfEntityTypeIds()
		{
			List<Druid> input = new List<Druid> ()
			{
				new Druid ("[J1A4]"),
				new Druid ("[J1AE]"),
				new Druid ("[J1AJ]"),
			};

			List<Druid> output = new List<Druid> ()
			{
				new Druid ("[J1A4]"),
				new Druid ("[J1A6]"),
				new Druid ("[J1A9]"),
				new Druid ("[J1AE]"),
				new Druid ("[J1AG]"),
				new Druid ("[J1AJ]"),
			};

			return System.Tuple.Create (input, output);
		}


	}


}
