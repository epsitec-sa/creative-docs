using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;



namespace Epsitec.Cresus.DataLayer.Tests.Vs.Schema
{


	[TestClass]
	public sealed class UnitTestSchemaEngine
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
		public void SchemaEngineConstructorTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				SchemaEngine schemaEngine = new SchemaEngine (dbInfrastructure, entityTypeEngine);
			}
		}


		[TestMethod]
		public void SchemaEngineConstructorArgumentCheck()
		{
			DbInfrastructure dbInfrastructure = null;
			EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new SchemaEngine (dbInfrastructure, entityTypeEngine)
			);
		}


		[TestMethod]
		public void GetEntityColumnNameTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				SchemaEngine engine = new SchemaEngine (dbInfrastructure, entityTypeEngine);
				SchemaEngine_Accessor engineAccessor = new SchemaEngine_Accessor (new PrivateObject (engine));

				List<System.Tuple<Druid, string>> data = new List<System.Tuple<Druid, string>>
				{
					System.Tuple.Create (Druid.Parse ("[J1A4]"), "J1A4"),
					System.Tuple.Create (Druid.Parse ("[J1AJ]"), "J1AJ"),
					System.Tuple.Create (Druid.Parse ("[J1AQ]"), "J1AQ"),
					System.Tuple.Create (Druid.Parse ("[J1AO1]"), "J1AO1"),
					System.Tuple.Create (Druid.Parse ("[J1AA1]"), "J1AA1"),
					System.Tuple.Create (Druid.Parse ("[J1A91]"), "J1A91"),
				};

				foreach (var d in data)
				{
					Druid entityId = d.Item1;
					string name = d.Item2;

					Assert.AreEqual (name, engineAccessor.GetEntityColumnName (entityId));
				}
			}
		}


		[TestMethod]
		public void GetEntityTableNameTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				SchemaEngine engine = new SchemaEngine (dbInfrastructure, entityTypeEngine);
				SchemaEngine_Accessor engineAccessor = new SchemaEngine_Accessor (new PrivateObject (engine));

				List<System.Tuple<Druid,string>> data = new List<System.Tuple<Druid, string>> ()
				{
					System.Tuple.Create (Druid.Parse ("[J1AB1]"), "J1AB1"),
					System.Tuple.Create (Druid.Parse ("[J1AJ1]"), "J1AJ1"),
					System.Tuple.Create (Druid.Parse ("[J1AT]"), "J1AT"),
					System.Tuple.Create (Druid.Parse ("[J1AN]"), "J1AN"),
					System.Tuple.Create (Druid.Parse ("[J1AQ]"), "J1AQ"),
					System.Tuple.Create (Druid.Parse ("[J1AA1]"), "J1AA1"),
					System.Tuple.Create (Druid.Parse ("[J1AV]"), "J1AV"),
					System.Tuple.Create (Druid.Parse ("[J1A41]"), "J1A41"),
					System.Tuple.Create (Druid.Parse ("[J1AE1]"), "J1AE1"),
					System.Tuple.Create (Druid.Parse ("[J1A11]"), "J1A11"),
				};

				foreach (var d in data)
				{
					Druid id = d.Item1;
					string name = d.Item2;

					Assert.AreEqual (name, engineAccessor.GetEntityTableName (id));
				}
			}
		}


		[TestMethod]
		public void GetRelationTableNameTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				SchemaEngine engine = new SchemaEngine (dbInfrastructure, entityTypeEngine);
				SchemaEngine_Accessor engineAccessor = new SchemaEngine_Accessor (new PrivateObject (engine));

				List<System.Tuple<Druid,Druid,string>> data = new List<System.Tuple<Druid, Druid, string>> ()
				{
					System.Tuple.Create (Druid.Parse ("[J1AB1]"), Druid.Parse ("[J1AC1]"), "J1AB1:J1AC1"),
					System.Tuple.Create (Druid.Parse ("[J1AN]"), Druid.Parse ("[J1AS]"), "J1AN:J1AS"),
					System.Tuple.Create (Druid.Parse ("[J1AA1]"), Druid.Parse ("[J1AP1]"), "J1AA1:J1AP1"),
					System.Tuple.Create (Druid.Parse ("[J1AA1]"), Druid.Parse ("[J1AQ1]"), "J1AA1:J1AQ1"),
				};

				foreach (var d in data)
				{
					Druid entityId = d.Item1;
					Druid fieldId = d.Item2;
					string name = d.Item3;

					Assert.AreEqual (name, engineAccessor.GetCollectionTableName (entityId, fieldId));
				}
			}
		}


		[TestMethod]
		public void LoadSchemaTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				SchemaEngine schemaEngine = new SchemaEngine (dbInfrastructure, entityTypeEngine);
				SchemaBuilder schemaBuilder = new SchemaBuilder (schemaEngine, entityTypeEngine, dbInfrastructure);

				schemaBuilder.RegisterSchema (DataInfrastructureHelper.GetEntityIds ());
			}

			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				List<Druid> entityIds = new List<Druid> ()
				{
					Druid.Parse ("[J1AB1]"),
					Druid.Parse ("[J1AJ1]"),
					Druid.Parse ("[J1AT]"),
					Druid.Parse ("[J1AN]"),
					Druid.Parse ("[J1AQ]"),
					Druid.Parse ("[J1AA1]"),
					Druid.Parse ("[J1AV]"),
					Druid.Parse ("[J1A41]"),
					Druid.Parse ("[J1AE1]"),
					Druid.Parse ("[J1A11]"),
				};

				List<System.Tuple<Druid,Druid>> collectionIds = new List<System.Tuple<Druid, Druid>> ()
				{
					System.Tuple.Create (Druid.Parse ("[J1AB1]"), Druid.Parse ("[J1AC1]")),
					System.Tuple.Create (Druid.Parse ("[J1AN]"), Druid.Parse ("[J1AS]")),
					System.Tuple.Create (Druid.Parse ("[J1AA1]"), Druid.Parse ("[J1AP1]")),
					System.Tuple.Create (Druid.Parse ("[J1AA1]"), Druid.Parse ("[J1AQ1]")),
				};

				EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				SchemaEngine engine = new SchemaEngine (dbInfrastructure, entityTypeEngine);

				engine.LoadSchema (Druid.Parse ("[J1AJ1]"));

				foreach (Druid entityId in entityIds)
				{
					Assert.IsNotNull (engine.GetEntityTableDefinition (entityId));
				}

				foreach (var relationId in collectionIds)
				{
					Assert.IsNotNull (engine.GetCollectionTableDefinition (relationId.Item1, relationId.Item2));
				}
			}
		}


	}


}
