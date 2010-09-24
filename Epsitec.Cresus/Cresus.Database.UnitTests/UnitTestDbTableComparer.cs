using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Cresus.Database.UnitTests
{
	
	
	[TestClass]
	public sealed class UnitTestDbTableComparer
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			TestHelper.CreateAndConnectToDatabase ();
		}


		[ClassCleanup]
		public static void ClassCleanup()
		{
			TestHelper.DisposeInfrastructure ();
		}


		[TestMethod]
		public void NullTest()
		{
			Assert.IsTrue (DbTableComparer.AreEqual (null, null));
			Assert.IsFalse (DbTableComparer.AreEqual (new DbTable (), null));
			Assert.IsFalse (DbTableComparer.AreEqual (null, new DbTable ()));
		}


		[TestMethod]
		public void DatabaseSameTest()
		{
			foreach (DbTable table in this.GetSampleTablesDatabase ())
			{
				Assert.IsTrue (DbTableComparer.AreEqual (table, table));
			}
		}


		[TestMethod]
		public void MemorySameTest()
		{
			foreach (DbTable table in this.GetSampleTablesMemory ())
			{
				Assert.IsTrue (DbTableComparer.AreEqual (table, table));
			}
		}


		[TestMethod]
		public void NameTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.DefineDisplayName ("name1");
			table2.DefineDisplayName ("name1");
			table3.DefineDisplayName ("name2");

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void CategoryTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.DefineCategory (DbElementCat.Internal);
			table2.DefineCategory (DbElementCat.Internal);
			table3.DefineCategory (DbElementCat.ManagedUserData);

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void CommentTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Comment = "comment1";
			table2.Comment = "comment1";
			table3.Comment = "comment3";

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void CaptionIdTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.DefineCaptionId (Druid.FromLong (1));
			table2.DefineCaptionId (Druid.FromLong (1));
			table3.DefineCaptionId (Druid.FromLong (2));

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void RevisionModeTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.DefineRevisionMode (DbRevisionMode.Immutable);
			table2.DefineRevisionMode (DbRevisionMode.Immutable);
			table3.DefineRevisionMode (DbRevisionMode.TrackChanges);

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void LocalizationsTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.DefineLocalizations (new string[] { "fr", "de", "en", });
			table2.DefineLocalizations (new string[] { "de", "en", "fr", });
			table3.DefineLocalizations (new string[] { "fr", "en", });

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void IndexesTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ("c1", this.GetDbTypeDef1 ()));
			table1.Columns.Add (new DbColumn ("c2", this.GetDbTypeDef2 ()));

			table2.Columns.Add (new DbColumn ("c1", this.GetDbTypeDef1 ()));
			table2.Columns.Add (new DbColumn ("c2", this.GetDbTypeDef2 ()));

			table3.Columns.Add (new DbColumn ("c1", this.GetDbTypeDef1 ()));
			table3.Columns.Add (new DbColumn ("c2", this.GetDbTypeDef2 ()));

			table1.AddIndex (table1.Columns[0], table1.Columns[1]);
			table2.AddIndex (table2.Columns[0], table2.Columns[1]);
			table3.AddIndex (table3.Columns[1], table3.Columns[0]);

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));

			table1 = new DbTable ();
			table2 = new DbTable ();
			table3 = new DbTable ();
			
			table1.Columns.Add (new DbColumn ("c1", this.GetDbTypeDef1 ()));
			table1.Columns.Add (new DbColumn ("c2", this.GetDbTypeDef2 ()));

			table2.Columns.Add (new DbColumn ("c1", this.GetDbTypeDef1 ()));
			table2.Columns.Add (new DbColumn ("c2", this.GetDbTypeDef2 ()));

			table3.Columns.Add (new DbColumn ("c1", this.GetDbTypeDef1 ()));
			table3.Columns.Add (new DbColumn ("c2", this.GetDbTypeDef2 ()));

			table1.AddIndex (table1.Columns[0]);
			table1.AddIndex (table1.Columns[1]);
			table2.AddIndex (table2.Columns[0]);
			table2.AddIndex (table2.Columns[1]);
			table3.AddIndex (table3.Columns[0]);

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));

			table1 = new DbTable ();
			table2 = new DbTable ();
			table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ("c1", this.GetDbTypeDef1 ()));
			table2.Columns.Add (new DbColumn ("c1", this.GetDbTypeDef1 ()));
			table3.Columns.Add (new DbColumn ("c1", this.GetDbTypeDef1 ()));

			table1.AddIndex (SqlSortOrder.Ascending, table1.Columns[0]);
			table2.AddIndex (SqlSortOrder.Ascending, table2.Columns[0]);
			table3.AddIndex (SqlSortOrder.Descending, table3.Columns[0]);

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void PrimaryKeysTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ("c1", this.GetDbTypeDef1 ()));
			table1.Columns.Add (new DbColumn ("c2", this.GetDbTypeDef2 ()));

			table2.Columns.Add (new DbColumn ("c1", this.GetDbTypeDef1 ()));
			table2.Columns.Add (new DbColumn ("c2", this.GetDbTypeDef2 ()));

			table3.Columns.Add (new DbColumn ("c1", this.GetDbTypeDef1 ()));
			table3.Columns.Add (new DbColumn ("c2", this.GetDbTypeDef2 ()));

			table1.PrimaryKeys.Add (table1.Columns[0]);
			table1.PrimaryKeys.Add (table1.Columns[1]);
			table2.PrimaryKeys.Add (table2.Columns[1]);
			table2.PrimaryKeys.Add (table2.Columns[0]);
			table3.PrimaryKeys.Add (table3.Columns[0]);

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void ForeignKeysTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			using (DbTransaction transaction = TestHelper.DbInfrastructure.BeginTransaction())
			{
				table1.Columns.Add (DbTable.CreateRefColumn (transaction, TestHelper.DbInfrastructure, "c1", "t1", DbNullability.No));
				table1.Columns.Add (DbTable.CreateRefColumn (transaction, TestHelper.DbInfrastructure, "c2", "t2", DbNullability.No));
				table2.Columns.Add (DbTable.CreateRefColumn (transaction, TestHelper.DbInfrastructure, "c2", "t2", DbNullability.No));
				table2.Columns.Add (DbTable.CreateRefColumn (transaction, TestHelper.DbInfrastructure, "c1", "t1", DbNullability.No));
				table3.Columns.Add (DbTable.CreateRefColumn (transaction, TestHelper.DbInfrastructure, "c1", "t1", DbNullability.No));
			}
			
			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void DbKeyTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.DefineKey (new DbKey (new DbId (1)));
			table2.DefineKey (new DbKey (new DbId (2)));

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsTrue (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void RelationSourceTableNameTest()
		{
			DbTable tableSource1 = new DbTable ();
			DbTable tableSource2 = new DbTable ();
			DbTable tableSource3 = new DbTable ();

			tableSource1.DefineDisplayName ("sourceA");
			tableSource2.DefineDisplayName ("sourceA");
			tableSource3.DefineDisplayName ("sourceB");

			DbTable tableTarget = new DbTable ();

			tableTarget.DefineDisplayName ("targetA");

			DbColumn columnSource1 = DbTable.CreateRelationColumn (null, null, Druid.Parse ("[1234]"), tableTarget, DbRevisionMode.IgnoreChanges, DbCardinality.Reference);
			DbColumn columnSource2 = DbTable.CreateRelationColumn (null, null, Druid.Parse ("[1234]"), tableTarget, DbRevisionMode.IgnoreChanges, DbCardinality.Reference);
			DbColumn columnSource3 = DbTable.CreateRelationColumn (null, null, Druid.Parse ("[1234]"), tableTarget, DbRevisionMode.IgnoreChanges, DbCardinality.Reference);

			DbTable relationTable1 = DbTable.CreateRelationTable (TestHelper.DbInfrastructure, tableSource1, columnSource1);
			DbTable relationTable2 = DbTable.CreateRelationTable (TestHelper.DbInfrastructure, tableSource2, columnSource2);
			DbTable relationTable3 = DbTable.CreateRelationTable (TestHelper.DbInfrastructure, tableSource3, columnSource3);

			Assert.IsTrue (DbTableComparer.AreEqual (relationTable1, relationTable2));
			Assert.IsFalse (DbTableComparer.AreEqual (relationTable1, relationTable3));
		}


		[TestMethod]
		public void RelationTargetTableNameTest()
		{
			DbTable tableSource = new DbTable ();

			tableSource.DefineDisplayName ("sourceA");

			DbTable tableTarget1 = new DbTable ();
			DbTable tableTarget2 = new DbTable ();
			DbTable tableTarget3 = new DbTable ();

			tableTarget1.DefineDisplayName ("targetA");
			tableTarget2.DefineDisplayName ("targetA");
			tableTarget3.DefineDisplayName ("targetB");

			DbColumn columnSource1 = DbTable.CreateRelationColumn (null, null, Druid.Parse ("[1234]"), tableTarget1, DbRevisionMode.IgnoreChanges, DbCardinality.Reference);
			DbColumn columnSource2 = DbTable.CreateRelationColumn (null, null, Druid.Parse ("[1234]"), tableTarget2, DbRevisionMode.IgnoreChanges, DbCardinality.Reference);
			DbColumn columnSource3 = DbTable.CreateRelationColumn (null, null, Druid.Parse ("[1234]"), tableTarget3, DbRevisionMode.IgnoreChanges, DbCardinality.Reference);

			DbTable relationTable1 = DbTable.CreateRelationTable (TestHelper.DbInfrastructure, tableSource, columnSource1);
			DbTable relationTable2 = DbTable.CreateRelationTable (TestHelper.DbInfrastructure, tableSource, columnSource2);
			DbTable relationTable3 = DbTable.CreateRelationTable (TestHelper.DbInfrastructure, tableSource, columnSource3);

			Assert.IsTrue (DbTableComparer.AreEqual (relationTable1, relationTable2));
			Assert.IsFalse (DbTableComparer.AreEqual (relationTable1, relationTable3));
		}


		[TestMethod]
		public void DbTableColumnCaptionIdTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ());
			table2.Columns.Add (new DbColumn ());
			table3.Columns.Add (new DbColumn ());

			table1.Columns[0].DefineCaptionId (Druid.FromLong (1));
			table2.Columns[0].DefineCaptionId (Druid.FromLong (1));
			table3.Columns[0].DefineCaptionId (Druid.FromLong (2));

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void DbTableColumnCardinalityTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ());
			table2.Columns.Add (new DbColumn ());
			table3.Columns.Add (new DbColumn ());

			table1.Columns[0].DefineCardinality (DbCardinality.Collection);
			table2.Columns[0].DefineCardinality (DbCardinality.Collection);
			table3.Columns[0].DefineCardinality (DbCardinality.Reference);			

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void DbTableColumnCategoryTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ());
			table2.Columns.Add (new DbColumn ());
			table3.Columns.Add (new DbColumn ());

			table1.Columns[0].DefineCategory (DbElementCat.ManagedUserData);
			table2.Columns[0].DefineCategory (DbElementCat.ManagedUserData);
			table3.Columns[0].DefineCategory (DbElementCat.Internal);

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void DbTableColumnClassTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ());
			table2.Columns.Add (new DbColumn ());
			table3.Columns.Add (new DbColumn ());

			table1.Columns[0].DefineColumnClass (DbColumnClass.Data);
			table2.Columns[0].DefineColumnClass (DbColumnClass.Data);
			table3.Columns[0].DefineColumnClass (DbColumnClass.KeyId);

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void DbTableColumnCommentTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ());
			table2.Columns.Add (new DbColumn ());
			table3.Columns.Add (new DbColumn ());

			table1.Columns[0].Comment = "comment1";
			table2.Columns[0].Comment = "comment1";
			table3.Columns[0].Comment = "comment2";

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void DbTableColumnIsAutoIncrementedTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ());
			table2.Columns.Add (new DbColumn ());
			table3.Columns.Add (new DbColumn ());

			table1.Columns[0].IsAutoIncremented = true;
			table2.Columns[0].IsAutoIncremented = true;
			table3.Columns[0].IsAutoIncremented = false;

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void DbTableColumnLocalizationTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ());
			table2.Columns.Add (new DbColumn ());
			table3.Columns.Add (new DbColumn ());

			table1.Columns[0].DefineLocalization (DbColumnLocalization.Localized);
			table2.Columns[0].DefineLocalization (DbColumnLocalization.Localized);
			table3.Columns[0].DefineLocalization (DbColumnLocalization.None);

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void DbTableColumnNameTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ());
			table2.Columns.Add (new DbColumn ());
			table3.Columns.Add (new DbColumn ());

			table1.Columns[0].DefineDisplayName ("name1");
			table2.Columns[0].DefineDisplayName ("name1");
			table3.Columns[0].DefineDisplayName ("name2");

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void DbTableColumnRevisionModeTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ());
			table2.Columns.Add (new DbColumn ());
			table3.Columns.Add (new DbColumn ());

			table1.Columns[0].DefineRevisionMode (DbRevisionMode.TrackChanges);
			table2.Columns[0].DefineRevisionMode (DbRevisionMode.TrackChanges);
			table3.Columns[0].DefineRevisionMode (DbRevisionMode.Immutable);

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void DbTableColumnTableTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.DefineDisplayName ("name1");
			table2.DefineDisplayName ("name1");
			table3.DefineDisplayName ("name2");

			table1.Columns.Add (new DbColumn ());
			table2.Columns.Add (new DbColumn ());
			table3.Columns.Add (new DbColumn ());

			table1.Columns[0].DefineTable (table1);
			table2.Columns[0].DefineTable (table2);
			table3.Columns[0].DefineTable (table3);

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void DbTableColumnTypeTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ());
			table2.Columns.Add (new DbColumn ());
			table3.Columns.Add (new DbColumn ());

			table1.Columns[0].DefineType (this.GetDbTypeDef1 ());
			table2.Columns[0].DefineType (this.GetDbTypeDef1 ());
			table3.Columns[0].DefineType (this.GetDbTypeDef2 ());

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void DbTableColumnTableTargetTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ());
			table2.Columns.Add (new DbColumn ());
			table3.Columns.Add (new DbColumn ());

			table1.Columns[0].DefineTargetTableName ("name1");
			table2.Columns[0].DefineTargetTableName ("name1");
			table3.Columns[0].DefineTargetTableName ("name2");

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void DbTableColumnIsPrimaryKeyTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ());
			table2.Columns.Add (new DbColumn ());
			table3.Columns.Add (new DbColumn ());

			table1.Columns[0].DefinePrimaryKey (true);
			table2.Columns[0].DefinePrimaryKey (true);
			table3.Columns[0].DefinePrimaryKey (false);

			Assert.IsTrue (DbTableComparer.AreEqual (table1, table2));
			Assert.IsFalse (DbTableComparer.AreEqual (table1, table3));
		}


		[TestMethod]
		public void DbTableMemoryDatabase()
		{
			DbTable table1 = this.CreateDbTableSample3 ();
			DbTable table2 = this.CreateDbTableSample3 ();

			TestHelper.DbInfrastructure.RegisterNewDbTable (table1);

			DbTable table3 = TestHelper.DbInfrastructure.ResolveDbTable (table1.Name);

			Assert.IsTrue (DbTableComparer.AreEqual (table2, table3));
		}

		
		private IEnumerable<DbTable> GetSampleTablesMemory()
		{
			yield return this.CreateDbTableSample1 ();
			yield return this.CreateDbTableSample2 ();
			yield return this.CreateDbTableSample3 ();
		}


		private DbTable CreateDbTableSample1()
		{
			return new DbTable ();
		}


		private DbTable CreateDbTableSample2()
		{
			DbTable table = new DbTable ();

			table.DefineCaptionId (Druid.FromLong (1));
			table.DefineCategory (DbElementCat.Internal);
			table.DefineLocalizations (new string[] { "fr", "en", "de" });
			table.DefineRevisionMode (DbRevisionMode.Immutable);

			return table;
		}


		private DbTable CreateDbTableSample3()
		{
			DbTable table = new DbTable ();

			table.DefineDisplayName ("myTable");
			table.DefineCategory (DbElementCat.ManagedUserData);
			table.DefineRevisionMode (DbRevisionMode.Immutable);

			table.Columns.Add (new DbColumn ("myColumn1", TestHelper.DbInfrastructure.ResolveDbType ("K008")));
			table.Columns.Add (new DbColumn ("myColumn2", TestHelper.DbInfrastructure.ResolveDbType ("K008")));

			table.Columns[0].DefineCategory (DbElementCat.ManagedUserData);
			table.Columns[1].DefineCategory (DbElementCat.ManagedUserData);

			table.Columns[0].DefineRevisionMode (DbRevisionMode.Immutable);
			table.Columns[1].DefineRevisionMode (DbRevisionMode.Immutable);

			return table;
		}


        private IEnumerable<DbTable> GetSampleTablesDatabase()
		{
			return from tableName in this.GetSampleTableNames ()
				   select TestHelper.DbInfrastructure.ResolveDbTable (tableName);
		}


		private IEnumerable<string> GetSampleTableNames()
		{
			yield return Tags.TableConnection;
			yield return Tags.TableInfo;
			yield return Tags.TableLock;
			yield return Tags.TableLog;
			yield return Tags.TableTableDef;
			yield return Tags.TableTypeDef;
			yield return Tags.TableUid;
		}


		private DbTypeDef GetDbTypeDef1()
		{
			return new DbTypeDef ("t1", DbSimpleType.String, null, 40, false, DbNullability.Yes);
		}


		private DbTypeDef GetDbTypeDef2()
		{
			return new DbTypeDef ("t2", DbSimpleType.String, null, 40, false, DbNullability.Yes);
		}


	}


}
