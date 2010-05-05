using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{


	[TestClass]
	public class UnitTestPerformance
	{


		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TestSetup.Initialize ();
		}


		[TestMethod]
		public void Check01CreateDatabase()
		{
			UnitTestPerformance.dbInfrastructure = new DbInfrastructure ();
			UnitTestPerformance.dbInfrastructure.AttachToDatabase (DbInfrastructure.CreateDatabaseAccess ("CORETEST"));
			//UnitTestPerformance.dbInfrastructure = TestSetup.CreateDbInfrastructure ();
			Assert.IsTrue (UnitTestPerformance.dbInfrastructure.IsConnectionOpen);
		}
       

		[TestMethod]
		public void Check02PopulateDatabase()
		{
		    using (DataContext dataContext = new DataContext (UnitTestPerformance.dbInfrastructure))
		    {
				//dataContext.CreateSchema<AbstractPersonEntity> ();
				//dataContext.CreateSchema<MailContactEntity> ();
				//dataContext.CreateSchema<TelecomContactEntity> ();
				//dataContext.CreateSchema<UriContactEntity> ();

				//ContactRoleEntity[] contactRoles = EntityBuilder.CreateContactRoles (dataContext, 10);
				//dataContext.SaveChanges ();

		//        CommentEntity[] uriComments = EntityBuilder.CreateComments (dataContext, 2500);
		//        dataContext.SaveChanges ();

		//        UriSchemeEntity[] uriSchemes = EntityBuilder.CreateUriSchemes (dataContext, 5);
		//        dataContext.SaveChanges ();

		//        UriContactEntity[] uriContacts = EntityBuilder.CreateUriContacts (dataContext, uriSchemes, 2500);
		//        dataContext.SaveChanges ();

		//        EntityBuilder.AssignRoles (uriContacts, contactRoles);
		//        dataContext.SaveChanges ();

		//        EntityBuilder.AssignComments (uriContacts, uriComments);
		//        dataContext.SaveChanges ();

				//CommentEntity[] telecomComments = EntityBuilder.CreateComments (dataContext, 2500);
				//dataContext.SaveChanges ();

				//TelecomTypeEntity[] telecomTypes = EntityBuilder.CreateTelecomTypes (dataContext, 5);
				//dataContext.SaveChanges ();

				//TelecomContactEntity[] telecomContacts = EntityBuilder.CreateTelecomContacts (dataContext, telecomTypes, 2500);
				//dataContext.SaveChanges ();

				//EntityBuilder.AssignRoles (telecomContacts, contactRoles);
				//dataContext.SaveChanges ();

				//EntityBuilder.AssignComments (telecomContacts, telecomComments);
				//dataContext.SaveChanges ();

		//        CommentEntity[] mailComments = EntityBuilder.CreateComments (dataContext, 2500);
		//        dataContext.SaveChanges ();

		//        CountryEntity[] countries = EntityBuilder.CreateCountries (dataContext, 5);
		//        dataContext.SaveChanges ();

		//        RegionEntity[] regions = EntityBuilder.CreateRegions (dataContext, countries, 50);
		//        dataContext.SaveChanges ();

		//        LocationEntity[] locations = EntityBuilder.CreateLocations (dataContext, regions, 100);
		//        dataContext.SaveChanges ();

		//        StreetEntity[] streets = EntityBuilder.CreateStreets (dataContext, 1500);
		//        dataContext.SaveChanges ();

		//        PostBoxEntity[] postBoxes = EntityBuilder.CreatePostBoxes (dataContext, 1500);
		//        dataContext.SaveChanges ();

		//        AddressEntity[] addresses = EntityBuilder.CreateAddresses (dataContext, streets, postBoxes, locations, 1500);
		//        dataContext.SaveChanges ();

		//        MailContactEntity[] mailContacts = EntityBuilder.CreateMailContact (dataContext, addresses, 2500);
		//        dataContext.SaveChanges ();

		//        EntityBuilder.AssignRoles (mailContacts, contactRoles);
		//        dataContext.SaveChanges ();

		//        EntityBuilder.AssignComments (mailContacts, mailComments);
		//        dataContext.SaveChanges ();

		//        LanguageEntity[] languages = EntityBuilder.CreateLanguages (dataContext, 15);
		//        dataContext.SaveChanges ();

		//        PersonTitleEntity[] titles = EntityBuilder.CreatePersonTitles (dataContext, 15);
		//        dataContext.SaveChanges ();

		//        PersonGenderEntity[] genders = EntityBuilder.CreatePersonGenders (dataContext, 3);
		//        dataContext.SaveChanges ();

		//        LegalPersonTypeEntity[] legalPersonTypes = EntityBuilder.CreateLegalPersonTypes (dataContext, 5);
		//        dataContext.SaveChanges ();

		//        NaturalPersonEntity[] naturalPersons = EntityBuilder.CreateNaturalPersons (dataContext, languages, titles, genders, 1000);
		//        dataContext.SaveChanges ();

		//        LegalPersonEntity[] legalPersons = EntityBuilder.CreateLegalPersons (dataContext, languages, legalPersonTypes, 500);
		//        dataContext.SaveChanges ();

		//        EntityBuilder.AssignContacts (uriContacts, naturalPersons, legalPersons);
		//        dataContext.SaveChanges ();

		//        EntityBuilder.AssignContacts (telecomContacts, naturalPersons, legalPersons);
		//        dataContext.SaveChanges ();

		//        EntityBuilder.AssignContacts (mailContacts, naturalPersons, legalPersons);
		//        dataContext.SaveChanges ();

		//        EntityBuilder.AssignParents (legalPersons);
		//        dataContext.SaveChanges ();
		    }
		}

		[TestMethod]
		public void Check03RetreiveData()
		{
			using (DataContext dataContext = new DataContext (UnitTestPerformance.dbInfrastructure))
			{
				Repository repository = new Repository (UnitTestPerformance.dbInfrastructure, dataContext);

				repository.GetEntitiesByExample<CountryEntity> (new CountryEntity ()).Count ();

				System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();

				watch.Start ();
				repository.GetEntitiesByExample<TelecomContactEntity> (new TelecomContactEntity ()).Count ();
				watch.Stop ();

				System.Diagnostics.Debug.WriteLine ("Time elapsed: " + watch.ElapsedMilliseconds);

				watch.Restart ();
				repository.GetEntitiesByExample<TelecomContactEntity> (new TelecomContactEntity ()).Count ();
				watch.Stop ();

				System.Diagnostics.Debug.WriteLine ("Time elapsed: " + watch.ElapsedMilliseconds);
			}
		}

		private static DbInfrastructure dbInfrastructure;


	}


}
