using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Expressions
{


	[TestClass]
	public sealed class UnitTestSetComparison
	{


		// TODO Add tests for CreateSqlCondition(...)
		// TODO Add tests for CheckFields(...)


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void ConstructorTest()
		{
			var fields = Enumerable.Range (0, 100)
				.Select (i => new ValueField (new NaturalPersonEntity (), Druid.FromLong (1)))
				.ToList ();

			var dice = new Random ();

			for (int i = 0; i < 100; i++)
			{
				var field = fields.Shuffle ().First ();
				var set = fields.Shuffle ().Take (10).ToList ();
				var comparator = dice.NextDouble () > 0.5 ? SetComparator.In: SetComparator.NotIn;

				var setComparison = new ValueSetComparison (field, comparator, set);

				Assert.AreSame (field, setComparison.Field);
				Assert.AreEqual (comparator, setComparison.Comparator);
				CollectionAssert.AreEqual (set, setComparison.Set.ToList ());
			}
		}


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			var field = new ValueField (new NaturalPersonEntity (), Druid.FromLong (1));

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new ValueSetComparison (null, SetComparator.In, new List<Value> () { field })
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new ValueSetComparison (field, SetComparator.In, null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new ValueSetComparison (field, SetComparator.In, new List<Value> ()
				{
				})
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new ValueSetComparison (field, SetComparator.In, new List<Value> () { null })
			);
		}


	}


}
