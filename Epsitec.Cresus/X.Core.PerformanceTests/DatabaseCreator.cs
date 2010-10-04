using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.PerformanceTests.Entities;

using System.Collections.Generic;


namespace Epsitec.Cresus.PerformanceTests
{
	
	
	internal static class DatabaseCreator
	{


		static DatabaseCreator()
		{
			DatabaseCreator.NbElements = new Dictionary<string, Dictionary<DatabaseSize, int>> ()
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
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					dataContext.CreateSchema<AbstractPersonEntity> ();
					dataContext.CreateSchema<MailContactEntity> ();
					dataContext.CreateSchema<TelecomContactEntity> ();
					dataContext.CreateSchema<UriContactEntity> ();

					System.Diagnostics.Debug.WriteLine ("Populating database. This might take a few minutes");

					int nbContactRoles = DatabaseCreator.NbElements["contactRoles"][size];
					ContactRoleEntity[] contactRoles = DatabaseHelper.CreateContactRoles (dataContext, nbContactRoles);
					dataContext.SaveChanges ();

					int nbUriComments = DatabaseCreator.NbElements["uriComments"][size];
					CommentEntity[] uriComments = DatabaseHelper.CreateComments (dataContext, nbUriComments);
					dataContext.SaveChanges ();

					int nbUriSchemes = DatabaseCreator.NbElements["uriSchemes"][size];
					UriSchemeEntity[] uriSchemes = DatabaseHelper.CreateUriSchemes (dataContext, nbUriSchemes);
					dataContext.SaveChanges ();

					int nbUriContacts = DatabaseCreator.NbElements["uriContacts"][size];
					UriContactEntity[] uriContacts = DatabaseHelper.CreateUriContacts (dataContext, uriSchemes, nbUriContacts);
					dataContext.SaveChanges ();

					DatabaseHelper.AssignRoles (uriContacts, contactRoles);
					dataContext.SaveChanges ();

					DatabaseHelper.AssignComments (uriContacts, uriComments);
					dataContext.SaveChanges ();

					int nbTelecomComments = DatabaseCreator.NbElements["telecomComments"][size];
					CommentEntity[] telecomComments = DatabaseHelper.CreateComments (dataContext, nbTelecomComments);
					dataContext.SaveChanges ();

					int nbTelecomTypes = DatabaseCreator.NbElements["telecomTypes"][size];
					TelecomTypeEntity[] telecomTypes = DatabaseHelper.CreateTelecomTypes (dataContext, nbTelecomTypes);
					dataContext.SaveChanges ();

					int nbTelecomContacts = DatabaseCreator.NbElements["telecomContacts"][size];
					TelecomContactEntity[] telecomContacts = DatabaseHelper.CreateTelecomContacts (dataContext, telecomTypes, nbTelecomContacts);
					dataContext.SaveChanges ();

					DatabaseHelper.AssignRoles (telecomContacts, contactRoles);
					dataContext.SaveChanges ();

					DatabaseHelper.AssignComments (telecomContacts, telecomComments);
					dataContext.SaveChanges ();

					int nbMailComments = DatabaseCreator.NbElements["mailComments"][size];
					CommentEntity[] mailComments = DatabaseHelper.CreateComments (dataContext, nbMailComments);
					dataContext.SaveChanges ();

					int nbCountries = DatabaseCreator.NbElements["countries"][size];
					CountryEntity[] countries = DatabaseHelper.CreateCountries (dataContext, nbCountries);
					dataContext.SaveChanges ();

					int nbRegions = DatabaseCreator.NbElements["regions"][size];
					RegionEntity[] regions = DatabaseHelper.CreateRegions (dataContext, countries, nbRegions);
					dataContext.SaveChanges ();

					int nbLocations = DatabaseCreator.NbElements["locations"][size];
					LocationEntity[] locations = DatabaseHelper.CreateLocations (dataContext, regions, nbLocations);
					dataContext.SaveChanges ();

					int nbStreets = DatabaseCreator.NbElements["streets"][size];
					StreetEntity[] streets = DatabaseHelper.CreateStreets (dataContext, nbStreets);
					dataContext.SaveChanges ();

					int nbPostBoxes = DatabaseCreator.NbElements["postBoxes"][size];
					PostBoxEntity[] postBoxes = DatabaseHelper.CreatePostBoxes (dataContext, nbPostBoxes);
					dataContext.SaveChanges ();

					int nbAddresses = DatabaseCreator.NbElements["addresses"][size];
					AddressEntity[] addresses = DatabaseHelper.CreateAddresses (dataContext, streets, postBoxes, locations, nbAddresses);
					dataContext.SaveChanges ();

					int nbMailContacts = DatabaseCreator.NbElements["mailContacts"][size];
					MailContactEntity[] mailContacts = DatabaseHelper.CreateMailContact (dataContext, addresses, nbMailContacts);
					dataContext.SaveChanges ();

					DatabaseHelper.AssignRoles (mailContacts, contactRoles);
					dataContext.SaveChanges ();

					DatabaseHelper.AssignComments (mailContacts, mailComments);
					dataContext.SaveChanges ();

					int nbLanguages = DatabaseCreator.NbElements["languages"][size];
					LanguageEntity[] languages = DatabaseHelper.CreateLanguages (dataContext, nbLanguages);
					dataContext.SaveChanges ();

					int nbTitles = DatabaseCreator.NbElements["titles"][size];
					PersonTitleEntity[] titles = DatabaseHelper.CreatePersonTitles (dataContext, nbTitles);
					dataContext.SaveChanges ();

					int nbGenders = DatabaseCreator.NbElements["genders"][size];
					PersonGenderEntity[] genders = DatabaseHelper.CreatePersonGenders (dataContext, nbGenders);
					dataContext.SaveChanges ();

					int nbLegalPersonTypes = DatabaseCreator.NbElements["legalPersonTypes"][size];
					LegalPersonTypeEntity[] legalPersonTypes = DatabaseHelper.CreateLegalPersonTypes (dataContext, nbLegalPersonTypes);
					dataContext.SaveChanges ();

					int nbNaturalPersons = DatabaseCreator.NbElements["naturalPersons"][size];
					NaturalPersonEntity[] naturalPersons = DatabaseHelper.CreateNaturalPersons (dataContext, languages, titles, genders, nbNaturalPersons);
					dataContext.SaveChanges ();

					int nbLegalPersons = DatabaseCreator.NbElements["legalPersons"][size];
					LegalPersonEntity[] legalPersons = DatabaseHelper.CreateLegalPersons (dataContext, languages, legalPersonTypes, nbLegalPersons);
					dataContext.SaveChanges ();

					DatabaseHelper.AssignContacts (uriContacts, naturalPersons, legalPersons);
					dataContext.SaveChanges ();

					DatabaseHelper.AssignContacts (telecomContacts, naturalPersons, legalPersons);
					dataContext.SaveChanges ();

					DatabaseHelper.AssignContacts (mailContacts, naturalPersons, legalPersons);
					dataContext.SaveChanges ();

					DatabaseHelper.AssignParents (legalPersons);
					dataContext.SaveChanges ();
				}
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
