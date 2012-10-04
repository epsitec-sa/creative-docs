using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;

using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;

using System.Linq;
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
			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void ArgumentCheck()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				ExceptionAssert.Throw<ArgumentNullException>
				(
				   () => LambdaConverter.Convert
				   (
						null,
						new ValueDataEntity (),
						(ValueDataEntity e) => e.IntegerValue == 0
				   )
				);
				
				ExceptionAssert.Throw<ArgumentNullException>
				(
				   () => LambdaConverter.Convert
				   (
						dataContext,
						null,
						(ValueDataEntity e) => e.IntegerValue == 0
				   )
				);

				ExceptionAssert.Throw<ArgumentNullException>
				(
					() => LambdaConverter.Convert
					(
						dataContext,
						new ValueDataEntity (),
						null
					)
				);

				ExceptionAssert.Throw<ArgumentException>
				(
					() => LambdaConverter.Convert
					(
						dataContext,
						new ValueDataEntity (),
						(LambdaExpression) (Expression<Func<object, bool>>) (i => i == null)
					)
				);

				ExceptionAssert.Throw<ArgumentException>
				(
					() => LambdaConverter.Convert
					(
						dataContext,
						new NaturalPersonEntity (),
						(Expression<Func<ValueDataEntity, bool>>) (e => e.IntegerValue == 0)
					)
				);

				ExceptionAssert.Throw<ArgumentException>
				(
					() => LambdaConverter.Convert
					(
						dataContext,
						new ValueDataEntity (),
						(Expression<Func<ValueDataEntity, ValueDataEntity, bool>>) ((e1, e2) => e2.IntegerValue == 0)
					)
				);

				ExceptionAssert.Throw<ArgumentException>
				(
					() => LambdaConverter.Convert
					(
						dataContext,
						new ValueDataEntity (),
						(Expression<Func<bool>>) (() => true)
					)
				);

				ExceptionAssert.Throw<ArgumentException>
				(
					() => LambdaConverter.Convert
					(
						dataContext,
						new ValueDataEntity (),
						(Expression<Func<ValueDataEntity, int>>) (e => e.IntegerValue)
					)
				);
			}
		}


		[TestMethod]
		public void RegularEqual()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void RegularEqualInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void RegularNotEqual()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void RegularGreater()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void RegularGreaterOrEqual()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void RegularLower()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void RegularLowerOrEqual()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void StringEqual()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void StringEqualInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void StringNotEqual()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void StringNotEqualInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void StringGreater()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void StringGreaterInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void StringGreaterOrEqual()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void StringGreaterOrEqualInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void StringLower()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void StringLowerInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void StringLowerOrEqual()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void StringLowerOrEqualInverted()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void Not()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void RegularIsNull()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
					entity,
					x => x.StringValue == null,
					new UnaryComparison
					(
						ValueField.Create (entity, x => x.StringValue),
						UnaryComparator.IsNull
					)
				);
			}
		}


		[TestMethod]
		public void RegularIsNullReversed()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
					entity,
					x => null == x.StringValue,
					new UnaryComparison
					(
						ValueField.Create (entity, x => x.StringValue),
						UnaryComparator.IsNull
					)
				);
			}
		}


		[TestMethod]
		public void RegularIsNotNull()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
					entity,
					x => x.StringValue != null,
					new UnaryComparison
					(
						ValueField.Create (entity, x => x.StringValue),
						UnaryComparator.IsNotNull
					)
				);
			}
		}


		[TestMethod]
		public void RegularIsNotNullReversed()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
					entity,
					x => null != x.StringValue,
					new UnaryComparison
					(
						ValueField.Create (entity, x => x.StringValue),
						UnaryComparator.IsNotNull
					)
				);
			}
		}


		[TestMethod]
		public void StringIsNull()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
					entity,
					x => SqlMethods.CompareTo (x.StringValue, null) == 0,
					new UnaryComparison
					(
						ValueField.Create (entity, x => x.StringValue),
						UnaryComparator.IsNull
					)
				);
			}
		}


		[TestMethod]
		public void StringIsNullReversed()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
					entity,
					x => SqlMethods.CompareTo (null, x.StringValue) == 0,
					new UnaryComparison
					(
						ValueField.Create (entity, x => x.StringValue),
						UnaryComparator.IsNull
					)
				);
			}
		}


		[TestMethod]
		public void StringIsNotNull()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
					entity,
					x => SqlMethods.CompareTo (x.StringValue, null) != 0,
					new UnaryComparison
					(
						ValueField.Create (entity, x => x.StringValue),
						UnaryComparator.IsNotNull
					)
				);
			}
		}


		[TestMethod]
		public void StringIsNotNullReversed()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
					entity,
					x => SqlMethods.CompareTo (null, x.StringValue) != 0,
					new UnaryComparison
					(
						ValueField.Create (entity, x => x.StringValue),
						UnaryComparator.IsNotNull
					)
				);
			}
		}


		[TestMethod]
		public void IsLike()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void IsLikeEscape()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void And()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void Or()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();

				this.Check
				(
					dataContext,
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
		}


		[TestMethod]
		public void MemberChain1()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new AddressEntity ()
				{
					Location = new LocationEntity (),
				};

				this.Check
				(
					dataContext,
					entity,
					x => x.Location.Name == "foo",
					new BinaryComparison
					(
						ValueField.Create (entity.Location, x => x.Name),
						BinaryComparator.IsEqual,
						new Constant ("foo")
					)
				);
			}
		}


		[TestMethod]
		public void MemberChain2()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new AddressEntity ()
				{
					Location = new LocationEntity ()
					{
						Region = new RegionEntity ()
					}
				};

				this.Check
				(
					dataContext,
					entity,
					x => x.Location.Region.Name == "foo",
					new BinaryComparison
					(
						ValueField.Create (entity.Location.Region, x => x.Name),
						BinaryComparator.IsEqual,
						new Constant ("foo")
					)
				);
			}
		}


		[TestMethod]
		public void MemberChain3()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new AddressEntity ()
				{
					Location = new LocationEntity ()
					{
						Region = new RegionEntity ()
						{
							Country = new CountryEntity ()
						}
					}
				};

				this.Check
				(
					dataContext,
					entity,
					x => x.Location.Region.Country.Name == "foo",
					new BinaryComparison
					(
						ValueField.Create (entity.Location.Region.Country, x => x.Name),
						BinaryComparator.IsEqual,
						new Constant ("foo")
					)
				);
			}
		}


		[TestMethod]
		public void ConstantCapturedVariable()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();
				var constant = 1;

				this.Check
				(
					dataContext,
					entity,
					x => x.IntegerValue == constant,
					new BinaryComparison
					(
						ValueField.Create (entity, x => x.IntegerValue),
						BinaryComparator.IsEqual,
						new Constant (constant)
					)
				);
			}
		}


		[TestMethod]
		public void ConstantCapturedPropertyAccess()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();
				var constant = DateTime.Now;

				this.Check
				(
					dataContext,
					entity,
					x => x.IntegerValue == constant.Year,
					new BinaryComparison
					(
						ValueField.Create (entity, x => x.IntegerValue),
						BinaryComparator.IsEqual,
						new Constant (constant.Year)
					)
				);
			}
		}


		[TestMethod]
		public void ConstantCapturedMethodCall()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();
				var constant = new List<int> ();

				this.Check
				(
					dataContext,
					entity,
					x => x.IntegerValue == constant.Count (),
					new BinaryComparison
					(
						ValueField.Create (entity, x => x.IntegerValue),
						BinaryComparator.IsEqual,
						new Constant (constant.Count ())
					)
				);
			}
		}


		[TestMethod]
		public void ConstantCapturedMember()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();
				this.Check
				(
					dataContext,
					entity,
					x => x.IntegerValue == this.member,
					new BinaryComparison
					(
						ValueField.Create (entity, x => x.IntegerValue),
						BinaryComparator.IsEqual,
						new Constant (this.member)
					)
				);
			}
		}


		[TestMethod]
		public void ConstantMathOperation()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();
				this.Check
				(
					dataContext,
					entity,
					x => x.IntegerValue == (this.member + 2) / 5 % 3,
					new BinaryComparison
					(
						ValueField.Create (entity, x => x.IntegerValue),
						BinaryComparator.IsEqual,
						new Constant ((this.member + 2) / 5 % 3)
					)
				);
			}
		}


		[TestMethod]
		public void EvaluableOperationWithinNonEvaluableOperation()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new LocationEntity ();

				this.Check
				(
					dataContext,
					entity,
					x => SqlMethods.Like (x.Name, this.member.ToString () + "___"),
					new BinaryComparison
					(
						ValueField.Create (entity, x => x.Name),
						BinaryComparator.IsLike,
						new Constant (this.member.ToString () + "___")
					)
				);
			}
		}


		[TestMethod]
		public void NullableMember()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new PersonTitleEntity ();
				this.Check
				(
					dataContext,
					entity,
					x => x.Rank == 0,
					new BinaryComparison
					(
						ValueField.Create (entity, x => x.Rank),
						BinaryComparator.IsEqual,
						new Constant (0)
					)
				);
			}
		}


		[TestMethod]
		public void Conversion1()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();
				this.Check
				(
					dataContext,
					entity,
					x => x.IntegerValue == null,
					new UnaryComparison
					(
						ValueField.Create (entity, x => x.IntegerValue),
						UnaryComparator.IsNull
					)
				);
			}
		}


		[TestMethod]
		public void Conversion2()
		{
			using (var dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var entity = new ValueDataEntity ();
				this.Check
				(
					dataContext,
					entity,
					x => x.IntegerValue == 1m,
					new BinaryComparison
					(
						ValueField.Create (entity, x => x.IntegerValue),
						BinaryComparator.IsEqual,
						new Constant (1)
					)
				);
			}
		}


		[TestMethod]
		public void EntityComparison1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var person1 = new NaturalPersonEntity ();
				var person2 = dataContext.GetByExample (new NaturalPersonEntity ()).First ();
				var person2Key = dataContext.GetNormalizedEntityKey (person2).Value;

				this.Check
				(
					dataContext,
					person1,
					x => x != person2,
					new BinaryComparison
					(
						InternalField.CreateId (person1),
						BinaryComparator.IsNotEqual,
						new Constant (person2Key.RowKey.Id.Value)
					)
				);
			}
		}


		[TestMethod]
		public void EntityComparison2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var person = new NaturalPersonEntity ();
				var gender = dataContext.GetByExample (new PersonGenderEntity ()).First ();
				var genderKey = dataContext.GetNormalizedEntityKey (gender).Value;

				this.Check
				(
					dataContext,
					person,
					x => x.Gender == gender,
					new BinaryComparison
					(
						ReferenceField.Create (person, x => x.Gender),
						BinaryComparator.IsEqual,
						new Constant (genderKey.RowKey.Id.Value)
					)
				);
			}
		}


		[TestMethod]
		public void AnyCall1()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var person = new NaturalPersonEntity ();
				var contact = new UriContactEntity ();
				person.Contacts.Add (contact);

				this.Check
				(
					dataContext,
					person,
					x => x.Contacts.Any (c => ((UriContactEntity) c).Uri == "blupi@hotmail.com"),
					new BinaryComparison
					(
						ValueField.Create (contact, x => x.Uri),
						BinaryComparator.IsEqual,
						new Constant ("blupi@hotmail.com")
					)
				);
			}
		}


		[TestMethod]
		public void AnyCall2()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var person = new NaturalPersonEntity ();
				var contact = new UriContactEntity ();
				person.Contacts.Add (contact);

				this.Check
				(
					dataContext,
					person,
					x => x.Firstname == "blupi" && x.Contacts.Any (c => ((UriContactEntity) c).Uri == "blupi@hotmail.com"),
					new BinaryOperation
					(
						new BinaryComparison
						(
							ValueField.Create (person, x => x.Firstname),
							BinaryComparator.IsEqual,
							new Constant ("blupi")
						),
						BinaryOperator.And,			
						new BinaryComparison
						(
							ValueField.Create (contact, x => x.Uri),
							BinaryComparator.IsEqual,
							new Constant ("blupi@hotmail.com")
						)
					)
				);
			}
		}


		[TestMethod]
		public void AnyCall3()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var person = new NaturalPersonEntity ();
				var contact = new UriContactEntity ();
				var role = new ContactRoleEntity ();

				person.Contacts.Add (contact);
				contact.Roles.Add (role);

				this.Check
				(
					dataContext,
					person,
					x => x.Contacts.Any (c => c.Roles.Any (r => r.Name == "mySuperRole")),
					new BinaryComparison
					(
						ValueField.Create (role, x => x.Name),
						BinaryComparator.IsEqual,
						new Constant ("mySuperRole")
					)
				);
			}
		}


		[TestMethod]
		public void AnyCall4()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				var person = new NaturalPersonEntity ();
				var contact = new UriContactEntity ();
				var role = new ContactRoleEntity ();

				person.Contacts.Add (contact);
				contact.Roles.Add (role);

				this.Check
				(
					dataContext,
					person,
					x => x.Contacts.Any
					(
						c => ((UriContactEntity) c).Uri == "blupi@hotmail.com" || c.Roles.Any
						(
							r => r.Name == "mySuperRole"
						)
					),
					new BinaryOperation
					(
						new BinaryComparison
						(
							ValueField.Create (contact, x => x.Uri),
							BinaryComparator.IsEqual,
							new Constant ("blupi@hotmail.com")
						),
						BinaryOperator.Or,
						new BinaryComparison
						(
							ValueField.Create (role, x => x.Name),
							BinaryComparator.IsEqual,
							new Constant ("mySuperRole")
						)
					)
				);
			}
		}


		private int member = 1;

		
		public void Check<T>(DataContext dataContext, T entity, Expression<Func<T, bool>> lambda, DataExpression result)
			where T : AbstractEntity
		{
			var actual = LambdaConverter.Convert (dataContext, entity, lambda);

			DeepAssert.AreEqual (result, actual);
		}


	}


}
