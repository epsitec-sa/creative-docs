using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Expressions
{

	
    [TestClass]
	public sealed class UnitTestEnumConverter
	{


		[TestMethod]
		public void ToSqlFunctionCodeTest1()
		{
			var matches = new Dictionary<UnaryComparator, SqlFunctionCode> ()
			{
				{ UnaryComparator.IsNull, SqlFunctionCode.CompareIsNull },
				{ UnaryComparator.IsNotNull, SqlFunctionCode.CompareIsNotNull },
			};

			this.Check (matches, EnumConverter.ToSqlFunctionCode);
		}
		

		[TestMethod]
		public void ToSqlFunctionCodeTest2()
		{
			var matches = new Dictionary<BinaryComparator, SqlFunctionCode> ()
			{
				{ BinaryComparator.IsEqual, SqlFunctionCode.CompareEqual },
				{ BinaryComparator.IsNotEqual, SqlFunctionCode.CompareNotEqual },
				{ BinaryComparator.IsLower, SqlFunctionCode.CompareLessThan },
				{ BinaryComparator.IsLowerOrEqual, SqlFunctionCode.CompareLessThanOrEqual },
				{ BinaryComparator.IsGreater, SqlFunctionCode.CompareGreaterThan },
				{ BinaryComparator.IsGreaterOrEqual, SqlFunctionCode.CompareGreaterThanOrEqual },
				{ BinaryComparator.IsLike, SqlFunctionCode.CompareLike },
				{ BinaryComparator.IsNotLike, SqlFunctionCode.CompareNotLike },
				{ BinaryComparator.IsLikeEscape, SqlFunctionCode.CompareLikeEscape },
				{ BinaryComparator.IsNotLikeEscape, SqlFunctionCode.CompareNotLikeEscape },
			};

			this.Check (matches, EnumConverter.ToSqlFunctionCode);
		}
		
		
		[TestMethod]
		public void ToSqlFunctionCodeTest3()
		{
			var matches = new Dictionary<UnaryOperator, SqlFunctionCode> ()
			{
				{ UnaryOperator.Not, SqlFunctionCode.LogicNot },
			};

			this.Check (matches, EnumConverter.ToSqlFunctionCode);
		}
		

		[TestMethod]
		public void ToSqlFunctionCodeTest4()
		{
			var matches = new Dictionary<BinaryOperator, SqlFunctionCode> ()
			{
				{ BinaryOperator.And, SqlFunctionCode.LogicAnd },
				{ BinaryOperator.Or, SqlFunctionCode.LogicOr },
			};

			this.Check (matches, EnumConverter.ToSqlFunctionCode);
		}
		

		[TestMethod]
		public void ToDbRawTypeTest()
		{
			var matches = new Dictionary<Type, DbRawType> ()
			{
				{ Type.Boolean, DbRawType.Boolean },
				{ Type.Int16, DbRawType.Int16 },
				{ Type.Int32, DbRawType.Int32 },
				{ Type.Int64, DbRawType.Int64 },
				{ Type.Decimal, DbRawType.LargeDecimal },
				{ Type.Date, DbRawType.Date },
				{ Type.Time, DbRawType.Time },
				{ Type.DateTime, DbRawType.DateTime },
				{ Type.String, DbRawType.String },
				{ Type.ByteArray, DbRawType.ByteArray },
				{ Type.Enum, DbRawType.Int32 },
			};

			this.Check (matches, EnumConverter.ToDbRawType);
		}


		[TestMethod]
		public void ToDbSimpleTypeTest()
		{
			var matches = new Dictionary<Type, DbSimpleType> ()
			{
				{ Type.Boolean, DbSimpleType.Decimal },
				{ Type.Int16, DbSimpleType.Decimal },
				{ Type.Int32, DbSimpleType.Decimal },
				{ Type.Int64, DbSimpleType.Decimal },
				{ Type.Decimal, DbSimpleType.Decimal },
				{ Type.Date, DbSimpleType.Date },
				{ Type.Time, DbSimpleType.Time },
				{ Type.DateTime, DbSimpleType.DateTime },
				{ Type.String, DbSimpleType.String },
				{ Type.ByteArray, DbSimpleType.ByteArray },
				{ Type.Enum, DbSimpleType.Decimal },
			};

			this.Check (matches, EnumConverter.ToDbSimpleType);
		}


		[TestMethod]
		public void ToSqlSortOrderTest()
		{
			var matches = new Dictionary<SortOrder, SqlSortOrder> ()
			{
				{ SortOrder.Ascending, SqlSortOrder.Ascending },
				{ SortOrder.Descending, SqlSortOrder.Descending },
			};

			this.Check (matches, EnumConverter.ToSqlSortOrder);
		}


		[TestMethod]
		public void ToDbNumDefTest()
		{
			var matches = new Dictionary<Type, DbNumDef> ()
			{
				{ Type.Boolean, DbNumDef.FromRawType (DbRawType.Boolean) },
				{ Type.Int16, DbNumDef.FromRawType (DbRawType.Int16) },
				{ Type.Int32, DbNumDef.FromRawType (DbRawType.Int32) },
				{ Type.Int64, DbNumDef.FromRawType (DbRawType.Int64) },
				{ Type.Decimal, DbNumDef.FromRawType (DbRawType.LargeDecimal) },
				{ Type.Date, DbNumDef.FromRawType (DbRawType.Date) },
				{ Type.Time, DbNumDef.FromRawType (DbRawType.Time) },
				{ Type.DateTime, DbNumDef.FromRawType (DbRawType.DateTime) },
				{ Type.String, DbNumDef.FromRawType (DbRawType.String) },
				{ Type.ByteArray, DbNumDef.FromRawType (DbRawType.ByteArray) },
				{ Type.Enum, DbNumDef.FromRawType (DbRawType.Int32) },
			};

			this.Check (matches, EnumConverter.ToDbNumDef);
		}


		private void Check<T1, T2>(Dictionary<T1, T2> matches, System.Func<T1, T2> converter)
		{
			foreach (var match in matches)
			{
				var v1 = match.Key;
				var v2 = match.Value;

				Assert.AreEqual (v2, converter (v1));
			}
		}


	}


}
