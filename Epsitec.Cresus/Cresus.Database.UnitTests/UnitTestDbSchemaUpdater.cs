using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;



namespace Epsitec.Cresus.Database.UnitTests
{


	[TestClass]
	public class UnitTestDbSchemaUpdater
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
		public void InvalidTypeAlterationTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef type1 = new DbTypeDef ("myNewType", DbSimpleType.String, null, 50, true, DbNullability.No);
				DbTypeDef type2 = new DbTypeDef ("myNewType", DbSimpleType.String, null, 50, true, DbNullability.Yes);

				DbTypeDef[] types1 = dbInfrastructure1.FindDbTypes ().Take (2).Append (type1).ToArray ();
				DbTypeDef[] types2 = dbInfrastructure1.FindDbTypes ().Take (2).Append (type2).ToArray ();

				DbTable table1 = this.BuildNewTableWithGivenTypes (3, types1);
				DbTable table2 = this.BuildNewTableWithGivenTypes (3, types2);

				dbInfrastructure1.AddType (type1);
				dbInfrastructure1.AddTable (table1);
				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				List<DbTable> tables = new List<DbTable> ()
				{
					table2,
				};

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables)
				);
			}
		}


		[TestMethod]
		public void InvalidTableAlterationTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable table1 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 3);
				DbTable table2 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 3);
				table2.DefineRevisionMode (DbRevisionMode.Immutable);

				dbInfrastructure1.AddTable (table1);
				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				List<DbTable> tables = new List<DbTable> ()
				{
					table2,
				};

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables)
				);
			}
		}


		[TestMethod]
		public void InvalidTableColumnAlterationTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef[] types1 = dbInfrastructure1.FindDbTypes ().Take (3).ToArray ();
				DbTypeDef[] types2 = dbInfrastructure1.FindDbTypes ().Skip (1).Take (3).ToArray ();

				DbTable table1 = this.BuildNewTableWithGivenTypes (3, types1);
				DbTable table2 = this.BuildNewTableWithGivenTypes (3, types2);

				dbInfrastructure1.AddTable (table1);
				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				List<DbTable> tables = new List<DbTable> ()
				{
					table2,
				};

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables)
				);
			}
		}


		[TestMethod]
		public void NoChangesTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable table = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2);

				dbInfrastructure1.AddTable (table);

				List<DbTable> tables = new List<DbTable> ()
				{
					table,
				};
				
				DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables);

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));
			}

			this.CheckCoreAndServiceTables ();
		}


		[TestMethod]
		public void AddTableTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable table = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2);

				List<DbTable> tables = new List<DbTable> ()
				{
					table,
				};

				DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables);

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));
			}

			this.CheckCoreAndServiceTables ();
		}


		[TestMethod]
		public void RemoveTableTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable table = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2);

				dbInfrastructure1.AddTable (table);
				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				List<DbTable> tables = new List<DbTable> ();

				DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables);

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));
			}

			this.CheckCoreAndServiceTables ();
		}


		[TestMethod]
		public void AddRegularTableColumnTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable table1 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2);
				DbTable table2 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 3);

				dbInfrastructure1.AddTable (table1);
				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				List<DbTable> tables = new List<DbTable> ()
				{
					table2,
				};

				DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables);

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));
			}

			this.CheckCoreAndServiceTables ();
		}


		[TestMethod]
		public void RemoveRegularTableColumnTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable table1 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 3);
				DbTable table2 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2);

				dbInfrastructure1.AddTable (table1);
				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				List<DbTable> tables = new List<DbTable> ()
				{
					table2,
				};

				DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables);

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));
			}

			this.CheckCoreAndServiceTables ();
		}


		[TestMethod]
		public void AddRelationTableColumnTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable table1 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2);
				DbTable table2 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2);

				DbColumn relationColumn = new DbColumn("myRelCol", null, DbColumnClass.Virtual, DbElementCat.ManagedUserData, DbRevisionMode.Immutable);

				relationColumn.DefineCardinality (DbCardinality.Collection);
				relationColumn.DefineTargetTableName (table1.Name);

				table2.Columns.Add (relationColumn);
				
				dbInfrastructure1.AddTable (table1);
				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				List<DbTable> tables = new List<DbTable> ()
				{
					table2,
				};

				DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables);

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));
			}

			this.CheckCoreAndServiceTables ();
		}


		[TestMethod]
		public void RemoveRelationTableColumnTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable table1 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2);
				DbTable table2 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2);

				DbColumn relationColumn = new DbColumn ("myRelCol", null, DbColumnClass.Virtual, DbElementCat.ManagedUserData, DbRevisionMode.Immutable);

				relationColumn.DefineCardinality (DbCardinality.Collection);
				relationColumn.DefineTargetTableName (table1.Name);

				table1.Columns.Add (relationColumn);

				dbInfrastructure1.AddTable (table1);
				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				List<DbTable> tables = new List<DbTable> ()
				{
					table2,
				};

				DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables);

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));
			}

			this.CheckCoreAndServiceTables ();
		}
		

		[TestMethod]
		public void AddTypeTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef type = new DbTypeDef ("myNewType", DbSimpleType.String, null, 50, true, DbNullability.No);

				DbTypeDef[] types = dbInfrastructure1.FindDbTypes ().Take (2).Append (type).ToArray ();

				DbTable table = this.BuildNewTableWithGivenTypes (3, types);

				List<DbTable> tables = new List<DbTable> ()
				{
					table,
				};

				DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables);

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));
			}

			this.CheckCoreAndServiceTables ();
		}


		[TestMethod]
		public void RemoveTypeTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef type = new DbTypeDef ("myNewType", DbSimpleType.String, null, 50, true, DbNullability.No);

				DbTypeDef[] types = dbInfrastructure1.FindDbTypes ().Take (2).Append (type).ToArray ();

				DbTable table = this.BuildNewTableWithGivenTypes (3, types);

				dbInfrastructure1.AddType (type);
				dbInfrastructure1.AddTable (table);
				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				List<DbTable> tables = new List<DbTable> ();

				DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables);

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));
			}

			this.CheckCoreAndServiceTables ();
		}


		[TestMethod]
		public void ComplexTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef type1 = new DbTypeDef ("type1", DbSimpleType.String, null, 50, true, DbNullability.No);
				DbTypeDef type2 = new DbTypeDef ("type2", DbSimpleType.String, null, 50, true, DbNullability.No);
				DbTypeDef type3 = new DbTypeDef ("type3", DbSimpleType.String, null, 50, true, DbNullability.No);

				DbTable table1 = dbInfrastructure1.CreateDbTable ("table1", DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges, true);
				DbTable table2 = dbInfrastructure1.CreateDbTable ("table2", DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges, true);
				DbTable table3 = dbInfrastructure1.CreateDbTable ("table3", DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges, true);

				DbColumn column1 = DbTable.CreateUserDataColumn ("column1", type1, DbRevisionMode.IgnoreChanges);
				DbColumn column2 = DbTable.CreateUserDataColumn ("column2", type2, DbRevisionMode.IgnoreChanges);
				DbColumn column3 = DbTable.CreateUserDataColumn ("column3", type3, DbRevisionMode.IgnoreChanges);

				table1.Columns.Add (column1);
				table2.Columns.Add (column2);
				table3.Columns.Add (column3);

				DbColumn columnRelation1 = DbTable.CreateRelationColumn (Druid.FromLong (0), table2, DbRevisionMode.IgnoreChanges, DbCardinality.Reference);
				DbColumn columnRelation2 = DbTable.CreateRelationColumn (Druid.FromLong (0), table3, DbRevisionMode.IgnoreChanges, DbCardinality.Reference);
				DbColumn columnRelation3 = DbTable.CreateRelationColumn (Druid.FromLong (0), table1, DbRevisionMode.IgnoreChanges, DbCardinality.Reference);

				columnRelation1.DefineDisplayName ("relation1");
				columnRelation2.DefineDisplayName ("relation2");
				columnRelation3.DefineDisplayName ("relation3");

				table1.Columns.Add (columnRelation1);
				table2.Columns.Add (columnRelation2);
				table3.Columns.Add (columnRelation3);

				List<DbTable> tables = new List<DbTable> ()
				{
					table1, table2, table3,
				};

				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				DbSchemaUpdater.UpdateSchema (dbInfrastructure1, tables);

				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure1, tables));
				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));

				DbTable tableA = dbInfrastructure1.ResolveDbTable ("table1");
				DbTable tableB = dbInfrastructure1.ResolveDbTable ("table2");
				DbTable tableC = dbInfrastructure1.CreateDbTable ("table4", DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges, true);

				tableA.Columns.Remove ("column1");
				tableB.Columns.Remove ("0");

				DbColumn columnA = DbTable.CreateUserDataColumn ("columnA", type1, DbRevisionMode.IgnoreChanges);
				DbColumn columnRelationB = DbTable.CreateRelationColumn (Druid.FromLong (1), tableC, DbRevisionMode.IgnoreChanges, DbCardinality.Reference);
				DbColumn columnRelationC = DbTable.CreateRelationColumn (Druid.FromLong (1), dbInfrastructure1.FindBuiltInDbTables ().First (), DbRevisionMode.IgnoreChanges, DbCardinality.Reference);

				columnRelationB.DefineDisplayName ("relationB");
				columnRelationC.DefineDisplayName ("relationC");

				tableA.Columns.Add (columnA);
				tableB.Columns.Add (columnRelationB);
				tableC.Columns.Add (columnRelationC);

				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				tables = new List<DbTable> ()
				{
					tableA, tableB, tableC,
				};

				DbSchemaUpdater.UpdateSchema (dbInfrastructure1, tables);

				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure1, tables));
				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));
			}

			this.CheckCoreAndServiceTables ();
		}


		private DbTable BuildNewTableWithExistingTypes(DbInfrastructure dbInfrastructure, int nbColumns)
		{
			DbTypeDef[] types = dbInfrastructure.FindDbTypes ();

			return this.BuildNewTableWithGivenTypes (nbColumns, types);
		}


		private DbTable BuildNewTableWithGivenTypes(int nbColumns, DbTypeDef[] types)
		{
			DbTable table = new DbTable ("myNewTable");

			table.DefineCategory (DbElementCat.ManagedUserData);

			for (int i = 0; i < nbColumns; i++)
			{
				table.Columns.Add (new DbColumn ("myNewColumn" + i, types[i % types.Length], DbColumnClass.Data, DbElementCat.ManagedUserData, DbRevisionMode.Immutable));
			}

			return table;
		}


		private void CheckCoreAndServiceTables()
		{
			DbInfrastructureHelper.ConnectToTestDatabase ().Dispose ();
		}
                

	}


}
