using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.General
{


	[TestClass]
	public sealed class UnitTestRelatedDataLoader
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void SimpleTest()
		{
			using (var db = DB.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
			{
				var entities = dataContext.GetByExample (new UriContactEntity ()).Take (2).ToList ();
				var expression = new List<LambdaExpression> ()
				{
					UnitTestRelatedDataLoader.Convert ((UriContactEntity c) => c.Uri),
					UnitTestRelatedDataLoader.Convert ((UriContactEntity c) => c.UriScheme),
					UnitTestRelatedDataLoader.Convert ((UriContactEntity c) => c.NaturalPerson.Lastname),
					UnitTestRelatedDataLoader.Convert ((UriContactEntity c) => c.NaturalPerson.Lastname),
					UnitTestRelatedDataLoader.Convert ((UriContactEntity c) => c.NaturalPerson.Title),
					UnitTestRelatedDataLoader.Convert ((UriContactEntity c) => c.NaturalPerson.Title.ComptatibleGenders),
					UnitTestRelatedDataLoader.Convert ((UriContactEntity c) => c.NaturalPerson.Gender),
					UnitTestRelatedDataLoader.Convert ((UriContactEntity c) => c.NaturalPerson.FavouriteBeer),
				};

				dataContext.LoadRelatedData (entities, expression);
			}
		}


		[TestMethod]
		public void EmptySetTest()
		{
			using (var db = DB.ConnectToTestDatabase ())
			using (var dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
			{
				var entities = dataContext.GetByExample (new UriContactEntity ()).Skip (3).Take (1).ToList ();
				var expression = new List<LambdaExpression> ()
				{
					UnitTestRelatedDataLoader.Convert ((UriContactEntity c) => c.Uri),
					UnitTestRelatedDataLoader.Convert ((UriContactEntity c) => c.NaturalPerson),
					UnitTestRelatedDataLoader.Convert ((UriContactEntity c) => c.NaturalPerson.Title),
				};

				dataContext.LoadRelatedData (entities, expression);
			}
		}


		private static LambdaExpression Convert<T1, T2>(Expression<Func<T1, T2>> func)
		{
			return (LambdaExpression) func;
		}

	}


}