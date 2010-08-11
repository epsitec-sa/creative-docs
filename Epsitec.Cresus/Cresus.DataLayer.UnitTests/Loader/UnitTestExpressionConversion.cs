using Epsitec.Common.Support;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.UnitTests.Loader
{


	 [TestClass]
	public sealed class UnitTestExpressionConversion
	{


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void CreateDbConditionUnaryOperationTest1()
		{
			Dictionary<Druid, DbTableColumn> resolver = this.GetResolver ();

			Field field = new Field (Druid.FromLong (1));

			UnaryComparator comparator = UnaryComparator.IsNull;

			UnaryComparison comparison = new UnaryComparison (field, comparator);

			UnaryOperator op = UnaryOperator.Not;

			UnaryOperation operation = new UnaryOperation (op, comparison);

			DbAbstractCondition condition1 = ExpressionConversion_Accessor.CreateDbCondition (operation, (id) => resolver[id]);

			Assert.IsInstanceOfType (condition1, typeof (DbConditionModifier));

			DbConditionModifier condition2 = condition1 as DbConditionModifier;

			Assert.AreEqual (condition2.Operator, EnumConverter_Accessor.ToDbConditionModifierOperator (op));

			DbAbstractCondition condition3 = condition2.Condition;

			Assert.IsInstanceOfType(condition3, typeof (DbSimpleCondition));

			DbSimpleCondition_Accessor condition4 = new DbSimpleCondition_Accessor (new PrivateObject (condition3 as DbSimpleCondition));

			Assert.AreSame (condition4.Left, resolver[field.FieldId]);
			Assert.AreEqual (condition4.Operator, EnumConverter_Accessor.ToDbSimpleConditionOperator (comparator));
			Assert.IsNull (condition4.RightColumn);
			Assert.AreEqual (1, condition4.argumentCount);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void CreateDbConditionUnaryOperationTest2()
		{
			UnaryOperation operation = null;
			System.Func<Druid, DbTableColumn> resolver = d => null;

			ExpressionConversion_Accessor.CreateDbCondition (operation, resolver);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void CreateDbConditionUnaryOperationTest3()
		{
			UnaryOperation operation = new UnaryOperation (
				UnaryOperator.Not,
				new UnaryComparison (
					new Field (Druid.FromLong (1)),
					UnaryComparator.IsNull
				)
			);
			System.Func<Druid, DbTableColumn> resolver = null;

			ExpressionConversion_Accessor.CreateDbCondition (operation, resolver);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[DeploymentItem ("Cresusl.Database.dll")]
		public void CreateDbConditionBinaryOperationTest1()
		{
			Dictionary<Druid, DbTableColumn> resolver = this.GetResolver ();

			Field field1 = new Field (Druid.FromLong (1));
			Field field2 = new Field (Druid.FromLong (1));

			UnaryComparator comparator = UnaryComparator.IsNull;

			UnaryComparison comparison1 = new UnaryComparison (field1, comparator);
			UnaryComparison comparison2 = new UnaryComparison (field2, comparator);
			
			BinaryOperator op = BinaryOperator.And;

			BinaryOperation operation = new BinaryOperation (
				comparison1,
				op,
				comparison2
			);

			DbAbstractCondition condition1 = ExpressionConversion_Accessor.CreateDbCondition (operation, (id) => resolver[id]);

			Assert.IsInstanceOfType (condition1, typeof (DbConditionCombiner));

			DbConditionCombiner_Accessor condition2 = new DbConditionCombiner_Accessor (new PrivateObject (condition1 as DbConditionCombiner));

			Assert.AreEqual (condition2.Combiner, EnumConverter_Accessor.ToDbConditionCombinerOperator (op));
			
			List<DbTableColumn> fields = new List<DbTableColumn> ()
			{
				resolver[field1.FieldId],
				resolver[field2.FieldId],
			};

			foreach (DbAbstractCondition condition3 in condition2.conditions)
			{
				Assert.IsTrue (condition3 is DbSimpleCondition);

				DbSimpleCondition_Accessor condition4 = new DbSimpleCondition_Accessor (new PrivateObject (condition3 as DbSimpleCondition));

				CollectionAssert.Contains (fields, condition4.Left);
				Assert.AreEqual (condition4.Operator, EnumConverter_Accessor.ToDbSimpleConditionOperator (comparator));
				Assert.IsNull (condition4.RightColumn);
				Assert.AreEqual (1, condition4.argumentCount);

				fields.Remove (condition4.Left);
			}

			Assert.IsTrue (fields.Count == 0);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void CreateDbConditionBinaryOperationTest2()
		{
			BinaryOperation operation = null;
			System.Func<Druid, DbTableColumn> resolver = d => null;

			ExpressionConversion_Accessor.CreateDbCondition (operation, resolver);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void CreateDbConditionBinaryOperationTest3()
		{
			BinaryOperation operation = new BinaryOperation (
				new UnaryComparison (
					new Field (Druid.FromLong (1)),
					UnaryComparator.IsNull
				),
				BinaryOperator.And,
				new UnaryComparison (
					new Field (Druid.FromLong (2)),
					UnaryComparator.IsNull
				)
			);
			System.Func<Druid, DbTableColumn> resolver = null;

			ExpressionConversion_Accessor.CreateDbCondition (operation, resolver);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void CreateDbConditionUnaryComparisonTest1()
		{
			Dictionary<Druid, DbTableColumn> resolver = this.GetResolver ();

			Field field = new Field (Druid.FromLong (1));
			UnaryComparator comparator = UnaryComparator.IsNull;

			UnaryComparison comparison = new UnaryComparison (field, comparator);

			DbAbstractCondition condition1 = ExpressionConversion_Accessor.CreateDbCondition (comparison, (id) => resolver[id]);

			Assert.IsInstanceOfType (condition1, typeof (DbSimpleCondition));

			DbSimpleCondition_Accessor condition2 = new DbSimpleCondition_Accessor (new PrivateObject (condition1 as DbSimpleCondition));

			Assert.AreSame (condition2.Left, resolver[field.FieldId]);
			Assert.AreEqual (condition2.Operator, EnumConverter_Accessor.ToDbSimpleConditionOperator (comparator));
			Assert.IsNull (condition2.RightColumn);
			Assert.AreEqual (1, condition2.argumentCount);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void CreateDbConditionUnaryComparisonTest2()
		{
			UnaryComparison comparison = null;
			System.Func<Druid, DbTableColumn> resolver = d => null;

			ExpressionConversion_Accessor.CreateDbCondition (comparison, resolver);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void CreateDbConditionUnaryComparisonTest3()
		{
			UnaryComparison comparison = new UnaryComparison (
				new Field (Druid.FromLong (1)),
				UnaryComparator.IsNull
			);
			System.Func<Druid, DbTableColumn> resolver = null;

			ExpressionConversion_Accessor.CreateDbCondition (comparison, resolver);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void CreateDbConditionComparisonFieldFieldTest1()
		{
			Dictionary<Druid, DbTableColumn> resolver = this.GetResolver ();

			Field field1 = new Field (Druid.FromLong (1));
			BinaryComparator comparator = BinaryComparator.IsEqual;
			Field field2 = new Field (Druid.FromLong (2));

			ComparisonFieldField comparison = new ComparisonFieldField (field1, comparator, field2);

			DbAbstractCondition condition1 = ExpressionConversion_Accessor.CreateDbCondition (comparison, (id) => resolver[id]);

			Assert.IsInstanceOfType (condition1, typeof (DbSimpleCondition));

			DbSimpleCondition_Accessor condition2 = new DbSimpleCondition_Accessor (new PrivateObject (condition1 as DbSimpleCondition));

			Assert.AreSame (condition2.Left, resolver[field1.FieldId]);
			Assert.AreEqual (condition2.Operator, EnumConverter_Accessor.ToDbSimpleConditionOperator (comparator));
			Assert.AreSame (condition2.RightColumn, resolver[field2.FieldId]);
			Assert.AreEqual (2, condition2.argumentCount);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void CreateDbConditionComparisonFieldFieldTest2()
		{
			ComparisonFieldField comparison = null;
			System.Func<Druid, DbTableColumn> resolver = d => null;

			ExpressionConversion_Accessor.CreateDbCondition (comparison, resolver);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void CreateDbConditionComparisonFieldFieldTest3()
		{
			ComparisonFieldField comparison = new ComparisonFieldField (
				new Field (Druid.FromLong (1)),
				BinaryComparator.IsEqual,
				new Field (Druid.FromLong (2))
			);
			System.Func<Druid, DbTableColumn> resolver = null;

			ExpressionConversion_Accessor.CreateDbCondition (comparison, resolver);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void CreateDbConditionComparisonFieldValueTest1()
		{
			Dictionary<Druid, DbTableColumn> resolver = this.GetResolver();

			Field field = new Field (Druid.FromLong (1));
			BinaryComparator comparator = BinaryComparator.IsEqual;
			Constant constant = new Constant (1);

			ComparisonFieldValue comparison = new ComparisonFieldValue (field, comparator, constant);

			DbAbstractCondition condition1 = ExpressionConversion_Accessor.CreateDbCondition (comparison, (id) => resolver[id]);

			Assert.IsInstanceOfType (condition1, typeof (DbSimpleCondition));

			DbSimpleCondition_Accessor condition2 = new DbSimpleCondition_Accessor(new PrivateObject(condition1 as DbSimpleCondition));

			Assert.AreSame (condition2.Left, resolver[field.FieldId]);
			Assert.AreEqual (condition2.Operator, EnumConverter_Accessor.ToDbSimpleConditionOperator (comparator));
			Assert.AreEqual (condition2.RightConstantRawType, EnumConverter_Accessor.ToDbRawType (Type_Accessor.Int32));
			Assert.AreEqual (condition2.RightConstantValue, 1);
			Assert.IsNull (condition2.RightColumn);
			Assert.AreEqual (2, condition2.argumentCount);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void CreateDbConditionComparisonFieldValueTest2()
		{
			ComparisonFieldValue comparison = null;
			System.Func<Druid, DbTableColumn> resolver = d => null;

			ExpressionConversion_Accessor.CreateDbCondition (comparison, resolver);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void CreateDbConditionComparisonFieldValueTest3()
		{
			ComparisonFieldValue comparison = new ComparisonFieldValue (
				new Field (Druid.FromLong (1)),
				BinaryComparator.IsEqual,
				new Constant (0)
			);
			System.Func<Druid, DbTableColumn> resolver = null;

			ExpressionConversion_Accessor.CreateDbCondition (comparison, resolver);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetDbTableColumnTest1()
		{
			Dictionary<Druid, DbTableColumn> resolver = this.GetResolver ();

			foreach (Druid id in resolver.Keys)
			{
				Field field = new Field (id);

				DbTableColumn column1 = resolver[id];
				DbTableColumn column2 = ExpressionConversion_Accessor.GetDbTableColumn (field, druid => resolver[druid]);

				Assert.AreSame (column1, column2);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void GetDbTableColumnTest2()
		{
			Field field = null;
			System.Func<Druid, DbTableColumn> resolver = d => null;

			ExpressionConversion_Accessor.GetDbTableColumn (field, resolver);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void GetDbTableColumnTest3()
		{
			Field field = new Field (Druid.FromLong (1));
			System.Func<Druid, DbTableColumn> resolver = null;

			ExpressionConversion_Accessor.GetDbTableColumn (field, resolver);
		}


		private Dictionary<Druid, DbTableColumn> GetResolver()
		{
			DbTable table = new DbTable ("myTableName");

			List<DbColumn> columns = new List<DbColumn> ()
			{
				new DbColumn(Druid.FromLong(0), new DbTypeDef(IntegerType.Default)),
				new DbColumn(Druid.FromLong(1), new DbTypeDef(IntegerType.Default)),
				new DbColumn(Druid.FromLong(2), new DbTypeDef(IntegerType.Default)),
				new DbColumn(Druid.FromLong(3), new DbTypeDef(IntegerType.Default)),
				new DbColumn(Druid.FromLong(4), new DbTypeDef(IntegerType.Default)),
				new DbColumn(Druid.FromLong(5), new DbTypeDef(IntegerType.Default)),
				new DbColumn(Druid.FromLong(6), new DbTypeDef(IntegerType.Default)),
				new DbColumn(Druid.FromLong(7), new DbTypeDef(IntegerType.Default)),
				new DbColumn(Druid.FromLong(8), new DbTypeDef(IntegerType.Default)),
				new DbColumn(Druid.FromLong(9), new DbTypeDef(IntegerType.Default)),
			};

			table.Columns.AddRange (columns);

			Dictionary<Druid, DbTableColumn> resolver = new Dictionary<Druid, DbTableColumn> ()
			{
			    { Druid.FromLong(0), new DbTableColumn(columns[0]) },
				{ Druid.FromLong(1), new DbTableColumn(columns[1]) },
				{ Druid.FromLong(2), new DbTableColumn(columns[2]) },
				{ Druid.FromLong(3), new DbTableColumn(columns[3]) },
				{ Druid.FromLong(4), new DbTableColumn(columns[4]) },
				{ Druid.FromLong(5), new DbTableColumn(columns[5]) },
				{ Druid.FromLong(6), new DbTableColumn(columns[6]) },
				{ Druid.FromLong(7), new DbTableColumn(columns[7]) },
				{ Druid.FromLong(8), new DbTableColumn(columns[8]) },
				{ Druid.FromLong(9), new DbTableColumn(columns[9]) },
			};

			return resolver;
		}


	}


}
