using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Saver;
using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestPersistenceJobProcessor
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

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				DatabaseCreator2.PupulateDatabase (dataContext);
			}
		}


		[TestMethod]
		public void PersistenceJobProcessorConstructorTest1()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				new PersistenceJobProcessor (dataContext);
			}
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void PersistenceJobProcessorConstructorTest2()
		{
			new PersistenceJobProcessor (null);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void ProcessJobsTest1()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);
				
				using (DbTransaction transaction = DatabaseHelper.DbInfrastructure.BeginTransaction())
				{
					processor.ProcessJobs (transaction, null);
				}
			}
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void ProcessJobsTest2()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);

				processor.ProcessJobs (null, new List<AbstractPersistenceJob> ());
			}
		}


		[TestMethod]
		public void ProcessJobsDeleteTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				List<AbstractPersistenceJob> jobs = new List<AbstractPersistenceJob> ()
				{
					new DeletePersistenceJob (entity),
				};

				Dictionary<AbstractEntity, DbKey> newKeys;
				
				using (DbTransaction transaction = DatabaseHelper.DbInfrastructure.BeginTransaction ())
				{
					newKeys = processor.ProcessJobs (transaction, jobs).ToDictionary (p => p.Key, p => p.Value);

					transaction.Commit ();
				}
				
				Assert.IsTrue (newKeys.Count == 0);
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				Assert.IsNull (entity);
			}
		}


		[TestMethod]
		public void ProcessJobsInsertValueTest()
		{
			DbKey newKey;
			
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);

				CountryEntity entity = new CountryEntity ();

				List<AbstractPersistenceJob> jobs = new List<AbstractPersistenceJob> ()
				{
					new ValuePersistenceJob
					(
						entity,
						Druid.Parse("[L0A1]"),
						new Dictionary<Druid,object>()
						{
							{ Druid.Parse ("[L0AD3]"), "code"},
							{ Druid.Parse ("[L0A3]"), "name"},
						},
						true,
						PersistenceJobType.Insert
					)
				};

				Dictionary<AbstractEntity, DbKey> newKeys;

				using (DbTransaction transaction = DatabaseHelper.DbInfrastructure.BeginTransaction ())
				{
					newKeys = processor.ProcessJobs (transaction, jobs).ToDictionary (p => p.Key, p => p.Value);

					transaction.Commit ();
				}

				Assert.IsTrue (newKeys.Count == 1);
				Assert.AreEqual (entity, newKeys.First ().Key);
				newKey = newKeys.First ().Value;
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				CountryEntity entity = dataContext.ResolveEntity<CountryEntity> (newKey);

				Assert.IsNotNull (entity);
				Assert.AreEqual ("code", entity.Code);
				Assert.AreEqual ("name", entity.Name);
			}
		}


		[TestMethod]
		public void ProcessJobsInsertReferenceTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				PersonTitleEntity target = dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1)));

				List<AbstractPersistenceJob> jobs = new List<AbstractPersistenceJob> ()
				{
					new ReferencePersistenceJob
					(
						entity,
						Druid.Parse("[L0AN]"),
						Druid.Parse("[L0AU]"),
						target,
						PersistenceJobType.Insert
					)
				};

				Dictionary<AbstractEntity, DbKey> newKeys;

				using (DbTransaction transaction = DatabaseHelper.DbInfrastructure.BeginTransaction ())
				{
					newKeys = processor.ProcessJobs (transaction, jobs).ToDictionary (p => p.Key, p => p.Value);

					transaction.Commit ();
				}

				Assert.IsTrue (newKeys.Count == 0);
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				PersonTitleEntity target = dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1)));

				Assert.IsNotNull (entity);
				Assert.IsNotNull (entity.Title);
				Assert.AreSame (target, entity.Title);
			}
		}


		[TestMethod]
		public void ProcessJobsInsertCollectionTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3)));
				List<AbstractContactEntity> targets = new List<AbstractContactEntity> ()
				{
					dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1))),
					dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (2))),
				};

				List<AbstractPersistenceJob> jobs = new List<AbstractPersistenceJob> ()
				{
					new CollectionPersistenceJob
					(
						entity,
						Druid.Parse("[L0AM]"),
						Druid.Parse("[L0AS]"),
						targets,
						PersistenceJobType.Insert
					)
				};

				Dictionary<AbstractEntity, DbKey> newKeys;

				using (DbTransaction transaction = DatabaseHelper.DbInfrastructure.BeginTransaction ())
				{
					newKeys = processor.ProcessJobs (transaction, jobs).ToDictionary (p => p.Key, p => p.Value);

					transaction.Commit ();
				}

				Assert.IsTrue (newKeys.Count == 0);
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3)));
				List<AbstractContactEntity> targets = new List<AbstractContactEntity> ()
				{
					dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1))),
					dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (2))),
				};

				Assert.IsNotNull (entity);
				Assert.IsTrue (entity.Contacts.Count == 2);
				Assert.AreSame (targets[0], entity.Contacts[0]);
				Assert.AreSame (targets[1], entity.Contacts[1]);
			}
		}


		[TestMethod]
		public void ProcessJobsUpdateValueTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				List<AbstractPersistenceJob> jobs = new List<AbstractPersistenceJob> ()
				{
					new ValuePersistenceJob
					(
						entity,
						Druid.Parse("[L0AN]"),
						new Dictionary<Druid,object>()
						{
							{ Druid.Parse ("[L0AV]"), "firstname"},
							{ Druid.Parse ("[L0A01]"), "lastname"},
							{ Druid.Parse ("[L0A61]"), null },
						},
						false,
						PersistenceJobType.Update
					)
				};

				Dictionary<AbstractEntity, DbKey> newKeys;

				using (DbTransaction transaction = DatabaseHelper.DbInfrastructure.BeginTransaction ())
				{
					newKeys = processor.ProcessJobs (transaction, jobs).ToDictionary (p => p.Key, p => p.Value);

					transaction.Commit ();
				}

				Assert.IsTrue (newKeys.Count == 0);
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				Assert.IsNotNull (entity);
				Assert.AreEqual ("firstname", entity.Firstname);
				Assert.AreEqual ("lastname", entity.Lastname);
				Assert.IsNull (entity.BirthDate);
			}
		}


		[TestMethod]
		public void ProcessJobsUpdateReferenceTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				PersonTitleEntity target = dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1)));

				List<AbstractPersistenceJob> jobs = new List<AbstractPersistenceJob> ()
				{
					new ReferencePersistenceJob
					(
						entity,
						Druid.Parse("[L0AN]"),
						Druid.Parse("[L0AU]"),
						target,
						PersistenceJobType.Update
					),
					new ReferencePersistenceJob
					(
						entity,
						Druid.Parse("[L0AM]"),
						Druid.Parse("[L0AD1]"),
						null,
						PersistenceJobType.Update
					)
				};

				Dictionary<AbstractEntity, DbKey> newKeys;

				using (DbTransaction transaction = DatabaseHelper.DbInfrastructure.BeginTransaction ())
				{
					newKeys = processor.ProcessJobs (transaction, jobs).ToDictionary (p => p.Key, p => p.Value);

					transaction.Commit ();
				}

				Assert.IsTrue (newKeys.Count == 0);
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				PersonTitleEntity target = dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1)));

				Assert.IsNotNull (entity);
				Assert.IsNotNull (entity.Title);
				Assert.AreSame (target, entity.Title);
				Assert.IsNull (entity.PreferredLanguage);
			}
		}


		[TestMethod]
		public void ProcessJobsUpdateCollectionTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				List<AbstractContactEntity> targets = new List<AbstractContactEntity> ()
				{
					dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (2))),
					dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (3))),
				};

				List<AbstractPersistenceJob> jobs = new List<AbstractPersistenceJob> ()
				{
					new CollectionPersistenceJob
					(
						entity,
						Druid.Parse("[L0AM]"),
						Druid.Parse("[L0AS]"),
						targets,
						PersistenceJobType.Update
					),
				};

				Dictionary<AbstractEntity, DbKey> newKeys;

				using (DbTransaction transaction = DatabaseHelper.DbInfrastructure.BeginTransaction ())
				{
					newKeys = processor.ProcessJobs (transaction, jobs).ToDictionary (p => p.Key, p => p.Value);

					transaction.Commit ();
				}

				Assert.IsTrue (newKeys.Count == 0);
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				List<AbstractContactEntity> targets = new List<AbstractContactEntity> ()
				{
					dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (2))),
					dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (3))),
				};

				Assert.IsNotNull (entity);
				Assert.IsNotNull (entity.Contacts.Count == 2);
				Assert.AreSame (targets[0], entity.Contacts[0]);
				Assert.AreSame (targets[1], entity.Contacts[1]);
			}
		}


		[TestMethod]
		public void ProcessJobsBigTest()
		{
			DbKey key1;
			DbKey key2;
			DbKey key3;

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);

				NaturalPersonEntity person1 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				NaturalPersonEntity person2 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2)));
				NaturalPersonEntity person3 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3)));
				NaturalPersonEntity person4 = dataContext.CreateEntity<NaturalPersonEntity> ();

				PersonGenderEntity gender1 = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (2)));
				PersonGenderEntity gender2 = dataContext.CreateEntity<PersonGenderEntity> ();

				AbstractContactEntity contact1 = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1)));
				AbstractContactEntity contact2 = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (2)));
				UriContactEntity contact3 = dataContext.CreateEntity<UriContactEntity> ();

				List<AbstractPersistenceJob> jobs = new List<AbstractPersistenceJob> ()
				{
					new CollectionPersistenceJob
					(
					    person4,
					    Druid.Parse ("[L0AM]"),
					    Druid.Parse ("[L0AS]"),
					    new List<AbstractContactEntity> ()
					    {
					        contact2,
					        contact3,
					    },
					    PersistenceJobType.Insert
					),
					new ReferencePersistenceJob
					(
					    person2,
					    Druid.Parse("[L0AN]"),
					    Druid.Parse("[L0A11]"),
					    null,
					    PersistenceJobType.Update
					),
					new ValuePersistenceJob
					(
					    person4,
					    Druid.Parse ("[L0AN]"),
					    new Dictionary<Druid,object> ()
					    {
					        { Druid.Parse("[L0AV]"), "fn" },
					        { Druid.Parse("[L0A01]"), "ln" },
					        { Druid.Parse("[L0A61]"), null },
					    },
					    false,
					    PersistenceJobType.Insert
					),
					new ReferencePersistenceJob
					(
					    person4,
					    Druid.Parse ("[L0AN]"),
					    Druid.Parse ("[L0A11]"),
					    gender1,
					    PersistenceJobType.Insert
					),
					new DeletePersistenceJob
					(
					    person3
					),
					new ValuePersistenceJob
					(
					    contact3,
					    Druid.Parse("[L0A52]"),
					    new Dictionary<Druid,object> ()
					    {
					        { Druid.Parse("[L0AA2]"), "uri" },
					    },
					    false,
					    PersistenceJobType.Insert
					),
					new CollectionPersistenceJob
					(
						person1,
						Druid.Parse("[L0AM]"),
						Druid.Parse("[L0AS]"),
						new List<AbstractContactEntity> (),
						PersistenceJobType.Update
					),
					new ValuePersistenceJob
					(
					    contact1,
					    Druid.Parse("[L0A52]"),
					    new Dictionary<Druid,object> ()
					    {
					        { Druid.Parse("[L0AA2]"), "uri" },
					    },
					    false,
					    PersistenceJobType.Update
					),
					new ValuePersistenceJob
					(
					    contact3,
					    Druid.Parse("[L0AP]"),
					    new Dictionary<Druid,object> (),
					    true,
					    PersistenceJobType.Insert
					),
					new ValuePersistenceJob
					(
					    person4,
					    Druid.Parse ("[L0AM]"),
					    new Dictionary<Druid,object> (),
					    true,
					    PersistenceJobType.Insert
					),
					new ValuePersistenceJob
					(
					    gender2,
					    Druid.Parse("[L0AA1]"),
					    new Dictionary<Druid,object> ()
					    {
					        { Druid.Parse("[L0A03]"), null },
					        { Druid.Parse("[L0AD3]"), "code" },
					        { Druid.Parse("[L0AC1]"), "name" },
					    },
					    true,
					    PersistenceJobType.Insert
					),
				};

				Dictionary<AbstractEntity, DbKey> newKeys;

				using (DbTransaction transaction = DatabaseHelper.DbInfrastructure.BeginTransaction ())
				{
					newKeys = processor.ProcessJobs (transaction, jobs).ToDictionary (p => p.Key, p => p.Value);

					transaction.Commit ();
				}

				Assert.IsTrue (newKeys.Count == 3);

				key1 = newKeys[person4];
				key2 = newKeys[gender2];
				key3 = newKeys[contact3];
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person1 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				NaturalPersonEntity person2 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2)));
				NaturalPersonEntity person3 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3)));
				NaturalPersonEntity person4 = dataContext.ResolveEntity<NaturalPersonEntity> (key1);

				PersonGenderEntity gender1 = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (2)));
				PersonGenderEntity gender2 = dataContext.ResolveEntity<PersonGenderEntity> (key2);

				UriContactEntity contact1 = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1)));
				UriContactEntity contact2 = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (2)));
				UriContactEntity contact3 = dataContext.ResolveEntity<UriContactEntity> (key3);

				Assert.IsNotNull (person1);
				Assert.IsTrue (person1.Contacts.Count == 0);

				Assert.IsNotNull (person2);
				Assert.IsNull (person2.Gender);

				Assert.IsNull (person3);

				Assert.IsNotNull (person4);
				Assert.AreEqual ("fn", person4.Firstname);
				Assert.AreEqual ("ln", person4.Lastname);
				Assert.IsNull (person4.BirthDate);
				Assert.IsTrue (person4.Contacts.Count == 2);
				Assert.AreSame (contact2, person4.Contacts[0]);
				Assert.AreSame (contact3, person4.Contacts[1]);
				Assert.AreSame (gender1, person4.Gender);

				Assert.IsNotNull (gender1);

				Assert.IsNotNull (gender2);
				Assert.AreEqual ("name", gender2.Name);
				Assert.AreEqual ("code", gender2.Code);
				Assert.IsNull (gender2.Rank);

				Assert.IsNotNull (contact1);
				Assert.AreEqual ("uri", contact1.Uri);

				Assert.IsNotNull (contact2);
				
				Assert.IsNotNull (contact3);
				Assert.AreEqual ("uri", contact3.Uri);
			}
		}


	}


}
