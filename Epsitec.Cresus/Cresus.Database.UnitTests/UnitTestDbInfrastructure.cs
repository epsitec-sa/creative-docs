using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.Exceptions;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.Database.UnitTests
{


	[TestClass]
	public sealed class UnitTestDbInfrastructure
	{


		// TODO Do not use DbInfrastructureHelper in this test? Because DbInfrastructure is precisely
		// what we want to test here.
		// Marc


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			IDbAbstractionHelper.ResetTestDatabase ();
		}


		[TestMethod]
		public void CreateDatabaseTest()
		{
			DbInfrastructureHelper.DeleteTestDatabase ();
			
			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

				infrastructure.CreateDatabase (dbAccess);

				DbTable table;

				table = infrastructure.ResolveDbTable ("CR_TABLE_DEF");

				Assert.IsNotNull (table);
				Assert.AreEqual (1L, table.Key.Id.Value);
				Assert.AreEqual (5, table.Columns.Count);

				table = infrastructure.ResolveDbTable ("CR_COLUMN_DEF");

				Assert.IsNotNull (table);
				Assert.AreEqual (2L, table.Key.Id.Value);
				Assert.AreEqual (8, table.Columns.Count);

				table = infrastructure.ResolveDbTable ("CR_TYPE_DEF");

				Assert.IsNotNull (table);
				Assert.AreEqual (3L, table.Key.Id.Value);
				Assert.AreEqual (5, table.Columns.Count);

				using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
				{
					Assert.AreEqual (0, infrastructure.CountMatchingRows (transaction, "CR_COLUMN_DEF", "CR_NAME", DbSqlStandard.MakeSimpleSqlName ("MyColumn")));
					Assert.AreEqual (3, infrastructure.CountMatchingRows (transaction, "CR_COLUMN_DEF", "CR_NAME", "CR_INFO"));
				}
			}
		}

		
		[TestMethod]
		public void AddTypeTest()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef dbTypeSample1 = new DbTypeDef ("Nom", DbSimpleType.String, null, 40, false, DbNullability.Yes);
				DbTypeDef dbTypeSample2 = new DbTypeDef ("NUPO", DbSimpleType.Decimal, new DbNumDef (4, 0, 1000, 9999), 0, false, DbNullability.Yes);
				DbTypeDef dbTypeSample3 = new DbTypeDef ("IsMale", DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Boolean), 0, false, DbNullability.Yes);

				DbTypeDef dbType1 = infrastructure.ResolveDbType ("Nom");
				DbTypeDef dbType2 = infrastructure.ResolveDbType ("NUPO");
				DbTypeDef dbType3 = infrastructure.ResolveDbType ("IsMale");

				Assert.IsNull (dbType1);
				Assert.IsNull (dbType2);
				Assert.IsNull (dbType3);

				infrastructure.AddType (dbTypeSample1);
				infrastructure.AddType (dbTypeSample2);
				infrastructure.AddType (dbTypeSample3);

				dbType1 = infrastructure.ResolveDbType ("Nom");
				dbType2 = infrastructure.ResolveDbType ("NUPO");
				dbType3 = infrastructure.ResolveDbType ("IsMale");

				Assert.IsTrue (DbSchemaChecker.AreDbTypeDefEqual (dbTypeSample1, dbType1));
				Assert.IsTrue (DbSchemaChecker.AreDbTypeDefEqual (dbTypeSample2, dbType2));
				Assert.IsTrue (DbSchemaChecker.AreDbTypeDefEqual (dbTypeSample3, dbType3));
			}
		}


		[TestMethod]
		public void AddExistingTypeExceptionTest()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef dbTypeSample1 = new DbTypeDef ("Nom", DbSimpleType.String, null, 40, false, DbNullability.Yes);
				DbTypeDef dbTypeSample2 = new DbTypeDef ("Nom", DbSimpleType.String, null, 40, false, DbNullability.Yes);
				
				infrastructure.AddType (dbTypeSample1);

				ExceptionAssert.Throw<GenericException>
				(
					() => infrastructure.AddType (dbTypeSample2)
				);
			}
		}


		[TestMethod]
		public void RemoveTypeTest()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef dbTypeSample1 = new DbTypeDef ("Nom", DbSimpleType.String, null, 40, false, DbNullability.Yes);
				DbTypeDef dbTypeSample2 = new DbTypeDef ("NUPO", DbSimpleType.Decimal, new DbNumDef (4, 0, 1000, 9999), 0, false, DbNullability.Yes);
				DbTypeDef dbTypeSample3 = new DbTypeDef ("IsMale", DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Boolean), 0, false, DbNullability.Yes);

				infrastructure.AddType (dbTypeSample1);
				infrastructure.AddType (dbTypeSample2);
				infrastructure.AddType (dbTypeSample3);

				DbTypeDef dbType1 = infrastructure.ResolveDbType ("Nom");
				DbTypeDef dbType2 = infrastructure.ResolveDbType ("NUPO");
				DbTypeDef dbType3 = infrastructure.ResolveDbType ("IsMale");

				Assert.IsTrue (DbSchemaChecker.AreDbTypeDefEqual (dbTypeSample1, dbType1));
				Assert.IsTrue (DbSchemaChecker.AreDbTypeDefEqual (dbTypeSample2, dbType2));
				Assert.IsTrue (DbSchemaChecker.AreDbTypeDefEqual (dbTypeSample3, dbType3));

				infrastructure.RemoveType (dbType1);
				infrastructure.RemoveType (dbType2);
				infrastructure.RemoveType (dbType3);
				
				dbType1 = infrastructure.ResolveDbType ("Nom");
				dbType2 = infrastructure.ResolveDbType ("NUPO");
				dbType3 = infrastructure.ResolveDbType ("IsMale");
				
				Assert.IsNull (dbType1);
				Assert.IsNull (dbType2);
				Assert.IsNull (dbType3);
			}
		}


		[TestMethod]
		public void RemoveUnexistingTypeExceptionTest()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef dbTypeSample = new DbTypeDef ("Nom", DbSimpleType.String, null, 40, false, DbNullability.Yes);
				
				ExceptionAssert.Throw<GenericException>
				(
					() => infrastructure.RemoveType (dbTypeSample)
				);
			}
		}


		[TestMethod]
		public void RemoveBuiltInTypeExceptionTest()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef dbType = infrastructure.FindBuiltInDbTypes ().First ();

				ExceptionAssert.Throw<GenericException>
				(
					() => infrastructure.RemoveType (dbType)
				);
			}
		}


		[TestMethod]
		public void RemoveUsedTypeExceptionTest()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef dbType  = new DbTypeDef ("Name", DbSimpleType.String, null, 80, false, DbNullability.No);
				DbTable dbTable = infrastructure.CreateDbTable ("SimpleTest", DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges, false);
				DbColumn dbColumn = DbTable.CreateUserDataColumn ("Name", dbType, DbRevisionMode.TrackChanges);

				dbTable.Columns.Add (dbColumn);
				dbTable.UpdateRevisionMode ();

				infrastructure.AddType (dbType);
				infrastructure.AddTable (dbTable);

				ExceptionAssert.Throw<GenericException>
				(
					() => infrastructure.RemoveType (dbType)
				);
			}
		}


		[TestMethod]
		public void ResolveDbTypeTest()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef dbType1 = new DbTypeDef ("Nom", DbSimpleType.String, null, 40, false, DbNullability.Yes);
				DbTypeDef dbType2 = new DbTypeDef ("NUPO", DbSimpleType.Decimal, new DbNumDef (4, 0, 1000, 9999), 0, false, DbNullability.Yes);
				DbTypeDef dbType3 = new DbTypeDef ("IsMale", DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Boolean), 0, false, DbNullability.Yes);

				infrastructure.AddType (dbType1);
				infrastructure.AddType (dbType2);
				infrastructure.AddType (dbType3);
			}

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef dbType1 = infrastructure.ResolveDbType ("Nom");
				DbTypeDef dbType2 = infrastructure.ResolveDbType ("NUPO");
				DbTypeDef dbType3 = infrastructure.ResolveDbType ("IsMale");
				
				Assert.IsNotNull (dbType1);
				Assert.IsNotNull (dbType2);
				Assert.IsNotNull (dbType3);

				Assert.AreEqual ("Nom", dbType1.Name);
				Assert.AreEqual ("NUPO", dbType2.Name);
				Assert.AreEqual ("IsMale", dbType3.Name);
			}
		}


		// TODO Complete test.
		// Marc

		[TestMethod]
		public void CreateDbTableTest()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				infrastructure.DefaultLocalizations = new string[] { "fr", "de", "it", "en" };

				DbTable dbTable1 = infrastructure.CreateDbTable ("SimpleTest", DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges, false);

				DbTypeDef dbTypeName  = new DbTypeDef ("Name", DbSimpleType.String, null, 80, false, DbNullability.No);
				DbTypeDef dbTypeLevel = new DbTypeDef ("Level", DbSimpleType.String, null, 4, false, DbNullability.No);
				DbTypeDef dbTypeType  = new DbTypeDef ("Type", DbSimpleType.String, null, 25, false, DbNullability.Yes, true);
				DbTypeDef dbTypeData  = new DbTypeDef ("Data", DbSimpleType.ByteArray, null, 0, false, DbNullability.Yes);
				DbTypeDef dbTypeGuid  = new DbTypeDef ("Guid", DbSimpleType.Guid, null, 0, false, DbNullability.Yes);

				infrastructure.AddType (dbTypeName);
				infrastructure.AddType (dbTypeLevel);
				infrastructure.AddType (dbTypeType);
				infrastructure.AddType (dbTypeData);
				infrastructure.AddType (dbTypeGuid);

				DbColumn col1 = DbTable.CreateUserDataColumn ("Name", dbTypeName, DbRevisionMode.TrackChanges);
				DbColumn col2 = DbTable.CreateUserDataColumn ("Level", dbTypeLevel);
				DbColumn col3 = DbTable.CreateUserDataColumn ("Type", dbTypeType);
				DbColumn col4 = DbTable.CreateUserDataColumn ("Data", dbTypeData);
				DbColumn col5 = DbTable.CreateUserDataColumn ("Guid", dbTypeGuid);

				dbTable1.Columns.AddRange (new DbColumn[] { col1, col2, col3, col4, col5 });
				dbTable1.DefineLocalizations (infrastructure.DefaultLocalizations);
				dbTable1.UpdateRevisionMode ();
				dbTable1.AddIndex (col1);
				dbTable1.AddIndex (col2);

				infrastructure.AddTable (dbTable1);
				infrastructure.ClearCaches ();

				DbTable dbTable2 = infrastructure.ResolveDbTable ("SimpleTest");

				Assert.IsNotNull (dbTable2);
				Assert.AreEqual (dbTable1.Name, dbTable2.Name);
				Assert.AreEqual (dbTable1.Category, dbTable2.Category);
				Assert.AreEqual (dbTable1.PrimaryKeys.Count, dbTable2.PrimaryKeys.Count);
				Assert.AreEqual (dbTable1.PrimaryKeys[0].Name, dbTable2.PrimaryKeys[0].Name);
				Assert.AreEqual (dbTable1.Columns.Count, dbTable2.Columns.Count);
			}
		}


		// TODO Complete test.
		// Marc

		[TestMethod]
		public void CreateDbTableException()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTypeDef dbTypeName  = new DbTypeDef ("Name", DbSimpleType.String, null, 80, false, DbNullability.No);
				infrastructure.AddType (dbTypeName);
				
				DbTable dbTable1 = infrastructure.CreateDbTable ("SimpleTest", DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges, false);
				infrastructure.AddTable (dbTable1);

				DbTable dbTable2 = infrastructure.CreateDbTable ("SimpleTest", DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges, false);
				
				ExceptionAssert.Throw<Exceptions.GenericException>
				(
					() => infrastructure.AddTable (dbTable2)
				);
			}
		}


		// TODO Complete test.
		// Marc

		[TestMethod]
		public void UnregisterDbTableTest()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable dbTable = infrastructure.CreateDbTable ("SimpleTest", DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges, false);
				infrastructure.AddTable (dbTable);
			}

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable dbTable = infrastructure.ResolveDbTable ("SimpleTest");
				Assert.IsNotNull (dbTable);

				infrastructure.RemoveTable (dbTable);
				Assert.IsNull (infrastructure.ResolveDbTable ("SimpleTest"));
			}
		}


		// TODO Complete test.
		// Marc

		[TestMethod]
		public void RegisterDbTableSameAsUnregisteredTest()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable dbTable = infrastructure.CreateDbTable ("SimpleTest", DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges, false);
				infrastructure.AddTable (dbTable);
			}

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable dbTable = infrastructure.ResolveDbTable ("SimpleTest");			
				infrastructure.RemoveTable (dbTable);
			}

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable dbTable = infrastructure.CreateDbTable ("SimpleTest", DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges, false);
				infrastructure.AddTable (dbTable);

				Assert.IsNotNull (infrastructure.ResolveDbTable ("SimpleTest"));
				Assert.AreEqual (DbRowStatus.Live, dbTable.Key.Status);
			}
		}


		// TODO Complete test.
		// Marc

		[TestMethod]
		public void UnregisterDbTableExeptionTest()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable dbTable = infrastructure.CreateDbTable ("SimpleTest", DbElementCat.ManagedUserData, DbRevisionMode.IgnoreChanges, false);
				infrastructure.AddTable (dbTable);
			}

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTable dbTable = infrastructure.ResolveDbTable ("SimpleTest");
				infrastructure.RemoveTable (dbTable);

				ExceptionAssert.Throw<Exceptions.GenericException>
				(
					() => infrastructure.RemoveTable (dbTable)
				);
			}
		}


		[TestMethod]
		public void MultipleTransactionsTest()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				Assert.IsNotNull (infrastructure);

				using (IDbAbstraction dbAbstraction1 = infrastructure.CreateDatabaseAbstraction ())
				using (IDbAbstraction dbAbstraction2 = infrastructure.CreateDatabaseAbstraction ())
				{
					Assert.AreNotSame (dbAbstraction1, dbAbstraction2);
					Assert.AreNotSame (dbAbstraction1.SqlBuilder, dbAbstraction2.SqlBuilder);
					Assert.AreSame (dbAbstraction1.Factory, dbAbstraction2.Factory);

					using (DbTransaction transaction1 = infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
					using (DbTransaction transaction2 = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, dbAbstraction1))
					using (DbTransaction transaction3 = infrastructure.BeginTransaction (DbTransactionMode.ReadOnly, dbAbstraction2))
					{
						Assert.AreEqual (3, infrastructure.LiveTransactions.Length);
						
						using (DbTransaction transaction4 = infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
						{
							Assert.AreEqual (3, infrastructure.LiveTransactions.Length);
							transaction4.Commit ();
						}

						transaction3.Commit ();
						Assert.AreEqual (2, infrastructure.LiveTransactions.Length);
						
						transaction2.Rollback ();
						Assert.AreEqual (1, infrastructure.LiveTransactions.Length);
						
						transaction1.Commit ();
						Assert.AreEqual (0, infrastructure.LiveTransactions.Length);
					}
				}
			}
		}


		[TestMethod]
		public void MultipleTransactionsExeptionTest1()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTransaction transaction1 = infrastructure.BeginTransaction (DbTransactionMode.ReadOnly);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite)
				);

				transaction1.Commit ();
				transaction1.Dispose ();
			}
		}


		[TestMethod]
		public void MultipleTransactionsExeptionTest2()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTransaction transaction1 = infrastructure.BeginTransaction (DbTransactionMode.ReadOnly);
				DbTransaction transaction2 = infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => transaction2.Dispose ()
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => transaction1.Dispose ()
				);
			}
		}


		[TestMethod]
		public void MultipleTransactionsExeptionTest3()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{		
				DbTransaction transaction1 = infrastructure.BeginTransaction (DbTransactionMode.ReadOnly);
				DbTransaction transaction2 = infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => transaction1.Commit ()
				);
			}
		}


		[TestMethod]
		public void MultipleTransactionsExceptionTest4()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTransaction transaction1 = infrastructure.BeginTransaction (DbTransactionMode.ReadOnly);
				DbTransaction transaction2 = infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => transaction1.Rollback ()
				);
			}
		}


		[TestMethod]
		public void MultipleTransactionsExceptionTest5()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbTransaction transaction1 = infrastructure.BeginTransaction (DbTransactionMode.ReadOnly);
				DbTransaction transaction2 = infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => transaction2.Rollback ()
				);
			}
		}


	}


}
