using Epsitec.Common.Support;

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
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetFieldsTestUnaryOperation1()
		{
			foreach (Field field1 in this.GetSampleFields ())
			{
				foreach (Field field2 in this.GetSampleFields ())
				{
					UnaryOperation operation = new UnaryOperation (
						UnaryOperator.Not,
						new ComparisonFieldField (field1, BinaryComparator.IsEqual, field2)
					);

					List<Druid> fields = ExpressionFields_Accessor.GetFields (operation).ToList ();

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
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void GetFieldsUnaryOperation2()
		{
			UnaryOperation operation = null;

			ExpressionFields_Accessor.GetFields (operation);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetFieldsTestBinaryOperation1()
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

					List<Druid> fields1 = ExpressionFields_Accessor.GetFields(operation1).ToList() ;
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

						List<Druid> fields3 = ExpressionFields_Accessor.GetFields (operation2).ToList ();
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

							List<Druid> fields5 = ExpressionFields_Accessor.GetFields (operation3).ToList ();
							HashSet<Druid> fields6 = new HashSet<Druid> () { field1.FieldId, field2.FieldId, field3.FieldId, field4.FieldId };

							Assert.IsTrue (fields5.Count == fields6.Count);
							Assert.IsTrue (fields5.Except (fields6).Count () == 0);
						}
					}
				}
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void GetFieldsBinaryOperation2()
		{
			BinaryOperation operation = null;

			ExpressionFields_Accessor.GetFields (operation);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetFieldsTestUnaryComparison1()
		{
			foreach (Field field in this.GetSampleFields ())
			{
				UnaryComparison comparison = new UnaryComparison (field, UnaryComparator.IsNull);

				List<Druid> fields = ExpressionFields_Accessor.GetFields (comparison).ToList ();

				Assert.IsTrue (fields.Count () == 1);
				Assert.AreEqual (field.FieldId, fields.Single ());
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void GetFieldsUnaryComparison2()
		{
			UnaryComparison comparison = null;

			ExpressionFields_Accessor.GetFields (comparison);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetFieldsTestComparisonFieldField1()
		{
			foreach (Field field1 in this.GetSampleFields ())
			{
				foreach (Field field2 in this.GetSampleFields ())
				{
					ComparisonFieldField comparison = new ComparisonFieldField (field1, BinaryComparator.IsEqual, field2);

					List<Druid> fields = ExpressionFields_Accessor.GetFields (comparison).ToList ();

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
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void GetFieldsComparisonFieldField2()
		{
			ComparisonFieldField comparison = null;

			ExpressionFields_Accessor.GetFields (comparison);
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetFieldsTestComparisonFieldValue1()
		{
			foreach (Field field in this.GetSampleFields ())
			{
				ComparisonFieldValue comparison = new ComparisonFieldValue (field, BinaryComparator.IsEqual, new Constant (0));

				List<Druid> fields = ExpressionFields_Accessor.GetFields (comparison).ToList ();

                Assert.IsTrue (fields.Count () == 1);
				Assert.AreEqual (field.FieldId, fields.Single ());
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void GetFieldsComparisonFieldValue2()
		{
			ComparisonFieldValue comparison = null;

			ExpressionFields_Accessor.GetFields (comparison);
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
