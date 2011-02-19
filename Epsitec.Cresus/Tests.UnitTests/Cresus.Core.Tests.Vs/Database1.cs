using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core
{
	
	
	public class Database1 : Database
	{


		static Database1()
		{
			Database1.NbElements = new Dictionary<string, Dictionary<DatabaseSize, int>> ()
			{
				{"contactRoles", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 10},
						{DatabaseSize.Medium, 10},
						{DatabaseSize.Large, 10},
						{DatabaseSize.Huge, 10},
					}
				},
				{"uriComments", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 200},
						{DatabaseSize.Medium, 2000},
						{DatabaseSize.Large, 20000},
						{DatabaseSize.Huge, 200000},
					}
				},
				{"uriSchemes", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 5},
						{DatabaseSize.Medium, 5},
						{DatabaseSize.Large, 5},
						{DatabaseSize.Huge, 5},
					}
				},
				{"uriContacts", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 200},
						{DatabaseSize.Medium, 2000},
						{DatabaseSize.Large, 20000},
						{DatabaseSize.Huge, 200000},
					}
				},
				{"telecomComments", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 200},
						{DatabaseSize.Medium, 2000},
						{DatabaseSize.Large, 20000},
						{DatabaseSize.Huge, 200000},
					}
				},
				{"telecomTypes", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 5},
						{DatabaseSize.Medium, 5},
						{DatabaseSize.Large, 5},
						{DatabaseSize.Huge, 5},
					}
				},
				{"telecomContacts", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 200},
						{DatabaseSize.Medium, 2000},
						{DatabaseSize.Large, 20000},
						{DatabaseSize.Huge, 200000},
					}
				},
				{"mailComments", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 200},
						{DatabaseSize.Medium, 2000},
						{DatabaseSize.Large, 20000},
						{DatabaseSize.Huge, 200000},
					}
				},
				{"countries", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 1},
						{DatabaseSize.Medium, 5},
						{DatabaseSize.Large, 25},
						{DatabaseSize.Huge, 125},
					}
				},
				{"regions", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 10},
						{DatabaseSize.Medium, 50},
						{DatabaseSize.Large, 250},
						{DatabaseSize.Huge, 1250},
					}
				},
				{"locations", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 75},
						{DatabaseSize.Medium, 750},
						{DatabaseSize.Large, 7500},
						{DatabaseSize.Huge, 75000},
					}
				},
				{"streets", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 150},
						{DatabaseSize.Medium, 1500},
						{DatabaseSize.Large, 15000},
						{DatabaseSize.Huge, 150000},
					}
				},
				{"postBoxes", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 150},
						{DatabaseSize.Medium, 1500},
						{DatabaseSize.Large, 15000},
						{DatabaseSize.Huge, 150000},
					}
				},
				{"addresses", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 150},
						{DatabaseSize.Medium, 1500},
						{DatabaseSize.Large, 15000},
						{DatabaseSize.Huge, 150000},
					}
				},
				{"mailContacts", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 200},
						{DatabaseSize.Medium, 2000},
						{DatabaseSize.Large, 20000},
						{DatabaseSize.Huge, 200000},
					}
				},
				{"languages", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 15},
						{DatabaseSize.Medium, 15},
						{DatabaseSize.Large, 15},
						{DatabaseSize.Huge, 15},
					}
				},
				{"titles", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 15},
						{DatabaseSize.Medium, 15},
						{DatabaseSize.Large, 15},
						{DatabaseSize.Huge, 15},
					}
				},
				{"genders", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 2},
						{DatabaseSize.Medium, 2},
						{DatabaseSize.Large, 2},
						{DatabaseSize.Huge, 2},
					}
				},
				{"legalPersonTypes", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 5},
						{DatabaseSize.Medium, 5},
						{DatabaseSize.Large, 5},
						{DatabaseSize.Huge, 5},
					}
				},
				{"naturalPersons", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 100},
						{DatabaseSize.Medium, 1000},
						{DatabaseSize.Large, 10000},
						{DatabaseSize.Huge, 100000},
					}
				},
				{"legalPersons", new Dictionary<DatabaseSize, int>()
					{
						{DatabaseSize.Small, 100},
						{DatabaseSize.Medium, 1000},
						{DatabaseSize.Large, 10000},
						{DatabaseSize.Huge, 100000},
					}
				},
			};
		}


		public static void PopulateDatabase(DatabaseSize size)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				dataContext.CreateSchema<AbstractPersonEntity> ();
				dataContext.CreateSchema<MailContactEntity> ();
				dataContext.CreateSchema<TelecomContactEntity> ();
				dataContext.CreateSchema<UriContactEntity> ();

				System.Diagnostics.Debug.WriteLine ("Populating database. This might take a few minutes");

				int nbContactRoles = Database1.NbElements["contactRoles"][size];
				ContactRoleEntity[] contactRoles = Database.CreateContactRoles (dataContext, nbContactRoles);
				dataContext.SaveChanges ();

				int nbUriComments = Database1.NbElements["uriComments"][size];
				CommentEntity[] uriComments = Database.CreateComments (dataContext, nbUriComments);
				dataContext.SaveChanges ();

				int nbUriSchemes = Database1.NbElements["uriSchemes"][size];
				UriSchemeEntity[] uriSchemes = Database.CreateUriSchemes (dataContext, nbUriSchemes);
				dataContext.SaveChanges ();

				int nbUriContacts = Database1.NbElements["uriContacts"][size];
				UriContactEntity[] uriContacts = Database.CreateUriContacts (dataContext, uriSchemes, nbUriContacts);
				dataContext.SaveChanges ();

				Database.AssignRoles (uriContacts, contactRoles);
				dataContext.SaveChanges ();

				Database.AssignComments (uriContacts, uriComments);
				dataContext.SaveChanges ();

				int nbTelecomComments = Database1.NbElements["telecomComments"][size];
				CommentEntity[] telecomComments = Database.CreateComments (dataContext, nbTelecomComments);
				dataContext.SaveChanges ();

				int nbTelecomTypes = Database1.NbElements["telecomTypes"][size];
				TelecomTypeEntity[] telecomTypes = Database.CreateTelecomTypes (dataContext, nbTelecomTypes);
				dataContext.SaveChanges ();

				int nbTelecomContacts = Database1.NbElements["telecomContacts"][size];
				TelecomContactEntity[] telecomContacts = Database.CreateTelecomContacts (dataContext, telecomTypes, nbTelecomContacts);
				dataContext.SaveChanges ();

				Database.AssignRoles (telecomContacts, contactRoles);
				dataContext.SaveChanges ();

				Database.AssignComments (telecomContacts, telecomComments);
				dataContext.SaveChanges ();

				int nbMailComments = Database1.NbElements["mailComments"][size];
				CommentEntity[] mailComments = Database.CreateComments (dataContext, nbMailComments);
				dataContext.SaveChanges ();

				int nbCountries = Database1.NbElements["countries"][size];
				CountryEntity[] countries = Database.CreateCountries (dataContext, nbCountries);
				dataContext.SaveChanges ();

				int nbRegions = Database1.NbElements["regions"][size];
				RegionEntity[] regions = Database.CreateRegions (dataContext, countries, nbRegions);
				dataContext.SaveChanges ();

				int nbLocations = Database1.NbElements["locations"][size];
				LocationEntity[] locations = Database.CreateLocations (dataContext, regions, nbLocations);
				dataContext.SaveChanges ();

				int nbStreets = Database1.NbElements["streets"][size];
				StreetEntity[] streets = Database.CreateStreets (dataContext, nbStreets);
				dataContext.SaveChanges ();

				int nbPostBoxes = Database1.NbElements["postBoxes"][size];
				PostBoxEntity[] postBoxes = Database.CreatePostBoxes (dataContext, nbPostBoxes);
				dataContext.SaveChanges ();

				int nbAddresses = Database1.NbElements["addresses"][size];
				AddressEntity[] addresses = Database.CreateAddresses (dataContext, streets, postBoxes, locations, nbAddresses);
				dataContext.SaveChanges ();

				int nbMailContacts = Database1.NbElements["mailContacts"][size];
				MailContactEntity[] mailContacts = Database.CreateMailContact (dataContext, addresses, nbMailContacts);
				dataContext.SaveChanges ();

				Database.AssignRoles (mailContacts, contactRoles);
				dataContext.SaveChanges ();

				Database.AssignComments (mailContacts, mailComments);
				dataContext.SaveChanges ();

				int nbLanguages = Database1.NbElements["languages"][size];
				LanguageEntity[] languages = Database.CreateLanguages (dataContext, nbLanguages);
				dataContext.SaveChanges ();

				int nbTitles = Database1.NbElements["titles"][size];
				PersonTitleEntity[] titles = Database.CreatePersonTitles (dataContext, nbTitles);
				dataContext.SaveChanges ();

				int nbGenders = Database1.NbElements["genders"][size];
				PersonGenderEntity[] genders = Database.CreatePersonGenders (dataContext, nbGenders);
				dataContext.SaveChanges ();

				int nbLegalPersonTypes = Database1.NbElements["legalPersonTypes"][size];
				LegalPersonTypeEntity[] legalPersonTypes = Database.CreateLegalPersonTypes (dataContext, nbLegalPersonTypes);
				dataContext.SaveChanges ();

				int nbNaturalPersons = Database1.NbElements["naturalPersons"][size];
				NaturalPersonEntity[] naturalPersons = Database.CreateNaturalPersons (dataContext, languages, titles, genders, nbNaturalPersons);
				dataContext.SaveChanges ();

				int nbLegalPersons = Database1.NbElements["legalPersons"][size];
				LegalPersonEntity[] legalPersons = Database.CreateLegalPersons (dataContext, languages, legalPersonTypes, nbLegalPersons);
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


		private static Dictionary<string, Dictionary<DatabaseSize, int>> NbElements;


	}


	public enum DatabaseSize
	{
		Small,
		Medium,
		Large,
		Huge,
	}


}
