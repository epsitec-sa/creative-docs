using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Data;


namespace Epsitec.Cresus.Database.UnitTests
{


	[TestClass]
	public sealed class UnitTestISqlBuilder
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DbTools.DeleteDatabase ("test");
		}


		[TestMethod]
		public void InsertTablePrimaryKeyException()
		{
			using (IDbAbstraction dbAbstraction = TestHelper.CreateDbAbstraction (true))
			{
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;

				SqlTable  sqlTable = new SqlTable ("Test");
				SqlColumn sqlColA = new SqlColumn ("A", DbRawType.Int32);
				SqlColumn sqlColB = new SqlColumn ("B", DbRawType.Int64, DbNullability.Yes);
				SqlColumn sqlColC = new SqlColumn ("C", DbRawType.Int32);

				sqlTable.Columns.Add (sqlColA);
				sqlTable.Columns.Add (sqlColC);

				sqlTable.PrimaryKey = new SqlColumn[] { sqlColA, sqlColB };

				ExceptionAssert.Throw<Exceptions.SyntaxException>
				(
					() => sqlBuilder.InsertTable (sqlTable)
				);
			}
		}


		[TestMethod]
		public void InsertTableTest()
		{
			using (IDbAbstraction dbAbstraction = TestHelper.CreateDbAbstraction (true))
			{
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;

				SqlTable sqlTable = new SqlTable ();

				SqlColumn sqlCol1 = new SqlColumn ("Cr_ID", DbRawType.Int32);
				SqlColumn sqlCol2 = new SqlColumn ("Cr_REV", DbRawType.Int32);
				SqlColumn sqlCol3 = new SqlColumn ("StringDynamic", DbRawType.String, 100, false, DbNullability.Yes);
				SqlColumn sqlCol4 = new SqlColumn ("StringFixed", DbRawType.String, 50, false, DbNullability.Yes);

				sqlTable.Name = "FbTestTable";
				sqlTable.Columns.Add (sqlCol1);
				sqlTable.Columns.Add (sqlCol2);
				sqlTable.Columns.Add (sqlCol3);
				sqlTable.Columns.Add (sqlCol4);
				sqlTable.PrimaryKey = new SqlColumn[] { sqlCol1, sqlCol2 };

				sqlBuilder.InsertTable (sqlTable);

				IDbCommand command = sqlBuilder.Command;

				string[] commands = command.CommandText.Split (new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

				command.Transaction = dbAbstraction.BeginReadWriteTransaction ();

				foreach (string c in commands)
				{
					command.CommandText = c;
					command.ExecuteNonQuery ();
				}

				IDbTransaction transaction = command.Transaction;

				transaction.Commit ();
				transaction.Dispose ();
				command.Dispose ();
			}
		}


		[TestMethod]
		public void InsertTableColumnsTest()
		{
			this.InsertTableTest ();

			using (IDbAbstraction dbAbstraction = TestHelper.CreateDbAbstraction (false))
			{
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;
				ISqlEngine sqlEngine = dbAbstraction.SqlEngine;

				SqlColumn sqlCol1 = new SqlColumn ("Cr_ID2", DbRawType.Int32);
				SqlColumn sqlCol2 = new SqlColumn ("Cr_REV2", DbRawType.Int32);
				SqlColumn sqlCol3 = new SqlColumn ("StringDynamic2", DbRawType.String, 100, true, DbNullability.Yes);
				SqlColumn sqlCol4 = new SqlColumn ("StringFixed2", DbRawType.String, 50, false, DbNullability.Yes);

				SqlColumn[] columns = { sqlCol1, sqlCol2, sqlCol3, sqlCol4 };
				sqlBuilder.InsertTableColumns ("FbTestTable", columns);

				IDbCommand command = sqlBuilder.Command;

				int result;
				command.Transaction = dbAbstraction.BeginReadWriteTransaction ();
				sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out result);

				Assert.AreEqual (-4, result);

				IDbTransaction transaction = command.Transaction;

				transaction.Commit ();
				transaction.Dispose ();
				command.Dispose ();
			}
		}


		[TestMethod]
		public void RemoveTableColumnsTest()
		{
			this.InsertTableColumnsTest ();

			using (IDbAbstraction dbAbstraction = TestHelper.CreateDbAbstraction (false))
			{
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;
				ISqlEngine sqlEngine = dbAbstraction.SqlEngine;

				SqlColumn sqlCol1 = new SqlColumn ("Cr_ID2", DbRawType.Int32);
				SqlColumn sqlCol2 = new SqlColumn ("Cr_REV2", DbRawType.Int32);
				SqlColumn sqlCol3 = new SqlColumn ("StringDynamic2", DbRawType.String, 100, true, DbNullability.Yes);
				SqlColumn sqlCol4 = new SqlColumn ("StringFixed2", DbRawType.String, 50, false, DbNullability.Yes);

				SqlColumn[] columns = { sqlCol1, sqlCol2, sqlCol3, sqlCol4 };
				sqlBuilder.RemoveTableColumns ("FbTestTable", columns);

				IDbCommand command = sqlBuilder.Command;

				int result;
				command.Transaction = dbAbstraction.BeginReadWriteTransaction ();
				sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out result);

				Assert.AreEqual (-4, result);

				IDbTransaction transaction = command.Transaction;

				transaction.Commit ();
				transaction.Dispose ();
				command.Dispose ();
			}
		}


		[TestMethod]
		public void InsertDataTest()
		{
			this.InsertTableTest ();

			using (IDbAbstraction dbAbstraction = TestHelper.CreateDbAbstraction (false))
			{
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;
				ISqlEngine sqlEngine = dbAbstraction.SqlEngine;

				Collections.SqlFieldList fields = new Collections.SqlFieldList ();

				SqlField field1 = SqlField.CreateConstant (123, DbRawType.Int32);
				field1.Alias = "Cr_ID";
				fields.Add (field1);

				SqlField field2 = SqlField.CreateConstant (456, DbRawType.Int32);
				field2.Alias = "Cr_REV";
				fields.Add (field2);

				SqlField field3 = SqlField.CreateConstant ("Test © Copyright 2003", DbRawType.String);
				field3.Alias = "StringDynamic";
				fields.Add (field3);

				sqlBuilder.InsertData ("FbTestTable", fields);

				IDbCommand command = sqlBuilder.Command;

				int result;
				command.Transaction = dbAbstraction.BeginReadWriteTransaction ();
				sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out result);

				Assert.AreEqual (1, result);

				IDbTransaction transaction = command.Transaction;

				transaction.Commit ();
				transaction.Dispose ();
				command.Dispose ();
			}
		}


		[TestMethod]
		public void InsertDataMoreTest()
		{
			this.InsertTableTest ();

			using (IDbAbstraction dbAbstraction = TestHelper.CreateDbAbstraction (false))
			{
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;
				ISqlEngine sqlEngine = dbAbstraction.SqlEngine;

				Collections.SqlFieldList fields1 = new Collections.SqlFieldList ();

				SqlField field1 = SqlField.CreateConstant (200, DbRawType.Int32);
				field1.Alias = "Cr_ID";
				fields1.Add (field1);

				SqlField field2 = SqlField.CreateConstant (0, DbRawType.Int32);
				field2.Alias = "Cr_REV";
				fields1.Add (field2);

				SqlField field3 = SqlField.CreateConstant ("First line...", DbRawType.String);
				field3.Alias = "StringDynamic";
				fields1.Add (field3);

				sqlBuilder.InsertData ("FbTestTable", fields1);
				sqlBuilder.AppendMore ();

				Collections.SqlFieldList fields2 = new Collections.SqlFieldList ();

				SqlField field4 = SqlField.CreateConstant (201, DbRawType.Int32);
				field4.Alias = "Cr_ID";
				fields2.Add (field4);

				SqlField field5 = SqlField.CreateConstant (0, DbRawType.Int32);
				field5.Alias = "Cr_REV";
				fields2.Add (field5);

				SqlField field6 = SqlField.CreateConstant ("Second line...", DbRawType.String);
				field6.Alias = "StringDynamic";
				fields2.Add (field6);

				sqlBuilder.InsertData ("FbTestTable", fields2);

				IDbCommand command = sqlBuilder.Command;

				command.Transaction = dbAbstraction.BeginReadWriteTransaction ();

				int result;

				sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out result);

				IDbTransaction transaction = command.Transaction;

				Assert.AreEqual (2, result);

				transaction.Commit ();
				transaction.Dispose ();
				command.Dispose ();
			}
		}


		[TestMethod]
		public void RemoveDataTest()
		{
			this.InsertDataTest ();

			using (IDbAbstraction dbAbstraction = TestHelper.CreateDbAbstraction (false))
			{
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;
				ISqlEngine sqlEngine = dbAbstraction.SqlEngine;

				Collections.SqlFieldList conditions = new Collections.SqlFieldList ();

				SqlField field1 = SqlField.CreateConstant (123, DbRawType.Int32);
				field1.Alias = "Cr_ID";

				SqlField field2 = SqlField.CreateConstant (456, DbRawType.Int32);
				field2.Alias = "Cr_REV";

				SqlFunction condition1 = new SqlFunction
				(
					SqlFunctionCode.CompareEqual,
					SqlField.CreateName ("Cr_ID"),
					field1
				);

				conditions.Add (SqlField.CreateFunction (condition1));

				SqlFunction condition2 = new SqlFunction
				(
					SqlFunctionCode.CompareEqual,
					SqlField.CreateName ("Cr_REV"),
					field2
				);

				conditions.Add (SqlField.CreateFunction (condition2));

				sqlBuilder.RemoveData ("FbTestTable", conditions);

				IDbCommand command = sqlBuilder.Command;

				int result;
				command.Transaction = dbAbstraction.BeginReadWriteTransaction ();
				sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out result);

				Assert.AreEqual (1, result);

				IDbTransaction transaction = command.Transaction;

				transaction.Commit ();
				transaction.Dispose ();
				command.Dispose ();
			}
		}


		[TestMethod]
		public void RemoveTableTest()
		{
			this.InsertTableTest ();

			using (IDbAbstraction dbAbstraction = TestHelper.CreateDbAbstraction (false))
			{
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;

				SqlTable sqlTable = new SqlTable ("FbTestTable");

				sqlBuilder.RemoveTable (sqlTable);

				IDbCommand command = sqlBuilder.Command;

				command.Transaction = dbAbstraction.BeginReadWriteTransaction ();
				command.ExecuteNonQuery ();

				IDbTransaction transaction = command.Transaction;

				transaction.Commit ();
				transaction.Dispose ();
				command.Dispose ();
			}
		}


		[TestMethod]
		public void InsertTableWithSqlEngineTest()
		{
			using (IDbAbstraction dbAbstraction = TestHelper.CreateDbAbstraction (true))
			{
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;
				ISqlEngine sqlEngine = dbAbstraction.SqlEngine;

				SqlTable  sqlTable = new SqlTable ();

				SqlColumn sqlCol1 = new SqlColumn ("Cr_ID", DbRawType.Int32);
				SqlColumn sqlCol2 = new SqlColumn ("Cr_REV", DbRawType.Int32);
				SqlColumn sqlCol3 = new SqlColumn ("StringDynamic", DbRawType.String, 100, false, DbNullability.Yes);
				SqlColumn sqlCol4 = new SqlColumn ("StringFixed", DbRawType.String, 50, false, DbNullability.Yes);

				sqlTable.Name = "FbTestTable";
				sqlTable.Columns.AddRange (new SqlColumn[] { sqlCol1, sqlCol2, sqlCol3, sqlCol4 });
				sqlTable.PrimaryKey = new SqlColumn[] { sqlCol1, sqlCol2 };

				sqlBuilder.InsertTable (sqlTable);

				IDbCommand command = sqlBuilder.Command;
				DbCommandType command_type = sqlBuilder.CommandType;

				int result;
				command.Transaction = dbAbstraction.BeginReadWriteTransaction ();
				sqlEngine.Execute (command, command_type, sqlBuilder.CommandCount, out result);

				Assert.AreEqual (-2, result);

				IDbTransaction transaction = command.Transaction;

				transaction.Commit ();
				transaction.Dispose ();
				command.Dispose ();
			}
		}


		[TestMethod]
		public void RemoveTableWithSqlEngineTest()
		{
			this.InsertTableWithSqlEngineTest ();

			using (IDbAbstraction dbAbstraction = TestHelper.CreateDbAbstraction (false))
			{
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;
				ISqlEngine sqlEngine = dbAbstraction.SqlEngine;

				SqlTable sqlTable = new SqlTable ("FbTestTable");

				sqlBuilder.RemoveTable (sqlTable);

				IDbCommand command = sqlBuilder.Command;
				DbCommandType command_type = sqlBuilder.CommandType;

				int result;
				command.Transaction = dbAbstraction.BeginReadWriteTransaction ();
				sqlEngine.Execute (command, command_type, sqlBuilder.CommandCount, out result);

				Assert.AreEqual (-1, result);

				IDbTransaction transaction = command.Transaction;

				transaction.Commit ();
				transaction.Dispose ();
				command.Dispose ();
			}
		}


		[TestMethod]
		public void ArrayTableTest()
		{
			using (IDbAbstraction dbAbstraction = TestHelper.CreateDbAbstraction (true))
			{
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;
				ISqlEngine sqlEngine = dbAbstraction.SqlEngine;

				sqlBuilder.AutoClear = true;

				IDbCommand command;

				SqlTable  sqlTable = new SqlTable ();

				SqlColumn sqlCol1 = new SqlColumn ("Cr_ID", DbRawType.Int32);
				SqlColumn sqlCol2 = new SqlColumn ("ArrayDynamic", DbRawType.ByteArray);

				sqlTable.Name = "FbTestArrayTable";
				sqlTable.Columns.Add (sqlCol1);
				sqlTable.Columns.Add (sqlCol2);
				sqlTable.PrimaryKey = new SqlColumn[] { sqlCol1 };

				sqlBuilder.InsertTable (sqlTable);

				command	= sqlBuilder.Command;

				int result;
				command.Transaction = dbAbstraction.BeginReadWriteTransaction ();
				sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out result);

				Assert.AreEqual (-2, result);

				command.Transaction.Commit ();
				command.Dispose ();

				byte[] insertValues = new byte[10000];

				for (int i = 0; i < 10000; i++)
				{
					insertValues[i] = (byte) (i);
				}

				Collections.SqlFieldList fields = new Collections.SqlFieldList ();

				SqlField fieldId = SqlField.CreateConstant (1, DbRawType.Int32);
				fieldId.Alias = "Cr_ID";
				fields.Add (fieldId);

				SqlField fieldBlob = SqlField.CreateConstant (insertValues, DbRawType.ByteArray);
				fieldBlob.Alias = "ArrayDynamic";
				fields.Add (fieldBlob);

				sqlBuilder.InsertData ("FbTestArrayTable", fields);

				command = sqlBuilder.Command;

				command.Transaction = dbAbstraction.BeginReadWriteTransaction ();
				sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out result);

				Assert.AreEqual (1, result);

				command.Transaction.Commit ();
				command.Dispose ();

				SqlSelect sqlSelect = new SqlSelect ();

				sqlSelect.Fields.Add (SqlField.CreateName ("Cr_ID"));
				sqlSelect.Fields.Add (SqlField.CreateName ("ArrayDynamic"));
				sqlSelect.Tables.Add (SqlField.CreateName ("FbTestArrayTable"));

				SqlFunction condition = new SqlFunction
				(
					SqlFunctionCode.CompareEqual,
					SqlField.CreateName ("Cr_ID"),
					fieldId
				);
				sqlSelect.Conditions.Add (SqlField.CreateFunction (condition));

				sqlBuilder.SelectData (sqlSelect);

				command = sqlBuilder.Command;
				command.Transaction = dbAbstraction.BeginReadWriteTransaction ();

				DataSet dataSet;
				sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out dataSet);

				Assert.AreEqual (typeof (int), dataSet.Tables[0].Columns[0].DataType);
				Assert.AreEqual (typeof (byte[]), dataSet.Tables[0].Columns[1].DataType);

				DataTable  dataTable  = dataSet.Tables[0];
				DataColumn blobColumn = dataTable.Columns[1];
				System.Type blobType  = blobColumn.DataType;

				Assert.AreEqual (typeof (byte[]), blobType);

				DataRow dataRow = dataTable.Rows[0];
				object blob = dataRow[1];
				System.Type dataType = blob.GetType ();

				Assert.AreEqual (typeof (byte[]), dataType);

				byte[] dataArray = dataRow[1] as byte[];

				Assert.IsNotNull (dataArray);
				Assert.AreEqual (10000, dataArray.Length);

				for (int i = 0; i < 10000; i++)
				{
					Assert.AreEqual (insertValues[i], dataArray[i]);
				}
			}
		}


	}


}
