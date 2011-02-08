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
					System.Tuple.Create (Druid.Parse ("[L0A1]"), "L0A1"),
					System.Tuple.Create (Druid.Parse ("[L0AD]"), "L0AD"),
					System.Tuple.Create (Druid.Parse ("[L0AA1]"), "L0AA1"),
					System.Tuple.Create (Druid.Parse ("[L0A61]"), "L0A61"),
					System.Tuple.Create (Druid.Parse ("[L0AP]"), "L0AP"),
					System.Tuple.Create (Druid.Parse ("[L0A82]"), "L0A82"),
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
					System.Tuple.Create (Druid.Parse ("[L0AM]"), "L0AM"),
					System.Tuple.Create (Druid.Parse ("[L0AN]"), "L0AN"),
					System.Tuple.Create (Druid.Parse ("[L0A21]"), "L0A21"),
					System.Tuple.Create (Druid.Parse ("[L0AT]"), "L0AT"),
					System.Tuple.Create (Druid.Parse ("[L0AA1]"), "L0AA1"),
					System.Tuple.Create (Druid.Parse ("[L0AP]"), "L0AP"),
					System.Tuple.Create (Druid.Parse ("[L0AE1]"), "L0AE1"),
					System.Tuple.Create (Druid.Parse ("[L0AQ1]"), "L0AQ1"),
					System.Tuple.Create (Druid.Parse ("[L0AO]"), "L0AO"),
					System.Tuple.Create (Druid.Parse ("[L0AL1]"), "L0AL1"),
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
					System.Tuple.Create (Druid.Parse ("[L0AN]"), Druid.Parse ("[L0AU]"), "L0AU:L0AN"),
					System.Tuple.Create (Druid.Parse ("[L0AN]"), Druid.Parse ("[L0A11]"), "L0A11:L0AN"),
					System.Tuple.Create (Druid.Parse ("[L0AM]"), Druid.Parse ("[L0AS]"), "L0AS:L0AM"),
					System.Tuple.Create (Druid.Parse ("[L0AM]"), Druid.Parse ("[L0AD1]"), "L0AD1:L0AM"),
					System.Tuple.Create (Druid.Parse ("[L0AT]"), Druid.Parse ("[L0AB3]"), "L0AB3:L0AT"),
					System.Tuple.Create (Druid.Parse ("[L0AP]"), Druid.Parse ("[L0AG1]"), "L0AG1:L0AP"),
					System.Tuple.Create (Druid.Parse ("[L0AP]"), Druid.Parse ("[L0AP1]"), "L0AP1:L0AP"),
					System.Tuple.Create (Druid.Parse ("[L0AP]"), Druid.Parse ("[L0A81]"), "L0A81:L0AP"),
					System.Tuple.Create (Druid.Parse ("[L0AP]"), Druid.Parse ("[L0A71]"), "L0A71:L0AP"),
					System.Tuple.Create (Druid.Parse ("[L0AO]"), Druid.Parse ("[L0AO1]"), "L0AO1:L0AO"),
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

				schemaBuilder.RegisterSchema (new List<Druid> () { Druid.Parse ("[L0AM]") });
			}

			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				List<Druid> entityIds = new List<Druid> ()
				{
					Druid.Parse ("[L0AM]"),
					Druid.Parse ("[L0AN]"),
					Druid.Parse ("[L0A21]"),
					Druid.Parse ("[L0AT]"),
					Druid.Parse ("[L0AA1]"),
					Druid.Parse ("[L0AP]"),
					Druid.Parse ("[L0AE1]"),
					Druid.Parse ("[L0AQ1]"),
					Druid.Parse ("[L0AO]"),
					Druid.Parse ("[L0AL1]"),
				};

				List<System.Tuple<Druid,Druid>> relationIds = new List<System.Tuple<Druid, Druid>> ()
				{
					System.Tuple.Create (Druid.Parse ("[L0AN]"), Druid.Parse ("[L0AU]")),
					System.Tuple.Create (Druid.Parse ("[L0AN]"), Druid.Parse ("[L0A11]")),
					System.Tuple.Create (Druid.Parse ("[L0AM]"), Druid.Parse ("[L0AS]")),
					System.Tuple.Create (Druid.Parse ("[L0AM]"), Druid.Parse ("[L0AD1]")),
					System.Tuple.Create (Druid.Parse ("[L0AT]"), Druid.Parse ("[L0AB3]")),
					System.Tuple.Create (Druid.Parse ("[L0AP]"), Druid.Parse ("[L0AG1]")),
					System.Tuple.Create (Druid.Parse ("[L0AP]"), Druid.Parse ("[L0AP1]")),
					System.Tuple.Create (Druid.Parse ("[L0AP]"), Druid.Parse ("[L0A81]")),
					System.Tuple.Create (Druid.Parse ("[L0AP]"), Druid.Parse ("[L0A71]")),
					System.Tuple.Create (Druid.Parse ("[L0AO]"), Druid.Parse ("[L0AO1]")),
				};

				SchemaEngine engine = new SchemaEngine (dbInfrastructure);

				engine.LoadSchema (Druid.Parse ("[L0AN]"));

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
