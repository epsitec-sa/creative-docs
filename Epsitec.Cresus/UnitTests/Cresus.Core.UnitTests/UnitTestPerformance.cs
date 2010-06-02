//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
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
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void CreateAndPopulateDatabase()
		{
			if (UnitTestPerformance.createDatabase)
			{
				TestHelper.PrintStartTest ("Database creation");
							
				Database.CreateAndConnectToDatabase ();

				TestHelper.MeasureAndDisplayTime(
					"database population",
					() => Database1.PopulateDatabase (UnitTestPerformance.databaseSize)
				);
			}
			else
			{
				TestHelper.PrintStartTest ("Database connection");

				Database.ConnectToDatabase ();
			}
		}


		[TestMethod]
		public void RetrieveAllDataWithWarmup()
		{
			TestHelper.PrintStartTest ("Retrieve all data with warmup");

			this.RetrieveAllData<AbstractPersonEntity> (false);
			this.RetrieveAllData<AbstractPersonEntity> (true);

			this.RetrieveAllData<NaturalPersonEntity> (false);
			this.RetrieveAllData<NaturalPersonEntity> (true);

			this.RetrieveAllData<LegalPersonEntity> (false);
			this.RetrieveAllData<LegalPersonEntity> (true);

			this.RetrieveAllData<LegalPersonTypeEntity> (false);
			this.RetrieveAllData<LegalPersonTypeEntity> (true);

			this.RetrieveAllData<PersonTitleEntity> (false);
			this.RetrieveAllData<PersonTitleEntity> (true);

			this.RetrieveAllData<PersonGenderEntity> (false);
			this.RetrieveAllData<PersonGenderEntity> (true);

			this.RetrieveAllData<AbstractContactEntity> (false);
			this.RetrieveAllData<AbstractContactEntity> (true);

			this.RetrieveAllData<ContactRoleEntity> (false);
			this.RetrieveAllData<ContactRoleEntity> (true);

			this.RetrieveAllData<CommentEntity> (false);
			this.RetrieveAllData<CommentEntity> (true);

			this.RetrieveAllData<MailContactEntity> (false);
			this.RetrieveAllData<MailContactEntity> (true);

			this.RetrieveAllData<AddressEntity> (false);
			this.RetrieveAllData<AddressEntity> (true);

			this.RetrieveAllData<StreetEntity> (false);
			this.RetrieveAllData<StreetEntity> (true);

			this.RetrieveAllData<PostBoxEntity> (false);
			this.RetrieveAllData<PostBoxEntity> (true);

			this.RetrieveAllData<LocationEntity> (false);
			this.RetrieveAllData<LocationEntity> (true);

			this.RetrieveAllData<RegionEntity> (false);
			this.RetrieveAllData<RegionEntity> (true);

			this.RetrieveAllData<CountryEntity> (false);
			this.RetrieveAllData<CountryEntity> (true);

			this.RetrieveAllData<TelecomContactEntity> (false);
			this.RetrieveAllData<TelecomContactEntity> (true);

			this.RetrieveAllData<TelecomTypeEntity> (false);
			this.RetrieveAllData<TelecomTypeEntity> (true);

			this.RetrieveAllData<UriContactEntity> (false);
			this.RetrieveAllData<UriContactEntity> (true);

			this.RetrieveAllData<UriSchemeEntity> (false);
			this.RetrieveAllData<UriSchemeEntity> (true);
		}


		public void RetrieveAllData<EntityType>(bool bulkMode) where EntityType : AbstractEntity, new()
		{
			TestHelper.MeasureAndDisplayTime (
				TestHelper.extendString (new EntityType ().GetType ().Name, 30) + "\twarmup: false\tbulkMode: " + bulkMode,
				() =>
				{
					using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
					{
						Repository repository = new Repository (Database.DbInfrastructure, dataContext);
						repository.GetEntitiesByExample<EntityType> (new EntityType ()).Count ();
					}
				},
				5
			);
			
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);
				repository.GetEntitiesByExample<EntityType> (new EntityType ()).Count ();

				TestHelper.MeasureAndDisplayTime (
					TestHelper.extendString (new EntityType().GetType().Name, 30) + "\twarmup: true\tbulkMode: " + bulkMode,
					() => repository.GetEntitiesByExample<EntityType> (new EntityType ()).Count (),
					5
				);
			}
		}


		[TestMethod]
		public void GetUriContactWithGivenUriScheme()
		{
			TestHelper.PrintStartTest ("Retrieve uri contacts with uri schemes");

			this.GetUriContactWithGivenUriScheme (false);
			this.GetUriContactWithGivenUriScheme (true);
		}


		public void GetUriContactWithGivenUriScheme(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				UriSchemeEntity uriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000000001)));
				UriContactEntity example = new UriContactEntity ()
				{
					UriScheme = uriScheme,
				};

				TestHelper.MeasureAndDisplayTime (
					"bulkMode: " + bulkMode,
					() => repository.GetEntitiesByExample<UriContactEntity> (example).Count (),
					5
				);
			}
		}


		[TestMethod]
		public void GetLocationsGivenCountry()
		{
			TestHelper.PrintStartTest ("Retrieve locations given country");

			this.GetLocationsGivenCountry (false);
			this.GetLocationsGivenCountry (true);
		}


		public void GetLocationsGivenCountry(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				CountryEntity country = dataContext.ResolveEntity<CountryEntity> (new DbKey (new DbId (1000000000001)));
				LocationEntity example = new LocationEntity ()
				{
					Country = country,
				};

				TestHelper.MeasureAndDisplayTime (
					"bulkMode: " + bulkMode,
					() => repository.GetEntitiesByExample<LocationEntity> (example).Count (),
					5
				);
			}
		}


		[TestMethod]
		public void GetLegalPersonsGivenType()
		{
			TestHelper.PrintStartTest ("Retrieve legal persons given type");

			this.GetLegalPersonsGivenType (false);
			this.GetLegalPersonsGivenType (true);
		}


		public void GetLegalPersonsGivenType(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				LegalPersonTypeEntity type = dataContext.ResolveEntity<LegalPersonTypeEntity> (new DbKey (new DbId (1000000000001)));
				LegalPersonEntity example = new LegalPersonEntity ()
				{
					LegalPersonType = type,
				};

				TestHelper.MeasureAndDisplayTime (
					"bulkMode: " + bulkMode,
					() => repository.GetEntitiesByExample<LegalPersonEntity> (example).Count (),
					5
				);
			}
		}


		[TestMethod]
		public void GetContactsGivenPerson()
		{
			TestHelper.PrintStartTest ("Retrieve contacts given person");

			this.GetContactsGivenPerson (false);
			this.GetContactsGivenPerson (true);
		}

		public void GetContactsGivenPerson(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));
				AbstractContactEntity example = new AbstractContactEntity ()
				{
					NaturalPerson = person,
				};

				TestHelper.MeasureAndDisplayTime (
					"bulkMode: " + bulkMode,
					() => repository.GetEntitiesByExample<AbstractContactEntity> (example).Count (),
					5
				);
			}
		}


		[TestMethod]
		public void GetPersonGivenLocation()
		{
			TestHelper.PrintStartTest ("Retrieve person given location");

			this.GetPersonGivenLocation (false);
			this.GetPersonGivenLocation (true);
		}

		public void GetPersonGivenLocation(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				LocationEntity location = dataContext.ResolveEntity<LocationEntity> (new DbKey (new DbId (1000000000001)));
				
				NaturalPersonEntity example = new NaturalPersonEntity ();
				example.Contacts.Add (new MailContactEntity ()
					{
						Address = new AddressEntity ()
						{
							Location = location,
						}
					}
				);


				TestHelper.MeasureAndDisplayTime (
					"bulkMode: " + bulkMode,
					() => repository.GetEntitiesByExample<NaturalPersonEntity> (example).Count (),
					5
				);
			}
		}


		[TestMethod]
		public void GetAddressGivenLegalPerson()
		{
			TestHelper.PrintStartTest ("Retrieve address given legal person");

			this.GetAddressGivenLegalPerson (false);
			this.GetAddressGivenLegalPerson (true);
		}


		public void GetAddressGivenLegalPerson(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				LegalPersonEntity person = dataContext.ResolveEntity<LegalPersonEntity> (new DbKey (new DbId (1000000000001)));
				MailContactEntity example = new MailContactEntity ()
				{
					LegalPerson = person,
				};


				TestHelper.MeasureAndDisplayTime (
					"bulkMode: " + bulkMode,
					() => repository.GetEntitiesByExample<MailContactEntity> (example).Select (c => c.Address).Count (),
					5
				);
			}
		}



		private static bool createDatabase = true;


		private static DatabaseSize databaseSize = DatabaseSize.Small;


	}
}
