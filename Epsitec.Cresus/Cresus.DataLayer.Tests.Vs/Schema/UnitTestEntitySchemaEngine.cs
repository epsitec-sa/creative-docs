using Epsitec.Common.Support;

using Epsitec.Common.Types;

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
	public sealed class UnitTestEntitySchemaEngine
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseCreator2.ResetEmptyTestDatabase ();
		}


		[TestMethod]
		public void SchemaEngineConstructorTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityTypeEngine entityTypeEngine = new EntityTypeEngine (EntityEngineHelper.GetEntityTypeIds ());
				EntitySchemaEngine schemaEngine = new EntitySchemaEngine (dbInfrastructure, entityTypeEngine);
			}
		}


		[TestMethod]
		public void SchemaEngineConstructorArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityTypeEngine entityTypeEngine = new EntityTypeEngine (EntityEngineHelper.GetEntityTypeIds ());

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new EntitySchemaEngine (null, entityTypeEngine)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new EntitySchemaEngine (dbInfrastructure, null)
				);

				EntityTypeEngine entityTypeEngine1 = entityTypeEngine;
				string tableName1 = EntitySchemaBuilder.GetEntityFieldTableName (new Druid ("[J1AN]"), new Druid ("[J1AS]"));
				DbTable table1 = dbInfrastructure.ResolveDbTable (tableName1);
				dbInfrastructure.RemoveTable (table1);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new EntitySchemaEngine (dbInfrastructure, entityTypeEngine1)
				);

				EntityTypeEngine entityTypeEngine2 = new EntityTypeEngine (new List<Druid> () { new Druid ("[J1A6]"), new Druid ("[J1A4]") });
				string tableName2 = EntitySchemaBuilder.GetEntityTableName (new Druid ("[J1A6]"));
				DbTable table2 = dbInfrastructure.ResolveDbTable (tableName2);
				dbInfrastructure.RemoveTable (table2);

				ExceptionAssert.Throw<System.ArgumentException>
				(
				  () => new EntitySchemaEngine (dbInfrastructure, entityTypeEngine2)
				);

				EntityTypeEngine entityTypeEngine3 = new EntityTypeEngine (new List<Druid> () { new Druid ("[J1A4]") });
				string tableName3 = EntitySchemaBuilder.GetEntityTableName (new Druid ("[J1A4]"));
				string columnName3 = EntitySchemaBuilder.GetEntityFieldColumnName (new Druid ("[J1A3]"));
				DbTable table3 = dbInfrastructure.ResolveDbTable (tableName3);
				DbColumn column3 = table2.Columns[columnName3];
				dbInfrastructure.RemoveColumnFromTable (table3, column3);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new EntitySchemaEngine (dbInfrastructure, entityTypeEngine3)
				);
			}
		}


		[TestMethod]
		public void GetEntityTableArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				var entityTypeEngine = new EntityTypeEngine (EntityEngineHelper.GetEntityTypeIds ());
				var entitySchemaEngine = new EntitySchemaEngine (dbInfrastructure, entityTypeEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => entitySchemaEngine.GetEntityTable (Druid.FromLong (999999))
				);
			}
		}


		[TestMethod]
		public void GetEntityTableTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				var entityTypeEngine = new EntityTypeEngine (EntityEngineHelper.GetEntityTypeIds ());
				var entitySchemaEngine = new EntitySchemaEngine (dbInfrastructure, entityTypeEngine);

				var tables = from entityType in entityTypeEngine.GetEntityTypes ()
							let entityTypeId = entityType.CaptionId
							 let entityTableName = EntitySchemaBuilder.GetEntityTableName (entityTypeId)
							let entityTable = dbInfrastructure.ResolveDbTable (entityTableName)
							select new
							{
								Id = entityTypeId,
								Table = entityTable
							};

				foreach (var table in tables)
				{
					DbTable expected = table.Table;
					DbTable actual = entitySchemaEngine.GetEntityTable (table.Id);

					Assert.AreEqual (expected, actual);
				}
			}
		}


		[TestMethod]
		public void GetEntityFieldTableArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				var entityTypeEngine = new EntityTypeEngine (EntityEngineHelper.GetEntityTypeIds ());
				var entitySchemaEngine = new EntitySchemaEngine (dbInfrastructure, entityTypeEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => entitySchemaEngine.GetEntityFieldTable (Druid.FromLong (999999), Druid.FromLong (999999))
				);
			}
		}


		[TestMethod]
		public void GetEntityFieldTableTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				var entityTypeEngine = new EntityTypeEngine (EntityEngineHelper.GetEntityTypeIds ());
				var entitySchemaEngine = new EntitySchemaEngine (dbInfrastructure, entityTypeEngine);

				var tables = from entityType in entityTypeEngine.GetEntityTypes ()
							 let entityTypeId = entityType.CaptionId
							 from field in entityTypeEngine.GetLocalCollectionFields(entityTypeId)
							 let fieldId = field.CaptionId
							 let entityFieldTableName = EntitySchemaBuilder.GetEntityFieldTableName (entityTypeId, fieldId)
							 let entityFieldTable = dbInfrastructure.ResolveDbTable (entityFieldTableName)
							 select new
							 {
								 TypeId = entityTypeId,
								 FieldId = fieldId,
								 Table = entityFieldTable
							 };

				foreach (var table in tables)
				{
					DbTable expected = table.Table;
					DbTable actual = entitySchemaEngine.GetEntityFieldTable (table.TypeId, table.FieldId);

					Assert.AreEqual (expected, actual);
				}
			}
		}


		[TestMethod]
		public void GetEntityFieldColumnArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				var entityTypeEngine = new EntityTypeEngine (EntityEngineHelper.GetEntityTypeIds ());
				var entitySchemaEngine = new EntitySchemaEngine (dbInfrastructure, entityTypeEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => entitySchemaEngine.GetEntityFieldColumn (Druid.FromLong (999999), Druid.FromLong (999999))
				);
			}
		}


		[TestMethod]
		public void GetEntityFieldColumnTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				var entityTypeEngine = new EntityTypeEngine (EntityEngineHelper.GetEntityTypeIds ());
				var entitySchemaEngine = new EntitySchemaEngine (dbInfrastructure, entityTypeEngine);

				var columns = from entityType in entityTypeEngine.GetEntityTypes ()
							  let entityTypeId = entityType.CaptionId
							  let entityTableName = EntitySchemaBuilder.GetEntityTableName (entityTypeId)
							  let entityTable = dbInfrastructure.ResolveDbTable (entityTableName)
							  from field in entityTypeEngine.GetLocalFields (entityTypeId)
							  where field.Relation == FieldRelation.None || field.Relation == FieldRelation.Reference
							  let fieldId = field.CaptionId
							  let entityFieldColumnName = EntitySchemaBuilder.GetEntityFieldColumnName (fieldId)
							  let entityFieldColumn = entityTable.Columns[entityFieldColumnName]
							  select new
							  {
								  TypeId = entityTypeId,
								  FieldId = fieldId,
								  Column = entityFieldColumn,
							  };

				foreach (var column in columns)
				{
					DbColumn expected = column.Column;
					DbColumn actual = entitySchemaEngine.GetEntityFieldColumn (column.TypeId, column.FieldId);

					Assert.AreEqual (expected, actual);
				}
			}
		}


	}


}
