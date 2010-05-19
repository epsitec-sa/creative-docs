//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public static class Database
	{
		public static DbInfrastructure DbInfrastructure
		{
			get
			{
				return Database.dbInfrastructure;
			}
		}

		public static void CreateAndConnectToDatabase()
		{
			Database.dbInfrastructure = TestSetup.CreateDbInfrastructure ();
		}

		public static void ConnectToDatabase()
		{
			Database.dbInfrastructure = new DbInfrastructure ();
			Database.dbInfrastructure.AttachToDatabase (DbInfrastructure.CreateDatabaseAccess ("CORETEST"));
		}

		public static void PopulateDatabase(bool big)
		{
			using (DataContext dataContext = new DataContext (Database.dbInfrastructure))
		    {
				dataContext.CreateSchema<AbstractPersonEntity> ();
				dataContext.CreateSchema<MailContactEntity> ();
				dataContext.CreateSchema<TelecomContactEntity> ();
				dataContext.CreateSchema<UriContactEntity> ();

				System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
				System.Diagnostics.Debug.WriteLine ("Populating database. This will take a few minutes");
				
				watch.Start ();

				ContactRoleEntity[] contactRoles = EntityBuilder.CreateContactRoles (dataContext, 10);
				dataContext.SaveChanges ();
				
				CommentEntity[] uriComments = EntityBuilder.CreateComments (dataContext, big ? 2500 : 100);
				dataContext.SaveChanges ();

				
				UriSchemeEntity[] uriSchemes = EntityBuilder.CreateUriSchemes (dataContext, 5);
				dataContext.SaveChanges ();

				UriContactEntity[] uriContacts = EntityBuilder.CreateUriContacts (dataContext, uriSchemes, big ? 2500 : 100);
				dataContext.SaveChanges ();

				EntityBuilder.AssignRoles (uriContacts, contactRoles);
				dataContext.SaveChanges ();

				EntityBuilder.AssignComments (uriContacts, uriComments);
				dataContext.SaveChanges ();

				CommentEntity[] telecomComments = EntityBuilder.CreateComments (dataContext, big ? 2500 : 100);
				dataContext.SaveChanges ();

				TelecomTypeEntity[] telecomTypes = EntityBuilder.CreateTelecomTypes (dataContext, 5);
				dataContext.SaveChanges ();

				TelecomContactEntity[] telecomContacts = EntityBuilder.CreateTelecomContacts (dataContext, telecomTypes, big ? 2500 : 100);
				dataContext.SaveChanges ();

				EntityBuilder.AssignRoles (telecomContacts, contactRoles);
				dataContext.SaveChanges ();

				EntityBuilder.AssignComments (telecomContacts, telecomComments);
				dataContext.SaveChanges ();

				CommentEntity[] mailComments = EntityBuilder.CreateComments (dataContext, big ? 2500 : 100);
				dataContext.SaveChanges ();

				CountryEntity[] countries = EntityBuilder.CreateCountries (dataContext, 5);
				dataContext.SaveChanges ();

				RegionEntity[] regions = EntityBuilder.CreateRegions (dataContext, countries, big ? 50 : 10);
				dataContext.SaveChanges ();

				LocationEntity[] locations = EntityBuilder.CreateLocations (dataContext, regions, big ? 100 : 15);
				dataContext.SaveChanges ();

				StreetEntity[] streets = EntityBuilder.CreateStreets (dataContext, big ? 1500 : 25);
				dataContext.SaveChanges ();

				PostBoxEntity[] postBoxes = EntityBuilder.CreatePostBoxes (dataContext, big ? 1500 : 25);
				dataContext.SaveChanges ();

				AddressEntity[] addresses = EntityBuilder.CreateAddresses (dataContext, streets, postBoxes, locations, big ? 1500 : 50);
				dataContext.SaveChanges ();

				MailContactEntity[] mailContacts = EntityBuilder.CreateMailContact (dataContext, addresses, big ? 2500 : 100);
				dataContext.SaveChanges ();

				EntityBuilder.AssignRoles (mailContacts, contactRoles);
				dataContext.SaveChanges ();

				EntityBuilder.AssignComments (mailContacts, mailComments);
				dataContext.SaveChanges ();

				LanguageEntity[] languages = EntityBuilder.CreateLanguages (dataContext, big ? 15 : 5);
				dataContext.SaveChanges ();

				PersonTitleEntity[] titles = EntityBuilder.CreatePersonTitles (dataContext, big ? 15 : 5);
				dataContext.SaveChanges ();

				PersonGenderEntity[] genders = EntityBuilder.CreatePersonGenders (dataContext, 3);
				dataContext.SaveChanges ();

				LegalPersonTypeEntity[] legalPersonTypes = EntityBuilder.CreateLegalPersonTypes (dataContext, 5);
				dataContext.SaveChanges ();

				NaturalPersonEntity[] naturalPersons = EntityBuilder.CreateNaturalPersons (dataContext, languages, titles, genders, big ? 1000 : 25);
				dataContext.SaveChanges ();

				LegalPersonEntity[] legalPersons = EntityBuilder.CreateLegalPersons (dataContext, languages, legalPersonTypes, big ? 500 : 25);
				dataContext.SaveChanges ();

				EntityBuilder.AssignContacts (uriContacts, naturalPersons, legalPersons);
				dataContext.SaveChanges ();

				EntityBuilder.AssignContacts (telecomContacts, naturalPersons, legalPersons);
				dataContext.SaveChanges ();

				EntityBuilder.AssignContacts (mailContacts, naturalPersons, legalPersons);
				dataContext.SaveChanges ();

				EntityBuilder.AssignParents (legalPersons);
				dataContext.SaveChanges ();

				watch.Stop ();
				System.Diagnostics.Debug.WriteLine ("Time elapsed: " + watch.ElapsedMilliseconds);
		    }
		}


		private static DbInfrastructure dbInfrastructure;
	}
}
