using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.Collections;
using Epsitec.Cresus.Database.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.IO;

using System.Linq;

using System.Xml;



namespace Epsitec.Cresus.Database.Tests.Vs
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

				DbTable table1 = this.BuildNewTableWithGivenTypes (3, types1, DbElementCat.ManagedUserData);
				DbTable table2 = this.BuildNewTableWithGivenTypes (3, types2, DbElementCat.ManagedUserData);

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
				DbTable table1 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 3, DbElementCat.ManagedUserData);
				DbTable table2 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 3, DbElementCat.ExternalUserData);

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

				DbTable table1 = this.BuildNewTableWithGivenTypes (3, types1, DbElementCat.ManagedUserData);
				DbTable table2 = this.BuildNewTableWithGivenTypes (3, types2, DbElementCat.ManagedUserData);

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
				DbTable table = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);

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
				DbTable table = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);

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
				DbTable table = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);

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
				DbTable table1 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);
				DbTable table2 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 3, DbElementCat.ManagedUserData);

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
				DbTable table1 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 3, DbElementCat.ManagedUserData);
				DbTable table2 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);

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
				DbTable table1 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);
				DbTable table2 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);

				DbColumn relationColumn = new DbColumn ("myRelCol", null, DbColumnClass.Virtual, DbElementCat.ManagedUserData);

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
				DbTable table1 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);
				DbTable table2 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);

				DbColumn relationColumn = new DbColumn ("myRelCol", null, DbColumnClass.Virtual, DbElementCat.ManagedUserData);

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

				DbTable table = this.BuildNewTableWithGivenTypes (3, types, DbElementCat.ManagedUserData);

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

				DbTable table = this.BuildNewTableWithGivenTypes (3, types, DbElementCat.ManagedUserData);

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
		public void AddIndexTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable table1 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);
				DbTable table2 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);

				table2.AddIndex ("idx", SqlSortOrder.Ascending, table2.Columns.Last ());

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
		public void AlterIndexTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable table1 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);
				DbTable table2 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);

				table1.AddIndex ("idx", SqlSortOrder.Ascending, table1.Columns.Last ());
				table2.AddIndex ("idx", SqlSortOrder.Descending, table2.Columns.First ());

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
		public void RemoveIndexTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable table1 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);
				DbTable table2 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);

				table1.AddIndex ("idx", SqlSortOrder.Ascending, table1.Columns.Last ());

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
		public void AlterColumnNullabilityTest1()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable table1 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);
				DbTable table2 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);

				table1.Columns[1].IsNullable = false;
				table2.Columns[1].IsNullable = true;

				dbInfrastructure1.AddTable (table1);
				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				List<DbTable> tables = new List<DbTable> ()
				{
					table2,
				};

				using (DbTransaction transaction = dbInfrastructure1.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					for (int i = 0; i < 10; i++)
					{
						var sqlFields = new SqlFieldList ()
						{
							dbInfrastructure1.CreateSqlFieldFromAdoValue (table1.Columns[0], i),
							dbInfrastructure1.CreateSqlFieldFromAdoValue (table1.Columns[1], i),
						};

						transaction.SqlBuilder.InsertData (table1.GetSqlName (), sqlFields);
						dbInfrastructure1.ExecuteSilent (transaction);
					}

					transaction.Commit ();
				}

				DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables);

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));

				using (DbTransaction transaction = dbInfrastructure2.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					var table2B = dbInfrastructure2.ResolveDbTable (transaction, table2.Name);
					var sqlQuery = new SqlSelect ();

					sqlQuery.Tables.Add ("t", SqlField.CreateName (table2B.GetSqlName ()));
					sqlQuery.Fields.Add ("c", SqlField.CreateName ("t", table2B.Columns[1].GetSqlName ()));

					var result = dbInfrastructure2.ExecuteSqlSelect (transaction, sqlQuery, 0);

					transaction.Commit ();

					for (int i = 0; i < 10; i++)
					{
						Assert.AreEqual ((long) i, result.Rows[i][0]);
					}
				}
			}

			this.CheckCoreAndServiceTables ();
		}


		[TestMethod]
		public void AlterColumnNullabilityTest2()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable table1 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);
				DbTable table2 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);

				table1.Columns[1].IsNullable = true;
				table2.Columns[1].IsNullable = false;

				dbInfrastructure1.AddTable (table1);
				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				List<DbTable> tables = new List<DbTable> ()
				{
					table2,
				};

				using (DbTransaction transaction = dbInfrastructure1.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					for (int i = 0; i < 15; i++)
					{
						var sqlFields = new SqlFieldList ()
						{
							dbInfrastructure1.CreateSqlFieldFromAdoValue (table1.Columns[0], i),
							dbInfrastructure1.CreateSqlFieldFromAdoValue (table1.Columns[1], i < 10 ? (int?) i : null),
						};

						transaction.SqlBuilder.InsertData (table1.GetSqlName (), sqlFields);
						dbInfrastructure1.ExecuteSilent (transaction);
					}

					transaction.Commit ();
				}

				DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables);

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));

				using (DbTransaction transaction = dbInfrastructure2.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					var table2B = dbInfrastructure2.ResolveDbTable (transaction, table2.Name);
					var sqlQuery = new SqlSelect ();

					sqlQuery.Tables.Add ("t", SqlField.CreateName (table2B.GetSqlName ()));
					sqlQuery.Fields.Add ("c", SqlField.CreateName ("t", table2B.Columns[1].GetSqlName ()));

					var result = dbInfrastructure2.ExecuteSqlSelect (transaction, sqlQuery, 0);

					transaction.Commit ();

					for (int i = 0; i < 15; i++)
					{
						long actual = (long) result.Rows[i][0];
						long expected = i < 10 ? i : 0;

						Assert.AreEqual (expected, actual);
					}
				}
			}

			this.CheckCoreAndServiceTables ();
		}


		[TestMethod]
		public void AlterColumnTypeTest1()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef[] typesInfrastructure1 = dbInfrastructure1.FindDbTypes ();
				DbTypeDef[] typesInfrastructure2 = dbInfrastructure2.FindDbTypes ();
				
				DbTypeDef typeInt1 = typesInfrastructure1[11];
				DbTypeDef typeInt2 = typesInfrastructure2[11];
				DbTypeDef typeLong2 = typesInfrastructure2[12];

				DbTypeDef[] types1 = new DbTypeDef[] { typeInt1, typeInt1, };
				DbTypeDef[] types2 = new DbTypeDef[] { typeInt2, typeLong2, };

				DbTable table1 = this.BuildNewTableWithGivenTypes (2, types1, DbElementCat.ManagedUserData);
				DbTable table2 = this.BuildNewTableWithGivenTypes (2, types2, DbElementCat.ManagedUserData);

				dbInfrastructure1.AddTable (table1);
				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				List<DbTable> tables = new List<DbTable> ()
				{
					table2,
				};

				using (DbTransaction transaction = dbInfrastructure1.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					for (int i = 0; i < 10; i++)
					{
						var sqlFields = new SqlFieldList ()
				        {
				            dbInfrastructure1.CreateSqlFieldFromAdoValue (table1.Columns[0], i),
				            dbInfrastructure1.CreateSqlFieldFromAdoValue (table1.Columns[1], i),
				        };

						transaction.SqlBuilder.InsertData (table1.GetSqlName (), sqlFields);
						dbInfrastructure1.ExecuteSilent (transaction);
					}

					transaction.Commit ();
				}

				DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables);

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));

				using (DbTransaction transaction = dbInfrastructure2.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					var table2B = dbInfrastructure2.ResolveDbTable (transaction, table2.Name);
					var sqlQuery = new SqlSelect ();

					sqlQuery.Tables.Add ("t", SqlField.CreateName (table2B.GetSqlName ()));
					sqlQuery.Fields.Add ("c", SqlField.CreateName ("t", table2B.Columns[1].GetSqlName ()));

					var result = dbInfrastructure2.ExecuteSqlSelect (transaction, sqlQuery, 0);

					transaction.Commit ();

					for (int i = 0; i < 10; i++)
					{
						Assert.AreEqual ((long) i, (long) result.Rows[i][0]);
					}
				}
			}

			this.CheckCoreAndServiceTables ();
		}


		[TestMethod]
		public void AlterColumnTypeTest2()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef[] typesInfrastructure1 = dbInfrastructure1.FindDbTypes ();
				DbTypeDef[] typesInfrastructure2 = dbInfrastructure2.FindDbTypes ();

				DbTypeDef typeInt1 = typesInfrastructure1[11];
				DbTypeDef typeInt2 = typesInfrastructure2[11];
				DbTypeDef typeString2 = typesInfrastructure2[13];

				DbTypeDef[] types1 = new DbTypeDef[] { typeInt1, typeInt1, };
				DbTypeDef[] types2 = new DbTypeDef[] { typeInt2, typeString2, };

				DbTable table1 = this.BuildNewTableWithGivenTypes (2, types1, DbElementCat.ManagedUserData);
				DbTable table2 = this.BuildNewTableWithGivenTypes (2, types2, DbElementCat.ManagedUserData);

				dbInfrastructure1.AddTable (table1);
				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				List<DbTable> tables = new List<DbTable> ()
				{
					table2,
				};

				using (DbTransaction transaction = dbInfrastructure1.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					for (int i = 0; i < 10; i++)
					{
						var sqlFields = new SqlFieldList ()
				        {
				            dbInfrastructure1.CreateSqlFieldFromAdoValue (table1.Columns[0], i),
				            dbInfrastructure1.CreateSqlFieldFromAdoValue (table1.Columns[1], i),
				        };

						transaction.SqlBuilder.InsertData (table1.GetSqlName (), sqlFields);
						dbInfrastructure1.ExecuteSilent (transaction);
					}

					transaction.Commit ();
				}

				DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables);

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));

				using (DbTransaction transaction = dbInfrastructure2.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					var table2B = dbInfrastructure2.ResolveDbTable (transaction, table2.Name);
					var sqlQuery = new SqlSelect ();

					sqlQuery.Tables.Add ("t", SqlField.CreateName (table2B.GetSqlName ()));
					sqlQuery.Fields.Add ("c", SqlField.CreateName ("t", table2B.Columns[1].GetSqlName ()));

					var result = dbInfrastructure2.ExecuteSqlSelect (transaction, sqlQuery, 0);

					transaction.Commit ();

					for (int i = 0; i < 10; i++)
					{
						long expected = (long) i;
						long actual = long.Parse ((string) result.Rows[i][0]);

						Assert.AreEqual (expected, actual);
					}
				}
			}

			this.CheckCoreAndServiceTables ();
		}


		[TestMethod]
		public void AlterTableCommentTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable table1 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);
				DbTable table2 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);

				table1.Comment = "old comment";
				table2.Comment = "new comment";

				dbInfrastructure1.AddTable (table1);
				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				List<DbTable> tables = new List<DbTable> ()
				{
					table2,
				};

				DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables);

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure1, tables));
				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));
			}

			this.CheckCoreAndServiceTables ();
		}


		[TestMethod]
		public void AlterColumnCommentTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable table1 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);
				DbTable table2 = this.BuildNewTableWithExistingTypes (dbInfrastructure1, 2, DbElementCat.ManagedUserData);

				table1.Columns["myNewColumn1"].Comment = "old comment";
				table2.Columns["myNewColumn1"].Comment = "new comment";

				dbInfrastructure1.AddTable (table1);
				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				List<DbTable> tables = new List<DbTable> ()
				{
					table2,
				};

				DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables);

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure1, tables));
				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));
			}

			this.CheckCoreAndServiceTables ();
		}


		[TestMethod]
		public void AlterColumnCollationTest1()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef[] typesInfrastructure1 = dbInfrastructure1.FindDbTypes ();
				DbTypeDef[] typesInfrastructure2 = dbInfrastructure2.FindDbTypes ();
				
				DbTypeDef typeString1 = typesInfrastructure1[13];
				DbTypeDef typeString2 = typesInfrastructure2[13];

				DbTypeDef[] types1 = new DbTypeDef[] { typeString1, };
				DbTypeDef[] types2 = new DbTypeDef[] { typeString2, };		
				
				DbTable table1 = this.BuildNewTableWithGivenTypes (2, types1, DbElementCat.ManagedUserData);
				DbTable table2 = this.BuildNewTableWithGivenTypes (2, types2, DbElementCat.ManagedUserData);

				table1.Columns["myNewColumn1"].DefineCollation (DbCollation.Unicode);
				table2.Columns["myNewColumn1"].DefineCollation (DbCollation.UnicodeCi);

				dbInfrastructure1.AddTable (table1);
				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				List<DbTable> tables = new List<DbTable> ()
				{
					table2,
				};

				DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables);

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure1, tables));
				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));
			}

			this.CheckCoreAndServiceTables ();
		}


		[TestMethod]
		public void AlterColumnCollationTest2()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef[] typesInfrastructure1 = dbInfrastructure1.FindDbTypes ();
				DbTypeDef[] typesInfrastructure2 = dbInfrastructure2.FindDbTypes ();

				DbTypeDef typeString1 = typesInfrastructure1[13];
				DbTypeDef typeString2 = typesInfrastructure2[13];

				DbTypeDef[] types1 = new DbTypeDef[] { typeString1, };
				DbTypeDef[] types2 = new DbTypeDef[] { typeString2, };

				DbTable table1 = this.BuildNewTableWithGivenTypes (2, types1, DbElementCat.ManagedUserData);
				DbTable table2 = this.BuildNewTableWithGivenTypes (2, types2, DbElementCat.ManagedUserData);

				var column1 = table1.Columns["myNewColumn1"];
				var column2 = table2.Columns["myNewColumn1"];
				
				column1.DefineCollation (DbCollation.Unicode);
				column2.DefineCollation (DbCollation.UnicodeCi);

				table1.AddIndex ("myIdx", SqlSortOrder.Ascending, column1);
				table2.AddIndex ("myIdx", SqlSortOrder.Ascending, column2);

				dbInfrastructure1.AddTable (table1);
				dbInfrastructure1.ClearCaches ();
				dbInfrastructure2.ClearCaches ();

				List<DbTable> tables = new List<DbTable> ()
				{
					table2,
				};

				DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables);

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure1, tables));
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

				DbTable table1 = dbInfrastructure1.CreateDbTable ("table1", DbElementCat.ManagedUserData, true);
				DbTable table2 = dbInfrastructure1.CreateDbTable ("table2", DbElementCat.ManagedUserData, true);
				DbTable table3 = dbInfrastructure1.CreateDbTable ("table3", DbElementCat.ManagedUserData, true);

				DbColumn column1 = DbTable.CreateUserDataColumn ("column1", type1);
				DbColumn column2a = DbTable.CreateUserDataColumn ("column2a", type2);
				DbColumn column2b = DbTable.CreateUserDataColumn ("column2b", type2);
				DbColumn column3 = DbTable.CreateUserDataColumn ("column3", type3);

				table1.Columns.Add (column1);
				table2.Columns.Add (column2a);
				table2.Columns.Add (column2b);
				table3.Columns.Add (column3);

				DbColumn columnRelation1 = DbTable.CreateRelationColumn (Druid.FromLong (0), table2, DbCardinality.Reference);
				DbColumn columnRelation2 = DbTable.CreateRelationColumn (Druid.FromLong (0), table3, DbCardinality.Reference);
				DbColumn columnRelation3 = DbTable.CreateRelationColumn (Druid.FromLong (0), table1, DbCardinality.Reference);

				columnRelation1.DefineDisplayName ("relation1");
				columnRelation2.DefineDisplayName ("relation2");
				columnRelation3.DefineDisplayName ("relation3");

				table1.Columns.Add (columnRelation1);
				table2.Columns.Add (columnRelation2);
				table3.Columns.Add (columnRelation3);

				table2.AddIndex ("idx2", SqlSortOrder.Ascending, column2b);
				table3.AddIndex ("idx3", SqlSortOrder.Descending, column3);

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
				DbTable tableC = dbInfrastructure1.CreateDbTable ("table4", DbElementCat.ManagedUserData, true);

				tableA.Columns.Remove ("column1");
				tableB.Columns.Remove ("0");

				DbColumn columnA = DbTable.CreateUserDataColumn ("columnA", type1);
				DbColumn columnRelationB = DbTable.CreateRelationColumn (Druid.FromLong (1), tableC, DbCardinality.Reference);
				DbColumn columnRelationC = DbTable.CreateRelationColumn (Druid.FromLong (1), dbInfrastructure1.FindBuiltInDbTables ().First (), DbCardinality.Reference);

				columnRelationB.DefineDisplayName ("relationB");
				columnRelationC.DefineDisplayName ("relationC");

				tableA.Columns.Add (columnA);
				tableB.Columns.Add (columnRelationB);
				tableC.Columns.Add (columnRelationC);

				tableA.AddIndex ("idx1", SqlSortOrder.Ascending, columnA);
				tableB.Indexes.Clear ();
				tableB.AddIndex ("idx2", SqlSortOrder.Descending, tableB.Columns["column2b"]);

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


		[TestMethod]
		public void NewTableWithCopyOfExistingTypeTest()
		{
			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef[] typesCopy =  dbInfrastructure1.FindBuiltInDbTypes ()
					.Take (2)
					.Select (t =>
					{
						using (StringWriter stringWriter = new StringWriter ())
						{
							XmlTextWriter xmlTextWriter = new XmlTextWriter (stringWriter);
							t.Serialize (xmlTextWriter);
							
							using (StringReader stringReader = new StringReader (stringWriter.ToString ()))
							{
								XmlTextReader xmlTextReader = new XmlTextReader (stringReader);
								xmlTextReader.Read ();

								DbTypeDef type = DbTypeDef.Deserialize (xmlTextReader);
								type.DefineKey (DbKey.Empty);

								return type;
							}
						}					
					})
					.ToArray ();

				DbTable table = this.BuildNewTableWithGivenTypes (2, typesCopy, DbElementCat.ManagedUserData);

				List<DbTable> tables = new List<DbTable> ()
				{
					table,
				};

				DbSchemaUpdater.UpdateSchema (dbInfrastructure2, tables);

				Assert.IsTrue (DbSchemaChecker.CheckSchema (dbInfrastructure2, tables));
			}

			this.CheckCoreAndServiceTables ();
		}


		private DbTable BuildNewTableWithExistingTypes(DbInfrastructure dbInfrastructure, int nbColumns, DbElementCat category)
		{
			DbTypeDef[] types = dbInfrastructure.FindDbTypes ();

			return this.BuildNewTableWithGivenTypes (nbColumns, types, category);
		}


		private DbTable BuildNewTableWithGivenTypes(int nbColumns, DbTypeDef[] types, DbElementCat category)
		{
			DbTable table = new DbTable ("myNewTable");

			table.DefineCategory (category);

			for (int i = 0; i < nbColumns; i++)
			{
				table.Columns.Add (new DbColumn ("myNewColumn" + i, types[i % types.Length], DbColumnClass.Data, DbElementCat.ManagedUserData));
			}

			return table;
		}


		private void CheckCoreAndServiceTables()
		{
			DbInfrastructureHelper.ConnectToTestDatabase ().Dispose ();
		}
                

	}


}
