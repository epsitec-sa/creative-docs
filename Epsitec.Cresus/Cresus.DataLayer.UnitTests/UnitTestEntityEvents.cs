using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public class UnitTestEntityEvents
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
		public static void TestInitialize()
		{
			DatabaseHelper.CreateAndConnectToDatabase ();

			Assert.IsTrue (DatabaseHelper.DbInfrastructure.IsConnectionOpen);

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				DatabaseCreator2.PupulateDatabase (dataContext);
			}
		}
		

		[TestMethod]
		public void ExternalEntityCreatedEventTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				List<EntityEventArgs> eventArgs = new List<EntityEventArgs>();

				dataContext.EntityEvent += (s, a) => eventArgs.Add (a);

				Assert.IsTrue (eventArgs.Count == 0);

				NaturalPersonEntity person = dataContext.CreateEntity<NaturalPersonEntity> ();

				Assert.IsTrue (eventArgs.Count == 1);
				Assert.AreSame (person, eventArgs[0].Entity);
				Assert.AreEqual (EntityEventType.Created, eventArgs[0].EventType);
				Assert.AreEqual (EntityEventSource.External, eventArgs[0].EventSource);
			}
		}


		[TestMethod]
		public void ExternalEntityUpdatedValueEventTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				List<EntityEventArgs> eventArgs = new List<EntityEventArgs> ();

				dataContext.EntityEvent += (s, a) => eventArgs.Add (a);

				LanguageEntity language = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1)));

				Assert.IsTrue (eventArgs.Count == 0);

				language.Name = "new name";

				Assert.IsTrue (eventArgs.Count == 1);
				Assert.AreSame (language, eventArgs[0].Entity);
				Assert.AreEqual (EntityEventType.Updated, eventArgs[0].EventType);
				Assert.AreEqual (EntityEventSource.External, eventArgs[0].EventSource);
			}
		}


		[TestMethod]
		public void ExternalEntityUpdatedReferenceEventTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				List<EntityEventArgs> eventArgs = new List<EntityEventArgs> ();

				dataContext.EntityEvent += (s, a) => eventArgs.Add (a);

				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				PersonGenderEntity gender = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (2)));
				
				Assert.IsTrue (eventArgs.Count == 0);

				person.Gender = gender;

				Assert.IsTrue (eventArgs.Count == 1);
				Assert.AreSame (person, eventArgs[0].Entity);
				Assert.AreEqual (EntityEventType.Updated, eventArgs[0].EventType);
				Assert.AreEqual (EntityEventSource.External, eventArgs[0].EventSource);
			}
		}


		[TestMethod]
		public void ExternalEntityUpdatedCollectionEventTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				List<EntityEventArgs> eventArgs = new List<EntityEventArgs> ();

				dataContext.EntityEvent += (s, a) =>
				{
					eventArgs.Add (a);
				};

				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (4)));

				Assert.IsTrue (eventArgs.Count == 0);

				person.Contacts.Add (contact);

				Assert.IsTrue (eventArgs.Count == 1);
				Assert.AreSame (person, eventArgs[0].Entity);
				Assert.AreEqual (EntityEventType.Updated, eventArgs[0].EventType);
				Assert.AreEqual (EntityEventSource.External, eventArgs[0].EventSource);
			}
		}


		[TestMethod]
		public void ExternalEntityDeletedEventTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				List<EntityEventArgs> eventArgs = new List<EntityEventArgs> ();

				dataContext.EntityEvent += (s, a) => eventArgs.Add (a);

				LanguageEntity language = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1)));

				Assert.IsTrue (eventArgs.Count == 0);

				dataContext.DeleteEntity (language);

				Assert.IsTrue (eventArgs.Count == 0);

				dataContext.SaveChanges ();

				Assert.IsTrue (eventArgs.Count == 1);
				Assert.AreSame (language, eventArgs[0].Entity);
				Assert.AreEqual (EntityEventType.Deleted, eventArgs[0].EventType);
				Assert.AreEqual (EntityEventSource.External, eventArgs[0].EventSource);
			}
		}


		[TestMethod]
		public void ExternalEntityReferenceDeletedEventTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				List<EntityEventArgs> eventArgs = new List<EntityEventArgs> ();

				dataContext.EntityEvent += (s, a) => eventArgs.Add (a);

				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				LanguageEntity language = person.PreferredLanguage;

				Assert.IsTrue (eventArgs.Count == 0);

				dataContext.DeleteEntity (language);

				Assert.IsTrue (eventArgs.Count == 0);

				dataContext.SaveChanges ();

				Assert.IsTrue (eventArgs.Count == 2);
				Assert.AreSame (person, eventArgs[0].Entity);
				Assert.AreEqual (EntityEventType.Updated, eventArgs[0].EventType);
				Assert.AreEqual (EntityEventSource.Internal, eventArgs[0].EventSource);

				Assert.AreSame (language, eventArgs[1].Entity);
				Assert.AreEqual (EntityEventType.Deleted, eventArgs[1].EventType);
				Assert.AreEqual (EntityEventSource.External, eventArgs[1].EventSource);

			}
		}


		[TestMethod]
		public void ExternalEntityCollectionDeletedEventTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				List<EntityEventArgs> eventArgs = new List<EntityEventArgs> ();

				dataContext.EntityEvent += (s, a) =>
				{
					eventArgs.Add (a);
				};

				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				AbstractContactEntity contact = person.Contacts[0];

				Assert.IsTrue (eventArgs.Count == 0);

				dataContext.DeleteEntity (contact);

				Assert.IsTrue (eventArgs.Count == 0);

				dataContext.SaveChanges ();

				Assert.IsTrue (eventArgs.Count == 2);
				Assert.AreSame (person, eventArgs[0].Entity);
				Assert.AreEqual (EntityEventType.Updated, eventArgs[0].EventType);
				Assert.AreEqual (EntityEventSource.Internal, eventArgs[0].EventSource);

				Assert.AreSame (contact, eventArgs[1].Entity);
				Assert.AreEqual (EntityEventType.Deleted, eventArgs[1].EventType);
				Assert.AreEqual (EntityEventSource.External, eventArgs[1].EventSource);
			}
		}


		[TestMethod]
		public void SynchronizationEntityValueUpdatedEventTest()
		{
			using (DataContext dataContext1 = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext2 = new DataContext (DatabaseHelper.DbInfrastructure))
				{
					DataContextPool.Instance.Add (dataContext1);
					DataContextPool.Instance.Add (dataContext2);

					try
					{
						List<EntityEventArgs> eventArgs1 = new List<EntityEventArgs> ();
						List<EntityEventArgs> eventArgs2 = new List<EntityEventArgs> ();

						dataContext1.EntityEvent += (s, a) => eventArgs1.Add (a);
						dataContext2.EntityEvent += (s, a) => eventArgs2.Add (a);

						LanguageEntity language1 = dataContext1.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1)));
						LanguageEntity language2 = dataContext2.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1)));

						Assert.IsTrue (eventArgs1.Count == 0);
						Assert.IsTrue (eventArgs2.Count == 0);

						language1.Name = "new name";

						Assert.IsTrue (eventArgs1.Count == 1);
						Assert.AreSame (language1, eventArgs1[0].Entity);
						Assert.AreEqual (EntityEventType.Updated, eventArgs1[0].EventType);
						Assert.AreEqual (EntityEventSource.External, eventArgs1[0].EventSource);

						Assert.IsTrue (eventArgs2.Count == 0);

						dataContext1.SaveChanges ();

						Assert.IsTrue (eventArgs1.Count == 1);

						Assert.IsTrue (eventArgs2.Count == 1);
						Assert.AreSame (language2, eventArgs2[0].Entity);
						Assert.AreEqual (EntityEventType.Updated, eventArgs2[0].EventType);
						Assert.AreEqual (EntityEventSource.Synchronization, eventArgs2[0].EventSource);
					}
					finally
					{
						DataContextPool.Instance.Remove (dataContext1);
						DataContextPool.Instance.Remove (dataContext2);
					}
				}
			}
		}


		[TestMethod]
		public void SynchronizationEntityReferenceUpdatedEventTest()
		{
			using (DataContext dataContext1 = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext2 = new DataContext (DatabaseHelper.DbInfrastructure))
				{
					DataContextPool.Instance.Add (dataContext1);
					DataContextPool.Instance.Add (dataContext2);

					try
					{
						List<EntityEventArgs> eventArgs1 = new List<EntityEventArgs> ();
						List<EntityEventArgs> eventArgs2 = new List<EntityEventArgs> ();

						dataContext1.EntityEvent += (s, a) => eventArgs1.Add (a);
						dataContext2.EntityEvent += (s, a) => eventArgs2.Add (a);

						NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
						NaturalPersonEntity person2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
						PersonGenderEntity gender = dataContext1.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (2)));

						Assert.IsTrue (eventArgs1.Count == 0);
						Assert.IsTrue (eventArgs2.Count == 0);

						person1.Gender = gender;

						Assert.IsTrue (eventArgs1.Count == 1);
						Assert.AreSame (person1, eventArgs1[0].Entity);
						Assert.AreEqual (EntityEventType.Updated, eventArgs1[0].EventType);
						Assert.AreEqual (EntityEventSource.External, eventArgs1[0].EventSource);

						Assert.IsTrue (eventArgs2.Count == 0);

						dataContext1.SaveChanges ();

						Assert.IsTrue (eventArgs1.Count == 1);

						Assert.IsTrue (eventArgs2.Count == 1);
						Assert.AreSame (person2, eventArgs2[0].Entity);
						Assert.AreEqual (EntityEventType.Updated, eventArgs2[0].EventType);
						Assert.AreEqual (EntityEventSource.Synchronization, eventArgs2[0].EventSource);
					}
					finally
					{
						DataContextPool.Instance.Remove (dataContext1);
						DataContextPool.Instance.Remove (dataContext2);
					}
				}
			}
		}


		[TestMethod]
		public void SynchronizationEntityCollectionUpdatedEventTest()
		{
			using (DataContext dataContext1 = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext2 = new DataContext (DatabaseHelper.DbInfrastructure))
				{
					DataContextPool.Instance.Add (dataContext1);
					DataContextPool.Instance.Add (dataContext2);

					try
					{
						List<EntityEventArgs> eventArgs1 = new List<EntityEventArgs> ();
						List<EntityEventArgs> eventArgs2 = new List<EntityEventArgs> ();

						dataContext1.EntityEvent += (s, a) => eventArgs1.Add (a);
						dataContext2.EntityEvent += (s, a) => eventArgs2.Add (a);

						NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
						NaturalPersonEntity person2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
						AbstractContactEntity contact = dataContext1.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (4)));

						Assert.IsTrue (eventArgs1.Count == 0);
						Assert.IsTrue (eventArgs2.Count == 0);

						person1.Contacts.Add (contact);

						Assert.IsTrue (eventArgs1.Count == 1);
						Assert.AreSame (person1, eventArgs1[0].Entity);
						Assert.AreEqual (EntityEventType.Updated, eventArgs1[0].EventType);
						Assert.AreEqual (EntityEventSource.External, eventArgs1[0].EventSource);

						Assert.IsTrue (eventArgs2.Count == 0);

						dataContext1.SaveChanges ();

						Assert.IsTrue (eventArgs1.Count == 1);

						Assert.IsTrue (eventArgs2.Count == 1);
						Assert.AreSame (person2, eventArgs2[0].Entity);
						Assert.AreEqual (EntityEventType.Updated, eventArgs2[0].EventType);
						Assert.AreEqual (EntityEventSource.Synchronization, eventArgs2[0].EventSource);
					}
					finally
					{
						DataContextPool.Instance.Remove (dataContext1);
						DataContextPool.Instance.Remove (dataContext2);	
					}				
				}
			}
		}


		[TestMethod]
		public void SynchronizationEntityDeletedEventTest()
		{
			using (DataContext dataContext1 = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext2 = new DataContext (DatabaseHelper.DbInfrastructure))
				{
					DataContextPool.Instance.Add (dataContext1);
					DataContextPool.Instance.Add (dataContext2);

					try
					{
						List<EntityEventArgs> eventArgs1 = new List<EntityEventArgs> ();
						List<EntityEventArgs> eventArgs2 = new List<EntityEventArgs> ();

						dataContext1.EntityEvent += (s, a) => eventArgs1.Add (a);
						dataContext2.EntityEvent += (s, a) => eventArgs2.Add (a);

						LanguageEntity language1 = dataContext1.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1)));
						LanguageEntity language2 = dataContext2.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1)));

						Assert.IsTrue (eventArgs1.Count == 0);
						Assert.IsTrue (eventArgs2.Count == 0);

						dataContext1.DeleteEntity (language1);

						Assert.IsTrue (eventArgs1.Count == 0);
						Assert.IsTrue (eventArgs2.Count == 0);

						dataContext1.SaveChanges ();

						Assert.IsTrue (eventArgs1.Count == 1);
						Assert.AreSame (language1, eventArgs1[0].Entity);
						Assert.AreEqual (EntityEventType.Deleted, eventArgs1[0].EventType);
						Assert.AreEqual (EntityEventSource.External, eventArgs1[0].EventSource);

						Assert.IsTrue (eventArgs2.Count == 1);
						Assert.AreSame (language2, eventArgs2[0].Entity);
						Assert.AreEqual (EntityEventType.Deleted, eventArgs2[0].EventType);
						Assert.AreEqual (EntityEventSource.Synchronization, eventArgs2[0].EventSource);
					}
					finally
					{
						DataContextPool.Instance.Remove (dataContext1);
						DataContextPool.Instance.Remove (dataContext2);
					}
				}
			}
		}


		[TestMethod]
		public void SynchronizationEntityReferenceDeletedEventTest()
		{
			using (DataContext dataContext1 = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext2 = new DataContext (DatabaseHelper.DbInfrastructure))
				{
					DataContextPool.Instance.Add (dataContext1);
					DataContextPool.Instance.Add (dataContext2);

					try
					{
						List<EntityEventArgs> eventArgs1 = new List<EntityEventArgs> ();
						List<EntityEventArgs> eventArgs2 = new List<EntityEventArgs> ();

						dataContext1.EntityEvent += (s, a) => eventArgs1.Add (a);
						dataContext2.EntityEvent += (s, a) => eventArgs2.Add (a);

						LanguageEntity language1 = dataContext1.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1)));
						NaturalPersonEntity person2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
						LanguageEntity language2 = person2.PreferredLanguage;

						Assert.IsTrue (eventArgs1.Count == 0);
						Assert.IsTrue (eventArgs2.Count == 0);

						dataContext1.DeleteEntity (language1);

						Assert.IsTrue (eventArgs1.Count == 0);
						Assert.IsTrue (eventArgs2.Count == 0);

						dataContext1.SaveChanges ();

						Assert.IsTrue (eventArgs1.Count == 1);
						Assert.AreSame (language1, eventArgs1[0].Entity);
						Assert.AreEqual (EntityEventType.Deleted, eventArgs1[0].EventType);
						Assert.AreEqual (EntityEventSource.External, eventArgs1[0].EventSource);

						Assert.IsTrue (eventArgs2.Count == 2);
						Assert.AreSame (person2, eventArgs2[0].Entity);
						Assert.AreEqual (EntityEventType.Updated, eventArgs2[0].EventType);
						Assert.AreEqual (EntityEventSource.Synchronization, eventArgs2[0].EventSource);

						Assert.AreSame (language2, eventArgs2[1].Entity);
						Assert.AreEqual (EntityEventType.Deleted, eventArgs2[1].EventType);
						Assert.AreEqual (EntityEventSource.Synchronization, eventArgs2[1].EventSource);
					}
					finally
					{
						DataContextPool.Instance.Remove (dataContext1);
						DataContextPool.Instance.Remove (dataContext2);	
					}
				}
			}
		}


		[TestMethod]
		public void SynchronizationEntityCollectionDeletedEventTest()
		{
			using (DataContext dataContext1 = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext2 = new DataContext (DatabaseHelper.DbInfrastructure))
				{
					DataContextPool.Instance.Add (dataContext1);
					DataContextPool.Instance.Add (dataContext2);

					try
					{
						List<EntityEventArgs> eventArgs1 = new List<EntityEventArgs> ();
						List<EntityEventArgs> eventArgs2 = new List<EntityEventArgs> ();

						dataContext1.EntityEvent += (s, a) => eventArgs1.Add (a);
						dataContext2.EntityEvent += (s, a) => eventArgs2.Add (a);

						AbstractContactEntity contact1 = dataContext1.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1)));
						NaturalPersonEntity person2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
						AbstractContactEntity contact2 = person2.Contacts[0];

						Assert.IsTrue (eventArgs1.Count == 0);
						Assert.IsTrue (eventArgs2.Count == 0);

						dataContext1.DeleteEntity (contact1);

						Assert.IsTrue (eventArgs1.Count == 0);
						Assert.IsTrue (eventArgs2.Count == 0);

						dataContext1.SaveChanges ();

						Assert.IsTrue (eventArgs1.Count == 1);
						Assert.AreSame (contact1, eventArgs1[0].Entity);
						Assert.AreEqual (EntityEventType.Deleted, eventArgs1[0].EventType);
						Assert.AreEqual (EntityEventSource.External, eventArgs1[0].EventSource);

						Assert.IsTrue (eventArgs2.Count == 2);
						Assert.AreSame (person2, eventArgs2[0].Entity);
						Assert.AreEqual (EntityEventType.Updated, eventArgs2[0].EventType);
						Assert.AreEqual (EntityEventSource.Synchronization, eventArgs2[0].EventSource);

						Assert.AreSame (contact2, eventArgs2[1].Entity);
						Assert.AreEqual (EntityEventType.Deleted, eventArgs2[1].EventType);
						Assert.AreEqual (EntityEventSource.Synchronization, eventArgs2[1].EventSource);
					}
					finally
					{
						DataContextPool.Instance.Remove (dataContext1);
						DataContextPool.Instance.Remove (dataContext2);
					}	
				}
			}
		}


	}


}
