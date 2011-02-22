using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Services;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Saver;
using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.Saver
{


	[TestClass]
	public sealed class UnitTestPersistenceJobProcessor
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void PersistenceJobProcessorConstructorTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				new PersistenceJobProcessor (dataContext);
			}
		}


		[TestMethod]
		public void PersistenceJobProcessorConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new PersistenceJobProcessor (null)
			);
		}


		[TestMethod]
		public void ProcessJobsArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);

				using (DbTransaction transaction = dbInfrastructure.BeginTransaction ())
				{
					DbLogEntry dbLogEntry = dbInfrastructure.ServiceManager.Logger.CreateLogEntry (new DbId (0));

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => processor.ProcessJobs (transaction, dbLogEntry, null)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => processor.ProcessJobs (null, dbLogEntry, new List<AbstractPersistenceJob> ())
					);

					transaction.Commit ();
				}
			}
		}


		[TestMethod]
		public void ProcessJobsDeleteTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);

					NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					List<AbstractPersistenceJob> jobs = new List<AbstractPersistenceJob> ()
					{
						new DeletePersistenceJob (entity),
					};

					Dictionary<AbstractEntity, DbKey> newKeys;

					using (DbTransaction transaction = dbInfrastructure.BeginTransaction ())
					{
						DbLogEntry dbLogEntry = dbInfrastructure.ServiceManager.Logger.CreateLogEntry (new DbId (0));

						newKeys = processor.ProcessJobs (transaction, dbLogEntry, jobs).ToDictionary (p => p.Key, p => p.Value);

						transaction.Commit ();
					}

					Assert.IsTrue (newKeys.Count == 0);
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					Assert.IsNull (entity);
				}

				DbEntityDeletionLogger logger = dbInfrastructure.ServiceManager.EntityDeletionLogger;

				DbEntityDeletionLogEntry entry = logger.GetEntityDeletionLogEntries (new DbId (0))
					.Where (e => e.InstanceType == EntityInfo<NaturalPersonEntity>.GetTypeId ().ToLong () && e.EntityId == 1000000001).FirstOrDefault ();

				Assert.IsNotNull (entry);
			}
		}


		[TestMethod]
		public void ProcessJobsInsertValueTest()
		{
			DbKey newKey;

			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);

					CountryEntity entity = new CountryEntity ();

					List<AbstractPersistenceJob> jobs = new List<AbstractPersistenceJob> ()
					{
						new ValuePersistenceJob
						(
							entity,
							Druid.Parse("[J1A4]"),
							new Dictionary<Druid,object>()
							{
								{ Druid.Parse ("[J1A3]"), "code"},
								{ Druid.Parse ("[J1A5]"), "name"},
							},
							true,
							PersistenceJobType.Insert
						)
					};

					Dictionary<AbstractEntity, DbKey> newKeys;

					using (DbTransaction transaction = dbInfrastructure.BeginTransaction ())
					{
						DbLogEntry dbLogEntry = dbInfrastructure.ServiceManager.Logger.CreateLogEntry (new DbId (0));

						newKeys = processor.ProcessJobs (transaction, dbLogEntry, jobs).ToDictionary (p => p.Key, p => p.Value);

						transaction.Commit ();
					}

					Assert.IsTrue (newKeys.Count == 1);
					Assert.AreEqual (entity, newKeys.First ().Key);
					newKey = newKeys.First ().Value;
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					CountryEntity entity = dataContext.ResolveEntity<CountryEntity> (newKey);

					Assert.IsNotNull (entity);
					Assert.AreEqual ("code", entity.Code);
					Assert.AreEqual ("name", entity.Name);
				}
			}
		}


		[TestMethod]
		public void ProcessJobsInsertReferenceTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);

					NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					PersonTitleEntity target = dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1000000001)));

					List<AbstractPersistenceJob> jobs = new List<AbstractPersistenceJob> ()
					{
						new ReferencePersistenceJob
						(
							entity,
							Druid.Parse("[J1AJ1]"),
							Druid.Parse("[J1AK1]"),
							target,
							PersistenceJobType.Insert
						)
					};

					Dictionary<AbstractEntity, DbKey> newKeys;

					using (DbTransaction transaction = dbInfrastructure.BeginTransaction ())
					{
						DbLogEntry dbLogEntry = dbInfrastructure.ServiceManager.Logger.CreateLogEntry (new DbId (0));

						newKeys = processor.ProcessJobs (transaction, dbLogEntry, jobs).ToDictionary (p => p.Key, p => p.Value);

						transaction.Commit ();
					}

					Assert.IsTrue (newKeys.Count == 0);
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					PersonTitleEntity target = dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1000000001)));

					Assert.IsNotNull (entity);
					Assert.IsNotNull (entity.Title);
					Assert.AreSame (target, entity.Title);
				}
			}
		}


		[TestMethod]
		public void ProcessJobsInsertCollectionTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);

					NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000003)));
					List<AbstractContactEntity> targets = new List<AbstractContactEntity> ()
					{
						dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000001))),
						dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000002))),
					};

					List<AbstractPersistenceJob> jobs = new List<AbstractPersistenceJob> ()
					{
						new CollectionPersistenceJob
						(
							entity,
							Druid.Parse("[J1AB1]"),
							Druid.Parse("[J1AC1]"),
							targets,
							PersistenceJobType.Insert
						)
					};

					Dictionary<AbstractEntity, DbKey> newKeys;

					using (DbTransaction transaction = dbInfrastructure.BeginTransaction ())
					{
						DbLogEntry dbLogEntry = dbInfrastructure.ServiceManager.Logger.CreateLogEntry (new DbId (0));

						newKeys = processor.ProcessJobs (transaction, dbLogEntry, jobs).ToDictionary (p => p.Key, p => p.Value);

						transaction.Commit ();
					}

					Assert.IsTrue (newKeys.Count == 0);
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000003)));
					List<AbstractContactEntity> targets = new List<AbstractContactEntity> ()
					{
						dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000001))),
						dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000002))),
					};

					Assert.IsNotNull (entity);
					Assert.IsTrue (entity.Contacts.Count == 2);
					Assert.AreSame (targets[0], entity.Contacts[0]);
					Assert.AreSame (targets[1], entity.Contacts[1]);
				}
			}
		}


		[TestMethod]
		public void ProcessJobsUpdateValueTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);

					NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					List<AbstractPersistenceJob> jobs = new List<AbstractPersistenceJob> ()
					{
						new ValuePersistenceJob
						(
							entity,
							Druid.Parse("[J1AJ1]"),
							new Dictionary<Druid,object>()
							{
								{ Druid.Parse ("[J1AL1]"), "firstname"},
								{ Druid.Parse ("[J1AM1]"), "lastname"},
								{ Druid.Parse ("[J1AO1]"), null },
							},
							false,
							PersistenceJobType.Update
						)
					};

					Dictionary<AbstractEntity, DbKey> newKeys;

					using (DbTransaction transaction = dbInfrastructure.BeginTransaction ())
					{
						DbLogEntry dbLogEntry = dbInfrastructure.ServiceManager.Logger.CreateLogEntry (new DbId (0));

						newKeys = processor.ProcessJobs (transaction, dbLogEntry, jobs).ToDictionary (p => p.Key, p => p.Value);

						transaction.Commit ();
					}

					Assert.IsTrue (newKeys.Count == 0);
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					Assert.IsNotNull (entity);
					Assert.AreEqual ("firstname", entity.Firstname);
					Assert.AreEqual ("lastname", entity.Lastname);
					Assert.IsNull (entity.BirthDate);
				}
			}
		}


		[TestMethod]
		public void ProcessJobsUpdateReferenceTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);

					NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					PersonTitleEntity target = dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1000000001)));

					List<AbstractPersistenceJob> jobs = new List<AbstractPersistenceJob> ()
					{
						new ReferencePersistenceJob
						(
							entity,
							Druid.Parse("[J1AJ1]"),
							Druid.Parse("[J1AK1]"),
							target,
							PersistenceJobType.Update
						),
						new ReferencePersistenceJob
						(
							entity,
							Druid.Parse("[J1AB1]"),
							Druid.Parse("[J1AD1]"),
							null,
							PersistenceJobType.Update
						)
					};

					Dictionary<AbstractEntity, DbKey> newKeys;

					using (DbTransaction transaction = dbInfrastructure.BeginTransaction ())
					{
						DbLogEntry dbLogEntry = dbInfrastructure.ServiceManager.Logger.CreateLogEntry (new DbId (0));

						newKeys = processor.ProcessJobs (transaction, dbLogEntry, jobs).ToDictionary (p => p.Key, p => p.Value);

						transaction.Commit ();
					}

					Assert.IsTrue (newKeys.Count == 0);
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					PersonTitleEntity target = dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1000000001)));

					Assert.IsNotNull (entity);
					Assert.IsNotNull (entity.Title);
					Assert.AreSame (target, entity.Title);
					Assert.IsNull (entity.PreferredLanguage);
				}
			}
		}


		[TestMethod]
		public void ProcessJobsUpdateCollectionTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);

					NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					List<AbstractContactEntity> targets = new List<AbstractContactEntity> ()
					{
						dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000002))),
						dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000003))),
					};

					List<AbstractPersistenceJob> jobs = new List<AbstractPersistenceJob> ()
					{
						new CollectionPersistenceJob
						(
							entity,
							Druid.Parse("[J1AB1]"),
							Druid.Parse("[J1AC1]"),
							targets,
							PersistenceJobType.Update
						),
					};

					Dictionary<AbstractEntity, DbKey> newKeys;

					using (DbTransaction transaction = dbInfrastructure.BeginTransaction ())
					{
						DbLogEntry dbLogEntry = dbInfrastructure.ServiceManager.Logger.CreateLogEntry (new DbId (0));

						newKeys = processor.ProcessJobs (transaction, dbLogEntry, jobs).ToDictionary (p => p.Key, p => p.Value);

						transaction.Commit ();
					}

					Assert.IsTrue (newKeys.Count == 0);
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					List<AbstractContactEntity> targets = new List<AbstractContactEntity> ()
					{
						dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000002))),
						dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000003))),
					};

					Assert.IsNotNull (entity);
					Assert.IsNotNull (entity.Contacts.Count == 2);
					Assert.AreSame (targets[0], entity.Contacts[0]);
					Assert.AreSame (targets[1], entity.Contacts[1]);
				}
			}
		}


		[TestMethod]
		public void ProcessJobsBigTest()
		{
			DbKey key1;
			DbKey key2;
			DbKey key3;

			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					PersistenceJobProcessor processor = new PersistenceJobProcessor (dataContext);

					NaturalPersonEntity person1 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					NaturalPersonEntity person2 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));
					NaturalPersonEntity person3 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000003)));
					NaturalPersonEntity person4 = dataContext.CreateEntity<NaturalPersonEntity> ();

					PersonGenderEntity gender1 = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000002)));
					PersonGenderEntity gender2 = dataContext.CreateEntity<PersonGenderEntity> ();

					AbstractContactEntity contact1 = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000001)));
					AbstractContactEntity contact2 = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000002)));
					UriContactEntity contact3 = dataContext.CreateEntity<UriContactEntity> ();

					List<AbstractPersistenceJob> jobs = new List<AbstractPersistenceJob> ()
					{
						new CollectionPersistenceJob
						(
							person4,
							Druid.Parse ("[J1AB1]"),
							Druid.Parse ("[J1AC1]"),
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
							Druid.Parse("[J1AJ1]"),
							Druid.Parse("[J1AN1]"),
							null,
							PersistenceJobType.Update
						),
						new ValuePersistenceJob
						(
							person4,
							Druid.Parse ("[J1AJ1]"),
							new Dictionary<Druid,object> ()
							{
								{ Druid.Parse("[J1AL1]"), "fn" },
								{ Druid.Parse("[J1AM1]"), "ln" },
								{ Druid.Parse("[J1AO1]"), null },
							},
							false,
							PersistenceJobType.Insert
						),
						new ReferencePersistenceJob
						(
							person4,
							Druid.Parse ("[J1AJ1]"),
							Druid.Parse ("[J1AN1]"),
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
							Druid.Parse("[J1A42]"),
							new Dictionary<Druid,object> ()
							{
								{ Druid.Parse("[J1A62]"), "uri" },
							},
							false,
							PersistenceJobType.Insert
						),
						new CollectionPersistenceJob
						(
							person1,
							Druid.Parse("[J1AB1]"),
							Druid.Parse("[J1AC1]"),
							new List<AbstractContactEntity> (),
							PersistenceJobType.Update
						),
						new ValuePersistenceJob
						(
							contact1,
							Druid.Parse("[J1A42]"),
							new Dictionary<Druid,object> ()
							{
								{ Druid.Parse("[J1A62]"), "uri" },
							},
							false,
							PersistenceJobType.Update
						),
						new ValuePersistenceJob
						(
							contact3,
							Druid.Parse("[J1AA1]"),
							new Dictionary<Druid,object> (),
							true,
							PersistenceJobType.Insert
						),
						new ValuePersistenceJob
						(
							person4,
							Druid.Parse ("[J1AB1]"),
							new Dictionary<Druid,object> (),
							true,
							PersistenceJobType.Insert
						),
						new ValuePersistenceJob
						(
							gender2,
							Druid.Parse("[J1AQ]"),
							new Dictionary<Druid,object> ()
							{
								{ Druid.Parse("[J1A1]"), null },
								{ Druid.Parse("[J1A3]"), "code" },
								{ Druid.Parse("[J1AR]"), "name" },
							},
							true,
							PersistenceJobType.Insert
						),
					};

					Dictionary<AbstractEntity, DbKey> newKeys;

					using (DbTransaction transaction = dbInfrastructure.BeginTransaction ())
					{
						DbLogEntry dbLogEntry = dbInfrastructure.ServiceManager.Logger.CreateLogEntry (new DbId (0));

						newKeys = processor.ProcessJobs (transaction, dbLogEntry, jobs).ToDictionary (p => p.Key, p => p.Value);

						transaction.Commit ();
					}

					Assert.IsTrue (newKeys.Count == 3);

					key1 = newKeys[person4];
					key2 = newKeys[gender2];
					key3 = newKeys[contact3];
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					NaturalPersonEntity person2 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));
					NaturalPersonEntity person3 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000003)));
					NaturalPersonEntity person4 = dataContext.ResolveEntity<NaturalPersonEntity> (key1);

					PersonGenderEntity gender1 = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000002)));
					PersonGenderEntity gender2 = dataContext.ResolveEntity<PersonGenderEntity> (key2);

					UriContactEntity contact1 = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001)));
					UriContactEntity contact2 = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000002)));
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


}
