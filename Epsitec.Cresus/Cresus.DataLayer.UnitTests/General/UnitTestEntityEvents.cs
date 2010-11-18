using Epsitec.Common.Support;
using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;



namespace Epsitec.Cresus.DataLayer.UnitTests.General
{


	[TestClass]
	public sealed class UnitTestEntityEvents
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[ClassCleanup]
		public static void ClassCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseHelper.CreateAndConnectToDatabase ();

			Assert.IsTrue (DatabaseHelper.DbInfrastructure.IsConnectionOpen);

			DatabaseCreator2.PupulateDatabase ();
		}
		

		[TestMethod]
		public void ExternalEntityCreatedEventTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					List<EntityChangedEventArgs> eventArgs = new List<EntityChangedEventArgs> ();

					dataContext.EntityChanged += (s, a) => eventArgs.Add (a);

					Assert.IsTrue (eventArgs.Count == 0);

					NaturalPersonEntity person = dataContext.CreateEntity<NaturalPersonEntity> ();

					Assert.IsTrue (eventArgs.Count == 1);
					Assert.AreSame (person, eventArgs[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Created, eventArgs[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.External, eventArgs[0].EventSource);
				}
			}
		}


		[TestMethod]
		public void ExternalEntityUpdatedValueEventTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					List<EntityChangedEventArgs> eventArgs = new List<EntityChangedEventArgs> ();

					dataContext.EntityChanged += (s, a) => eventArgs.Add (a);

					LanguageEntity language = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001)));

					Assert.IsTrue (eventArgs.Count == 0);

					language.Name = "new name";

					Assert.IsTrue (eventArgs.Count == 1);
					Assert.AreSame (language, eventArgs[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Updated, eventArgs[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.External, eventArgs[0].EventSource);
				}
			}
		}


		[TestMethod]
		public void ExternalEntityUpdatedReferenceEventTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					List<EntityChangedEventArgs> eventArgs = new List<EntityChangedEventArgs> ();

					dataContext.EntityChanged += (s, a) => eventArgs.Add (a);

					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					PersonGenderEntity gender = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000002)));

					Assert.IsTrue (eventArgs.Count == 0);

					person.Gender = gender;

					Assert.IsTrue (eventArgs.Count == 1);
					Assert.AreSame (person, eventArgs[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Updated, eventArgs[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.External, eventArgs[0].EventSource);
				}
			}
		}


		[TestMethod]
		public void ExternalEntityUpdatedCollectionEventTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					List<EntityChangedEventArgs> eventArgs = new List<EntityChangedEventArgs> ();

					dataContext.EntityChanged += (s, a) =>
					{
						eventArgs.Add (a);
					};

					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000004)));

					Assert.IsTrue (eventArgs.Count == 0);

					person.Contacts.Add (contact);

					Assert.IsTrue (eventArgs.Count == 1);
					Assert.AreSame (person, eventArgs[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Updated, eventArgs[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.External, eventArgs[0].EventSource);
				}
			}
		}


		[TestMethod]
		public void ExternalEntityDeletedEventTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					List<EntityChangedEventArgs> eventArgs = new List<EntityChangedEventArgs> ();

					dataContext.EntityChanged += (s, a) => eventArgs.Add (a);

					LanguageEntity language = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001)));

					Assert.IsTrue (eventArgs.Count == 0);

					dataContext.DeleteEntity (language);

					Assert.IsTrue (eventArgs.Count == 0);

					dataContext.SaveChanges ();

					Assert.IsTrue (eventArgs.Count == 1);
					Assert.AreSame (language, eventArgs[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Deleted, eventArgs[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.External, eventArgs[0].EventSource);
				}
			}
		}


		[TestMethod]
		public void ExternalEntityReferenceDeletedEventTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					List<EntityChangedEventArgs> eventArgs = new List<EntityChangedEventArgs> ();

					dataContext.EntityChanged += (s, a) => eventArgs.Add (a);

					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					LanguageEntity language = person.PreferredLanguage;

					Assert.IsTrue (eventArgs.Count == 0);

					dataContext.DeleteEntity (language);

					Assert.IsTrue (eventArgs.Count == 0);

					dataContext.SaveChanges ();

					Assert.IsTrue (eventArgs.Count == 2);
					Assert.AreSame (person, eventArgs[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Updated, eventArgs[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.Internal, eventArgs[0].EventSource);

					Assert.AreSame (language, eventArgs[1].Entity);
					Assert.AreEqual (EntityChangedEventType.Deleted, eventArgs[1].EventType);
					Assert.AreEqual (EntityChangedEventSource.External, eventArgs[1].EventSource);
				}
			}
		}


		[TestMethod]
		public void ExternalEntityCollectionDeletedEventTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					List<EntityChangedEventArgs> eventArgs = new List<EntityChangedEventArgs> ();

					dataContext.EntityChanged += (s, a) =>
					{
						eventArgs.Add (a);
					};

					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					AbstractContactEntity contact = person.Contacts[0];

					Assert.IsTrue (eventArgs.Count == 0);

					dataContext.DeleteEntity (contact);

					Assert.IsTrue (eventArgs.Count == 0);

					dataContext.SaveChanges ();

					Assert.IsTrue (eventArgs.Count == 2);
					Assert.AreSame (person, eventArgs[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Updated, eventArgs[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.Internal, eventArgs[0].EventSource);

					Assert.AreSame (contact, eventArgs[1].Entity);
					Assert.AreEqual (EntityChangedEventType.Deleted, eventArgs[1].EventType);
					Assert.AreEqual (EntityChangedEventSource.External, eventArgs[1].EventSource);
				}
			}
		}


		[TestMethod]
		public void SynchronizationEntityValueUpdatedEventTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
				using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
				{
					List<EntityChangedEventArgs> eventArgs1 = new List<EntityChangedEventArgs> ();
					List<EntityChangedEventArgs> eventArgs2 = new List<EntityChangedEventArgs> ();

					dataContext1.EntityChanged += (s, a) => eventArgs1.Add (a);
					dataContext2.EntityChanged += (s, a) => eventArgs2.Add (a);

					LanguageEntity language1 = dataContext1.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001)));
					LanguageEntity language2 = dataContext2.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001)));

					Assert.IsTrue (eventArgs1.Count == 0);
					Assert.IsTrue (eventArgs2.Count == 0);

					language1.Name = "new name";

					Assert.IsTrue (eventArgs1.Count == 1);
					Assert.AreSame (language1, eventArgs1[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Updated, eventArgs1[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.External, eventArgs1[0].EventSource);

					Assert.IsTrue (eventArgs2.Count == 0);

					dataContext1.SaveChanges ();

					Assert.IsTrue (eventArgs1.Count == 1);

					Assert.IsTrue (eventArgs2.Count == 1);
					Assert.AreSame (language2, eventArgs2[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Updated, eventArgs2[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.Synchronization, eventArgs2[0].EventSource);
				}
			}
		}


		[TestMethod]
		public void SynchronizationEntityReferenceUpdatedEventTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
				using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
				{
					List<EntityChangedEventArgs> eventArgs1 = new List<EntityChangedEventArgs> ();
					List<EntityChangedEventArgs> eventArgs2 = new List<EntityChangedEventArgs> ();

					dataContext1.EntityChanged += (s, a) => eventArgs1.Add (a);
					dataContext2.EntityChanged += (s, a) => eventArgs2.Add (a);

					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					NaturalPersonEntity person2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					PersonGenderEntity gender = dataContext1.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000002)));

					Assert.IsTrue (eventArgs1.Count == 0);
					Assert.IsTrue (eventArgs2.Count == 0);

					person1.Gender = gender;

					Assert.IsTrue (eventArgs1.Count == 1);
					Assert.AreSame (person1, eventArgs1[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Updated, eventArgs1[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.External, eventArgs1[0].EventSource);

					Assert.IsTrue (eventArgs2.Count == 0);

					dataContext1.SaveChanges ();

					Assert.IsTrue (eventArgs1.Count == 1);

					Assert.IsTrue (eventArgs2.Count == 1);
					Assert.AreSame (person2, eventArgs2[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Updated, eventArgs2[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.Synchronization, eventArgs2[0].EventSource);
				}
			}
		}


		[TestMethod]
		public void SynchronizationEntityCollectionUpdatedEventTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
				using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
				{
					List<EntityChangedEventArgs> eventArgs1 = new List<EntityChangedEventArgs> ();
					List<EntityChangedEventArgs> eventArgs2 = new List<EntityChangedEventArgs> ();

					dataContext1.EntityChanged += (s, a) => eventArgs1.Add (a);
					dataContext2.EntityChanged += (s, a) => eventArgs2.Add (a);

					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					NaturalPersonEntity person2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					AbstractContactEntity contact = dataContext1.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000004)));

					Assert.IsTrue (eventArgs1.Count == 0);
					Assert.IsTrue (eventArgs2.Count == 0);

					person1.Contacts.Add (contact);

					Assert.IsTrue (eventArgs1.Count == 1);
					Assert.AreSame (person1, eventArgs1[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Updated, eventArgs1[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.External, eventArgs1[0].EventSource);

					Assert.IsTrue (eventArgs2.Count == 0);

					dataContext1.SaveChanges ();

					Assert.IsTrue (eventArgs1.Count == 1);

					Assert.IsTrue (eventArgs2.Count == 1);
					Assert.AreSame (person2, eventArgs2[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Updated, eventArgs2[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.Synchronization, eventArgs2[0].EventSource);
				}
			}
		}


		[TestMethod]
		public void SynchronizationEntityDeletedEventTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
				using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
				{
					List<EntityChangedEventArgs> eventArgs1 = new List<EntityChangedEventArgs> ();
					List<EntityChangedEventArgs> eventArgs2 = new List<EntityChangedEventArgs> ();

					dataContext1.EntityChanged += (s, a) => eventArgs1.Add (a);
					dataContext2.EntityChanged += (s, a) => eventArgs2.Add (a);

					LanguageEntity language1 = dataContext1.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001)));
					LanguageEntity language2 = dataContext2.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001)));

					Assert.IsTrue (eventArgs1.Count == 0);
					Assert.IsTrue (eventArgs2.Count == 0);

					dataContext1.DeleteEntity (language1);

					Assert.IsTrue (eventArgs1.Count == 0);
					Assert.IsTrue (eventArgs2.Count == 0);

					dataContext1.SaveChanges ();

					Assert.IsTrue (eventArgs1.Count == 1);
					Assert.AreSame (language1, eventArgs1[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Deleted, eventArgs1[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.External, eventArgs1[0].EventSource);

					Assert.IsTrue (eventArgs2.Count == 1);
					Assert.AreSame (language2, eventArgs2[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Deleted, eventArgs2[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.Synchronization, eventArgs2[0].EventSource);
				}
			}
		}


		[TestMethod]
		public void SynchronizationEntityReferenceDeletedEventTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
				using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
				{
					List<EntityChangedEventArgs> eventArgs1 = new List<EntityChangedEventArgs> ();
					List<EntityChangedEventArgs> eventArgs2 = new List<EntityChangedEventArgs> ();

					dataContext1.EntityChanged += (s, a) => eventArgs1.Add (a);
					dataContext2.EntityChanged += (s, a) => eventArgs2.Add (a);

					LanguageEntity language1 = dataContext1.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001)));
					NaturalPersonEntity person2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					LanguageEntity language2 = person2.PreferredLanguage;

					Assert.IsTrue (eventArgs1.Count == 0);
					Assert.IsTrue (eventArgs2.Count == 0);

					dataContext1.DeleteEntity (language1);

					Assert.IsTrue (eventArgs1.Count == 0);
					Assert.IsTrue (eventArgs2.Count == 0);

					dataContext1.SaveChanges ();

					Assert.IsTrue (eventArgs1.Count == 1);
					Assert.AreSame (language1, eventArgs1[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Deleted, eventArgs1[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.External, eventArgs1[0].EventSource);

					Assert.IsTrue (eventArgs2.Count == 2);
					Assert.AreSame (person2, eventArgs2[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Updated, eventArgs2[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.Synchronization, eventArgs2[0].EventSource);

					Assert.AreSame (language2, eventArgs2[1].Entity);
					Assert.AreEqual (EntityChangedEventType.Deleted, eventArgs2[1].EventType);
					Assert.AreEqual (EntityChangedEventSource.Synchronization, eventArgs2[1].EventSource);
				}
			}
		}


		[TestMethod]
		public void SynchronizationEntityCollectionDeletedEventTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
				using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
				{
					List<EntityChangedEventArgs> eventArgs1 = new List<EntityChangedEventArgs> ();
					List<EntityChangedEventArgs> eventArgs2 = new List<EntityChangedEventArgs> ();

					dataContext1.EntityChanged += (s, a) => eventArgs1.Add (a);
					dataContext2.EntityChanged += (s, a) => eventArgs2.Add (a);

					AbstractContactEntity contact1 = dataContext1.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000001)));
					NaturalPersonEntity person2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					AbstractContactEntity contact2 = person2.Contacts[0];

					Assert.IsTrue (eventArgs1.Count == 0);
					Assert.IsTrue (eventArgs2.Count == 0);

					dataContext1.DeleteEntity (contact1);

					Assert.IsTrue (eventArgs1.Count == 0);
					Assert.IsTrue (eventArgs2.Count == 0);

					dataContext1.SaveChanges ();

					Assert.IsTrue (eventArgs1.Count == 1);
					Assert.AreSame (contact1, eventArgs1[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Deleted, eventArgs1[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.External, eventArgs1[0].EventSource);

					Assert.IsTrue (eventArgs2.Count == 2);
					Assert.AreSame (person2, eventArgs2[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Updated, eventArgs2[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.Synchronization, eventArgs2[0].EventSource);

					Assert.AreSame (contact2, eventArgs2[1].Entity);
					Assert.AreEqual (EntityChangedEventType.Deleted, eventArgs2[1].EventType);
					Assert.AreEqual (EntityChangedEventSource.Synchronization, eventArgs2[1].EventSource);
				}
			}
		}


		[TestMethod]
		public void EntityReloadUpdateEvent()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					alfred.Firstname = "Albert";

					List<EntityChangedEventArgs> eventArgs = new List<EntityChangedEventArgs> ();

					dataContext.EntityChanged += (s, a) => eventArgs.Add (a);

					dataContext.ReloadEntity (alfred);

					Assert.AreEqual (1, eventArgs.Count);
					Assert.AreSame (alfred, eventArgs[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Updated, eventArgs[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.Reload, eventArgs[0].EventSource);
				}
			}
		}


		[TestMethod]
		public void EntityReloadDeleteEvent()
		{
			using (DbInfrastructure dbInfrastructure1 = new DbInfrastructure ())
			using (DbInfrastructure dbInfrastructure2 = new DbInfrastructure ())
			{
				dbInfrastructure1.AttachToDatabase (TestHelper.CreateDbAccess ());
				dbInfrastructure2.AttachToDatabase (TestHelper.CreateDbAccess ());

				using (DataInfrastructure dataInfrastructure1 = new DataInfrastructure (dbInfrastructure1))
				using (DataInfrastructure dataInfrastructure2 = new DataInfrastructure (dbInfrastructure2))
				{
					dataInfrastructure1.OpenConnection ("id");
					dataInfrastructure2.OpenConnection ("id");

					using (DataContext dataContext1 = dataInfrastructure1.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure2.CreateDataContext ())
					{
						NaturalPersonEntity alfred1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
						NaturalPersonEntity alfred2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

						dataContext1.DeleteEntity (alfred1);
						dataContext1.SaveChanges ();

						List<EntityChangedEventArgs> eventArgs = new List<EntityChangedEventArgs> ();

						dataContext2.EntityChanged += (s, a) => eventArgs.Add (a);

						dataContext2.ReloadEntity (alfred2);

						Assert.AreEqual (1, eventArgs.Count);
						Assert.AreSame (alfred2, eventArgs[0].Entity);
						Assert.AreEqual (EntityChangedEventType.Deleted, eventArgs[0].EventType);
						Assert.AreEqual (EntityChangedEventSource.Reload, eventArgs[0].EventSource);
					}
				}
			}
		}


		[TestMethod]
		public void EntityReloadFieldUpdateEvent()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					alfred.Firstname = "Albert";

					List<EntityChangedEventArgs> eventArgs = new List<EntityChangedEventArgs> ();

					dataContext.EntityChanged += (s, a) => eventArgs.Add (a);

					dataContext.ReloadEntityField (alfred, Druid.Parse("[L0AV]"));

					Assert.AreEqual (1, eventArgs.Count);
					Assert.AreSame (alfred, eventArgs[0].Entity);
					Assert.AreEqual (EntityChangedEventType.Updated, eventArgs[0].EventType);
					Assert.AreEqual (EntityChangedEventSource.Reload, eventArgs[0].EventSource);
				}
			}
		}


	}


}
