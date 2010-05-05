using System.Linq;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.Core;



namespace Cresus.Core
{
	class Program
	{
		static void Main(string[] args)
		{
			TestSetup.Initialize ();

			new UnitTestPerformance ().RetreiveData ();
		}
	}


	public class UnitTestPerformance
	{


		public void InsertData()
		{
			UnitTestPerformance.dbInfrastructure = TestSetup.CreateDbInfrastructure ();

			using (DataContext dataContext = new DataContext (UnitTestPerformance.dbInfrastructure))
			{
				dataContext.CreateSchema<AbstractPersonEntity> ();
				dataContext.CreateSchema<MailContactEntity> ();
				dataContext.CreateSchema<TelecomContactEntity> ();
				dataContext.CreateSchema<UriContactEntity> ();
				
				ContactRoleEntity[] contactRoles = EntityBuilder.CreateContactRoles (dataContext, 10);
				dataContext.SaveChanges ();

				//CommentEntity[] uriComments = EntityBuilder.CreateComments (dataContext, 2500);
				//dataContext.SaveChanges ();

				//UriSchemeEntity[] uriSchemes = EntityBuilder.CreateUriSchemes (dataContext, 5);
				//dataContext.SaveChanges ();

				//UriContactEntity[] uriContacts = EntityBuilder.CreateUriContacts (dataContext, uriSchemes, 2500);
				//dataContext.SaveChanges ();

				//EntityBuilder.AssignRoles (uriContacts, contactRoles);
				//dataContext.SaveChanges ();

				//EntityBuilder.AssignComments (uriContacts, uriComments);
				//dataContext.SaveChanges ();

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

				//CommentEntity[] mailComments = EntityBuilder.CreateComments (dataContext, 2500);
				//dataContext.SaveChanges ();

				//CountryEntity[] countries = EntityBuilder.CreateCountries (dataContext, 5);
				//dataContext.SaveChanges ();

				//RegionEntity[] regions = EntityBuilder.CreateRegions (dataContext, countries, 50);
				//dataContext.SaveChanges ();

				//LocationEntity[] locations = EntityBuilder.CreateLocations (dataContext, regions, 100);
				//dataContext.SaveChanges ();

				//StreetEntity[] streets = EntityBuilder.CreateStreets (dataContext, 1500);
				//dataContext.SaveChanges ();

				//PostBoxEntity[] postBoxes = EntityBuilder.CreatePostBoxes (dataContext, 1500);
				//dataContext.SaveChanges ();

				//AddressEntity[] addresses = EntityBuilder.CreateAddresses (dataContext, streets, postBoxes, locations, 1500);
				//dataContext.SaveChanges ();

				//MailContactEntity[] mailContacts = EntityBuilder.CreateMailContact (dataContext, addresses, 2500);
				//dataContext.SaveChanges ();

				//EntityBuilder.AssignRoles (mailContacts, contactRoles);
				//dataContext.SaveChanges ();

				//EntityBuilder.AssignComments (mailContacts, mailComments);
				//dataContext.SaveChanges ();

				//LanguageEntity[] languages = EntityBuilder.CreateLanguages (dataContext, 15);
				//dataContext.SaveChanges ();

				//PersonTitleEntity[] titles = EntityBuilder.CreatePersonTitles (dataContext, 15);
				//dataContext.SaveChanges ();

				//PersonGenderEntity[] genders = EntityBuilder.CreatePersonGenders (dataContext, 3);
				//dataContext.SaveChanges ();

				//LegalPersonTypeEntity[] legalPersonTypes = EntityBuilder.CreateLegalPersonTypes (dataContext, 5);
				//dataContext.SaveChanges ();

				//NaturalPersonEntity[] naturalPersons = EntityBuilder.CreateNaturalPersons (dataContext, languages, titles, genders, 1000);
				//dataContext.SaveChanges ();

				//LegalPersonEntity[] legalPersons = EntityBuilder.CreateLegalPersons (dataContext, languages, legalPersonTypes, 500);
				//dataContext.SaveChanges ();

				//EntityBuilder.AssignContacts (uriContacts, naturalPersons, legalPersons);
				//dataContext.SaveChanges ();

				//EntityBuilder.AssignContacts (telecomContacts, naturalPersons, legalPersons);
				//dataContext.SaveChanges ();

				//EntityBuilder.AssignContacts (mailContacts, naturalPersons, legalPersons);
				//dataContext.SaveChanges ();

				//EntityBuilder.AssignParents (legalPersons);
				//dataContext.SaveChanges ();
			}

		}

		public void RetreiveData()
		{
			UnitTestPerformance.dbInfrastructure = new DbInfrastructure ();
			UnitTestPerformance.dbInfrastructure.AttachToDatabase (DbInfrastructure.CreateDatabaseAccess ("CORETEST"));

			using (DataContext dataContext = new DataContext (UnitTestPerformance.dbInfrastructure))
			{
				Repository repository = new Repository (UnitTestPerformance.dbInfrastructure, dataContext);

				System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
				watch.Start ();
				repository.GetEntitiesByExample<CountryEntity> (new CountryEntity ()).Count ();
				watch.Stop ();
				System.Diagnostics.Debug.WriteLine (watch.ElapsedMilliseconds);

				watch.Start ();
				repository.GetEntitiesByExample<TelecomContactEntity> (new TelecomContactEntity ()).Count ();
				watch.Stop ();
				System.Diagnostics.Debug.WriteLine (watch.ElapsedMilliseconds);
				watch.Start ();
				repository.GetEntitiesByExample<TelecomContactEntity> (new TelecomContactEntity ()).Count ();
				watch.Stop ();
				System.Diagnostics.Debug.WriteLine (watch.ElapsedMilliseconds);
			}
		}

		private static DbInfrastructure dbInfrastructure;


	}


}
