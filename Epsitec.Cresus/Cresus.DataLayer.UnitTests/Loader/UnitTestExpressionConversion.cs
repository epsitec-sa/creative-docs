using Epsitec.Common.Support;

using Epsitec.Common.Types;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;



namespace Epsitec.Cresus.DataLayer.UnitTests.Loader
{


	 [TestClass]
	public sealed class UnitTestExpressionConversion
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseHelper.CreateAndConnectToDatabase ();
		}


		[ClassCleanup]
		public static void ClassCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestMethod]
		public void CreateDbConditionUnaryOperationTest()
		{
			Dictionary<Druid, DbTableColumn> resolver = this.GetResolver ();

			Field field = new Field (Druid.FromLong (1));

			UnaryComparator comparator = UnaryComparator.IsNull;

			UnaryComparison comparison = new UnaryComparison (field, comparator);

			UnaryOperator op = UnaryOperator.Not;

			UnaryOperation operation = new UnaryOperation (op, comparison);

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					ExpressionConverter converter = new ExpressionConverter (dataContext);

					DbAbstractCondition condition1 = converter.CreateDbCondition (operation, (id) => resolver[id]);

					Assert.IsInstanceOfType (condition1, typeof (DbConditionModifier));

					DbConditionModifier condition2 = condition1 as DbConditionModifier;

					Assert.AreEqual (condition2.Operator, EnumConverter_Accessor.ToDbConditionModifierOperator (op));

					DbAbstractCondition condition3 = condition2.Condition;

					Assert.IsInstanceOfType (condition3, typeof (DbSimpleCondition));

					DbSimpleCondition_Accessor condition4 = new DbSimpleCondition_Accessor (new PrivateObject (condition3 as DbSimpleCondition));

					Assert.AreSame (condition4.Left, resolver[field.FieldId]);
					Assert.AreEqual (condition4.Operator, EnumConverter_Accessor.ToDbSimpleConditionOperator (comparator));
					Assert.IsNull (condition4.RightColumn);
					Assert.AreEqual (1, condition4.argumentCount);
				}
			}
		}


		[TestMethod]
		public void CreateDbConditionUnaryOperationArgumentCheck()
		{
			UnaryOperation operation = new UnaryOperation (
				UnaryOperator.Not,
				new UnaryComparison (
					new Field (Druid.FromLong (1)),
					UnaryComparator.IsNull
				)
			);
			System.Func<Druid, DbTableColumn> resolver = d => null;

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					ExpressionConverter converter = new ExpressionConverter (dataContext);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => converter.CreateDbCondition ((UnaryOperation) null, resolver)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => converter.CreateDbCondition (operation, null)
					);
				}
			}
		}


		[TestMethod]
		public void CreateDbConditionBinaryOperationTest()
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

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					ExpressionConverter converter = new ExpressionConverter (dataContext);

					DbAbstractCondition condition1 = converter.CreateDbCondition (operation, (id) => resolver[id]);

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
			}
		}


		[TestMethod]
		public void CreateDbConditionBinaryOperationArgumentCheck()
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
			System.Func<Druid, DbTableColumn> resolver = d => null;

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					ExpressionConverter converter = new ExpressionConverter (dataContext);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => converter.CreateDbCondition ((BinaryOperation) null, resolver)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => converter.CreateDbCondition (operation, null)
					);
				}
			}
		}


		[TestMethod]
		public void CreateDbConditionUnaryComparisonTest()
		{
			Dictionary<Druid, DbTableColumn> resolver = this.GetResolver ();

			Field field = new Field (Druid.FromLong (1));
			UnaryComparator comparator = UnaryComparator.IsNull;

			UnaryComparison comparison = new UnaryComparison (field, comparator);

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					ExpressionConverter converter = new ExpressionConverter (dataContext);

					DbAbstractCondition condition1 = converter.CreateDbCondition (comparison, (id) => resolver[id]);

					Assert.IsInstanceOfType (condition1, typeof (DbSimpleCondition));

					DbSimpleCondition_Accessor condition2 = new DbSimpleCondition_Accessor (new PrivateObject (condition1 as DbSimpleCondition));

					Assert.AreSame (condition2.Left, resolver[field.FieldId]);
					Assert.AreEqual (condition2.Operator, EnumConverter_Accessor.ToDbSimpleConditionOperator (comparator));
					Assert.IsNull (condition2.RightColumn);
					Assert.AreEqual (1, condition2.argumentCount);
				}
			}
		}


		[TestMethod]
		public void CreateDbConditionUnaryComparisonArgumentCheck()
		{
			UnaryComparison comparison = new UnaryComparison (
				new Field (Druid.FromLong (1)),
				UnaryComparator.IsNull
			);
			System.Func<Druid, DbTableColumn> resolver = d => null;

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					ExpressionConverter converter = new ExpressionConverter (dataContext);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => converter.CreateDbCondition ((UnaryComparison) null, resolver)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => converter.CreateDbCondition (comparison, null)
					);
				}
			}
		}


		[TestMethod]
		public void CreateDbConditionComparisonFieldFieldTest()
		{
			Dictionary<Druid, DbTableColumn> resolver = this.GetResolver ();

			Field field1 = new Field (Druid.FromLong (1));
			BinaryComparator comparator = BinaryComparator.IsEqual;
			Field field2 = new Field (Druid.FromLong (2));

			ComparisonFieldField comparison = new ComparisonFieldField (field1, comparator, field2);

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					ExpressionConverter converter = new ExpressionConverter (dataContext);

					DbAbstractCondition condition1 = converter.CreateDbCondition (comparison, (id) => resolver[id]);

					Assert.IsInstanceOfType (condition1, typeof (DbSimpleCondition));

					DbSimpleCondition_Accessor condition2 = new DbSimpleCondition_Accessor (new PrivateObject (condition1 as DbSimpleCondition));

					Assert.AreSame (condition2.Left, resolver[field1.FieldId]);
					Assert.AreEqual (condition2.Operator, EnumConverter_Accessor.ToDbSimpleConditionOperator (comparator));
					Assert.AreSame (condition2.RightColumn, resolver[field2.FieldId]);
					Assert.AreEqual (2, condition2.argumentCount);
				}
			}
		}


		[TestMethod]
		public void CreateDbConditionComparisonFieldFieldArgumentCheck()
		{
			ComparisonFieldField comparison = new ComparisonFieldField (
				new Field (Druid.FromLong (1)),
				BinaryComparator.IsEqual,
				new Field (Druid.FromLong (2))
			);
			System.Func<Druid, DbTableColumn> resolver = d => null;

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					ExpressionConverter converter = new ExpressionConverter (dataContext);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => converter.CreateDbCondition ((ComparisonFieldField) null, null)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => converter.CreateDbCondition (comparison, null)
					);
				}
			}
		}


		[TestMethod]
		public void CreateDbConditionComparisonFieldValueTest()
		{
			Dictionary<Druid, DbTableColumn> resolver = this.GetResolver();

			Field field = new Field (Druid.FromLong (1));
			BinaryComparator comparator = BinaryComparator.IsEqual;
			Constant constant = new Constant (1);

			ComparisonFieldValue comparison = new ComparisonFieldValue (field, comparator, constant);

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					ExpressionConverter converter = new ExpressionConverter (dataContext);

					DbAbstractCondition condition1 = converter.CreateDbCondition (comparison, (id) => resolver[id]);

					Assert.IsInstanceOfType (condition1, typeof (DbSimpleCondition));

					DbSimpleCondition_Accessor condition2 = new DbSimpleCondition_Accessor (new PrivateObject (condition1 as DbSimpleCondition));

					Assert.AreSame (condition2.Left, resolver[field.FieldId]);
					Assert.AreEqual (condition2.Operator, EnumConverter_Accessor.ToDbSimpleConditionOperator (comparator));
					Assert.AreEqual (condition2.RightConstantRawType, EnumConverter_Accessor.ToDbRawType (Type_Accessor.Int32));
					Assert.AreEqual (condition2.RightConstantValue, 1);
					Assert.IsNull (condition2.RightColumn);
					Assert.AreEqual (2, condition2.argumentCount);
				}
			}
		}


		[TestMethod]
		public void CreateDbConditionComparisonFieldValueArgumentCheck()
		{
			ComparisonFieldValue comparison = new ComparisonFieldValue (
				new Field (Druid.FromLong (1)),
				BinaryComparator.IsEqual,
				new Constant (0)
			);
			System.Func<Druid, DbTableColumn> resolver = d => null;

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					ExpressionConverter converter = new ExpressionConverter (dataContext);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => converter.CreateDbCondition ((ComparisonFieldValue) null, null)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => converter.CreateDbCondition (comparison, null)
					);
				}
			}
		}


		[TestMethod]
		public void GetDbTableColumnTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					ExpressionConverter converter = new ExpressionConverter (dataContext);
					ExpressionConverter_Accessor converterAccessor = new ExpressionConverter_Accessor (new PrivateObject (converter));

					Dictionary<Druid, DbTableColumn> resolver = this.GetResolver ();

					foreach (Druid id in resolver.Keys)
					{
						Field field = new Field (id);

						DbTableColumn column1 = resolver[id];
						DbTableColumn column2 =  converterAccessor.GetDbTableColumn (field, druid => resolver[druid]);

						Assert.AreSame (column1, column2);
					}
				}
			}
		}


		[TestMethod]
		public void GetDbTableColumnArgumentCheck()
		{
			Field field = new Field (Druid.FromLong (1));
			System.Func<Druid, DbTableColumn> resolver = d => null;

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					ExpressionConverter converter = new ExpressionConverter (dataContext);
					ExpressionConverter_Accessor converterAccessor = new ExpressionConverter_Accessor (new PrivateObject (converter));

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => converterAccessor.GetDbTableColumn (null, resolver)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => converterAccessor.GetDbTableColumn (field, null)
					);
				}
			}
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
