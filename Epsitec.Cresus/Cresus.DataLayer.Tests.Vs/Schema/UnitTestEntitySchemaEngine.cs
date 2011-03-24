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
		
			DatabaseCreator2.ResetEmptyTestDatabase ();
		}


		[TestMethod]
		public void SchemaEngineConstructorTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				EntitySchemaEngine schemaEngine = new EntitySchemaEngine (dbInfrastructure, entityTypeEngine);
			}
		}


		[TestMethod]
		public void SchemaEngineConstructorArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityTypeEngine entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new EntitySchemaEngine (null, entityTypeEngine)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new EntitySchemaEngine (dbInfrastructure, null)
				);
			}
		}


		[TestMethod]
		public void GetEntityTableNameArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => EntitySchemaEngine.GetEntityTableName (Druid.Empty)
			);
		}


		[TestMethod]
		public void GetEntityTableNameTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			foreach (var type in ete.GetEntityTypes())
			{
				string expected = Druid.ToFullString (type.CaptionId.ToLong ());

				string actual = EntitySchemaEngine.GetEntityTableName (type.CaptionId);

				Assert.AreEqual (expected, actual);
			}
		}


		[TestMethod]
		public void GetEntityFieldTableNameArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => EntitySchemaEngine.GetEntityFieldTableName (Druid.Empty, Druid.FromLong (1))
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => EntitySchemaEngine.GetEntityFieldTableName (Druid.FromLong (1), Druid.Empty)
			);
		}


		[TestMethod]
		public void GetEntityFieldTableNameTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			var fields =
				from entityType in ete.GetEntityTypes ()
				let entityTypeId = entityType.CaptionId
				from field in ete.GetLocalCollectionFields (entityTypeId)
				select new { EntityTypeId = entityTypeId, FieldId = field.CaptionId };

			foreach (var field in fields)
			{
				string expected = Druid.ToFullString (field.EntityTypeId.ToLong ())
					+ ":"
					+ Druid.ToFullString (field.FieldId.ToLong ());

				string actual = EntitySchemaEngine.GetEntityFieldTableName (field.EntityTypeId, field.FieldId);

				Assert.AreEqual (expected, actual);
			}
		}


		[TestMethod]
		public void GetEntityFieldColumnNameArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => EntitySchemaEngine.GetEntityFieldColumnName (Druid.Empty)
			);
		}


		[TestMethod]
		public void GetEntityFieldColumnNameTest()
		{
			EntityTypeEngine ete = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());

			var fieldIds =
				from entityType in ete.GetEntityTypes ()
				let entityTypeId = entityType.CaptionId
				from field in ete.GetLocalFields (entityTypeId)
				where field.Relation == FieldRelation.None || field.Relation == FieldRelation.Reference
				select field.CaptionId;

			foreach (var fieldId in fieldIds)
			{
				string expected = Druid.ToFullString (fieldId.ToLong ());
				string actual = EntitySchemaEngine.GetEntityFieldColumnName (fieldId);

				Assert.AreEqual (expected, actual);
			}
		}


		[TestMethod]
		public void GetEntityTableArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				var entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
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
				var entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				var entitySchemaEngine = new EntitySchemaEngine (dbInfrastructure, entityTypeEngine);

				var tables = from entityType in entityTypeEngine.GetEntityTypes ()
							let entityTypeId = entityType.CaptionId
							let entityTableName = EntitySchemaEngine.GetEntityTableName (entityTypeId)
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
				var entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
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
				var entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				var entitySchemaEngine = new EntitySchemaEngine (dbInfrastructure, entityTypeEngine);

				var tables = from entityType in entityTypeEngine.GetEntityTypes ()
							 let entityTypeId = entityType.CaptionId
							 from field in entityTypeEngine.GetLocalCollectionFields(entityTypeId)
							 let fieldId = field.CaptionId
							 let entityFieldTableName = EntitySchemaEngine.GetEntityFieldTableName (entityTypeId, fieldId)
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
				var entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
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
				var entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				var entitySchemaEngine = new EntitySchemaEngine (dbInfrastructure, entityTypeEngine);

				var columns = from entityType in entityTypeEngine.GetEntityTypes ()
							  let entityTypeId = entityType.CaptionId
							  let entityTableName = EntitySchemaEngine.GetEntityTableName (entityTypeId)
							  let entityTable = dbInfrastructure.ResolveDbTable (entityTableName)
							  from field in entityTypeEngine.GetLocalFields (entityTypeId)
							  where field.Relation == FieldRelation.None || field.Relation == FieldRelation.Reference
							  let fieldId = field.CaptionId
							  let entityFieldColumnName = EntitySchemaEngine.GetEntityFieldColumnName (fieldId)
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


		[TestMethod]
		public void GetEntityTablesTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				var entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				var entitySchemaEngine = new EntitySchemaEngine (dbInfrastructure, entityTypeEngine);

				var expected = from entityType in entityTypeEngine.GetEntityTypes ()
							   let entityTypeId = entityType.CaptionId
							   let entityTableName = EntitySchemaEngine.GetEntityTableName (entityTypeId)
							   let entityTable = dbInfrastructure.ResolveDbTable (entityTableName)
							   orderby entityTable.Name
							   select entityTable;

				var actual = entitySchemaEngine.GetEntityTables ().OrderBy (t => t.Name);

				CollectionAssert.AreEqual (expected.ToList (), actual.ToList ());
			}
		}


		[TestMethod]
		public void GetEntityFieldTablesTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				var entityTypeEngine = new EntityTypeEngine (DataInfrastructureHelper.GetEntityIds ());
				var entitySchemaEngine = new EntitySchemaEngine (dbInfrastructure, entityTypeEngine);

				var expected = from entityType in entityTypeEngine.GetEntityTypes ()
							   let entityTypeId = entityType.CaptionId
							   from field in entityTypeEngine.GetLocalCollectionFields (entityTypeId)
							   let fieldId = field.CaptionId
							   let entityFieldTableName = EntitySchemaEngine.GetEntityFieldTableName (entityTypeId, fieldId)
							   let entityFieldTable = dbInfrastructure.ResolveDbTable (entityFieldTableName)
							   orderby entityFieldTable.Name
							   select entityFieldTable;

				var actual = entitySchemaEngine.GetEntityFieldTables ().OrderBy (t => t.Name);

				CollectionAssert.AreEqual (expected.ToList (), actual.ToList ());
			}
		}
		

	}


}
