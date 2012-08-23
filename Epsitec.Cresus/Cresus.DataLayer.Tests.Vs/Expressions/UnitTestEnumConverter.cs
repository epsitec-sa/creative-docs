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
		public void ToSqlSortOrderTest()
		{
			var matches = new Dictionary<SortOrder, SqlSortOrder> ()
			{
				{ SortOrder.Ascending, SqlSortOrder.Ascending },
				{ SortOrder.Descending, SqlSortOrder.Descending },
			};

			this.Check (matches, EnumConverter.ToSqlSortOrder);
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
