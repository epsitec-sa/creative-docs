using Epsitec.Common.Support;

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
					LambdaUtils.Convert ((UriContactEntity c) => c.Uri),
					LambdaUtils.Convert ((UriContactEntity c) => c.UriScheme),
					LambdaUtils.Convert ((UriContactEntity c) => c.NaturalPerson.Lastname),
					LambdaUtils.Convert ((UriContactEntity c) => c.NaturalPerson.Lastname),
					LambdaUtils.Convert ((UriContactEntity c) => c.NaturalPerson.Title),
					LambdaUtils.Convert ((UriContactEntity c) => c.NaturalPerson.Title.ComptatibleGenders),
					LambdaUtils.Convert ((UriContactEntity c) => c.NaturalPerson.Gender),
					LambdaUtils.Convert ((UriContactEntity c) => c.NaturalPerson.FavouriteBeer),
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
					LambdaUtils.Convert ((UriContactEntity c) => c.Uri),
					LambdaUtils.Convert ((UriContactEntity c) => c.NaturalPerson),
					LambdaUtils.Convert ((UriContactEntity c) => c.NaturalPerson.Title),
				};

				dataContext.LoadRelatedData (entities, expression);
			}
		}

	}


}