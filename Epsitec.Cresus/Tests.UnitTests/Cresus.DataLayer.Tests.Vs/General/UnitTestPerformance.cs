//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace Epsitec.Cresus.DataLayer.UnitTests.General
{


	[TestClass]
	public sealed class UnitTestPerformance
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			File.Delete (UnitTestPerformance.logFile);

			if (UnitTestPerformance.createDatabase)
			{
				TestHelper.WriteStartTest ("Database creation", UnitTestPerformance.logFile);

				DbInfrastructureHelper.ResetTestDatabase ();

				TestHelper.MeasureAndWriteTime (
					"database population",
					UnitTestPerformance.logFile,
					() => DatabaseCreator1.PopulateDatabase (UnitTestPerformance.databaseSize),
					1
				);
			}
			else
			{
				TestHelper.WriteStartTest ("Database connection", UnitTestPerformance.logFile);
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


		public void RetrieveAllData<EntityType>() where EntityType : AbstractEntity, new ()
		{
			TestHelper.MeasureAndWriteTime (
				TestHelper.extendString (new EntityType ().GetType ().Name, 30) + "\twarmup: false",
				UnitTestPerformance.logFile,
				() =>
				{
					using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
					using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
					using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
					{
						dataContext.GetByExample<EntityType> (new EntityType ()).Count ();
					}
				},
				UnitTestPerformance.nbRuns
			);

			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				UriSchemeEntity uriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001)));

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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				CountryEntity country = dataContext.ResolveEntity<CountryEntity> (new DbKey (new DbId (1000000001)));

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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				LegalPersonTypeEntity legalPersonType = dataContext.ResolveEntity<LegalPersonTypeEntity> (new DbKey (new DbId (1000000001)));

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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity naturalPerson = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				LocationEntity location = dataContext.ResolveEntity<LocationEntity> (new DbKey (new DbId (1000000001)));

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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				LegalPersonEntity legalPerson = dataContext.ResolveEntity<LegalPersonEntity> (new DbKey (new DbId (1000000000 + UnitTestPerformance.legalPersonId[UnitTestPerformance.databaseSize])));

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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
		public void DeleteEntities()
		{
			TestHelper.WriteStartTest ("DeleteEntities", UnitTestPerformance.logFile);

			if (UnitTestPerformance.runDeleteTests)
			{
				this.DeleteEntity<NaturalPersonEntity> (10);
				this.DeleteEntity<AbstractContactEntity> (10);
				this.DeleteEntity<RegionEntity> (10);
				this.DeleteEntity<ContactRoleEntity> (10);
			}
		}


		public void DeleteEntity<EntityType>(long id) where EntityType : AbstractEntity, new ()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				EntityType entity = dataContext.ResolveEntity<EntityType> (new DbKey (new DbId (1000000000 + id)));

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


		private readonly static string logFile = @"results.txt";


	}
}
