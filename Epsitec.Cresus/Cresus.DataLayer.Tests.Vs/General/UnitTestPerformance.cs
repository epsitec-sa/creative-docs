//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.IO;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.General
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
					TestHelper.extendString ("database population", 46),
					UnitTestPerformance.logFile,
					() => DatabaseCreator1.PopulateDatabase (UnitTestPerformance.databaseSize),
					1
				);
			}
			else
			{
				TestHelper.WriteStartTest ("Database connection", UnitTestPerformance.logFile);
			}

			using (DbInfrastructure infrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					foreach (DbTable table in infrastructure.FindDbTables (DbElementCat.Any))
					{
						foreach (DbIndex index in table.Indexes)
						{
							infrastructure.ResetIndex (transaction, table, index);
						}
					}
				}
			}
		}


		[TestMethod]
		public void RetrieveAllDataTest()
		{
			TestHelper.WriteStartTest ("Retrieve all data", UnitTestPerformance.logFile);

			for (int i = 1; i <= 3; i++)
			{
				foreach (Druid entityTypeId in EntityEngineHelper.GetEntityTypeIds ())
				{
					this.RetrieveAllData (entityTypeId, i);
				}				
			}
		}


		public void RetrieveAllData(Druid entityTypeId, int warmupLevel)
		{
			AbstractEntity entity = new EntityContext ().CreateEntity (entityTypeId);

			string title = TestHelper.extendString (entity.GetType ().Name, 40) + "warmup: level " + warmupLevel;
			System.Action<DataContext> action = (dataContext) =>
			{
				dataContext.GetByExample (entity).Count ();
			};

			this.Execute (title, action, warmupLevel, UnitTestPerformance.nbRuns);
		}


		[TestMethod]
		public void GetUriContactWithGivenUriSchemeTest()
		{
			TestHelper.WriteStartTest ("Retrieve uri contacts with uri scheme", UnitTestPerformance.logFile);

			for (int i = 1; i <= 3; i++)
			{
				this.GetUriContactWithGivenUriSchemeReference (i);
				this.GetUriContactWithGivenUriSchemeValue (i);
			}
		}


		public void GetUriContactWithGivenUriSchemeReference(int warmupLevel)
		{
			string title = TestHelper.extendString ("mode: reference", 40) + "warmup: level " + warmupLevel;
			System.Action<DataContext> action = (dataContext) =>
			{
				UriSchemeEntity uriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001)));

				UriContactEntity example = new UriContactEntity ()
				{
					UriScheme = uriScheme,
				};

				dataContext.GetByExample<UriContactEntity> (example).Count ();
			};

			this.Execute (title, action, warmupLevel, UnitTestPerformance.nbRuns);
		}


		public void GetUriContactWithGivenUriSchemeValue(int warmupLevel)
		{
			string title = TestHelper.extendString ("mode: value", 40) + "warmup: level " + warmupLevel;
			System.Action<DataContext> action = (dataContext) =>
			{
				UriContactEntity example = new UriContactEntity ()
				{
					UriScheme = new UriSchemeEntity ()
					{
						Name = "name1",
					},
				};

				dataContext.GetByExample<UriContactEntity> (example).Count ();
			};

			this.Execute (title, action, warmupLevel, UnitTestPerformance.nbRuns);
		}

		
		[TestMethod]
		public void GetLocationsGivenCountryTest()
		{
			TestHelper.WriteStartTest ("Retrieve locations given country", UnitTestPerformance.logFile);

			for (int i = 1; i <= 3; i++)
			{
				this.GetLocationsGivenCountryReference (i);
				this.GetLocationsGivenCountryValue (i);
			}
		}


		public void GetLocationsGivenCountryReference(int warmupLevel)
		{
			string title = TestHelper.extendString ("mode: reference", 40) + "warmup: level " + warmupLevel;
			System.Action<DataContext> action = (dataContext) =>
			{
				CountryEntity country = dataContext.ResolveEntity<CountryEntity> (new DbKey (new DbId (1000000001)));

				LocationEntity example = new LocationEntity ()
				{
					Country = country,
				};

				dataContext.GetByExample<LocationEntity> (example).Count ();
			};

			this.Execute (title, action, warmupLevel, UnitTestPerformance.nbRuns);
		}


		public void GetLocationsGivenCountryValue(int warmupLevel)
		{
			string title = TestHelper.extendString ("mode: value", 40) + "warmup: level " + warmupLevel;
			System.Action<DataContext> action = (dataContext) =>
			{
				LocationEntity example = new LocationEntity ()
				{
					Country = new CountryEntity ()
					{
						Name = "name1",
					},
				};

				dataContext.GetByExample<LocationEntity> (example).Count ();
			};

			this.Execute (title, action, warmupLevel, UnitTestPerformance.nbRuns);
		}


		[TestMethod]
		public void GetLegalPersonsGivenTypeTest()
		{
			TestHelper.WriteStartTest ("Retrieve legal persons given type", UnitTestPerformance.logFile);

			for (int i = 1; i <= 3; i++)
			{
				this.GetLegalPersonsGivenTypeReference (i);
				this.GetLegalPersonsGivenTypeValue (i);
			}
		}


		public void GetLegalPersonsGivenTypeReference(int warmupLevel)
		{
			string title = TestHelper.extendString ("mode: reference", 40) + "warmup: level " + warmupLevel;
			System.Action<DataContext> action = (dataContext) =>
			{
				LegalPersonTypeEntity legalPersonType = dataContext.ResolveEntity<LegalPersonTypeEntity> (new DbKey (new DbId (1000000001)));

				LegalPersonEntity example = new LegalPersonEntity ()
				{
					LegalPersonType = legalPersonType,
				};

				dataContext.GetByExample<LegalPersonEntity> (example).Count ();
			};

			this.Execute (title, action, warmupLevel, UnitTestPerformance.nbRuns);
		}


		public void GetLegalPersonsGivenTypeValue(int warmupLevel)
		{
			string title = TestHelper.extendString ("mode: value", 40) + "warmup: level " + warmupLevel;
			System.Action<DataContext> action = (dataContext) =>
			{
				LegalPersonEntity example = new LegalPersonEntity ()
				{
					LegalPersonType = new LegalPersonTypeEntity ()
					{
						Name = "name1",
					},
				};

				dataContext.GetByExample<LegalPersonEntity> (example).Count ();
			};

			this.Execute (title, action, warmupLevel, UnitTestPerformance.nbRuns);
		}


		[TestMethod]
		public void GetContactsGivenPersonTest()
		{
			TestHelper.WriteStartTest ("Retrieve contacts given person", UnitTestPerformance.logFile);

			for (int i = 1; i <= 3; i++)
			{
				this.GetContactsGivenPersonReference (i);
				this.GetContactsGivenPersonValue (i);
			}
		}


		public void GetContactsGivenPersonReference(int warmupLevel)
		{
			string title = TestHelper.extendString ("mode: reference", 40) + "warmup: level " + warmupLevel;
			System.Action<DataContext> action = (dataContext) =>
			{
				NaturalPersonEntity naturalPerson = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				AbstractContactEntity example = new AbstractContactEntity ()
				{
					NaturalPerson = naturalPerson,
				};

				dataContext.GetByExample<AbstractContactEntity> (example).Count ();
			};

			this.Execute (title, action, warmupLevel, UnitTestPerformance.nbRuns);
		}


		public void GetContactsGivenPersonValue(int warmupLevel)
		{
			string title = TestHelper.extendString ("mode: value", 40) + "warmup: level " + warmupLevel;
			System.Action<DataContext> action = (dataContext) =>
			{
				AbstractContactEntity example = new AbstractContactEntity ()
				{
					NaturalPerson = new NaturalPersonEntity ()
					{
						Lastname = "lastname1",
					}
				};

				dataContext.GetByExample<AbstractContactEntity> (example).Count ();
			};

			this.Execute (title, action, warmupLevel, UnitTestPerformance.nbRuns);
		}


		[TestMethod]
		public void GetPersonGivenLocationTest()
		{
			TestHelper.WriteStartTest ("Retrieve person given location", UnitTestPerformance.logFile);

			for (int i = 1; i <= 3; i++)
			{
				this.GetPersonGivenLocationReference (i);
				this.GetPersonGivenLocationValue (i);
			}
		}


		public void GetPersonGivenLocationReference(int warmupLevel)
		{
			string title = TestHelper.extendString ("mode: reference", 40) + "warmup: level " + warmupLevel;
			System.Action<DataContext> action = (dataContext) =>
			{
				LocationEntity location = dataContext.ResolveEntity<LocationEntity> (new DbKey (new DbId (1000000001)));

				NaturalPersonEntity example = new NaturalPersonEntity ();
				example.Contacts.Add
				(
					new MailContactEntity ()
					{
						Address = new AddressEntity ()
						{
							Location = location,
						}
					}
				);

				dataContext.GetByExample<NaturalPersonEntity> (example).Count ();
			};

			this.Execute (title, action, warmupLevel, UnitTestPerformance.nbRuns);
		}


		public void GetPersonGivenLocationValue(int warmupLevel)
		{
			string title = TestHelper.extendString ("mode: value", 40) + "warmup: level " + warmupLevel;
			System.Action<DataContext> action = (dataContext) =>
			{
				NaturalPersonEntity example = new NaturalPersonEntity ();
				example.Contacts.Add
				(
					new MailContactEntity ()
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

				dataContext.GetByExample<NaturalPersonEntity> (example).Count ();
			};

			this.Execute (title, action, warmupLevel, UnitTestPerformance.nbRuns);
		}


		[TestMethod]
		public void GetAddressGivenLegalPersonTest()
		{
			TestHelper.WriteStartTest ("Retrieve address given legal person", UnitTestPerformance.logFile);

			for (int i = 1; i <= 3; i++)
			{
				this.GetAddressGivenLegalPersonReference (i);
				this.GetAddressGivenLegalPersonValue (i);
			}
		}


		public void GetAddressGivenLegalPersonReference(int warmupLevel)
		{
			string title = TestHelper.extendString ("mode: reference", 40) + "warmup: level " + warmupLevel;
			System.Action<DataContext> action = (dataContext) =>
			{
				LegalPersonEntity legalPerson = dataContext.ResolveEntity<LegalPersonEntity> (new DbKey (new DbId (1000000000 + UnitTestPerformance.legalPersonId[UnitTestPerformance.databaseSize])));

				MailContactEntity example = new MailContactEntity ()
				{
					LegalPerson = legalPerson,
				};

				dataContext.GetByExample<MailContactEntity> (example).Select (c => c.Address).Count ();
			};

			this.Execute (title, action, warmupLevel, UnitTestPerformance.nbRuns);
		}


		public void GetAddressGivenLegalPersonValue(int warmupLevel)
		{
			string title = TestHelper.extendString ("mode: value", 40) + "warmup: level " + warmupLevel;
			System.Action<DataContext> action = (dataContext) =>
			{
				MailContactEntity example = new MailContactEntity ()
				{
					LegalPerson = new LegalPersonEntity ()
					{
						Name = "name1",
					}
				};

				dataContext.GetByExample<MailContactEntity> (example).Select (c => c.Address).Count ();
			};

			this.Execute (title, action, warmupLevel, UnitTestPerformance.nbRuns);
		}


		[TestMethod]
		public void DeleteEntitiesTest()
		{
			TestHelper.WriteStartTest ("DeleteEntities", UnitTestPerformance.logFile);

			if (UnitTestPerformance.runDeleteTests)
			{
				for (int i = 1; i <= 2; i++)
				{
					foreach (Druid entityTypeId in EntityEngineHelper.GetEntityTypeIds ())
					{
						this.DeleteEntity (entityTypeId, i);
					}
				}			
			}
		}


		private void DeleteEntity(Druid entityTypeId, int warmupLevel)
		{
			AbstractEntity entity = new EntityContext ().CreateEntity (entityTypeId);
			
			string title = TestHelper.extendString (entity.GetType ().Name, 40) + "warmup: level " + warmupLevel;
			System.Action<DataContext> action = (dataContext) =>
			{
				AbstractEntity e = dataContext.ResolveEntity (entityTypeId, new DbKey (new DbId (1000000000 + warmupLevel)));

				if (e != null)
				{
					dataContext.DeleteEntity (e);
					dataContext.SaveChanges ();
				}
				else
				{
					string message = "Entity to delete is null"
						+ ": Type = " + entity.GetType ().Name
						+ ", Warmup level = " + warmupLevel;

					System.Diagnostics.Debug.WriteLine (message);
					System.Console.WriteLine (message);
				}
			};

			this.Execute (title, action, warmupLevel, 1);
		}


		public void Execute(string title, System.Action<DataContext> action, int warmupLevel, int nbRuns)
		{
			switch (warmupLevel)
			{
				case 1:
					this.ExecuteWarmupLevel1 (title, action, nbRuns);
					break;

				case 2:
					this.ExecuteWarmupLevel2 (title, action, nbRuns);
					break;

				case 3:
					this.ExecuteWarmupLevel3 (title, action, nbRuns);
					break;

				default:
					Assert.Fail ();
					break;
			}
		}


		private void ExecuteWarmupLevel1(string title, System.Action<DataContext> action, int nbRuns)
		{
			TestHelper.MeasureAndWriteTime (
				title,
				UnitTestPerformance.logFile,
				() =>
				{					
					using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
					using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
					{
						action (dataContext);
					}
				},
				nbRuns
			);
		}


		private void ExecuteWarmupLevel2(string title, System.Action<DataContext> action, int nbRuns)
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				TestHelper.MeasureAndWriteTime (
					title,
					UnitTestPerformance.logFile,
					() =>
					{
						using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
						{
							action (dataContext);
						}
					},
					nbRuns
				);
			}
		}


		private void ExecuteWarmupLevel3(string title, System.Action<DataContext> action, int nbRun)
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				action (dataContext);

				TestHelper.MeasureAndWriteTime (
					title,
					UnitTestPerformance.logFile,
					() => action (dataContext),
					nbRuns
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


		private static readonly int nbRuns = 1;
				


		private static readonly bool createDatabase = true;
				

		private static readonly bool runDeleteTests = true;
				

		private static readonly DatabaseSize databaseSize = DatabaseSize.Small;


		private const string logFile = @"results.txt";


	}
}
