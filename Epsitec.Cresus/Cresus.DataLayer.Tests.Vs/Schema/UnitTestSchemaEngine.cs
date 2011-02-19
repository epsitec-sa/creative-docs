using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;



namespace Epsitec.Cresus.DataLayer.UnitTests.Schema
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
				new SchemaEngine (dbInfrastructure);
			}
		}


		[TestMethod]
		public void SchemaEngineConstructorArgumentCheck()
		{
			DbInfrastructure dbInfrastructure = null;

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new SchemaEngine (dbInfrastructure)
			);
		}


		[TestMethod]
		public void GetEntityColumnNameTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				SchemaEngine engine = new SchemaEngine (dbInfrastructure);
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
				SchemaEngine engine = new SchemaEngine (dbInfrastructure);
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
				SchemaEngine engine = new SchemaEngine (dbInfrastructure);
				SchemaEngine_Accessor engineAccessor = new SchemaEngine_Accessor (new PrivateObject (engine));

				List<System.Tuple<Druid,Druid,string>> data = new List<System.Tuple<Druid, Druid, string>> ()
				{
					System.Tuple.Create (Druid.Parse ("[J1AJ1]"), Druid.Parse ("[J1AK1]"), "J1AK1:J1AJ1"),
					System.Tuple.Create (Druid.Parse ("[J1AJ1]"), Druid.Parse ("[J1AN1]"), "J1AN1:J1AJ1"),
					System.Tuple.Create (Druid.Parse ("[J1AB1]"), Druid.Parse ("[J1AC1]"), "J1AC1:J1AB1"),
					System.Tuple.Create (Druid.Parse ("[J1AB1]"), Druid.Parse ("[J1AD1]"), "J1AD1:J1AB1"),
					System.Tuple.Create (Druid.Parse ("[J1AN]"), Druid.Parse ("[J1AS]"), "J1AS:J1AN"),
					System.Tuple.Create (Druid.Parse ("[J1AA1]"), Druid.Parse ("[J1AP1]"), "J1AP1:J1AA1"),
					System.Tuple.Create (Druid.Parse ("[J1AA1]"), Druid.Parse ("[J1AQ1]"), "J1AQ1:J1AA1"),
					System.Tuple.Create (Druid.Parse ("[J1AA1]"), Druid.Parse ("[J1AS1]"), "J1AS1:J1AA1"),
					System.Tuple.Create (Druid.Parse ("[J1AA1]"), Druid.Parse ("[J1AR1]"), "J1AR1:J1AA1"),
					System.Tuple.Create (Druid.Parse ("[J1AE1]"), Druid.Parse ("[J1AF1]"), "J1AF1:J1AE1"),
				};

				foreach (var d in data)
				{
					Druid entityId = d.Item1;
					Druid fieldId = d.Item2;
					string name = d.Item3;

					Assert.AreEqual (name, engineAccessor.GetRelationTableName (entityId, fieldId));
				}
			}
		}


		[TestMethod]
		public void LoadSchemaTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				SchemaBuilder schemaBuilder = new SchemaBuilder (dbInfrastructure);

				schemaBuilder.RegisterSchema (new List<Druid> () { Druid.Parse ("[J1AB1]") });
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

				List<System.Tuple<Druid,Druid>> relationIds = new List<System.Tuple<Druid, Druid>> ()
				{
					System.Tuple.Create (Druid.Parse ("[J1AJ1]"), Druid.Parse ("[J1AK1]")),
					System.Tuple.Create (Druid.Parse ("[J1AJ1]"), Druid.Parse ("[J1AN1]")),
					System.Tuple.Create (Druid.Parse ("[J1AB1]"), Druid.Parse ("[J1AC1]")),
					System.Tuple.Create (Druid.Parse ("[J1AB1]"), Druid.Parse ("[J1AD1]")),
					System.Tuple.Create (Druid.Parse ("[J1AN]"), Druid.Parse ("[J1AS]")),
					System.Tuple.Create (Druid.Parse ("[J1AA1]"), Druid.Parse ("[J1AP1]")),
					System.Tuple.Create (Druid.Parse ("[J1AA1]"), Druid.Parse ("[J1AQ1]")),
					System.Tuple.Create (Druid.Parse ("[J1AA1]"), Druid.Parse ("[J1AS1]")),
					System.Tuple.Create (Druid.Parse ("[J1AA1]"), Druid.Parse ("[J1AR1]")),
					System.Tuple.Create (Druid.Parse ("[J1AE1]"), Druid.Parse ("[J1AF1]")),
				};

				SchemaEngine engine = new SchemaEngine (dbInfrastructure);

				engine.LoadSchema (Druid.Parse ("[J1AJ1]"));

				foreach (Druid entityId in entityIds)
				{
					Assert.IsNotNull (engine.GetEntityTableDefinition (entityId));
				}

				foreach (var relationId in relationIds)
				{
					Assert.IsNotNull (engine.GetRelationTableDefinition (relationId.Item1, relationId.Item2));
				}
			}
		}


	}


}
