using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;
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
			using (DbInfrastructureHelper.ResetTestDatabase ())
			{
			}
		}


		[TestMethod]
		public void CreateArgumentCheck()
		{
			var entityTypeIds = EntityEngineHelper.GetEntityTypeIds ();

			using (var dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => EntityEngine.Create (null, entityTypeIds)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => EntityEngine.Create (dbInfrastructure, null)
				);
			}
		}


		[TestMethod]
		public void CheckArgumentCheck()
		{
			var entityTypeIds = EntityEngineHelper.GetEntityTypeIds ();

			using (var dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => EntityEngine.Check (null, entityTypeIds)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => EntityEngine.Check (dbInfrastructure, null)
				);
			}
		}


		[TestMethod]
		public void UpdateArgumentCheck()
		{
			var entityTypeIds = EntityEngineHelper.GetEntityTypeIds ();

			using (var dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => EntityEngine.Update (null, entityTypeIds)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => EntityEngine.Update (dbInfrastructure, null)
				);
			}
		}


		[TestMethod]
		public void ConnectArgumentCheck()
		{
			var entityTypeIds = EntityEngineHelper.GetEntityTypeIds ();

			using (var dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => EntityEngine.Connect (null, entityTypeIds)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => EntityEngine.Connect (dbInfrastructure, null)
				);
			}
		}


		[TestMethod]
		public void CreateAndCheckTest()
		{
			var partialEntityTypeIds = this.GetPartialEntityTypeIds ().ToList ();
			var completeEntityTypeIds = EntityEngineHelper.GetEntityTypeIds ().ToList ();

			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				Assert.IsFalse (EntityEngine.Check (dbInfrastructure, completeEntityTypeIds));

				EntityEngine.Create (dbInfrastructure, partialEntityTypeIds);

				Assert.IsTrue (EntityEngine.Check (dbInfrastructure, completeEntityTypeIds));
			}
		}


		[TestMethod]
		public void UpdateAndCheckTest1()
		{
			using (var dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				var entityTypeIds1 = this.GetSubGraphOfEntityTypeIds ();
				var partialEntityTypeIds1 = entityTypeIds1.Item1;
				var completeEntityTypeIds1 = entityTypeIds1.Item2;

				var partialEntityTypeIds2 = this.GetPartialEntityTypeIds ().ToList ();
				var completeEntityTypeIds2 = EntityEngineHelper.GetEntityTypeIds ().ToList ();

				Assert.IsFalse (EntityEngine.Check (dbInfrastructure, completeEntityTypeIds1));
				Assert.IsFalse (EntityEngine.Check (dbInfrastructure, completeEntityTypeIds2));

				EntityEngine.Create (dbInfrastructure, partialEntityTypeIds1);

				Assert.IsTrue (EntityEngine.Check (dbInfrastructure, completeEntityTypeIds1));
				Assert.IsFalse (EntityEngine.Check (dbInfrastructure, completeEntityTypeIds2));

				EntityEngine.Update (dbInfrastructure, partialEntityTypeIds2);

				Assert.IsTrue (EntityEngine.Check (dbInfrastructure, completeEntityTypeIds1));
				Assert.IsTrue (EntityEngine.Check (dbInfrastructure, completeEntityTypeIds2));
			}
		}


		[TestMethod]
		public void UpdateAndCheckTest2()
		{
			using (var dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				var entityTypeIds = this.GetSubGraphOfEntityTypeIds ().Item2;

				EntityEngine.Create (dbInfrastructure, entityTypeIds);

				Assert.IsTrue (EntityEngine.Check (dbInfrastructure, entityTypeIds));

				DbTable connectionTable = dbInfrastructure.ResolveDbTable (ConnectionManager.TableFactory.TableName);
				DbColumn column = connectionTable.Columns[ConnectionManager.TableFactory.ColumnStatusName];

				dbInfrastructure.RemoveColumnFromTable (connectionTable, column);

				Assert.IsFalse (EntityEngine.Check (dbInfrastructure, entityTypeIds));

				EntityEngine.Update (dbInfrastructure, entityTypeIds);

				Assert.IsTrue (EntityEngine.Check (dbInfrastructure, entityTypeIds));
			}
		}


		[TestMethod]
		public void ConnectTest()
		{
			using (var dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				var partialEntityTypeIds = this.GetPartialEntityTypeIds ().ToList ();
				var completeEntityTypeIds = EntityEngineHelper.GetEntityTypeIds ().ToList ();

				EntityEngine.Create (dbInfrastructure, partialEntityTypeIds);

				EntityEngine engine = EntityEngine.Connect (dbInfrastructure, partialEntityTypeIds);

				Assert.IsNotNull (engine);
				Assert.IsNotNull (engine.EntityTypeEngine);
				Assert.IsNotNull (engine.EntitySchemaEngine);
				Assert.IsNotNull (engine.ServiceSchemaEngine);

				var expectedTypeIds = completeEntityTypeIds.OrderBy (id => id.ToLong ()).ToList ();
				var actualTypeIds = engine.EntityTypeEngine.GetEntityTypes ().Select (t => t.CaptionId).OrderBy (id => id.ToLong ()).ToList ();

				CollectionAssert.AreEqual (expectedTypeIds, actualTypeIds);

				var expectedEntityTableIds = completeEntityTypeIds.OrderBy (id => id.ToLong ()).ToList ();

				foreach (var tableId in expectedEntityTableIds)
				{
					Assert.IsNotNull (engine.EntitySchemaEngine.GetEntityTable (tableId));
				}

				var serviceTableNames = new List<string> ()
				{
					ConnectionManager.TableFactory.TableName,
					EntityDeletionLog.TableFactory.TableName,
					EntityModificationLog.TableFactory.TableName,
					InfoManager.TableFactory.TableName,
					LockManager.TableFactory.TableName,
					UidManager.TableFactory.TableName,
				};

				foreach (var serviceTableName in serviceTableNames)
				{
					Assert.IsNotNull (engine.ServiceSchemaEngine.GetServiceTable (serviceTableName));
				}
			}
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
