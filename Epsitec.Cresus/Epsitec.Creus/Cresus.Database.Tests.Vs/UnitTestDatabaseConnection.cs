using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.Database.Tests.Vs
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
			DbInfrastructureHelper.ResetTestDatabase ();
		}


		[TestMethod]
		public void CorrectDatabase()
		{
			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

				dbInfrastructure.AttachToDatabase (dbAccess);
			}
		}


		[TestMethod]
		public void MissingDatabase()
		{
			DbInfrastructureHelper.DeleteTestDatabase ();

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

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
				dbInfrastructure.AttachToDatabase (TestHelper.GetDbAccessForTestDatabase ());

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
				DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();
				
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
				dbInfrastructure.AttachToDatabase (TestHelper.GetDbAccessForTestDatabase ());

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
				DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

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
				dbInfrastructure.AttachToDatabase (TestHelper.GetDbAccessForTestDatabase ());

				using (DbTransaction transaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					DbTable dbTable = dbInfrastructure.ResolveDbTable (Tags.TableTableDef);
					SqlTable sqlTable = dbTable.CreateSqlTable (dbInfrastructure.Converter);

					SqlColumn[] sqlColumns = new SqlColumn[]
					{
						dbTable.Columns[2].CreateSqlColumn (dbInfrastructure.Converter)
					};

					transaction.SqlBuilder.RemoveTableColumns (sqlTable.Name, sqlColumns);

					dbInfrastructure.ExecuteSilent (transaction);

					transaction.Commit ();
				}
			}

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

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
				dbInfrastructure.AttachToDatabase (TestHelper.GetDbAccessForTestDatabase ());

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
				DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

				ExceptionAssert.Throw<System.Exception>
				(
					() => dbInfrastructure.AttachToDatabase (dbAccess)
				);
			}
		}


	}


}
