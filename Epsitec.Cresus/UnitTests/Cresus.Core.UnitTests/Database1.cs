using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.Core.Entities;


namespace Epsitec.Cresus.Core
{
	
	
	public class Database1 : Database
	{

		public static void PopulateDatabase(bool big)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				dataContext.CreateSchema<AbstractPersonEntity> ();
				dataContext.CreateSchema<MailContactEntity> ();
				dataContext.CreateSchema<TelecomContactEntity> ();
				dataContext.CreateSchema<UriContactEntity> ();

				System.Diagnostics.Debug.WriteLine ("Populating database. This might take a few minutes");

				ContactRoleEntity[] contactRoles = Database.CreateContactRoles (dataContext, 10);
				dataContext.SaveChanges ();

				CommentEntity[] uriComments = Database.CreateComments (dataContext, big ? 2500 : 100);
				dataContext.SaveChanges ();

				UriSchemeEntity[] uriSchemes = Database.CreateUriSchemes (dataContext, 5);
				dataContext.SaveChanges ();

				UriContactEntity[] uriContacts = Database.CreateUriContacts (dataContext, uriSchemes, big ? 2500 : 100);
				dataContext.SaveChanges ();

				Database.AssignRoles (uriContacts, contactRoles);
				dataContext.SaveChanges ();

				Database.AssignComments (uriContacts, uriComments);
				dataContext.SaveChanges ();

				CommentEntity[] telecomComments = Database.CreateComments (dataContext, big ? 2500 : 100);
				dataContext.SaveChanges ();

				TelecomTypeEntity[] telecomTypes = Database.CreateTelecomTypes (dataContext, 5);
				dataContext.SaveChanges ();

				TelecomContactEntity[] telecomContacts = Database.CreateTelecomContacts (dataContext, telecomTypes, big ? 2500 : 100);
				dataContext.SaveChanges ();

				Database.AssignRoles (telecomContacts, contactRoles);
				dataContext.SaveChanges ();

				Database.AssignComments (telecomContacts, telecomComments);
				dataContext.SaveChanges ();

				CommentEntity[] mailComments = Database.CreateComments (dataContext, big ? 2500 : 100);
				dataContext.SaveChanges ();

				CountryEntity[] countries = Database.CreateCountries (dataContext, 5);
				dataContext.SaveChanges ();

				RegionEntity[] regions = Database.CreateRegions (dataContext, countries, big ? 50 : 10);
				dataContext.SaveChanges ();

				LocationEntity[] locations = Database.CreateLocations (dataContext, regions, big ? 100 : 15);
				dataContext.SaveChanges ();

				StreetEntity[] streets = Database.CreateStreets (dataContext, big ? 1500 : 25);
				dataContext.SaveChanges ();

				PostBoxEntity[] postBoxes = Database.CreatePostBoxes (dataContext, big ? 1500 : 25);
				dataContext.SaveChanges ();

				AddressEntity[] addresses = Database.CreateAddresses (dataContext, streets, postBoxes, locations, big ? 1500 : 50);
				dataContext.SaveChanges ();

				MailContactEntity[] mailContacts = Database.CreateMailContact (dataContext, addresses, big ? 2500 : 100);
				dataContext.SaveChanges ();

				Database.AssignRoles (mailContacts, contactRoles);
				dataContext.SaveChanges ();

				Database.AssignComments (mailContacts, mailComments);
				dataContext.SaveChanges ();

				LanguageEntity[] languages = Database.CreateLanguages (dataContext, big ? 15 : 5);
				dataContext.SaveChanges ();

				PersonTitleEntity[] titles = Database.CreatePersonTitles (dataContext, big ? 15 : 5);
				dataContext.SaveChanges ();

				PersonGenderEntity[] genders = Database.CreatePersonGenders (dataContext, 3);
				dataContext.SaveChanges ();

				LegalPersonTypeEntity[] legalPersonTypes = Database.CreateLegalPersonTypes (dataContext, 5);
				dataContext.SaveChanges ();

				NaturalPersonEntity[] naturalPersons = Database.CreateNaturalPersons (dataContext, languages, titles, genders, big ? 1000 : 25);
				dataContext.SaveChanges ();

				LegalPersonEntity[] legalPersons = Database.CreateLegalPersons (dataContext, languages, legalPersonTypes, big ? 500 : 25);
				dataContext.SaveChanges ();

				Database.AssignContacts (uriContacts, naturalPersons, legalPersons);
				dataContext.SaveChanges ();

				Database.AssignContacts (telecomContacts, naturalPersons, legalPersons);
				dataContext.SaveChanges ();

				Database.AssignContacts (mailContacts, naturalPersons, legalPersons);
				dataContext.SaveChanges ();

				Database.AssignParents (legalPersons);
				dataContext.SaveChanges ();
			}
		}


	}


}
