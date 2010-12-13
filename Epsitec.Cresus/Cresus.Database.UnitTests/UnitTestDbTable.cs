using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.Database.UnitTests
{


	[TestClass]
	public sealed class UnitTestDbTable
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DbTools.DeleteDatabase ("tabletest");
			
			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				infrastructure.CreateDatabase (DbInfrastructure.CreateDatabaseAccess ("tabletest"));
			}

			UnitTestDbTable.infrastructure = TestHelper.GetInfrastructureFromBase ("tabletest");
		}


		[ClassCleanup]
		public static void ClassCleanUp()
		{
			UnitTestDbTable.infrastructure.Dispose ();
		}


		[TestMethod]
		public void ConstructorTest()
		{
			DbTable table = new DbTable ("Test");
			
			Assert.IsNotNull (table);
			Assert.AreEqual ("Test", table.Name);
			Assert.AreEqual (DbElementCat.Unknown, table.Category);
			Assert.AreEqual (false, table.HasPrimaryKeys);
			Assert.AreEqual (0, table.PrimaryKeys.Count);
			Assert.AreEqual (0, table.Columns.Count);
			
			table.DefineCategory (DbElementCat.Internal);
			
			Assert.AreEqual (DbElementCat.Internal, table.Category);
			
			table.DefineCategory (DbElementCat.Internal);
		}

		[TestMethod]
		public void ForeignKeysTest()
		{
			DbTable table = new DbTable ("Test");

			DbColumn column1;
			DbColumn column2;
			DbColumn column3;

			using (DbTransaction transaction = UnitTestDbTable.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				column1 = DbTable.CreateRefColumn (transaction, UnitTestDbTable.infrastructure, "A", "ParentTable", DbNullability.Yes);
				column2 = DbTable.CreateUserDataColumn ("X", new DbTypeDef (new DecimalType (-999999999.999999999M, 999999999.999999999M, 0.000000001M)));
				column3 = DbTable.CreateRefColumn (transaction, UnitTestDbTable.infrastructure, "Z", "Customer", DbNullability.Yes);

				transaction.Commit ();
			}

			table.Columns.Add (column1);
			table.Columns.Add (column2);
			table.Columns.Add (column3);

			DbForeignKey[] foreignKey = Collection.ToArray<DbForeignKey> (table.ForeignKeys);

			Assert.AreEqual (2, foreignKey.Length);
			Assert.AreEqual (1, foreignKey[0].Columns.Length);
			Assert.AreEqual (1, foreignKey[1].Columns.Length);
			Assert.AreEqual (column1, foreignKey[0].Columns[0]);
			Assert.AreEqual (column3, foreignKey[1].Columns[0]);
		}

		[TestMethod]
		public void DefineCategoryException()
		{
			DbTable table = new DbTable ("Test");
			
			table.DefineCategory (DbElementCat.Internal);

			ExceptionAssert.Throw<System.InvalidOperationException>
			(
				() => table.DefineCategory (DbElementCat.ExternalUserData)
			);
		}
		

		[TestMethod]
		public void RelationTableTest()
		{
			DbTable sourceTable = new DbTable ("A");
			DbTable targetTable = new DbTable ("B");
			DbColumn sourceColumn = DbTable.CreateRelationColumn (Druid.Parse ("[1234]"), targetTable, DbRevisionMode.IgnoreChanges, DbCardinality.Reference);

			Assert.AreEqual ("1234", sourceColumn.Name);

			DbTable table = DbTable.CreateRelationTable (UnitTestDbTable.infrastructure, sourceTable, sourceColumn);

			Assert.AreEqual ("1234:A", table.Name);
			Assert.AreEqual ("X_1234_A", table.GetSqlName ());
			Assert.AreEqual (5, table.Columns.Count);

			Assert.AreEqual ("CR_ID", table.Columns[0].Name);
			Assert.AreEqual ("CR_STAT", table.Columns[1].Name);
			Assert.AreEqual ("CREF_SOURCE_ID", table.Columns[2].Name);
			Assert.AreEqual ("CREF_TARGET_ID", table.Columns[3].Name);
			Assert.AreEqual ("CREF_RANK", table.Columns[4].Name);
		}


		private static DbInfrastructure infrastructure;


	}


}
