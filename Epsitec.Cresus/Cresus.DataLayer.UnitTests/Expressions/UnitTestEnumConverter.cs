using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.UnitTests.Expressions
{

	
    [TestClass]
	public sealed class UnitTestEnumConverter
	{
		

		[TestMethod]
		public void ToDbConditionCombinerOperatorTest()
		{
			var matches = new Dictionary<BinaryOperator, DbConditionCombinerOperator> ()
			{
				{ BinaryOperator.And, DbConditionCombinerOperator.And },
				{ BinaryOperator.Or, DbConditionCombinerOperator.Or },
			};

			foreach (BinaryOperator match in matches.Keys)
			{
				DbConditionCombinerOperator result1 = matches[match];
				DbConditionCombinerOperator result2 = EnumConverter.ToDbConditionCombinerOperator (match);

				Assert.AreEqual (result1, result2);
			}
		}

		
		[TestMethod]
		public void ToDbConditionModifierOperatorTest()
		{
			var matches = new Dictionary<UnaryOperator, DbConditionModifierOperator> ()
			{
				{ UnaryOperator.Not, DbConditionModifierOperator.Not },
			};

			foreach (UnaryOperator match in matches.Keys)
			{
				DbConditionModifierOperator result1 = matches[match];
				DbConditionModifierOperator result2 = EnumConverter.ToDbConditionModifierOperator (match);

				Assert.AreEqual (result1, result2);
			}
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
				{ Type.Double, DbRawType.LargeDecimal },
				{ Type.Date, DbRawType.Date },
				{ Type.Time, DbRawType.Time },
				{ Type.DateTime, DbRawType.DateTime },
				{ Type.String, DbRawType.String },
			};

			foreach (Type match in matches.Keys)
			{
				DbRawType result1 = matches[match];
				DbRawType result2 = EnumConverter.ToDbRawType (match);

				Assert.AreEqual (result1, result2);
			}
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
				{ Type.Double, DbSimpleType.Decimal },
				{ Type.Date, DbSimpleType.Date },
				{ Type.Time, DbSimpleType.Time },
				{ Type.DateTime, DbSimpleType.DateTime },
				{ Type.String, DbSimpleType.String },
			};

			foreach (Type match in matches.Keys)
			{
				DbSimpleType result1 = matches[match];
				DbSimpleType result2 = EnumConverter.ToDbSimpleType (match);

				Assert.AreEqual (result1, result2);
			}
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
				{ Type.Double, DbNumDef.FromRawType (DbRawType.LargeDecimal) },
				{ Type.Date, DbNumDef.FromRawType (DbRawType.Date) },
				{ Type.Time, DbNumDef.FromRawType (DbRawType.Time) },
				{ Type.DateTime, DbNumDef.FromRawType (DbRawType.DateTime) },
				{ Type.String, DbNumDef.FromRawType (DbRawType.String) },
			};

			foreach (Type match in matches.Keys)
			{
				DbNumDef result1 = matches[match];
				DbNumDef result2 = EnumConverter.ToDbNumDef (match);

				Assert.AreEqual (result1, result2);
			}
		}


		[TestMethod]
		public void ToDbSimpleConditionOperatorTest1()
		{
			var matches = new Dictionary<UnaryComparator, DbSimpleConditionOperator> ()
			{
				{ UnaryComparator.IsNull, DbSimpleConditionOperator.IsNull },
				{ UnaryComparator.IsNotNull, DbSimpleConditionOperator.IsNotNull },
			};

			foreach (UnaryComparator match in matches.Keys)
			{
				DbSimpleConditionOperator result1 = matches[match];
				DbSimpleConditionOperator result2 = EnumConverter.ToDbSimpleConditionOperator (match);

				Assert.AreEqual (result1, result2);
			}
		}


		[TestMethod]
		public void ToDbSimpleConditionOperatorTest2()
		{
			var matches = new Dictionary<BinaryComparator, DbSimpleConditionOperator> ()
			{
				{ BinaryComparator.IsEqual, DbSimpleConditionOperator.Equal },
				{ BinaryComparator.IsNotEqual, DbSimpleConditionOperator.NotEqual },
				{ BinaryComparator.IsLower, DbSimpleConditionOperator.LessThan },
				{ BinaryComparator.IsLowerOrEqual, DbSimpleConditionOperator.LessThanOrEqual },
				{ BinaryComparator.IsGreater, DbSimpleConditionOperator.GreaterThan },
				{ BinaryComparator.IsGreaterOrEqual, DbSimpleConditionOperator.GreaterThanOrEqual },
				{ BinaryComparator.IsLike, DbSimpleConditionOperator.Like },
				{ BinaryComparator.IsNotLike, DbSimpleConditionOperator.NotLike },
				{ BinaryComparator.IsLikeEscape, DbSimpleConditionOperator.LikeEscape },
				{ BinaryComparator.IsNotLikeEscape, DbSimpleConditionOperator.NotLikeEscape },
			};

			foreach (BinaryComparator match in matches.Keys)
			{
				DbSimpleConditionOperator result1 = matches[match];
				DbSimpleConditionOperator result2 = EnumConverter.ToDbSimpleConditionOperator (match);

				Assert.AreEqual (result1, result2);
			}
		}

	}


}
