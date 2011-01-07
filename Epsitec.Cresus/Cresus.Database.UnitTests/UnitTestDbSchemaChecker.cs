using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database.UnitTests
{
	
	
	[TestClass]
	public sealed class UnitTestDbSchemaChecker
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
			Assert.IsTrue (DbSchemaChecker.CheckTables (null, null));
			Assert.IsFalse (DbSchemaChecker.CheckTables (new DbTable (), null));
			Assert.IsFalse (DbSchemaChecker.CheckTables (null, new DbTable ()));
		}


		[TestMethod]
		public void DatabaseSameTest()
		{
			foreach (DbTable table in this.GetSampleTablesDatabase ())
			{
				Assert.IsTrue (DbSchemaChecker.CheckTables (table, table));
			}
		}


		[TestMethod]
		public void MemorySameTest()
		{
			foreach (DbTable table in this.GetSampleTablesMemory ())
			{
				Assert.IsTrue (DbSchemaChecker.CheckTables (table, table));
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

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
		}


		[TestMethod]
		public void IndexesTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ("c1", this.GetDbTypeDefs ().ElementAt (0)));
			table1.Columns.Add (new DbColumn ("c2", this.GetDbTypeDefs ().ElementAt (1)));

			table2.Columns.Add (new DbColumn ("c1", this.GetDbTypeDefs ().ElementAt (0)));
			table2.Columns.Add (new DbColumn ("c2", this.GetDbTypeDefs ().ElementAt (1)));

			table3.Columns.Add (new DbColumn ("c1", this.GetDbTypeDefs ().ElementAt (0)));
			table3.Columns.Add (new DbColumn ("c2", this.GetDbTypeDefs ().ElementAt (1)));

			table1.AddIndex (table1.Columns[0], table1.Columns[1]);
			table2.AddIndex (table2.Columns[0], table2.Columns[1]);
			table3.AddIndex (table3.Columns[1], table3.Columns[0]);

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));

			table1 = new DbTable ();
			table2 = new DbTable ();
			table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ("c1", this.GetDbTypeDefs ().ElementAt (0)));
			table1.Columns.Add (new DbColumn ("c2", this.GetDbTypeDefs ().ElementAt (1)));

			table2.Columns.Add (new DbColumn ("c1", this.GetDbTypeDefs ().ElementAt (0)));
			table2.Columns.Add (new DbColumn ("c2", this.GetDbTypeDefs ().ElementAt (1)));

			table3.Columns.Add (new DbColumn ("c1", this.GetDbTypeDefs ().ElementAt (0)));
			table3.Columns.Add (new DbColumn ("c2", this.GetDbTypeDefs ().ElementAt (1)));

			table1.AddIndex (table1.Columns[0]);
			table1.AddIndex (table1.Columns[1]);
			table2.AddIndex (table2.Columns[0]);
			table2.AddIndex (table2.Columns[1]);
			table3.AddIndex (table3.Columns[0]);

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));

			table1 = new DbTable ();
			table2 = new DbTable ();
			table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ("c1", this.GetDbTypeDefs ().ElementAt (0)));
			table2.Columns.Add (new DbColumn ("c1", this.GetDbTypeDefs ().ElementAt (0)));
			table3.Columns.Add (new DbColumn ("c1", this.GetDbTypeDefs ().ElementAt (0)));

			table1.AddIndex (SqlSortOrder.Ascending, table1.Columns[0]);
			table2.AddIndex (SqlSortOrder.Ascending, table2.Columns[0]);
			table3.AddIndex (SqlSortOrder.Descending, table3.Columns[0]);

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
		}


		[TestMethod]
		public void PrimaryKeysTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ("c1", this.GetDbTypeDefs ().ElementAt (0)));
			table1.Columns.Add (new DbColumn ("c2", this.GetDbTypeDefs ().ElementAt (1)));

			table2.Columns.Add (new DbColumn ("c1", this.GetDbTypeDefs ().ElementAt (0)));
			table2.Columns.Add (new DbColumn ("c2", this.GetDbTypeDefs ().ElementAt (1)));

			table3.Columns.Add (new DbColumn ("c1", this.GetDbTypeDefs ().ElementAt (0)));
			table3.Columns.Add (new DbColumn ("c2", this.GetDbTypeDefs ().ElementAt (1)));

			table1.PrimaryKeys.Add (table1.Columns[0]);
			table1.PrimaryKeys.Add (table1.Columns[1]);
			table2.PrimaryKeys.Add (table2.Columns[1]);
			table2.PrimaryKeys.Add (table2.Columns[0]);
			table3.PrimaryKeys.Add (table3.Columns[0]);

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
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
			
			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
		}


		[TestMethod]
		public void DbKeyTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.DefineKey (new DbKey (new DbId (1)));
			table2.DefineKey (new DbKey (new DbId (2)));

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table3));
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

			DbColumn columnSource1 = DbTable.CreateRelationColumn (Druid.Parse ("[1234]"), tableTarget, DbRevisionMode.IgnoreChanges, DbCardinality.Reference);
			DbColumn columnSource2 = DbTable.CreateRelationColumn (Druid.Parse ("[1234]"), tableTarget, DbRevisionMode.IgnoreChanges, DbCardinality.Reference);
			DbColumn columnSource3 = DbTable.CreateRelationColumn (Druid.Parse ("[1234]"), tableTarget, DbRevisionMode.IgnoreChanges, DbCardinality.Reference);

			DbTable relationTable1 = DbTable.CreateRelationTable (TestHelper.DbInfrastructure, tableSource1, columnSource1);
			DbTable relationTable2 = DbTable.CreateRelationTable (TestHelper.DbInfrastructure, tableSource2, columnSource2);
			DbTable relationTable3 = DbTable.CreateRelationTable (TestHelper.DbInfrastructure, tableSource3, columnSource3);

			Assert.IsTrue (DbSchemaChecker.CheckTables (relationTable1, relationTable2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (relationTable1, relationTable3));
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

			DbColumn columnSource1 = DbTable.CreateRelationColumn (Druid.Parse ("[1234]"), tableTarget1, DbRevisionMode.IgnoreChanges, DbCardinality.Reference);
			DbColumn columnSource2 = DbTable.CreateRelationColumn (Druid.Parse ("[1234]"), tableTarget2, DbRevisionMode.IgnoreChanges, DbCardinality.Reference);
			DbColumn columnSource3 = DbTable.CreateRelationColumn (Druid.Parse ("[1234]"), tableTarget3, DbRevisionMode.IgnoreChanges, DbCardinality.Reference);

			DbTable relationTable1 = DbTable.CreateRelationTable (TestHelper.DbInfrastructure, tableSource, columnSource1);
			DbTable relationTable2 = DbTable.CreateRelationTable (TestHelper.DbInfrastructure, tableSource, columnSource2);
			DbTable relationTable3 = DbTable.CreateRelationTable (TestHelper.DbInfrastructure, tableSource, columnSource3);

			Assert.IsTrue (DbSchemaChecker.CheckTables (relationTable1, relationTable2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (relationTable1, relationTable3));
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

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
		}


		[TestMethod]
		public void DbTableColumnIsAutoTimeStampOnInsertTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ());
			table2.Columns.Add (new DbColumn ());
			table3.Columns.Add (new DbColumn ());

			table1.Columns[0].IsAutoTimeStampOnInsert = true;
			table2.Columns[0].IsAutoTimeStampOnInsert = true;
			table3.Columns[0].IsAutoTimeStampOnInsert = false;

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
		}


		[TestMethod]
		public void DbTableColumnIsAutoTimeStampOnUpdateTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ());
			table2.Columns.Add (new DbColumn ());
			table3.Columns.Add (new DbColumn ());

			table1.Columns[0].IsAutoTimeStampOnUpdate = true;
			table2.Columns[0].IsAutoTimeStampOnUpdate = true;
			table3.Columns[0].IsAutoTimeStampOnUpdate = false;

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
		}


		[TestMethod]
		public void DbTableColumnTypeTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			List<DbTable> tables3 = new List<DbTable> ();

			table1.Columns.Add (new DbColumn ());
			table2.Columns.Add (new DbColumn ());

			table1.Columns[0].DefineType (this.GetDbTypeDefs ().ElementAt (0));
			table2.Columns[0].DefineType (this.GetDbTypeDefs ().ElementAt (0));

			foreach (DbTypeDef dbTypeDef in this.GetDbTypeDefs ().Skip (1))
			{
				DbTable table3 = new DbTable ();
				table3.Columns.Add (new DbColumn ());
				table3.Columns[0].DefineType (dbTypeDef);

				tables3.Add (table3);
			}

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));

			foreach (DbTable table3 in tables3)
			{
				Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
			}
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

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.CheckTables (table1, table2));
			Assert.IsFalse (DbSchemaChecker.CheckTables (table1, table3));
		}


		[TestMethod]
		public void DbTableMemoryDatabase()
		{
			DbTable table1 = this.CreateDbTableSample3 ("myTable");
			DbTable table2 = this.CreateDbTableSample3 ("myTable");

			TestHelper.DbInfrastructure.AddTable (table1);

			DbTable table3 = TestHelper.DbInfrastructure.ResolveDbTable (table1.Name);

			Assert.IsTrue (DbSchemaChecker.CheckTables (table2, table3));
		}


		[TestMethod]
		public void CheckSchemaArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => DbSchemaChecker.CheckSchema (null, new List<DbTable> () { new DbTable () })
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => DbSchemaChecker.CheckSchema (TestHelper.DbInfrastructure , null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => DbSchemaChecker.CheckSchema (TestHelper.DbInfrastructure, new List<DbTable> () { new DbTable (), null })
			);
		}


		[TestMethod]
		public void CheckSchema1()
		{
			List<DbTable> dbTables = new List<DbTable> ()
			{
				TestHelper.DbInfrastructure.ResolveDbTable (Tags.TableColumnDef),
				TestHelper.DbInfrastructure.ResolveDbTable (Tags.TableTableDef),
				TestHelper.DbInfrastructure.ResolveDbTable (Tags.TableTypeDef),
				TestHelper.DbInfrastructure.ResolveDbTable (Tags.TableConnection),
				TestHelper.DbInfrastructure.ResolveDbTable (Tags.TableInfo),
				TestHelper.DbInfrastructure.ResolveDbTable (Tags.TableLock),
				TestHelper.DbInfrastructure.ResolveDbTable (Tags.TableLog),
				TestHelper.DbInfrastructure.ResolveDbTable (Tags.TableUid),
			};

			Assert.IsTrue (DbSchemaChecker.CheckSchema (TestHelper.DbInfrastructure, dbTables));
		}


		[TestMethod]
		public void CheckSchema2()
		{
			List<DbTable> dbTables = new List<DbTable> ()
			{
				TestHelper.DbInfrastructure.ResolveDbTable (Tags.TableColumnDef),
				TestHelper.DbInfrastructure.ResolveDbTable (Tags.TableTableDef),
				TestHelper.DbInfrastructure.ResolveDbTable (Tags.TableTypeDef),
				TestHelper.DbInfrastructure.ResolveDbTable (Tags.TableConnection),
				this.CreateDbTableSample3 ("myUndefinedTable")
			};

			Assert.IsFalse (DbSchemaChecker.CheckSchema (TestHelper.DbInfrastructure, dbTables));
		}


		[TestMethod]
		public void CheckSchema3()
		{
			List<string> names = new List<string> ()
			{
				"myDefinedTable1",
				"myDefinedTable2",
				"myDefinedTable3",
				"myDefinedTable4",
				"myDefinedTable5",
			};

			List<DbTable> dbTables = names.Select (this.CreateDbTableSample3).ToList ();

			foreach (DbTable table in names.Select (this.CreateDbTableSample3).ToList ())
			{
				Assert.IsFalse (DbSchemaChecker.CheckSchema (TestHelper.DbInfrastructure, dbTables));

				TestHelper.DbInfrastructure.AddTable (table);
			}

			Assert.IsTrue (DbSchemaChecker.CheckSchema (TestHelper.DbInfrastructure, dbTables));
		}

				
		private IEnumerable<DbTable> GetSampleTablesMemory()
		{
			yield return this.CreateDbTableSample1 ();
			yield return this.CreateDbTableSample2 ();
			yield return this.CreateDbTableSample3 ("myTable");
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


		private DbTable CreateDbTableSample3(string name)
		{
			DbTable table = new DbTable ();

			table.DefineDisplayName (name);
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


		private IEnumerable<DbTypeDef> GetDbTypeDefs()
		{
			yield return new DbTypeDef ("t", DbSimpleType.String, null, 40, false, DbNullability.Yes);
			yield return new DbTypeDef ("type", DbSimpleType.String, null, 40, false, DbNullability.Yes);
			yield return new DbTypeDef ("t", DbSimpleType.Guid, null, 40, false, DbNullability.Yes);
			yield return new DbTypeDef ("t", DbSimpleType.String, new DbNumDef (1, 0, 0, 9), 40, false, DbNullability.Yes);
			yield return new DbTypeDef ("t", DbSimpleType.String, null, 50, false, DbNullability.Yes);
			yield return new DbTypeDef ("t", DbSimpleType.String, null, 40, true, DbNullability.Yes);
			yield return new DbTypeDef ("t", DbSimpleType.String, null, 40, false, DbNullability.No);
		}


	}


}
