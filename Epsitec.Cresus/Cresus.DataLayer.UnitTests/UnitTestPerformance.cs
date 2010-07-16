//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace Epsitec.Cresus.DataLayer.UnitTests
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

			this.RetrieveAllData<AbstractPersonEntity> ();
			this.RetrieveAllData<NaturalPersonEntity> ();
			this.RetrieveAllData<LegalPersonEntity> ();
			this.RetrieveAllData<LegalPersonTypeEntity> ();
			this.RetrieveAllData<PersonTitleEntity> ();
			this.RetrieveAllData<PersonGenderEntity> ();
			this.RetrieveAllData<AbstractContactEntity> ();
			this.RetrieveAllData<ContactRoleEntity> ();
			this.RetrieveAllData<CommentEntity> ();
			this.RetrieveAllData<MailContactEntity> ();
			this.RetrieveAllData<AddressEntity> ();
			this.RetrieveAllData<StreetEntity> ();
			this.RetrieveAllData<PostBoxEntity> ();
			this.RetrieveAllData<LocationEntity> ();
			this.RetrieveAllData<RegionEntity> ();
			this.RetrieveAllData<CountryEntity> ();
			this.RetrieveAllData<TelecomContactEntity> ();
			this.RetrieveAllData<TelecomTypeEntity> ();
			this.RetrieveAllData<UriContactEntity> ();
			this.RetrieveAllData<UriSchemeEntity> ();
		}


		public void RetrieveAllData<EntityType>() where EntityType : AbstractEntity, new()
		{
			TestHelper.MeasureAndWriteTime (
		        TestHelper.extendString (new EntityType ().GetType ().Name, 30) + "\twarmup: false",
		        UnitTestPerformance.logFile,
				() =>
		        {
		            using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
		            {
		               dataContext.GetByExample<EntityType> (new EntityType ()).Count ();
		            }
		        },
				UnitTestPerformance.nbRuns
		    );
			
		    using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
		    {
		        dataContext.GetByExample<EntityType> (new EntityType ()).Count ();

				TestHelper.MeasureAndWriteTime (
					TestHelper.extendString (new EntityType ().GetType ().Name, 30) + "\twarmup: true",
					UnitTestPerformance.logFile,
		            () => dataContext.GetByExample<EntityType> (new EntityType ()).Count (),
					UnitTestPerformance.nbRuns
		        );
		    }
		}


		[TestMethod]
		public void GetUriContactWithGivenUriScheme()
		{
			TestHelper.WriteStartTest ("Retrieve uri contacts with uri scheme", UnitTestPerformance.logFile);

			this.GetUriContactWithGivenUriSchemeReference ();

			this.GetUriContactWithGivenUriSchemeValue ();
		}


		public void GetUriContactWithGivenUriSchemeReference()
		{
		    using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
		    {
				UriSchemeEntity uriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1)));

		        UriContactEntity example = new UriContactEntity ()
		        {
		            UriScheme = uriScheme,
		        };

				TestHelper.MeasureAndWriteTime (
					"mode: reference",
					UnitTestPerformance.logFile,
					() => dataContext.GetByExample<UriContactEntity> (example).Count (),
					UnitTestPerformance.nbRuns
				);
		    }
		}


		public void GetUriContactWithGivenUriSchemeValue()
		{
			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				UriContactEntity example = new UriContactEntity ()
				{
					UriScheme = new UriSchemeEntity ()
					{
						Name = "name1",
					},
				};

				TestHelper.MeasureAndWriteTime (
					"mode: value",
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

		    this.GetLocationsGivenCountryReference ();

			this.GetLocationsGivenCountryValue ();
		}


		public void GetLocationsGivenCountryReference()
		{
		    using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
		    {
		        CountryEntity country = dataContext.ResolveEntity<CountryEntity> (new DbKey (new DbId (1)));

		        LocationEntity example = new LocationEntity ()
		        {
		            Country = country,
		        };

				TestHelper.MeasureAndWriteTime (
					"mode: reference",
					UnitTestPerformance.logFile,
		            () => dataContext.GetByExample<LocationEntity> (example).Count (),
					UnitTestPerformance.nbRuns
		        );
		    }
		}


		public void GetLocationsGivenCountryValue()
		{
			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				LocationEntity example = new LocationEntity ()
				{
					Country = new CountryEntity ()
					{
						Name = "name1",
					},
				};

				TestHelper.MeasureAndWriteTime (
					"mode: value",
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

		    this.GetLegalPersonsGivenTypeReference ();

			this.GetLegalPersonsGivenTypeValue ();
		}


		public void GetLegalPersonsGivenTypeReference()
		{
		    using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
		    {
		        LegalPersonTypeEntity legalPersonType = dataContext.ResolveEntity<LegalPersonTypeEntity> (new DbKey (new DbId (1)));

				LegalPersonEntity example = new LegalPersonEntity ()
				{
					LegalPersonType = legalPersonType,
		        };

				TestHelper.MeasureAndWriteTime (
					"mode: reference",
					UnitTestPerformance.logFile,
		            () => dataContext.GetByExample<LegalPersonEntity> (example).Count (),
					UnitTestPerformance.nbRuns
		        );
		    }
		}


		public void GetLegalPersonsGivenTypeValue()
		{
			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				LegalPersonEntity example = new LegalPersonEntity ()
				{
					LegalPersonType = new LegalPersonTypeEntity ()
					{
						Name = "name1",
					},
				};

				TestHelper.MeasureAndWriteTime (
					"mode: value",
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

		    this.GetContactsGivenPersonReference ();

			this.GetContactsGivenPersonValue ();
		}


		public void GetContactsGivenPersonReference()
		{
		    using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
		    {
		        NaturalPersonEntity naturalPerson = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				AbstractContactEntity example = new AbstractContactEntity ()
				{
					NaturalPerson = naturalPerson,
		        };

				TestHelper.MeasureAndWriteTime (
					"mode: reference",
					UnitTestPerformance.logFile,
		            () => dataContext.GetByExample<AbstractContactEntity> (example).Count (),
					UnitTestPerformance.nbRuns
		        );
		    }
		}


		public void GetContactsGivenPersonValue()
		{
			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				AbstractContactEntity example = new AbstractContactEntity ()
				{
					NaturalPerson = new NaturalPersonEntity ()
					{
						Lastname = "lastname1",
					}
				};

				TestHelper.MeasureAndWriteTime (
					"mode: value",
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

		    this.GetPersonGivenLocationReference ();

			UnitTestPerformance.GetPersonGivenLocationValue ();
		}


		public void GetPersonGivenLocationReference()
		{
		    using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
		    {
		        LocationEntity location = dataContext.ResolveEntity<LocationEntity> (new DbKey (new DbId (1)));

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
					"mode: reference",
					UnitTestPerformance.logFile,
		            () => dataContext.GetByExample<NaturalPersonEntity> (example).Count (),
					UnitTestPerformance.nbRuns
		        );
		    }
		}


		public static void GetPersonGivenLocationValue()
		{
			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
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
					"mode: value",
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

		    this.GetAddressGivenLegalPersonReference ();

			this.GetAddressGivenLegalPersonValue ();
		}


		public void GetAddressGivenLegalPersonReference()
		{
		    using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
		    {
		        LegalPersonEntity legalPerson = dataContext.ResolveEntity<LegalPersonEntity> (new DbKey (new DbId (UnitTestPerformance.legalPersonId[UnitTestPerformance.databaseSize])));

				MailContactEntity example = new MailContactEntity ()
				{
					LegalPerson = legalPerson,
		        };

				TestHelper.MeasureAndWriteTime (
					"mode: reference",
					UnitTestPerformance.logFile,
		            () => dataContext.GetByExample<MailContactEntity> (example).Select (c => c.Address).Count (),
					UnitTestPerformance.nbRuns
		        );
		    }
		}


		public void GetAddressGivenLegalPersonValue()
		{
			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				MailContactEntity example = new MailContactEntity ()
				{
					LegalPerson = new LegalPersonEntity ()
					{
						Name = "name1",
					}
				};

				TestHelper.MeasureAndWriteTime (
					"mode: value",
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

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				AddressEntity address = dataContext.ResolveEntity<AddressEntity> (new DbKey (new DbId (1)));

				TestHelper.MeasureAndWriteTime (
					"",
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

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				LegalPersonEntity person = dataContext.ResolveEntity<LegalPersonEntity> (new DbKey (new DbId (UnitTestPerformance.legalPersonId[UnitTestPerformance.databaseSize])));

				TestHelper.MeasureAndWriteTime (
					"results",
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
				this.DeleteEntity<NaturalPersonEntity> (1);
				this.DeleteEntity<AbstractContactEntity> (1);
				this.DeleteEntity<RegionEntity> (1);
				this.DeleteEntity<ContactRoleEntity> (1);
			}
		}


		public void DeleteEntity<EntityType>(long id) where EntityType : AbstractEntity, new()
		{
			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				EntityType entity = dataContext.ResolveEntity<EntityType> (new DbKey (new DbId (id)));

				TestHelper.MeasureAndWriteTime (
					TestHelper.extendString (entity.GetType ().Name, 30),
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
			{DatabaseSize.Small,	101},
			{DatabaseSize.Medium,  1001},
			{DatabaseSize.Large,  10001},
			{DatabaseSize.Huge,  100001},
		};


		private readonly static int nbRuns = 5;		


		private readonly static bool createDatabase = true;


		private readonly static bool runDeleteTests = true;


		private readonly static DatabaseSize databaseSize = DatabaseSize.Small;


		private readonly static string logFile = @"S:\Epsitec.Cresus\Cresus.DataLayer.UnitTests\bin\Debug\results.txt";


	}
}
