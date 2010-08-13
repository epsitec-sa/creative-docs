using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.Loader
{


	[TestClass]
	public sealed class UnitTestExpressionFields
	{
		
		
		[TestMethod]
		public void GetFieldsTestUnaryOperation()
		{
			foreach (Field field1 in this.GetSampleFields ())
			{
				foreach (Field field2 in this.GetSampleFields ())
				{
					UnaryOperation operation = new UnaryOperation (
						UnaryOperator.Not,
						new ComparisonFieldField (field1, BinaryComparator.IsEqual, field2)
					);

					List<Druid> fields = ExpressionFields.GetFields (operation).ToList ();

					if (field1.FieldId == field2.FieldId)
					{
						Assert.IsTrue (fields.Count () == 1);
						Assert.AreEqual (field1.FieldId, fields.Single ());
					}
					else
					{
						Assert.IsTrue (fields.Count () == 2);
						CollectionAssert.Contains (fields, field1.FieldId);
						CollectionAssert.Contains (fields, field2.FieldId);
					}
				}
			}
		}


		[TestMethod]
		public void GetFieldsUnaryOperationArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => ExpressionFields.GetFields ((UnaryOperation) null)
			);
		}


		[TestMethod]
		public void GetFieldsTestBinaryOperation()
		{
			foreach (Field field1 in this.GetSampleFields ())
			{
				foreach (Field field2 in this.GetSampleFields ())
				{
					BinaryOperation operation1 = new BinaryOperation (
						new UnaryComparison (field1, UnaryComparator.IsNull),
						BinaryOperator.And,
						new UnaryComparison (field2, UnaryComparator.IsNull)
					);

					List<Druid> fields1 = ExpressionFields.GetFields(operation1).ToList() ;
					HashSet<Druid> fields2 = new HashSet<Druid> () { field1.FieldId, field2.FieldId, };

					Assert.IsTrue (fields1.Count == fields2.Count);
					Assert.IsTrue (fields1.Except (fields2).Count () == 0);

					foreach (Field field3 in this.GetSampleFields ())
					{
						BinaryOperation operation2 = new BinaryOperation (
							new UnaryComparison (field1, UnaryComparator.IsNull),
							BinaryOperator.And,
							new ComparisonFieldField (field2, BinaryComparator.IsEqual, field3)
						);

						List<Druid> fields3 = ExpressionFields.GetFields (operation2).ToList ();
						HashSet<Druid> fields4 = new HashSet<Druid> () { field1.FieldId, field2.FieldId, field3.FieldId };

						Assert.IsTrue (fields3.Count == fields4.Count);
						Assert.IsTrue (fields3.Except (fields4).Count () == 0);

						foreach (Field field4 in this.GetSampleFields ())
						{
							BinaryOperation operation3 = new BinaryOperation (
								new ComparisonFieldField (field1, BinaryComparator.IsEqual, field2),
								BinaryOperator.And,
								new ComparisonFieldField (field3, BinaryComparator.IsEqual, field4)
							);

							List<Druid> fields5 = ExpressionFields.GetFields (operation3).ToList ();
							HashSet<Druid> fields6 = new HashSet<Druid> () { field1.FieldId, field2.FieldId, field3.FieldId, field4.FieldId };

							Assert.IsTrue (fields5.Count == fields6.Count);
							Assert.IsTrue (fields5.Except (fields6).Count () == 0);
						}
					}
				}
			}
		}


		[TestMethod]
		public void GetFieldsBinaryOperationArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => ExpressionFields.GetFields ((BinaryOperation) null)
			);
		}


		[TestMethod]
		public void GetFieldsTestUnaryComparison()
		{
			foreach (Field field in this.GetSampleFields ())
			{
				UnaryComparison comparison = new UnaryComparison (field, UnaryComparator.IsNull);

				List<Druid> fields = ExpressionFields.GetFields (comparison).ToList ();

				Assert.IsTrue (fields.Count () == 1);
				Assert.AreEqual (field.FieldId, fields.Single ());
			}
		}


		[TestMethod]
		public void GetFieldsUnaryComparisonArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => ExpressionFields.GetFields ((UnaryComparison) null)
			);
		}


		[TestMethod]
		public void GetFieldsTestComparisonFieldField1()
		{
			foreach (Field field1 in this.GetSampleFields ())
			{
				foreach (Field field2 in this.GetSampleFields ())
				{
					ComparisonFieldField comparison = new ComparisonFieldField (field1, BinaryComparator.IsEqual, field2);

					List<Druid> fields = ExpressionFields.GetFields (comparison).ToList ();

					if (field1.FieldId == field2.FieldId)
					{
						Assert.IsTrue (fields.Count () == 1);
						Assert.AreEqual (field1.FieldId, fields.Single ());
					}
					else
					{
						Assert.IsTrue (fields.Count () == 2);
						CollectionAssert.Contains (fields, field1.FieldId);
						CollectionAssert.Contains (fields, field2.FieldId);
					}
				}
			}
		}


		[TestMethod]
		public void GetFieldsComparisonFieldFieldArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => ExpressionFields.GetFields ((ComparisonFieldField) null)
			);
		}


		[TestMethod]
		public void GetFieldsTestComparisonFieldValue()
		{
			foreach (Field field in this.GetSampleFields ())
			{
				ComparisonFieldValue comparison = new ComparisonFieldValue (field, BinaryComparator.IsEqual, new Constant (0));

				List<Druid> fields = ExpressionFields.GetFields (comparison).ToList ();

                Assert.IsTrue (fields.Count () == 1);
				Assert.AreEqual (field.FieldId, fields.Single ());
			}
		}


		[TestMethod]
		public void GetFieldsComparisonFieldValueArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => ExpressionFields.GetFields ((ComparisonFieldValue) null)
			);
		}


		public IEnumerable<Field> GetSampleFields()
		{
			for (int i = 0; i < 5; i++)
			{
				Druid id = Druid.FromLong (i);

				yield return new Field (id);
			}
		}


	}


}
