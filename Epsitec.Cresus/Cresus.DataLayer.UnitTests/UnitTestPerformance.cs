//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace Epsitec.Cresus.DataLayer
{


	[TestClass]
	public class UnitTestPerformance
	{


		[ClassInitialize]
		public void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();
			File.Delete (UnitTestPerformance.logFile);
		}


		[TestMethod]
		public void CreateAndPopulateDatabase()
		{
			if (UnitTestPerformance.createDatabase)
			{
				TestHelper.WriteStartTest ("Database creation", UnitTestPerformance.logFile);
							
				Database.CreateAndConnectToDatabase ();

				TestHelper.MeasureAndWriteTime(
					"database population",
					UnitTestPerformance.logFile,
					() => Database1.PopulateDatabase (UnitTestPerformance.databaseSize),
					1
				);
			}
			else
			{
				TestHelper.WriteStartTest ("Database connection", UnitTestPerformance.logFile);

				Database.ConnectToDatabase ();
			}
		}


		[TestMethod]
		public void RetrieveAllDataWithWarmup()
		{
			TestHelper.WriteStartTest ("Retrieve all data", UnitTestPerformance.logFile);

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
			TestHelper.MeasureAndWriteTime (
		        TestHelper.extendString (new EntityType ().GetType ().Name, 30) + "\twarmup: false\tbulkMode: " + bulkMode,
		        UnitTestPerformance.logFile,
				() =>
		        {
		            using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
		            {
		               dataContext.GetByExample<EntityType> (new EntityType ()).Count ();
		            }
		        },
				UnitTestPerformance.nbRuns
		    );
			
		    using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
		    {
		        dataContext.GetByExample<EntityType> (new EntityType ()).Count ();

				TestHelper.MeasureAndWriteTime (
					TestHelper.extendString (new EntityType ().GetType ().Name, 30) + "\twarmup: true\tbulkMode: " + bulkMode,
					UnitTestPerformance.logFile,
		            () => dataContext.GetByExample<EntityType> (new EntityType ()).Count (),
					UnitTestPerformance.nbRuns
		        );
		    }
		}


		[TestMethod]
		public void GetUriContactWithGivenUriSchemeReference()
		{
			TestHelper.WriteStartTest ("Retrieve uri contacts with uri scheme", UnitTestPerformance.logFile);

			this.GetUriContactWithGivenUriSchemeReference (false);
			this.GetUriContactWithGivenUriSchemeReference (true);

			this.GetUriContactWithGivenUriSchemeValue (false);
			this.GetUriContactWithGivenUriSchemeValue (true);
		}


		public void GetUriContactWithGivenUriSchemeReference(bool bulkMode)
		{
		    using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
		    {
				UriSchemeEntity uriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000000001)));

		        UriContactEntity example = new UriContactEntity ()
		        {
		            UriScheme = uriScheme,
		        };

				TestHelper.MeasureAndWriteTime (
					"mode: reference\t\tbulkMode: " + bulkMode,
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<UriContactEntity> (example).Count (),
					UnitTestPerformance.nbRuns
				);
		    }
		}


		public void GetUriContactWithGivenUriSchemeValue(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				UriContactEntity example = new UriContactEntity ()
				{
					UriScheme = new UriSchemeEntity ()
					{
						Name = "name1",
					},
				};

				TestHelper.MeasureAndWriteTime (
					"mode: value\t\t\tbulkMode: " + bulkMode,
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<UriContactEntity> (example).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		[TestMethod]
		public void GetLocationsGivenCountry()
		{
			TestHelper.WriteStartTest ("Retrieve locations given country", UnitTestPerformance.logFile);

		    this.GetLocationsGivenCountryReference (false);
		    this.GetLocationsGivenCountryReference (true);

			this.GetLocationsGivenCountryValue (false);
			this.GetLocationsGivenCountryValue (true);
		}


		public void GetLocationsGivenCountryReference(bool bulkMode)
		{
		    using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
		    {
		        CountryEntity country = dataContext.ResolveEntity<CountryEntity> (new DbKey (new DbId (1000000000001)));

		        LocationEntity example = new LocationEntity ()
		        {
		            Country = country,
		        };

				TestHelper.MeasureAndWriteTime (
					"mode: reference\t\tbulkMode: " + bulkMode,
					UnitTestPerformance.logFile,
		            () => dataContext.GetByExample<LocationEntity> (example).Count (),
					UnitTestPerformance.nbRuns
		        );
		    }
		}


		public void GetLocationsGivenCountryValue(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				LocationEntity example = new LocationEntity ()
				{
					Country = new CountryEntity ()
					{
						Name = "name1",
					},
				};

				TestHelper.MeasureAndWriteTime (
					"mode: value\t\t\tbulkMode: " + bulkMode,
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<LocationEntity> (example).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		[TestMethod]
		public void GetLegalPersonsGivenType()
		{
			TestHelper.WriteStartTest ("Retrieve legal persons given type", UnitTestPerformance.logFile);

		    this.GetLegalPersonsGivenTypeReference (false);
		    this.GetLegalPersonsGivenTypeReference (true);

			this.GetLegalPersonsGivenTypeValue (false);
			this.GetLegalPersonsGivenTypeValue (true);
		}


		public void GetLegalPersonsGivenTypeReference(bool bulkMode)
		{
		    using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
		    {
		        LegalPersonTypeEntity legalPersonType = dataContext.ResolveEntity<LegalPersonTypeEntity> (new DbKey (new DbId (1000000000001)));

				LegalPersonEntity example = new LegalPersonEntity ()
				{
					LegalPersonType = legalPersonType,
		        };

				TestHelper.MeasureAndWriteTime (
					"mode: reference\t\tbulkMode: " + bulkMode,
					UnitTestPerformance.logFile,
		            () => dataContext.GetByExample<LegalPersonEntity> (example).Count (),
					UnitTestPerformance.nbRuns
		        );
		    }
		}


		public void GetLegalPersonsGivenTypeValue(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				LegalPersonEntity example = new LegalPersonEntity ()
				{
					LegalPersonType = new LegalPersonTypeEntity ()
					{
						Name = "name1",
					},
				};

				TestHelper.MeasureAndWriteTime (
					"mode: value\t\t\tbulkMode: " + bulkMode,
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<LegalPersonEntity> (example).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		[TestMethod]
		public void GetContactsGivenPerson()
		{
			TestHelper.WriteStartTest ("Retrieve contacts given person", UnitTestPerformance.logFile);

		    this.GetContactsGivenPersonReference (false);
		    this.GetContactsGivenPersonReference (true);

			this.GetContactsGivenPersonValue (false);
			this.GetContactsGivenPersonValue (true);
		}


		public void GetContactsGivenPersonReference(bool bulkMode)
		{
		    using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
		    {
		        NaturalPersonEntity naturalPerson = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));

				AbstractContactEntity example = new AbstractContactEntity ()
				{
					NaturalPerson = naturalPerson,
		        };

				TestHelper.MeasureAndWriteTime (
					"mode: reference\t\tbulkMode: " + bulkMode,
					UnitTestPerformance.logFile,
		            () => dataContext.GetByExample<AbstractContactEntity> (example).Count (),
					UnitTestPerformance.nbRuns
		        );
		    }
		}


		public void GetContactsGivenPersonValue(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				AbstractContactEntity example = new AbstractContactEntity ()
				{
					NaturalPerson = new NaturalPersonEntity ()
					{
						Lastname = "lastname1",
					}
				};

				TestHelper.MeasureAndWriteTime (
					"mode: value\t\t\tbulkMode: " + bulkMode,
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<AbstractContactEntity> (example).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		[TestMethod]
		public void GetPersonGivenLocation()
		{
			TestHelper.WriteStartTest ("Retrieve person given location", UnitTestPerformance.logFile);

		    this.GetPersonGivenLocationReference (false);
			this.GetPersonGivenLocationReference (true);

			UnitTestPerformance.GetPersonGivenLocationValue (false);
			UnitTestPerformance.GetPersonGivenLocationValue (true);
		}


		public void GetPersonGivenLocationReference(bool bulkMode)
		{
		    using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
		    {
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

				TestHelper.MeasureAndWriteTime (
					"mode: reference\t\tbulkMode: " + bulkMode,
					UnitTestPerformance.logFile,
		            () => dataContext.GetByExample<NaturalPersonEntity> (example).Count (),
					UnitTestPerformance.nbRuns
		        );
		    }
		}


		public static void GetPersonGivenLocationValue(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
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

				TestHelper.MeasureAndWriteTime (
					"mode: value\t\t\tbulkMode: " + bulkMode,
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<NaturalPersonEntity> (example).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		[TestMethod]
		public void GetAddressGivenLegalPerson()
		{
			TestHelper.WriteStartTest ("Retrieve address given legal person", UnitTestPerformance.logFile);

		    this.GetAddressGivenLegalPersonReference (false);
			this.GetAddressGivenLegalPersonReference (true);

			this.GetAddressGivenLegalPersonValue (false);
			this.GetAddressGivenLegalPersonValue (true);
		}


		public void GetAddressGivenLegalPersonReference(bool bulkMode)
		{
		    using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
		    {
		        LegalPersonEntity legalPerson = dataContext.ResolveEntity<LegalPersonEntity> (new DbKey (new DbId (UnitTestPerformance.legalPersonId[UnitTestPerformance.databaseSize])));

				MailContactEntity example = new MailContactEntity ()
				{
					LegalPerson = legalPerson,
		        };

				TestHelper.MeasureAndWriteTime (
					"mode: reference\t\tbulkMode: " + bulkMode,
					UnitTestPerformance.logFile,
		            () => dataContext.GetByExample<MailContactEntity> (example).Select (c => c.Address).Count (),
					UnitTestPerformance.nbRuns
		        );
		    }
		}


		public void GetAddressGivenLegalPersonValue(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				MailContactEntity example = new MailContactEntity ()
				{
					LegalPerson = new LegalPersonEntity ()
					{
						Name = "name1",
					}
				};

				TestHelper.MeasureAndWriteTime (
					"mode: value\t\t\tbulkMode: " + bulkMode,
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<MailContactEntity> (example).Select (c => c.Address).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		[TestMethod]
		public void GetAddressReferencers()
		{
			TestHelper.WriteStartTest ("Retrieve address referencers", UnitTestPerformance.logFile);

			this.GetAddressReferencers (false);
			this.GetAddressReferencers (true);
		}


		public void GetAddressReferencers(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				AddressEntity address = dataContext.ResolveEntity<AddressEntity> (new DbKey (new DbId (1000000000001)));

				TestHelper.MeasureAndWriteTime (
					"bulkMode: " + bulkMode,
					UnitTestPerformance.logFile,
					() => dataContext.GetReferencers (address).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		[TestMethod]
		public void GetLegalPersonReferencers()
		{
			TestHelper.WriteStartTest ("Retrieve legal person referencers", UnitTestPerformance.logFile);

			this.GetLegalPersonReferencers (false);
			this.GetLegalPersonReferencers (true);
		}


		public void GetLegalPersonReferencers(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				LegalPersonEntity person = dataContext.ResolveEntity<LegalPersonEntity> (new DbKey (new DbId (UnitTestPerformance.legalPersonId[UnitTestPerformance.databaseSize])));

				TestHelper.MeasureAndWriteTime (
					"bulkMode: " + bulkMode,
					UnitTestPerformance.logFile,
					() => dataContext.GetReferencers (person).Count (),
					UnitTestPerformance.nbRuns
				);
			}
		}


		[TestMethod]
		public void DeleteEntities()
		{
			TestHelper.WriteStartTest ("DeleteEntities", UnitTestPerformance.logFile);

			if (UnitTestPerformance.runDeleteTests)
			{
				this.DeleteEntity<NaturalPersonEntity> (1000000000001, false);
				this.DeleteEntity<NaturalPersonEntity> (1000000000002, true);

				this.DeleteEntity<AbstractContactEntity> (1000000000001, false);
				this.DeleteEntity<AbstractContactEntity> (1000000000002, true);

				this.DeleteEntity<RegionEntity> (1000000000001, false);
				this.DeleteEntity<RegionEntity> (1000000000002, true);

				this.DeleteEntity<ContactRoleEntity> (1000000000001, false);
				this.DeleteEntity<ContactRoleEntity> (1000000000002, true);
			}
		}


		public void DeleteEntity<EntityType>(long id, bool bulkMode) where EntityType : AbstractEntity, new()
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				EntityType entity = dataContext.ResolveEntity<EntityType> (new DbKey (new DbId (id)));

				TestHelper.MeasureAndWriteTime (
					TestHelper.extendString (entity.GetType ().Name, 30) + "\tbulkMode: " + bulkMode,
					UnitTestPerformance.logFile,
					() =>
					{
						dataContext.DeleteEntity (entity);
						dataContext.SaveChanges ();
					},
					1
				);
			}
		}


		private readonly static Dictionary<DatabaseSize, long> legalPersonId = new Dictionary<DatabaseSize, long> ()
		{
			{DatabaseSize.Small,	1000000000101},
			{DatabaseSize.Medium,	1000000001001},
			{DatabaseSize.Large,	1000000010001},
			{DatabaseSize.Huge,		1000000100001},
		};


		private readonly static int nbRuns = 5;		


		private readonly static bool createDatabase = true;


		private readonly static bool runDeleteTests = true;


		private readonly static DatabaseSize databaseSize = DatabaseSize.Large;


		private readonly static string logFile = @"S:\Epsitec.Cresus\Cresus.DataLayer.UnitTests\bin\Debug\results.txt";


	}
}
