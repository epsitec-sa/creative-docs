//	Copyright © 2003-2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Data;

using System.Linq;


namespace Epsitec.Cresus.Database.UnitTests
{


	[TestClass]
	public sealed class UnitTestSqlSelect
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			string file = TestHelper.GetEmployeeDatabaseFilePath ();

			IDbAbstractionHelper.RestoreDatabase(file);
		}


		[TestMethod]
		public void SqlSelectAllTest()
		{
			using (IDbAbstraction dbAbstraction = IDbAbstractionHelper.ConnectToTestDatabase ())
			{
				ISqlEngine sqlEngine = dbAbstraction.SqlEngine;
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;

				SqlSelect sqlSelect = new SqlSelect ();
				sqlSelect.Fields.Add (SqlField.CreateAll ());
				sqlSelect.Tables.Add (SqlField.CreateName ("EMPLOYEE"));

				sqlBuilder.SelectData (sqlSelect);

				IDbCommand command = sqlBuilder.Command;
				command.Transaction = dbAbstraction.BeginReadOnlyTransaction ();

				DataSet dataSet;
				sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out dataSet);

				int n = UnitTestSqlSelect.DumpDataSet (dataSet);

				Assert.AreEqual (42, n);

				dataSet.Dispose ();
			}
		}


		[TestMethod]
		public void SqlSelectOrderByTest()
		{
			using (IDbAbstraction dbAbstraction = IDbAbstractionHelper.ConnectToTestDatabase ())
			{
				ISqlEngine sqlEngine = dbAbstraction.SqlEngine;
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;

				SqlSelect sqlSelect = new SqlSelect
				{
					Predicate = SqlSelectPredicate.Distinct
				};

				sqlSelect.Fields.Add (SqlField.CreateName ("LAST_NAME"), SqlSortOrder.Descending);
				sqlSelect.Tables.Add (SqlField.CreateName ("EMPLOYEE"));

				sqlBuilder.SelectData (sqlSelect);

				IDbCommand command = sqlBuilder.Command;
				command.Transaction = dbAbstraction.BeginReadOnlyTransaction ();

				DataSet dataSet;
				sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out dataSet);

				int n = UnitTestSqlSelect.DumpDataSet (dataSet);

				Assert.AreEqual (40, n);

				dataSet.Dispose ();
			}
		}


		[TestMethod]
		public void SqlSelectWhereTest()
		{
			using (IDbAbstraction dbAbstraction = IDbAbstractionHelper.ConnectToTestDatabase ())
			{
				ISqlEngine sqlEngine = dbAbstraction.SqlEngine;
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;

				SqlSelect sqlSelect = new SqlSelect ();

				sqlSelect.Fields.Add (SqlField.CreateName ("LAST_NAME"));
				sqlSelect.Fields.Add (SqlField.CreateName ("FIRST_NAME"));
				sqlSelect.Fields.Add (SqlField.CreateName ("JOB_GRADE"));
				sqlSelect.Fields.Add (SqlField.CreateName ("JOB_COUNTRY"));
				sqlSelect.Tables.Add (SqlField.CreateName ("EMPLOYEE"));

				SqlFunction condition1 = new SqlFunction
				(
					SqlFunctionCode.CompareNotEqual,
					SqlField.CreateName ("JOB_COUNTRY"),
					SqlField.CreateConstant ("England", DbRawType.String)
				);

				sqlSelect.Conditions.Add (SqlField.CreateFunction (condition1));

				SqlFunction condition2 = new SqlFunction
				(
					SqlFunctionCode.CompareGreaterThan,
					SqlField.CreateName ("JOB_GRADE"),
					SqlField.CreateConstant (3, DbRawType.Int16)
				);

				sqlSelect.Conditions.Add (SqlField.CreateFunction (condition2));

				sqlBuilder.SelectData (sqlSelect);

				IDbCommand command = sqlBuilder.Command;
				command.Transaction = dbAbstraction.BeginReadOnlyTransaction ();

				DataSet dataSet;
				sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out dataSet);

				int n = UnitTestSqlSelect.DumpDataSet (dataSet);

				Assert.AreEqual (19, n);

				dataSet.Dispose ();
			}
		}


		[TestMethod]
		public void SqlSelectWhereMaxTest()
		{
			using (IDbAbstraction dbAbstraction = IDbAbstractionHelper.ConnectToTestDatabase ())
			{
				ISqlEngine sqlEngine = dbAbstraction.SqlEngine;
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;

				SqlSelect sqlSelect = new SqlSelect ();

				sqlSelect.Fields.Add (SqlField.CreateAll ());
				sqlSelect.Tables.Add (SqlField.CreateName ("EMPLOYEE"));

				SqlSelect subQuery = new SqlSelect ();
				subQuery.Fields.Add (SqlField.CreateAggregate (SqlAggregateFunction.Max, SqlField.CreateName ("JOB_GRADE")));
				subQuery.Tables.Add (SqlField.CreateName ("EMPLOYEE"));

				SqlFunction condition = new SqlFunction
				(
					SqlFunctionCode.CompareEqual,
					SqlField.CreateName ("JOB_GRADE"),
					SqlField.CreateSubQuery (subQuery)
				);

				sqlSelect.Conditions.Add (SqlField.CreateFunction (condition));

				sqlBuilder.SelectData (sqlSelect);

				IDbCommand command = sqlBuilder.Command;
				command.Transaction = dbAbstraction.BeginReadOnlyTransaction ();

				DataSet dataSet;
				sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out dataSet);

				int n = UnitTestSqlSelect.DumpDataSet (dataSet);

				Assert.AreEqual (6, n);

				dataSet.Dispose ();
			}
		}


		[TestMethod]
		public void SqlSelectInTest()
		{
			using (IDbAbstraction dbAbstraction = IDbAbstractionHelper.ConnectToTestDatabase ())
			{
				ISqlEngine sqlEngine = dbAbstraction.SqlEngine;
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;

				SqlSelect sqlSelect = new SqlSelect ();

				sqlSelect.Fields.Add (SqlField.CreateName ("LAST_NAME"));
				sqlSelect.Fields.Add (SqlField.CreateName ("FIRST_NAME"));

				SqlField table = SqlField.CreateName ("EMPLOYEE");
				table.Alias = "A";
				sqlSelect.Tables.Add (table);

				short[] ids = new short[] { 5, 8, 9};

				SqlFunction sqlFunction = new SqlFunction
				(
					SqlFunctionCode.SetIn,
					SqlField.CreateAliasedName ("A", "EMP_NO", "EMP_NO"),
					SqlField.CreateSet (new SqlSet (DbRawType.Int16, ids.Cast<object> ()))
				);
				sqlSelect.Conditions.Add (sqlFunction);

				sqlBuilder.SelectData (sqlSelect);

				IDbCommand command = sqlBuilder.Command;
				command.Transaction = dbAbstraction.BeginReadOnlyTransaction ();

				DataSet dataSet;
				sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out dataSet);

				int n = UnitTestSqlSelect.DumpDataSet (dataSet);

				Assert.AreEqual (3, n);

				dataSet.Dispose ();
			}
		}


		[TestMethod]
		public void SqlSelectInCapacityTest()
		{
			using (IDbAbstraction dbAbstraction = IDbAbstractionHelper.ConnectToTestDatabase ())
			{
				ISqlEngine sqlEngine = dbAbstraction.SqlEngine;
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;

				SqlSelect sqlSelect = new SqlSelect ();

				sqlSelect.Fields.Add (SqlField.CreateName ("LAST_NAME"));
				sqlSelect.Fields.Add (SqlField.CreateName ("FIRST_NAME"));

				SqlField table = SqlField.CreateName ("EMPLOYEE");
				table.Alias = "A";
				sqlSelect.Tables.Add (table);

				// NOTE : 1500 is the maximum number of elements we can put into an IN clause, so we
				// try it here, and it should work.
				// Marc

				SqlFunction sqlFunction = new SqlFunction
				(
					SqlFunctionCode.SetIn,
					SqlField.CreateAliasedName ("A", "EMP_NO", "EMP_NO"),
					SqlField.CreateSet (new SqlSet (DbRawType.Int16, Enumerable.Range (0, 1500).Cast<object> ()))
				);
				sqlSelect.Conditions.Add (sqlFunction);

				sqlBuilder.SelectData (sqlSelect);

				IDbCommand command = sqlBuilder.Command;

				command.Transaction = dbAbstraction.BeginReadOnlyTransaction ();

				DataSet dataSet;
				sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out dataSet);

				int n = UnitTestSqlSelect.DumpDataSet (dataSet);

				Assert.AreEqual (42, n);

				dataSet.Dispose ();
			}
		}


		[TestMethod]
		public void SqlSelectInnerJoinTest()
		{
			using (IDbAbstraction dbAbstraction = IDbAbstractionHelper.ConnectToTestDatabase ())
			{
				ISqlEngine sqlEngine = dbAbstraction.SqlEngine;
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;

				SqlSelect sqlSelect = new SqlSelect ();

				sqlSelect.Fields.Add (SqlField.CreateName ("LAST_NAME"));
				sqlSelect.Fields.Add (SqlField.CreateName ("FIRST_NAME"));
				sqlSelect.Fields.Add (SqlField.CreateName ("PROJ_NAME"));

				SqlField table1 = SqlField.CreateName ("EMPLOYEE_PROJECT");
				table1.Alias = "A1";
				sqlSelect.Tables.Add (table1);

				SqlField table2 = SqlField.CreateName ("EMPLOYEE");
				table2.Alias = "A2";
				sqlSelect.Tables.Add (table2);

				SqlField table3 = SqlField.CreateName ("PROJECT");
				table3.Alias = "A3";
				sqlSelect.Tables.Add (table3);

				SqlJoin sqlJoin1 = new SqlJoin
				(
					SqlField.CreateName ("A1", "EMP_NO"),
					SqlField.CreateName ("A2", "EMP_NO"),
					SqlJoinCode.Inner
				);
				sqlSelect.Joins.Add (SqlField.CreateJoin (sqlJoin1));

				SqlJoin sqlJoin2 = new SqlJoin
				(
					SqlField.CreateName ("A1", "PROJ_ID"),
					SqlField.CreateName ("A3", "PROJ_ID"),
					SqlJoinCode.Inner
				);
				sqlSelect.Joins.Add (sqlJoin2);

				sqlBuilder.SelectData (sqlSelect);

				IDbCommand command = sqlBuilder.Command;
				command.Transaction = dbAbstraction.BeginReadOnlyTransaction ();

				DataSet dataSet;
				sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out dataSet);

				int n = UnitTestSqlSelect.DumpDataSet (dataSet);

				Assert.AreEqual (28, n);

				dataSet.Dispose ();
			}
		}


		[TestMethod]
		public void SqlSelectAggregateOrderByTest()
		{
			using (IDbAbstraction dbAbstraction = IDbAbstractionHelper.ConnectToTestDatabase ())
			{
				ISqlEngine sqlEngine = dbAbstraction.SqlEngine;
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;

				SqlSelect sqlSelect = new SqlSelect ();

				sqlSelect.Fields.Add (SqlField.CreateName ("JOB_COUNTRY"));
				sqlSelect.Fields.Add (SqlField.CreateAggregate (SqlAggregateFunction.Sum, SqlField.CreateName ("SALARY")));
				sqlSelect.Fields.Add (SqlField.CreateName ("LAST_NAME"));

				SqlField table = SqlField.CreateName ("EMPLOYEE");
				table.Alias = "EMPL";
				sqlSelect.Tables.Add (table);

				SqlFunction condition1 = new SqlFunction
				(
					SqlFunctionCode.CompareGreaterThan,
					SqlField.CreateAggregate (SqlAggregateFunction.Sum, SqlField.CreateName ("SALARY")),
					SqlField.CreateConstant (50000, DbRawType.Int16)
				);
				sqlSelect.Conditions.Add (SqlField.CreateFunction (condition1));

				SqlFunction condition2 = new SqlFunction
				(
					SqlFunctionCode.CompareNotEqual,
					SqlField.CreateName ("JOB_COUNTRY"),
					SqlField.CreateConstant ("England", DbRawType.String)
				);
				sqlSelect.Conditions.Add (SqlField.CreateFunction (condition2));

				sqlBuilder.SelectData (sqlSelect);

				IDbCommand command = sqlBuilder.Command;
				command.Transaction = dbAbstraction.BeginReadOnlyTransaction ();

				DataSet dataSet;
				sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out dataSet);

				int n = UnitTestSqlSelect.DumpDataSet (dataSet);

				Assert.AreEqual (28, n);

				dataSet.Dispose ();
			}
		}


		[TestMethod]
		public void SqlSelectUnionTest()
		{
			using (IDbAbstraction dbAbstraction = IDbAbstractionHelper.ConnectToTestDatabase ())
			{
				ISqlEngine sqlEngine = dbAbstraction.SqlEngine;
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;

				SqlSelect sqlSelect1 = new SqlSelect ();

				sqlSelect1.Fields.Add (SqlField.CreateName ("LAST_NAME"));
				sqlSelect1.Fields.Add (SqlField.CreateName ("JOB_COUNTRY"));
				sqlSelect1.Fields.Add (SqlField.CreateName ("JOB_CODE"));
				sqlSelect1.Tables.Add (SqlField.CreateName ("EMPLOYEE"));

				SqlFunction condition1 = new SqlFunction
				(
					SqlFunctionCode.CompareEqual,
					SqlField.CreateName ("JOB_COUNTRY"),
					SqlField.CreateConstant ("England", DbRawType.String)
				);
				sqlSelect1.Conditions.Add (SqlField.CreateFunction (condition1));

				SqlSelect sqlSelect2 = new SqlSelect
				{
					Predicate = SqlSelectPredicate.All
				};

				sqlSelect2.Fields.Add (SqlField.CreateName ("LAST_NAME"));
				sqlSelect2.Fields.Add (SqlField.CreateName ("JOB_COUNTRY"));
				sqlSelect2.Fields.Add (SqlField.CreateName ("JOB_CODE"));
				sqlSelect2.Tables.Add (SqlField.CreateName ("EMPLOYEE"));

				SqlFunction condition2 = new SqlFunction
				(
					SqlFunctionCode.CompareEqual,
					SqlField.CreateName ("JOB_CODE"),
					SqlField.CreateConstant ("Admin", DbRawType.String)
				);
				sqlSelect2.Conditions.Add (SqlField.CreateFunction (condition2));

				sqlSelect1.Add (sqlSelect2, SqlSelectSetOp.Union);

				sqlBuilder.SelectData (sqlSelect1);

				IDbCommand command = sqlBuilder.Command;
				command.Transaction = dbAbstraction.BeginReadOnlyTransaction ();

				DataSet dataSet = new DataSet ();
				sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out dataSet);

				int n = UnitTestSqlSelect.DumpDataSet (dataSet);

				Assert.AreEqual (7, n);

				dataSet.Dispose ();
			}
		}


		[TestMethod]
		[Ignore]
		public void SqlSelectIntersectTest()
		{
			// FireBird does not implement the intersect method. Maybe we need to emulate it by
			// ourselves, or to remove this operator from the interface.
			// Marc

			using (IDbAbstraction dbAbstraction = IDbAbstractionHelper.ConnectToTestDatabase ())
			{
				ISqlEngine sqlEngine = dbAbstraction.SqlEngine;
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;

				SqlSelect sqlSelect1 = new SqlSelect ();

				sqlSelect1.Fields.Add (SqlField.CreateName ("LAST_NAME"));
				sqlSelect1.Fields.Add (SqlField.CreateName ("JOB_COUNTRY"));
				sqlSelect1.Fields.Add (SqlField.CreateName ("JOB_CODE"));
				sqlSelect1.Tables.Add (SqlField.CreateName ("EMPLOYEE"));

				SqlFunction condition1 = new SqlFunction
					(
					SqlFunctionCode.CompareEqual,
					SqlField.CreateName ("JOB_COUNTRY"),
					SqlField.CreateConstant ("England", DbRawType.String)
				);
				sqlSelect1.Conditions.Add (SqlField.CreateFunction (condition1));

				SqlSelect sqlSelect2 = new SqlSelect
				{
					Predicate = SqlSelectPredicate.All
				};

				sqlSelect2.Fields.Add (SqlField.CreateName ("LAST_NAME"));
				sqlSelect2.Fields.Add (SqlField.CreateName ("JOB_COUNTRY"));
				sqlSelect2.Fields.Add (SqlField.CreateName ("JOB_CODE"));
				sqlSelect2.Tables.Add (SqlField.CreateName ("EMPLOYEE"));

				SqlFunction condition2 = new SqlFunction
				(
					SqlFunctionCode.CompareEqual,
					SqlField.CreateName ("JOB_CODE"),
					SqlField.CreateConstant ("Admin", DbRawType.String)
				);
				sqlSelect2.Conditions.Add (SqlField.CreateFunction (condition2));

				sqlSelect1.Add (sqlSelect2, SqlSelectSetOp.Intersect);

				sqlBuilder.SelectData (sqlSelect1);

				IDbCommand command = sqlBuilder.Command;
				command.Transaction = dbAbstraction.BeginReadOnlyTransaction ();

				DataSet dataSet;
				sqlEngine.Execute (command, sqlBuilder.CommandType, sqlBuilder.CommandCount, out dataSet);

				int n = UnitTestSqlSelect.DumpDataSet (dataSet);

				Assert.AreEqual (0, n);

				dataSet.Dispose ();
			}
		}


		private static int DumpDataSet(DataSet dataSet)
		{
			int rowCount = 0;

			foreach (DataTable table in dataSet.Tables)
			{
				System.Console.Out.WriteLine ("TableName = " + table.TableName);

				foreach (DataColumn column in table.Columns)
				{
					System.Console.Out.Write (column.ColumnName + ", ");
				}

				System.Console.Out.WriteLine ();

				foreach (DataRow row in table.Rows)
				{
					foreach (DataColumn column in table.Columns)
					{
						System.Console.Out.Write (row[column] + ", ");
					}

					System.Console.Out.WriteLine ();
				}

				rowCount += table.Rows.Count;
			}

			System.Console.Out.WriteLine ("{0} rows", rowCount);

			return rowCount;
		}


	}


}
