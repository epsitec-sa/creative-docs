using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.UnitTests
{

	
    [TestClass]
	public sealed class UnitTestConverter
	{
		

		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
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
				DbConditionCombinerOperator result2 = Converter_Accessor.ToDbConditionCombinerOperator (match);

				Assert.AreEqual (result1, result2);
			}
		}

		
		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void ToDbConditionModifierOperatorTest()
		{
			var matches = new Dictionary<UnaryOperator, DbConditionModifierOperator> ()
			{
				{ UnaryOperator.Not, DbConditionModifierOperator.Not },
			};

			foreach (UnaryOperator match in matches.Keys)
			{
				DbConditionModifierOperator result1 = matches[match];
				DbConditionModifierOperator result2 = Converter_Accessor.ToDbConditionModifierOperator (match);

				Assert.AreEqual (result1, result2);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void ToDbRawTypeTest()
		{
			var matches = new Dictionary<Type_Accessor, DbRawType> ()
			{
				{ Type_Accessor.Boolean, DbRawType.Boolean },
				{ Type_Accessor.Int16, DbRawType.Int16 },
				{ Type_Accessor.Int32, DbRawType.Int32 },
				{ Type_Accessor.Int64, DbRawType.Int64 },
				{ Type_Accessor.Double, DbRawType.LargeDecimal },
				{ Type_Accessor.Date, DbRawType.Date },
				{ Type_Accessor.Time, DbRawType.Time },
				{ Type_Accessor.DateTime, DbRawType.DateTime },
				{ Type_Accessor.String, DbRawType.String },
			};

			foreach (Type_Accessor match in matches.Keys)
			{
				DbRawType result1 = matches[match];
				DbRawType result2 = Converter_Accessor.ToDbRawType (match);

				Assert.AreEqual (result1, result2);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
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
				DbSimpleConditionOperator result2 = Converter_Accessor.ToDbSimpleConditionOperator (match);

				Assert.AreEqual (result1, result2);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
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
				DbSimpleConditionOperator result2 = Converter_Accessor.ToDbSimpleConditionOperator (match);

				Assert.AreEqual (result1, result2);
			}
		}

	}


}
