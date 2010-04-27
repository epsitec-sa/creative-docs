
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.Core.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

namespace Epsitec.Cresus.Core
{

	[TestClass]
	public class UnitTestCountryRepository
	{

		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TestSetup.Initialize ();
		}


		[TestMethod]
		public void Check01CreateDatabase()
		{
			UnitTestCountryRepository.dbInfrastructure = TestSetup.CreateDbInfrastructure ();
			Assert.IsTrue (UnitTestCountryRepository.dbInfrastructure.IsConnectionOpen);
		}

		[TestMethod]
		public void Check02PupulateDatabase()
		{
			using (DataContext dataContext = new DataContext (UnitTestCountryRepository.dbInfrastructure))
			{
				Assert.IsTrue (dataContext.CreateSchema<Entities.CountryEntity> ());

				CountryEntity switzerland = dataContext.CreateEmptyEntity<CountryEntity> ();
				CountryEntity france = dataContext.CreateEmptyEntity<CountryEntity> ();
				CountryEntity italy = dataContext.CreateEmptyEntity<CountryEntity> ();

				switzerland.Code = "CH";
				switzerland.Name = "Switzerland";

				france.Code = "FR";
				france.Name = "France";

				italy.Code = "IT";
				italy.Name = "Italy";

				dataContext.SaveChanges ();
			}
		}

		[TestMethod]
		public void Check03GetCountryByName()
		{
			//using (DataContext dataContext = new DataContext (UnitTestCountryRepository.dbInfrastructure))
			//{
			//    CountryRepository countryRepository = new CountryRepository (UnitTestCountryRepository.dbInfrastructure, dataContext);

			//    CountryEntity switzerland = countryRepository.GetCountryByName ("Switzerland");
			//    CountryEntity france = countryRepository.GetCountryByName ("France");
			//    CountryEntity germany = countryRepository.GetCountryByName ("Germany");

			//    Assert.IsTrue (UnitTestCountryRepository.CheckCountry (switzerland, "CH", "Switzerland"));
			//    Assert.IsTrue (UnitTestCountryRepository.CheckCountry (france, "FR", "France"));

			//    Assert.IsNull (germany);
			//}
		}

		[TestMethod]
		public void Check04GetCountryByCode()
		{
			//using (DataContext dataContext = new DataContext (UnitTestCountryRepository.dbInfrastructure))
			//{
			//    CountryRepository countryRepository = new CountryRepository (UnitTestCountryRepository.dbInfrastructure, dataContext);

			//    CountryEntity switzerland = countryRepository.GetCountryByCode ("CH");
			//    CountryEntity france = countryRepository.GetCountryByCode ("FR");
			//    CountryEntity germany = countryRepository.GetCountryByCode ("DE");

			//    Assert.IsTrue (UnitTestCountryRepository.CheckCountry (switzerland, "CH", "Switzerland"));
			//    Assert.IsTrue (UnitTestCountryRepository.CheckCountry (france, "FR", "France"));

			//    Assert.IsNull (germany);
			//}
		}

		[TestMethod]
		public void Check05GetCountryByExample()
		{
			//CountryEntity exampleAllCountries = new CountryEntity ()
			//{
			//};

			//CountryEntity exampleSwitzerland = new CountryEntity ()
			//{
			//    Code = "CH",
			//    Name = "Switzerland",
			//};

			//CountryEntity exampleFrance = new CountryEntity ()
			//{
			//    Code = "FR",
			//};

			//CountryEntity exampleItaly = new CountryEntity ()
			//{
			//    Name = "Italy",
			//};

			//CountryEntity exampleGermany1 = new CountryEntity ()
			//{
			//    Code = "DE",
			//};

			//CountryEntity exampleGermany2 = new CountryEntity ()
			//{
			//    Name = "Germany",
			//};

			//CountryEntity exampleGermany3 = new CountryEntity ()
			//{
			//    Code = "DE",
			//    Name = "Germany",
			//};

			//using (DataContext dataContext = new DataContext (UnitTestCountryRepository.dbInfrastructure))
			//{
			//    CountryRepository countryRepository = new CountryRepository (UnitTestCountryRepository.dbInfrastructure, dataContext);

			//    List<CountryEntity> allCountries = new List<CountryEntity> (countryRepository.GetEntitiesByExample (exampleAllCountries));
			//    List<CountryEntity> switzerland = new List<CountryEntity> (countryRepository.GetEntitiesByExample (exampleSwitzerland));
			//    List<CountryEntity> france = new List<CountryEntity> (countryRepository.GetEntitiesByExample (exampleFrance));
			//    List<CountryEntity> italy = new List<CountryEntity> (countryRepository.GetEntitiesByExample (exampleItaly));
			//    List<CountryEntity> germany1 = new List<CountryEntity> (countryRepository.GetEntitiesByExample (exampleGermany1));
			//    List<CountryEntity> germany2 = new List<CountryEntity> (countryRepository.GetEntitiesByExample (exampleGermany2));
			//    List<CountryEntity> germany3 = new List<CountryEntity> (countryRepository.GetEntitiesByExample (exampleGermany3));

			//    Assert.IsTrue (allCountries.Count == 3);
			//    Assert.IsTrue (allCountries.FindAll (country => UnitTestCountryRepository.CheckCountry (country, "CH", "Switzerland")).Count == 1);
			//    Assert.IsTrue (allCountries.FindAll (country => UnitTestCountryRepository.CheckCountry (country, "FR", "France")).Count == 1);
			//    Assert.IsTrue (allCountries.FindAll (country => UnitTestCountryRepository.CheckCountry (country, "IT", "Italy")).Count == 1);

			//    Assert.IsTrue (switzerland.Count == 1);
			//    Assert.IsTrue (UnitTestCountryRepository.CheckCountry (switzerland[0], "CH", "Switzerland"));

			//    Assert.IsTrue (france.Count == 1);
			//    Assert.IsTrue (UnitTestCountryRepository.CheckCountry (france[0], "FR", "France"));

			//    Assert.IsTrue (italy.Count == 1);
			//    Assert.IsTrue (UnitTestCountryRepository.CheckCountry (italy[0], "IT", "Italy"));

			//    Assert.IsTrue (germany1.Count == 0);
			//    Assert.IsTrue (germany2.Count == 0);
			//    Assert.IsTrue (germany3.Count == 0);
			//}
		}

		private static bool CheckCountry(CountryEntity country, string code, string name)
		{
			return country != null && country.Code == code && country.Name == name;
		}

		private static DbInfrastructure dbInfrastructure;

	}

}
