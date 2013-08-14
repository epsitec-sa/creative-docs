﻿using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database.Tests.Vs
{


	[TestClass]
	public sealed class UnitTestDbSchemaChecker
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
		public void NullTest()
		{
			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (null, null));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (new DbTable (), null));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (null, new DbTable ()));
		}


		[TestMethod]
		public void DatabaseSameTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				foreach (DbTable table in this.GetSampleTablesDatabase (dbInfrastructure))
				{
					Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table, table));
				}
			}
		}


		[TestMethod]
		public void MemorySameTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				foreach (DbTable table in this.GetSampleTablesMemory (dbInfrastructure))
				{
					Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table, table));
				}
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

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
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

			table1.AddIndex ("idx", SqlSortOrder.Ascending, table1.Columns[0], table1.Columns[1]);
			table2.AddIndex ("idx", SqlSortOrder.Ascending, table2.Columns[0], table2.Columns[1]);
			table3.AddIndex ("idx", SqlSortOrder.Ascending, table3.Columns[1], table3.Columns[0]);

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));

			table1 = new DbTable ();
			table2 = new DbTable ();
			table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ("c1", this.GetDbTypeDefs ().ElementAt (0)));
			table1.Columns.Add (new DbColumn ("c2", this.GetDbTypeDefs ().ElementAt (1)));

			table2.Columns.Add (new DbColumn ("c1", this.GetDbTypeDefs ().ElementAt (0)));
			table2.Columns.Add (new DbColumn ("c2", this.GetDbTypeDefs ().ElementAt (1)));

			table3.Columns.Add (new DbColumn ("c1", this.GetDbTypeDefs ().ElementAt (0)));
			table3.Columns.Add (new DbColumn ("c2", this.GetDbTypeDefs ().ElementAt (1)));

			table1.AddIndex ("idx1", SqlSortOrder.Ascending, table1.Columns[0]);
			table1.AddIndex ("idx2", SqlSortOrder.Ascending, table1.Columns[1]);
			table2.AddIndex ("idx1", SqlSortOrder.Ascending, table2.Columns[0]);
			table2.AddIndex ("idx2", SqlSortOrder.Ascending, table2.Columns[1]);
			table3.AddIndex ("idx1", SqlSortOrder.Ascending, table3.Columns[0]);

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));

			table1 = new DbTable ();
			table2 = new DbTable ();
			table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ("c1", this.GetDbTypeDefs ().ElementAt (0)));
			table2.Columns.Add (new DbColumn ("c1", this.GetDbTypeDefs ().ElementAt (0)));
			table3.Columns.Add (new DbColumn ("c1", this.GetDbTypeDefs ().ElementAt (0)));

			table1.AddIndex ("idx", SqlSortOrder.Ascending, table1.Columns[0]);
			table2.AddIndex ("idx", SqlSortOrder.Ascending, table2.Columns[0]);
			table3.AddIndex ("idx", SqlSortOrder.Descending, table3.Columns[0]);

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
		}


		[TestMethod]
		public void ForeignKeysTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DbTransaction transaction = dbInfrastructure.BeginTransaction ())
				{
					table1.Columns.Add (DbTable.CreateRefColumn (transaction, dbInfrastructure, "c1", "t1", DbNullability.No));
					table1.Columns.Add (DbTable.CreateRefColumn (transaction, dbInfrastructure, "c2", "t2", DbNullability.No));
					table2.Columns.Add (DbTable.CreateRefColumn (transaction, dbInfrastructure, "c2", "t2", DbNullability.No));
					table2.Columns.Add (DbTable.CreateRefColumn (transaction, dbInfrastructure, "c1", "t1", DbNullability.No));
					table3.Columns.Add (DbTable.CreateRefColumn (transaction, dbInfrastructure, "c1", "t1", DbNullability.No));
				}
			}

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
		}


		[TestMethod]
		public void DbKeyTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.DefineKey (new DbKey (new DbId (1)));
			table2.DefineKey (new DbKey (new DbId (2)));

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));

			foreach (DbTable table3 in tables3)
			{
				Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
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

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
		}


		[TestMethod]
		public void DbTableColumnCollationTest()
		{
			DbTable table1 = new DbTable ();
			DbTable table2 = new DbTable ();
			DbTable table3 = new DbTable ();

			table1.Columns.Add (new DbColumn ());
			table2.Columns.Add (new DbColumn ());
			table3.Columns.Add (new DbColumn ());

			table1.Columns[0].DefineCollation (DbCollation.Unicode);
			table2.Columns[0].DefineCollation (DbCollation.Unicode);
			table3.Columns[0].DefineCollation (DbCollation.UnicodeCiAi);

			Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table1, table2));
			Assert.IsFalse (DbSchemaChecker.AreDbTablesEqual (table1, table3));
		}


		[TestMethod]
		public void DbTableMemoryDatabase()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable table1 = this.CreateDbTableSample3 (dbInfrastructure, "myTable");
				DbTable table2 = this.CreateDbTableSample3 (dbInfrastructure, "myTable");

				dbInfrastructure.AddTable (table1);

				DbTable table3 = dbInfrastructure.ResolveDbTable (table1.Name);

				Assert.IsTrue (DbSchemaChecker.AreDbTablesEqual (table2, table3));
			}
		}


		[TestMethod]
		public void CheckSchemaArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => DbSchemaChecker.CheckSchema (null, new List<DbTable> () { new DbTable () })
			);

			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => DbSchemaChecker.CheckSchema (dbInfrastructure, null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => DbSchemaChecker.CheckSchema (dbInfrastructure, new List<DbTable> () { new DbTable (), null })
				);
			}
		}


		[TestMethod]
		public void CheckSchema1()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				List<DbTable> dbTables = new List<DbTable> ()
				{
					dbInfrastructure.ResolveDbTable (Tags.TableColumnDef),
					dbInfrastructure.ResolveDbTable (Tags.TableTableDef),
					dbInfrastructure.ResolveDbTable (Tags.TableTypeDef),
				};

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure, dbTables));
			}
		}


		[TestMethod]
		public void CheckSchema2()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				List<DbTable> dbTables = new List<DbTable> ()
				{
					dbInfrastructure.ResolveDbTable (Tags.TableColumnDef),
					dbInfrastructure.ResolveDbTable (Tags.TableTableDef),
					dbInfrastructure.ResolveDbTable (Tags.TableTypeDef),
					this.CreateDbTableSample3 (dbInfrastructure, "myUndefinedTable")
				};

				Assert.IsFalse (DbSchemaChecker.CheckSchema (dbInfrastructure, dbTables));
			}
		}


		[TestMethod]
		public void CheckSchema3()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				List<string> names = new List<string> ()
				{
					"myDefinedTable1",
					"myDefinedTable2",
					"myDefinedTable3",
					"myDefinedTable4",
					"myDefinedTable5",
				};

				List<DbTable> dbTables1 = names
					.Select (n => this.CreateDbTableSample3 (dbInfrastructure, n))
					.ToList ();

				List<DbTable> dbTables2 = names
					.Select (n => this.CreateDbTableSample3 (dbInfrastructure, n))
					.ToList ();

				foreach (DbTable table in dbTables2)
				{
					Assert.IsFalse (DbSchemaChecker.CheckSchema (dbInfrastructure, dbTables1));

					dbInfrastructure.AddTable (table);
				}

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure, dbTables1));
			}
		}


		private IEnumerable<DbTable> GetSampleTablesMemory(DbInfrastructure dbInfrastructure)
		{
			yield return this.CreateDbTableSample1 ();
			yield return this.CreateDbTableSample2 ();
			yield return this.CreateDbTableSample3 (dbInfrastructure, "myTable");
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

			return table;
		}


		private DbTable CreateDbTableSample3(DbInfrastructure dbInfrastructure, string name)
		{
			DbTable table = new DbTable ();

			table.DefineDisplayName (name);
			table.DefineCategory (DbElementCat.ManagedUserData);

			table.Columns.Add (new DbColumn ("myColumn1", dbInfrastructure.ResolveDbType ("K008")));
			table.Columns.Add (new DbColumn ("myColumn2", dbInfrastructure.ResolveDbType ("K008")));

			table.Columns[0].DefineCategory (DbElementCat.ManagedUserData);
			table.Columns[1].DefineCategory (DbElementCat.ManagedUserData);

			return table;
		}


		private IEnumerable<DbTable> GetSampleTablesDatabase(DbInfrastructure dbInfrastructure)
		{
			return from tableName in this.GetSampleTableNames ()
				   select dbInfrastructure.ResolveDbTable (tableName);
		}


		private IEnumerable<string> GetSampleTableNames()
		{
			yield return Tags.TableColumnDef;
			yield return Tags.TableTableDef;
			yield return Tags.TableTypeDef;
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
