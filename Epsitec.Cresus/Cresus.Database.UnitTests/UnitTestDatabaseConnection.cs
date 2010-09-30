using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Cresus.Database.UnitTests
{


	[TestClass]
	public class UnitTestDatabaseConnection
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			this.DeleteDatabase ();

			TestHelper.CreateDatabase ();
		}


		[TestMethod]
		public void CorrectDatabase()
		{
			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess dbAccess = TestHelper.CreateDbAccess ();

				dbInfrastructure.AttachToDatabase (dbAccess);
			}
		}


		[TestMethod]
		public void MissingDatabase()
		{
			this.DeleteDatabase ();

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess dbAccess = TestHelper.CreateDbAccess ();

				ExceptionAssert.Throw<System.Exception>
				(
					() => dbInfrastructure.AttachToDatabase (dbAccess)
				);
			}
		}


		[TestMethod]
		public void MissingCoreTable()
		{
			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				dbInfrastructure.AttachToDatabase (TestHelper.CreateDbAccess ());

				using (DbTransaction transaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					DbTable dbTable = dbInfrastructure.ResolveDbTable (Tags.TableTableDef);
					SqlTable sqlTable = dbTable.CreateSqlTable (dbInfrastructure.Converter);
					
					transaction.SqlBuilder.RemoveTable (sqlTable);

					dbInfrastructure.ExecuteSilent (transaction);

					transaction.Commit ();
				}
			}

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess dbAccess = TestHelper.CreateDbAccess ();
				
				ExceptionAssert.Throw<System.Exception>
				(
					() => dbInfrastructure.AttachToDatabase (dbAccess)
				);
			}
		}


		[TestMethod]
		public void MissingCoreTableDefinition()
		{
			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				dbInfrastructure.AttachToDatabase (TestHelper.CreateDbAccess ());

				using (DbTransaction transaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					transaction.SqlBuilder.RemoveData (Tags.TableTableDef, new Epsitec.Cresus.Database.Collections.SqlFieldList ()
					{
						SqlField.CreateFunction
						(
							new SqlFunction
							(
								SqlFunctionCode.CompareEqual,
								SqlField.CreateName(Tags.ColumnName),
								SqlField.CreateConstant(Tags.TableTableDef, DbRawType.String)
							)
						)
					});

					dbInfrastructure.ExecuteSilent (transaction);

					transaction.Commit ();
				}
			}

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess dbAccess = TestHelper.CreateDbAccess ();

				ExceptionAssert.Throw<System.Exception>
				(
					() => dbInfrastructure.AttachToDatabase (dbAccess)
				);
			}
		}


		[TestMethod]
		public void ModifiedCoreTable()
		{
			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				dbInfrastructure.AttachToDatabase (TestHelper.CreateDbAccess ());

				using (DbTransaction transaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					DbTable dbTable = dbInfrastructure.ResolveDbTable (Tags.TableTableDef);
					SqlTable sqlTable = dbTable.CreateSqlTable (dbInfrastructure.Converter);

					SqlColumn[] sqlColumns = new SqlColumn[]
					{
						dbTable.Columns[2].CreateSqlColumn (dbInfrastructure.Converter, null)
					};

					transaction.SqlBuilder.RemoveTableColumns (sqlTable.Name, sqlColumns);

					dbInfrastructure.ExecuteSilent (transaction);

					transaction.Commit ();
				}
			}

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess dbAccess = TestHelper.CreateDbAccess ();

				ExceptionAssert.Throw<System.Exception>
				(
					() => dbInfrastructure.AttachToDatabase (dbAccess)
				);
			}
		}


		[TestMethod]
		public void ModifiedCoreTableDefinition()
		{
			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				dbInfrastructure.AttachToDatabase (TestHelper.CreateDbAccess ());

				using (DbTransaction transaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					transaction.SqlBuilder.RemoveData (Tags.TableColumnDef, new Epsitec.Cresus.Database.Collections.SqlFieldList ()
					{
						SqlField.CreateFunction
						(
							new SqlFunction
							(
								SqlFunctionCode.LogicAnd,
								SqlField.CreateFunction
								(
									new SqlFunction
									(
										SqlFunctionCode.CompareEqual,
										SqlField.CreateName(Tags.ColumnName),
										SqlField.CreateConstant(Tags.ColumnName, DbRawType.String)
									)
								),
								SqlField.CreateFunction
								(
									new SqlFunction
									(
										SqlFunctionCode.CompareEqual,
										SqlField.CreateName(Tags.ColumnRefTable),
										SqlField.CreateConstant(1, DbRawType.Int64)
									)
								)
							)
						)
					});

					dbInfrastructure.ExecuteSilent (transaction);

					transaction.Commit ();
				}
			}

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess dbAccess = TestHelper.CreateDbAccess ();

				ExceptionAssert.Throw<System.Exception>
				(
					() => dbInfrastructure.AttachToDatabase (dbAccess)
				);
			}
		}


		[TestMethod]
		public void MissingServiceTable()
		{
			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				dbInfrastructure.AttachToDatabase (TestHelper.CreateDbAccess ());

				using (DbTransaction transaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					DbTable dbTable = dbInfrastructure.ResolveDbTable (Tags.TableInfo);
					SqlTable sqlTable = dbTable.CreateSqlTable (dbInfrastructure.Converter);

					transaction.SqlBuilder.RemoveTable (sqlTable);

					dbInfrastructure.ExecuteSilent (transaction);

					transaction.Commit ();
				}
			}

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess dbAccess = TestHelper.CreateDbAccess ();

				ExceptionAssert.Throw<System.Exception>
				(
					() => dbInfrastructure.AttachToDatabase (dbAccess)
				);
			}
		}


		[TestMethod]
		public void MissingServiceTableDefinition()
		{
			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				dbInfrastructure.AttachToDatabase (TestHelper.CreateDbAccess ());

				using (DbTransaction transaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					transaction.SqlBuilder.RemoveData (Tags.TableTableDef, new Epsitec.Cresus.Database.Collections.SqlFieldList ()
					{
						SqlField.CreateFunction
						(
							new SqlFunction
							(
								SqlFunctionCode.CompareEqual,
								SqlField.CreateName(Tags.ColumnName),
								SqlField.CreateConstant(Tags.TableUid, DbRawType.String)
							)
						)
					});

					dbInfrastructure.ExecuteSilent (transaction);

					transaction.Commit ();
				}
			}

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess dbAccess = TestHelper.CreateDbAccess ();

				ExceptionAssert.Throw<System.Exception>
				(
					() => dbInfrastructure.AttachToDatabase (dbAccess)
				);
			}
		}


		[TestMethod]
		public void ModifiedServiceTable()
		{
			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				dbInfrastructure.AttachToDatabase (TestHelper.CreateDbAccess ());

				using (DbTransaction transaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					DbTable dbTable = dbInfrastructure.ResolveDbTable (Tags.TableUid);
					SqlTable sqlTable = dbTable.CreateSqlTable (dbInfrastructure.Converter);

					SqlColumn[] sqlColumns = new SqlColumn[]
					{
						dbTable.Columns[2].CreateSqlColumn (dbInfrastructure.Converter, null)
					};

					transaction.SqlBuilder.RemoveTableColumns (sqlTable.Name, sqlColumns);

					dbInfrastructure.ExecuteSilent (transaction);

					transaction.Commit ();
				}
			}

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess dbAccess = TestHelper.CreateDbAccess ();

				ExceptionAssert.Throw<System.Exception>
				(
					() => dbInfrastructure.AttachToDatabase (dbAccess)
				);
			}
		}


		[TestMethod]
		public void ModifiedServiceTableDefinition()
		{
			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				dbInfrastructure.AttachToDatabase (TestHelper.CreateDbAccess ());

				using (DbTransaction transaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					transaction.SqlBuilder.RemoveData (Tags.TableColumnDef, new Epsitec.Cresus.Database.Collections.SqlFieldList ()
					{
						SqlField.CreateFunction
						(
							new SqlFunction
							(
								SqlFunctionCode.CompareEqual,
								SqlField.CreateName(Tags.ColumnName),
								SqlField.CreateConstant(Tags.ColumnUidSlot, DbRawType.String)
							)
						),
					});

					dbInfrastructure.ExecuteSilent (transaction);

					transaction.Commit ();
				}
			}

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess dbAccess = TestHelper.CreateDbAccess ();

				ExceptionAssert.Throw<System.Exception>
				(
					() => dbInfrastructure.AttachToDatabase (dbAccess)
				);
			}
		}


		private void DeleteDatabase()
		{
			DbAccess access = TestHelper.CreateDbAccess ();

			string path = DbFactory.GetDatabaseFilePaths (access).First ();

			try
			{
				if (System.IO.File.Exists (path))
				{
					System.IO.File.Delete (path);
				}
			}
			catch (System.IO.IOException ex)
			{
				System.Console.Out.WriteLine ("Cannot delete database file. Error message :\n{0}\nWaiting for 5 seconds...", ex.ToString ());
				System.Threading.Thread.Sleep (5000);

				try
				{
					System.IO.File.Delete (path);
					System.Console.Out.WriteLine ("Finally succeeded");
				}
				catch
				{
					System.Console.Out.WriteLine ("Failed again, giving up");
					throw;
				}
			}
		}
		

	}


}
