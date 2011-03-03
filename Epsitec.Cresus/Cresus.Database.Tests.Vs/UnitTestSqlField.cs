//	Copyright © 2003-2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.Database.Tests.Vs
{


	[TestClass]
	public sealed class UnitTestSqlField
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			IDbAbstractionHelper.ResetTestDatabase ();
		}


		[TestMethod]
		public void CreateNameArgumentCheck()
		{
			ExceptionAssert.Throw<Exceptions.FormatException>
			(
				() => SqlField.CreateName ("TestNotQualifiedBut&Invalid")
			);

			ExceptionAssert.Throw<Exceptions.FormatException>
			(
				() => SqlField.CreateName ("Test", "Qualified-Invalid")
			);
		}


		[TestMethod]
		public void CreateNameTest()
		{
			SqlField sqlField = SqlField.CreateName ("TestNotQualified");

			Assert.AreEqual (SqlFieldType.Name, sqlField.FieldType);
			Assert.AreEqual ("TestNotQualified", sqlField.AsName);
			Assert.AreEqual (null, sqlField.AsQualifiedName);
			Assert.AreEqual (null, sqlField.Alias);
			Assert.AreEqual (SqlSortOrder.None, sqlField.SortOrder);
			Assert.AreEqual (DbRawType.Unknown, sqlField.RawType);
		}


		[TestMethod]
		public void CreateQualifiedNameTest()
		{
			SqlField sqlField = SqlField.CreateName ("Test", "Qualified");

			Assert.AreEqual (SqlFieldType.QualifiedName, sqlField.FieldType);
			Assert.AreEqual ("Test.Qualified", sqlField.AsQualifiedName);
			Assert.AreEqual ("Qualified", sqlField.AsName);
			Assert.AreEqual (null, sqlField.Alias);
			Assert.AreEqual (SqlSortOrder.None, sqlField.SortOrder);
			Assert.AreEqual (DbRawType.Unknown, sqlField.RawType);
		}


		[TestMethod]
		public void CreateAliasedNameTest()
		{
			SqlField sqlField = SqlField.CreateName ("TestNotQualified");
			sqlField.Alias = "NotQ";

			Assert.AreEqual (SqlFieldType.Name, sqlField.FieldType);
			Assert.AreEqual ("TestNotQualified", sqlField.AsName);
			Assert.AreEqual (null, sqlField.AsQualifiedName);
			Assert.AreEqual ("NotQ", sqlField.Alias);
			Assert.AreEqual (SqlSortOrder.None, sqlField.SortOrder);
			Assert.AreEqual (DbRawType.Unknown, sqlField.RawType);
		}


		[TestMethod]
		public void CreateAliasedSortedNameTest()
		{
			SqlField sqlField = SqlField.CreateName ("Test", "Qualified");
			sqlField.Alias = "TQ";
			sqlField.SortOrder = SqlSortOrder.Descending;

			Assert.AreEqual (SqlFieldType.QualifiedName, sqlField.FieldType);
			Assert.AreEqual ("Test.Qualified", sqlField.AsQualifiedName);
			Assert.AreEqual ("Qualified", sqlField.AsName);
			Assert.AreEqual ("TQ", sqlField.Alias);
			Assert.AreEqual (SqlSortOrder.Descending, sqlField.SortOrder);
			Assert.AreEqual (DbRawType.Unknown, sqlField.RawType);
		}


		[TestMethod]
		public void ValidateTest()
		{
			using (IDbAbstraction dbAbstraction = IDbAbstractionHelper.ConnectToTestDatabase ())
			{
				ISqlBuilder sqlBuilder = dbAbstraction.SqlBuilder;

				SqlField sqlField1 = SqlField.CreateName ("TestNotQualifiedButValid");
				Assert.AreEqual (SqlFieldType.Name, sqlField1.FieldType);
				Assert.AreEqual ("TestNotQualifiedButValid", sqlField1.AsName);
				Assert.AreEqual (true, sqlField1.Validate (sqlBuilder));

				SqlField sqlField2 = SqlField.CreateName ("Test", "QualifiedValid");
				sqlField2.Alias = "TQ";
				sqlField2.SortOrder = SqlSortOrder.Descending;

				Assert.AreEqual (SqlFieldType.QualifiedName, sqlField2.FieldType);
				Assert.AreEqual ("Test.QualifiedValid", sqlField2.AsQualifiedName);
				Assert.AreEqual (true, sqlField2.Validate (sqlBuilder));
			}
		}


		[TestMethod]
		public void CreateNullTest()
		{
			SqlField sqlField = SqlField.CreateNull ();

			Assert.AreEqual (SqlFieldType.Null, sqlField.FieldType);
		}


		[TestMethod]
		public void CreateDefaultTest()
		{
			SqlField sqlField = SqlField.CreateDefault ();

			Assert.AreEqual (SqlFieldType.Default, sqlField.FieldType);
		}


		[TestMethod]
		public void CreateConstantTest()
		{
			SqlField sqlField = SqlField.CreateConstant (123.45M, DbRawType.LargeDecimal);
			
			Assert.AreEqual (SqlFieldType.Constant, sqlField.FieldType);
			Assert.AreEqual (DbRawType.LargeDecimal, sqlField.RawType);
			Assert.AreEqual (123.45M, sqlField.AsConstant);
		}


		[TestMethod]
		public void CreateParameterInTest()
		{
			object val = System.Guid.NewGuid ();

			SqlField sqlField = SqlField.CreateParameterIn (val, DbRawType.Guid);

			Assert.AreEqual (SqlFieldType.ParameterIn, sqlField.FieldType);
			Assert.AreEqual (DbRawType.Guid, sqlField.RawType);
			Assert.AreEqual (val, sqlField.AsParameter);
		}


		[TestMethod]
		public void CreateParameterOutTest()
		{
			SqlField sqlField = SqlField.CreateParameterOut (DbRawType.String);

			Assert.AreEqual (SqlFieldType.ParameterOut, sqlField.FieldType);
			Assert.AreEqual (DbRawType.String, sqlField.RawType);
			Assert.AreEqual (null, sqlField.AsParameter);
		}


		[TestMethod]
		public void CreateParameterInOutTest()
		{
			SqlField sqlField = SqlField.CreateParameterInOut ("abc", DbRawType.String);
			
			Assert.AreEqual (SqlFieldType.ParameterInOut, sqlField.FieldType);
			Assert.AreEqual (DbRawType.String, sqlField.RawType);
			Assert.AreEqual ("abc", sqlField.AsParameter);
		}


		[TestMethod]
		public void CreateParameterResultTest()
		{
			SqlField sqlField = SqlField.CreateParameterResult (DbRawType.Time);
			
			Assert.AreEqual (SqlFieldType.ParameterResult, sqlField.FieldType);
			Assert.AreEqual (DbRawType.Time, sqlField.RawType);
			Assert.AreEqual (null, sqlField.AsParameter);
		
			sqlField.SetParameterOutResult ("abc");

			Assert.AreEqual ("abc", sqlField.AsParameter);
		}


		[TestMethod]
		public void CreateAllTest()
		{
			SqlField sqlField = SqlField.CreateAll ();

			Assert.AreEqual (SqlFieldType.All, sqlField.FieldType);
			Assert.AreEqual (null, sqlField.AsName);
			Assert.AreEqual (null, sqlField.AsQualifiedName);
			Assert.AreEqual (null, sqlField.Alias);
			Assert.AreEqual (SqlSortOrder.None, sqlField.SortOrder);
			Assert.AreEqual (DbRawType.Unknown, sqlField.RawType);
		}


		[TestMethod]
		public void CreateNameTest1()
		{
			SqlField sqlField = SqlField.CreateName ("Test", "Qualified");

			Assert.AreEqual (SqlFieldType.QualifiedName, sqlField.FieldType);
			Assert.AreEqual ("Test.Qualified", sqlField.AsQualifiedName);
			Assert.AreEqual ("Qualified", sqlField.AsName);
			Assert.AreEqual (null, sqlField.Alias);
			Assert.AreEqual (SqlSortOrder.None, sqlField.SortOrder);
			Assert.AreEqual (DbRawType.Unknown, sqlField.RawType);
		}


		[TestMethod]
		public void CreateNameTest2()
		{
			SqlField sqlField1 = SqlField.CreateName ("Table", "Colonne");
			
			Assert.AreEqual (SqlFieldType.QualifiedName, sqlField1.FieldType);
			Assert.AreEqual ("Table.Colonne", sqlField1.AsQualifiedName);
			Assert.AreEqual ("Colonne", sqlField1.AsName);
			Assert.AreEqual (null, sqlField1.Alias);
			Assert.AreEqual (SqlSortOrder.None, sqlField1.SortOrder);
			Assert.AreEqual (DbRawType.Unknown, sqlField1.RawType);

			SqlField sqlField2 = SqlField.CreateAggregate (new SqlAggregate (SqlAggregateFunction.Sum, sqlField1));
			
			Assert.AreEqual (SqlFieldType.Aggregate, sqlField2.FieldType);
			Assert.AreEqual (SqlAggregateFunction.Sum, sqlField2.AsAggregate.Function);
			Assert.AreEqual (null, sqlField2.AsQualifiedName);
			Assert.AreEqual (null, sqlField2.AsName);
			Assert.AreEqual (null, sqlField2.Alias);
			Assert.AreEqual (SqlSortOrder.None, sqlField2.SortOrder);
			Assert.AreEqual (DbRawType.Unknown, sqlField2.RawType);

			SqlField sqlField3 = SqlField.CreateAggregate (SqlAggregateFunction.Count, sqlField1);
			
			Assert.AreEqual (SqlFieldType.Aggregate, sqlField3.FieldType);
			Assert.AreEqual (SqlAggregateFunction.Count, sqlField3.AsAggregate.Function);
			Assert.AreEqual (null, sqlField3.AsQualifiedName);
			Assert.AreEqual (null, sqlField3.AsName);
			Assert.AreEqual (null, sqlField3.Alias);
			Assert.AreEqual (SqlSortOrder.None, sqlField3.SortOrder);
			Assert.AreEqual (DbRawType.Unknown, sqlField3.RawType);
		}


		[TestMethod]
		public void CreateVariableTest()
		{
			SqlField sqlField = SqlField.CreateVariable ();

			Assert.AreEqual (SqlFieldType.Variable, sqlField.FieldType);
		}


		[TestMethod]
		public void CreateFunctionTest()
		{
			SqlFunction function = new SqlFunction (SqlFunctionCode.CompareIsNull, SqlField.CreateName ("table", "column"));
			SqlField sqlField = SqlField.CreateFunction (function);

			Assert.AreEqual (SqlFieldType.Function, sqlField.FieldType);
			Assert.AreEqual (function, sqlField.AsFunction);
		}


		[TestMethod]
		public void CreateProcedureTest()
		{
			SqlField sqlField = SqlField.CreateProcedure ("ProcedureName");

			Assert.AreEqual (SqlFieldType.Procedure, sqlField.FieldType);
			Assert.AreEqual ("ProcedureName", sqlField.AsProcedure);
		}


		[TestMethod]
		public void CreateSubQueryTest()
		{
			SqlSelect select = new SqlSelect ();
			SqlField sqlField = SqlField.CreateSubQuery (select);
			
			Assert.AreEqual (SqlFieldType.SubQuery, sqlField.FieldType);
			Assert.AreEqual (select, sqlField.AsSubQuery);
		}


	}


}
