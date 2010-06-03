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
		public void GetUriContactWithGivenUriSchemeReference()
		{
		    TestHelper.PrintStartTest ("Retrieve uri contacts with uri scheme");

			this.GetUriContactWithGivenUriSchemeReference (false);
			this.GetUriContactWithGivenUriSchemeReference (true);

			this.GetUriContactWithGivenUriSchemeValue (false);
			this.GetUriContactWithGivenUriSchemeValue (true);
		}


		public void GetUriContactWithGivenUriSchemeReference(bool bulkMode)
		{
		    using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
		    {
		        Repository repository = new Repository (Database.DbInfrastructure, dataContext);

		        UriContactEntity example = new UriContactEntity ()
		        {
		            UriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000000001))),
		        };

		        TestHelper.MeasureAndDisplayTime (
					"mode: reference\t\tbulkMode: " + bulkMode,
		            () => repository.GetEntitiesByExample<UriContactEntity> (example).Count (),
		            5
		        );
		    }
		}


		public void GetUriContactWithGivenUriSchemeValue(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				UriContactEntity example = new UriContactEntity ()
				{
					UriScheme = new UriSchemeEntity ()
					{
						Name = "name1",
					},
				};

				TestHelper.MeasureAndDisplayTime (
					"mode: value\t\t\tbulkMode: " + bulkMode,
					() => repository.GetEntitiesByExample<UriContactEntity> (example).Count (),
					5
				);
			}
		}


		[TestMethod]
		public void GetLocationsGivenCountry()
		{
		    TestHelper.PrintStartTest ("Retrieve locations given country");

		    this.GetLocationsGivenCountryReference (false);
		    this.GetLocationsGivenCountryReference (true);

			this.GetLocationsGivenCountryValue (false);
			this.GetLocationsGivenCountryValue (true);
		}


		public void GetLocationsGivenCountryReference(bool bulkMode)
		{
		    using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
		    {
		        Repository repository = new Repository (Database.DbInfrastructure, dataContext);

		        LocationEntity example = new LocationEntity ()
		        {
		            Country = dataContext.ResolveEntity<CountryEntity> (new DbKey (new DbId (1000000000001))),
		        };

		        TestHelper.MeasureAndDisplayTime (
					"mode: reference\t\tbulkMode: " + bulkMode,
		            () => repository.GetEntitiesByExample<LocationEntity> (example).Count (),
		            5
		        );
		    }
		}


		public void GetLocationsGivenCountryValue(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				LocationEntity example = new LocationEntity ()
				{
					Country = new CountryEntity ()
					{
						Name = "name1",
					},
				};

				TestHelper.MeasureAndDisplayTime (
					"mode: value\t\t\tbulkMode: " + bulkMode,
					() => repository.GetEntitiesByExample<LocationEntity> (example).Count (),
					5
				);
			}
		}


		[TestMethod]
		public void GetLegalPersonsGivenType()
		{
		    TestHelper.PrintStartTest ("Retrieve legal persons given type");

		    this.GetLegalPersonsGivenTypeReference (false);
		    this.GetLegalPersonsGivenTypeReference (true);

			this.GetLegalPersonsGivenTypeValue (false);
			this.GetLegalPersonsGivenTypeValue (true);
		}


		public void GetLegalPersonsGivenTypeReference(bool bulkMode)
		{
		    using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
		    {
		        Repository repository = new Repository (Database.DbInfrastructure, dataContext);

		        LegalPersonEntity example = new LegalPersonEntity ()
		        {
		            LegalPersonType = dataContext.ResolveEntity<LegalPersonTypeEntity> (new DbKey (new DbId (1000000000001))),
		        };

		        TestHelper.MeasureAndDisplayTime (
					"mode: reference\t\tbulkMode: " + bulkMode,
		            () => repository.GetEntitiesByExample<LegalPersonEntity> (example).Count (),
		            5
		        );
		    }
		}


		public void GetLegalPersonsGivenTypeValue(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				LegalPersonEntity example = new LegalPersonEntity ()
				{
					LegalPersonType = new LegalPersonTypeEntity ()
					{
						Name = "name1",
					},
				};

				TestHelper.MeasureAndDisplayTime (
					"mode: value\t\t\tbulkMode: " + bulkMode,
					() => repository.GetEntitiesByExample<LegalPersonEntity> (example).Count (),
					5
				);
			}
		}


		[TestMethod]
		public void GetContactsGivenPerson()
		{
		    TestHelper.PrintStartTest ("Retrieve contacts given person");

		    this.GetContactsGivenPersonReference (false);
		    this.GetContactsGivenPersonReference (true);

			this.GetContactsGivenPersonValue (false);
			this.GetContactsGivenPersonValue (true);
		}


		public void GetContactsGivenPersonReference(bool bulkMode)
		{
		    using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
		    {
		        Repository repository = new Repository (Database.DbInfrastructure, dataContext);

		        AbstractContactEntity example = new AbstractContactEntity ()
		        {
		            NaturalPerson = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001))),
		        };

		        TestHelper.MeasureAndDisplayTime (
					"mode: reference\t\tbulkMode: " + bulkMode,
		            () => repository.GetEntitiesByExample<AbstractContactEntity> (example).Count (),
		            5
		        );
		    }
		}


		public void GetContactsGivenPersonValue(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				AbstractContactEntity example = new AbstractContactEntity ()
				{
					NaturalPerson = new NaturalPersonEntity ()
					{
						Lastname = "lastname1",
					}
				};

				TestHelper.MeasureAndDisplayTime (
					"mode: value\t\t\tbulkMode: " + bulkMode,
					() => repository.GetEntitiesByExample<AbstractContactEntity> (example).Count (),
					5
				);
			}
		}


		[TestMethod]
		public void GetPersonGivenLocation()
		{
		    TestHelper.PrintStartTest ("Retrieve person given location");

		    this.GetPersonGivenLocationReference (false);
			this.GetPersonGivenLocationReference (true);

			this.GetPersonGivenLocationValue (false);
			this.GetPersonGivenLocationValue (true);
		}


		public void GetPersonGivenLocationReference(bool bulkMode)
		{
		    using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
		    {
		        Repository repository = new Repository (Database.DbInfrastructure, dataContext);

		        NaturalPersonEntity example = new NaturalPersonEntity ();
		        example.Contacts.Add (new MailContactEntity ()
		            {
		                Address = new AddressEntity ()
		                {
		                    Location = dataContext.ResolveEntity<LocationEntity> (new DbKey (new DbId (1000000000001))),
		                }
		            }
		        );

		        TestHelper.MeasureAndDisplayTime (
					"mode: reference\t\tbulkMode: " + bulkMode,
		            () => repository.GetEntitiesByExample<NaturalPersonEntity> (example).Count (),
		            5
		        );
		    }
		}


		public void GetPersonGivenLocationValue(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = new NaturalPersonEntity ();
				example.Contacts.Add (new MailContactEntity ()
					{
						Address = new AddressEntity ()
						{
							Location = new LocationEntity ()
							{
								Name = "name1",
							}
						}
					}
				);

				TestHelper.MeasureAndDisplayTime (
					"mode: value\t\t\tbulkMode: " + bulkMode,
					() => repository.GetEntitiesByExample<NaturalPersonEntity> (example).Count (),
					5
				);
			}
		}


		[TestMethod]
		public void GetAddressGivenLegalPerson()
		{
		    TestHelper.PrintStartTest ("Retrieve address given legal person");

		    this.GetAddressGivenLegalPersonReference (false);
			this.GetAddressGivenLegalPersonReference (true);

			this.GetAddressGivenLegalPersonValue (false);
			this.GetAddressGivenLegalPersonValue (true);
		}


		public void GetAddressGivenLegalPersonReference(bool bulkMode)
		{
		    using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
		    {
		        Repository repository = new Repository (Database.DbInfrastructure, dataContext);

		        MailContactEntity example = new MailContactEntity ()
		        {
					LegalPerson = dataContext.ResolveEntity<LegalPersonEntity> (new DbKey (new DbId (1000000010001))),
		        };

		        TestHelper.MeasureAndDisplayTime (
					"mode: reference\t\tbulkMode: " + bulkMode,
		            () => repository.GetEntitiesByExample<MailContactEntity> (example).Select (c => c.Address).Count (),
		            1
		        );
		    }
		}


		public void GetAddressGivenLegalPersonValue(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				MailContactEntity example = new MailContactEntity ()
				{
					LegalPerson = new LegalPersonEntity ()
					{
						Name = "name1",
					}
				};

				TestHelper.MeasureAndDisplayTime (
					"mode: value\t\t\tbulkMode: " + bulkMode,
					() => repository.GetEntitiesByExample<MailContactEntity> (example).Select (c => c.Address).Count (),
					1
				);
			}
		}


		[TestMethod]
		public void GetAddressReferencers()
		{
			TestHelper.PrintStartTest ("Retrieve address referencers");

			this.GetAddressReferencers (false);
			this.GetAddressReferencers (true);
		}


		public void GetAddressReferencers(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				AddressEntity address = dataContext.ResolveEntity<AddressEntity> (new DbKey (new DbId (1000000000001)));

				TestHelper.MeasureAndDisplayTime (
					"bulkMode: " + bulkMode,
					() => repository.GetReferencers (address).Count (),
					5
				);
			}
		}


		[TestMethod]
		public void GetLegalPersonReferencers()
		{
			TestHelper.PrintStartTest ("Retrieve legal person referencers");

			this.GetLegalPersonReferencers (false);
			this.GetLegalPersonReferencers (true);
		}


		public void GetLegalPersonReferencers(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				LegalPersonEntity person = dataContext.ResolveEntity<LegalPersonEntity> (new DbKey (new DbId (1000000010001)));

				TestHelper.MeasureAndDisplayTime (
					"bulkMode: " + bulkMode,
					() => repository.GetReferencers (person).Count (),
					5
				);
			}
		}


				
		private static bool createDatabase = false;


		private static DatabaseSize databaseSize = DatabaseSize.Small;


	}
}
