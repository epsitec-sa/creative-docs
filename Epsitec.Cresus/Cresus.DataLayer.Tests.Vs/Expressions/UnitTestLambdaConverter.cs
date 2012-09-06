using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Expressions;

using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Linq.Expressions;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Expressions
{


	[TestClass]
	public sealed class UnitTestLambdaConverter
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void ArgumentCheck()
		{
			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => LambdaConverter.Convert
				(
					null,
					(ValueDataEntity e) => e.IntegerValue == 0
				)
			);

			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => LambdaConverter.Convert
				(
					new ValueDataEntity (),
					null
				)
			);

			ExceptionAssert.Throw<ArgumentException>
			(
				() => LambdaConverter.Convert
				(
					new ValueDataEntity (),
					(LambdaExpression) (Expression<Func<object, bool>>) (i => i == null)
				)
			);

			ExceptionAssert.Throw<ArgumentException>
			(
				() => LambdaConverter.Convert
				(
					new NaturalPersonEntity (),
					(Expression<Func<ValueDataEntity, bool>>) (e => e.IntegerValue == 0)
				)
			);

			ExceptionAssert.Throw<ArgumentException>
			(
				() => LambdaConverter.Convert
				(
					new ValueDataEntity (),
					(Expression<Func<ValueDataEntity, ValueDataEntity, bool>>) ((e1, e2) => e2.IntegerValue == 0)
				)
			);

			ExceptionAssert.Throw<ArgumentException>
			(
				() => LambdaConverter.Convert
				(
					new ValueDataEntity (),
					(Expression<Func<bool>>) (() => true)
				)
			);

			ExceptionAssert.Throw<ArgumentException>
			(
			  () => LambdaConverter.Convert
				  (
					  new ValueDataEntity (),
					 (Expression<Func<ValueDataEntity, int>>) (e => e.IntegerValue)
				  )
			);
		}


		[TestMethod]
		public void RegularEqual()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => x.IntegerValue == 1,
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.IntegerValue),
					BinaryComparator.IsEqual,
					new Constant (1)
				)
			);
		}


		[TestMethod]
		public void RegularEqualInverted()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => 1 == x.IntegerValue,
				new BinaryComparison
				(
					new Constant (1),
					BinaryComparator.IsEqual,
					ValueField.Create (entity, x => x.IntegerValue)
				)
			);
		}


		[TestMethod]
		public void RegularNotEqual()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => x.IntegerValue != 1,
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.IntegerValue),
					BinaryComparator.IsNotEqual,
					new Constant (1)
				)
			);
		}


		[TestMethod]
		public void RegularGreater()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => x.IntegerValue > 1,
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.IntegerValue),
					BinaryComparator.IsGreater,
					new Constant (1)
				)
			);
		}


		[TestMethod]
		public void RegularGreaterOrEqual()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => x.IntegerValue >= 1,
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.IntegerValue),
					BinaryComparator.IsGreaterOrEqual,
					new Constant (1)
				)
			);
		}


		[TestMethod]
		public void RegularLower()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => x.IntegerValue < 1,
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.IntegerValue),
					BinaryComparator.IsLower,
					new Constant (1)
				)
			);
		}


		[TestMethod]
		public void RegularLowerOrEqual()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => x.IntegerValue <= 1,
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.IntegerValue),
					BinaryComparator.IsLowerOrEqual,
					new Constant (1)
				)
			);
		}


		[TestMethod]
		public void StringEqual()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => SqlMethods.CompareTo (x.StringValue, "foo") == 0,
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.StringValue),
					BinaryComparator.IsEqual,
					new Constant ("foo")
				)
			);
		}


		[TestMethod]
		public void StringEqualInverted()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => 0 == SqlMethods.CompareTo (x.StringValue, "foo"),
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.StringValue),
					BinaryComparator.IsEqual,
					new Constant ("foo")
				)
			);
		}


		[TestMethod]
		public void StringNotEqual()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => SqlMethods.CompareTo (x.StringValue, "foo") != 0,
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.StringValue),
					BinaryComparator.IsNotEqual,
					new Constant ("foo")
				)
			);
		}


		[TestMethod]
		public void StringNotEqualInverted()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => 0 != SqlMethods.CompareTo (x.StringValue, "foo"),
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.StringValue),
					BinaryComparator.IsNotEqual,
					new Constant ("foo")
				)
			);
		}


		[TestMethod]
		public void StringGreater()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => SqlMethods.CompareTo (x.StringValue, "foo") > 0,
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.StringValue),
					BinaryComparator.IsGreater,
					new Constant ("foo")
				)
			);
		}


		[TestMethod]
		public void StringGreaterInverted()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => 0 > SqlMethods.CompareTo (x.StringValue, "foo"),
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.StringValue),
					BinaryComparator.IsLower,
					new Constant ("foo")
				)
			);
		}


		[TestMethod]
		public void StringGreaterOrEqual()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => SqlMethods.CompareTo (x.StringValue, "foo") >= 0,
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.StringValue),
					BinaryComparator.IsGreaterOrEqual,
					new Constant ("foo")
				)
			);
		}


		[TestMethod]
		public void StringGreaterOrEqualInverted()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => 0 >= SqlMethods.CompareTo (x.StringValue, "foo"),
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.StringValue),
					BinaryComparator.IsLowerOrEqual,
					new Constant ("foo")
				)
			);
		}


		[TestMethod]
		public void StringLower()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => SqlMethods.CompareTo (x.StringValue, "foo") < 0,
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.StringValue),
					BinaryComparator.IsLower,
					new Constant ("foo")
				)
			);
		}


		[TestMethod]
		public void StringLowerInverted()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => 0 < SqlMethods.CompareTo (x.StringValue, "foo"),
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.StringValue),
					BinaryComparator.IsGreater,
					new Constant ("foo")
				)
			);
		}


		[TestMethod]
		public void StringLowerOrEqual()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => SqlMethods.CompareTo (x.StringValue, "foo") <= 0,
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.StringValue),
					BinaryComparator.IsLowerOrEqual,
					new Constant ("foo")
				)
			);
		}


		[TestMethod]
		public void StringLowerOrEqualInverted()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => 0 <= SqlMethods.CompareTo (x.StringValue, "foo"),
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.StringValue),
					BinaryComparator.IsGreaterOrEqual,
					new Constant ("foo")
				)
			);
		}


		[TestMethod]
		public void Not()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => !(x.IntegerValue == 1),
				new UnaryOperation
				(
					UnaryOperator.Not,
					new BinaryComparison
					(
						ValueField.Create (entity, x => x.IntegerValue),
						BinaryComparator.IsEqual,
						new Constant (1)
					)
				)
			);
		}


		[TestMethod]
		public void IsNull()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => SqlMethods.IsNull (x.StringValue),
				new UnaryComparison
				(
					ValueField.Create (entity, x => x.StringValue),
					UnaryComparator.IsNull
				)
			);
		}


		[TestMethod]
		public void IsNotNull()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => SqlMethods.IsNotNull (x.StringValue),
				new UnaryComparison
				(
					ValueField.Create (entity, x => x.StringValue),
					UnaryComparator.IsNotNull
				)
			);
		}


		[TestMethod]
		public void IsLike()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => SqlMethods.Like (x.StringValue, "foo"),
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.StringValue),
					BinaryComparator.IsLike,
					new Constant ("foo")
				)
			);
		}


		[TestMethod]
		public void IsLikeEscape()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => SqlMethods.EscapedLike (x.StringValue, "foo"),
				new BinaryComparison
				(
					ValueField.Create (entity, x => x.StringValue),
					BinaryComparator.IsLikeEscape,
					new Constant ("foo")
				)
			);
		}


		[TestMethod]
		public void And()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => x.IntegerValue > 0 && x.IntegerValue < 10,
				new BinaryOperation
				(
					new BinaryComparison
					(
						ValueField.Create (entity, x => x.IntegerValue),
						BinaryComparator.IsGreater,
						new Constant (0)
					),
					BinaryOperator.And,
					new BinaryComparison
					(
						ValueField.Create (entity, x => x.IntegerValue),
						BinaryComparator.IsLower,
						new Constant (10)
					)
				)
			);
		}


		[TestMethod]
		public void Or()
		{
			var entity = new ValueDataEntity ();

			this.Check
			(
				entity,
				x => x.IntegerValue < 0 || x.IntegerValue > 10,
				new BinaryOperation
				(
					new BinaryComparison
					(
						ValueField.Create (entity, x => x.IntegerValue),
						BinaryComparator.IsLower,
						new Constant (0)
					),
					BinaryOperator.Or,
					new BinaryComparison
					(
						ValueField.Create (entity, x => x.IntegerValue),
						BinaryComparator.IsGreater,
						new Constant (10)
					)
				)
			);
		}

		
		public void Check<T>(T entity, Expression<Func<T, bool>> lambda, DataExpression result)
			where T : AbstractEntity
		{
			var actual = LambdaConverter.Convert (entity, lambda);

			DeepAssert.AreEqual (result, actual);
		}


	}


}
