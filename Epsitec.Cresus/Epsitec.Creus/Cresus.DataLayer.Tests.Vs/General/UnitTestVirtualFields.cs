using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.General
{


	[TestClass]
	public class UnitTestVirtualFields
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
		public void Test()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));

				Assert.IsNotNull (alfred.FavouriteBeer);
				Assert.IsNotNull (alfred.FavouriteBeerMates);

				alfred.FavouriteBeer = "Erdinger";
				Assert.AreEqual ("Erdinger", alfred.FavouriteBeer);

				Assert.IsTrue (alfred.FavouriteBeerMates.IsEmpty ());

				alfred.FavouriteBeerMates.Add (gertrude);

				Assert.AreEqual (gertrude, alfred.FavouriteBeerMates.Single ());

				alfred.BirthDate = new Date (2000, 1, 1);

				dataContext.SaveChanges ();
			}
		}


	}


}
