using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Saver;
using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestPersistenceJobGenerator
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseHelper.CreateAndConnectToDatabase ();

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				DatabaseCreator2.PupulateDatabase (dataContext);
			}
		}


		[ClassCleanup]
		public static void ClassCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestMethod]
		public void PersistenceJobGeneratorConstructorTest1()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				new PersistenceJobGenerator (dataContext);
			}
		}


		[TestMethod]
		[ExpectedException(typeof (System.ArgumentNullException))]
		public void PersistenceJobGeneratorConstructorTest2()
		{
			new PersistenceJobGenerator (null);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void InsertEntityTest1()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobGenerator generator = new PersistenceJobGenerator (dataContext);

				generator.CreateInsertionJobs (null);
			}
		}


		[TestMethod]
		public void InsertEntityTest2()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobGenerator generator = new PersistenceJobGenerator (dataContext);
				
				NaturalPersonEntity entity = dataContext.CreateEntity<NaturalPersonEntity> ();
				entity.Lastname = "new last name";

				List<AbstractPersistenceJob> jobs = generator.CreateInsertionJobs (entity).ToList ();

				Assert.IsTrue (jobs.Count == 2);
				Assert.IsTrue (jobs.All (j => j is ValuePersistenceJob));
				Assert.IsTrue (jobs.All (j => j.Entity == entity));
				Assert.IsTrue (jobs.Any (j => ((ValuePersistenceJob) j).JobType == PersistenceJobType.Insert));

				Assert.IsTrue (jobs.Any (j =>
				{
					return j is ValuePersistenceJob
						&& ((ValuePersistenceJob) j).IsRootTypeJob
						&& ((ValuePersistenceJob) j).LocalEntityId == Druid.Parse ("[L0AM]")
						&& ((ValuePersistenceJob) j).GetFieldIdsWithValues ().Count () == 0;
				}));

				Assert.IsTrue (jobs.Any (j =>
				{
					return j is ValuePersistenceJob
						&& !((ValuePersistenceJob) j).IsRootTypeJob
						&& ((ValuePersistenceJob) j).LocalEntityId == Druid.Parse ("[L0AN]")
						&& ((ValuePersistenceJob) j).GetFieldIdsWithValues ().Count () == 3
						&& ((ValuePersistenceJob) j).GetFieldIdsWithValues ().Any (k => k.Key == Druid.Parse ("[L0AV]") && k.Value == null)
						&& ((ValuePersistenceJob) j).GetFieldIdsWithValues ().Any (k => k.Key == Druid.Parse ("[L0A01]") && ((string) (k.Value)) == "new last name")
						&& ((ValuePersistenceJob) j).GetFieldIdsWithValues ().Any (k => k.Key == Druid.Parse ("[L0A61]") && k.Value == null);
				}));
			}
		}


		[TestMethod]
		public void InsertEntityTest3()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobGenerator generator = new PersistenceJobGenerator (dataContext);

				NaturalPersonEntity entity = dataContext.CreateEntity<NaturalPersonEntity> ();
				PersonGenderEntity target = dataContext.CreateEntity<PersonGenderEntity> ();

				entity.Gender = target;

				List<AbstractPersistenceJob> jobs = generator.CreateInsertionJobs (entity).ToList ();

				Assert.IsTrue (jobs.Count == 3);
				Assert.IsTrue (jobs.All (j => j.Entity == entity));
				Assert.IsTrue (jobs.Any (j => ((AbstractFieldPersistenceJob) j).JobType == PersistenceJobType.Insert));

				Assert.IsTrue (jobs.OfType<ReferencePersistenceJob> ().Count () == 1);

				Assert.IsTrue (jobs.OfType<ReferencePersistenceJob> ().First ().FieldId == Druid.Parse ("[L0A11]"));
				Assert.IsTrue (jobs.OfType<ReferencePersistenceJob> ().First ().LocalEntityId == Druid.Parse ("[L0AN]"));
				Assert.IsTrue (jobs.OfType<ReferencePersistenceJob> ().First ().Target == target);
			}
		}


		[TestMethod]
		public void InsertEntityTest4()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobGenerator generator = new PersistenceJobGenerator (dataContext);

				NaturalPersonEntity entity = dataContext.CreateEntity<NaturalPersonEntity> ();
				List<UriContactEntity> targets = new List<UriContactEntity> ()
				{
					dataContext.CreateEntity<UriContactEntity> (),
					dataContext.CreateEntity<UriContactEntity> (),
					dataContext.CreateEntity<UriContactEntity> (),
				};

				foreach (UriContactEntity target in targets)
				{
					entity.Contacts.Add (target);
				}

				List<AbstractPersistenceJob> jobs = generator.CreateInsertionJobs (entity).ToList ();

				Assert.IsTrue (jobs.Count == 3);
				Assert.IsTrue (jobs.All (j => j.Entity == entity));
				Assert.IsTrue (jobs.Any (j => ((AbstractFieldPersistenceJob) j).JobType == PersistenceJobType.Insert));

				Assert.IsTrue (jobs.OfType<CollectionPersistenceJob> ().Count () == 1);

				Assert.IsTrue (jobs.OfType<CollectionPersistenceJob> ().First ().FieldId == Druid.Parse ("[L0AS]"));
				Assert.IsTrue (jobs.OfType<CollectionPersistenceJob> ().First ().LocalEntityId == Druid.Parse ("[L0AM]"));
				Assert.IsTrue (jobs.OfType<CollectionPersistenceJob> ().First ().Targets.SequenceEqual (targets));
			}
		}


		[TestMethod]
		public void InsertEntityTest5()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobGenerator generator = new PersistenceJobGenerator (dataContext);

				NaturalPersonEntity entity = dataContext.CreateEntity<NaturalPersonEntity> ();

				entity.Lastname = "new last name";

				LanguageEntity target = dataContext.CreateEntity<LanguageEntity> ();
				entity.PreferredLanguage = target;
				
				List<UriContactEntity> targets = new List<UriContactEntity> ()
				{
					dataContext.CreateEntity<UriContactEntity> (),
					dataContext.CreateEntity<UriContactEntity> (),
					dataContext.CreateEntity<UriContactEntity> (),
				};

				foreach (UriContactEntity t in targets)
				{
					entity.Contacts.Add (t);
				}

				List<AbstractPersistenceJob> jobs = generator.CreateInsertionJobs (entity).ToList ();

				Assert.IsTrue (jobs.Count == 4);
				Assert.IsTrue (jobs.All (j => j.Entity == entity));
				Assert.IsTrue (jobs.Any (j => ((AbstractFieldPersistenceJob) j).JobType == PersistenceJobType.Insert));

				Assert.IsTrue (jobs.OfType<ValuePersistenceJob> ().Count () == 2);
				Assert.IsTrue (jobs.OfType<ValuePersistenceJob> ().Any (j =>
				{
					return j is ValuePersistenceJob
						&& ((ValuePersistenceJob) j).IsRootTypeJob
						&& ((ValuePersistenceJob) j).LocalEntityId == Druid.Parse ("[L0AM]")
						&& ((ValuePersistenceJob) j).GetFieldIdsWithValues ().Count () == 0;
				}));
				Assert.IsTrue (jobs.OfType<ValuePersistenceJob> ().Any (j =>
				{
					return j is ValuePersistenceJob
						&& !((ValuePersistenceJob) j).IsRootTypeJob
						&& ((ValuePersistenceJob) j).LocalEntityId == Druid.Parse ("[L0AN]")
						&& ((ValuePersistenceJob) j).GetFieldIdsWithValues ().Count () == 3
						&& ((ValuePersistenceJob) j).GetFieldIdsWithValues ().Any (k => k.Key == Druid.Parse ("[L0AV]") && k.Value == null)
						&& ((ValuePersistenceJob) j).GetFieldIdsWithValues ().Any (k => k.Key == Druid.Parse ("[L0A01]") && ((string) (k.Value)) == "new last name")
						&& ((ValuePersistenceJob) j).GetFieldIdsWithValues ().Any (k => k.Key == Druid.Parse ("[L0A61]") && k.Value == null);
				}));

				Assert.IsTrue (jobs.OfType<ReferencePersistenceJob> ().Count () == 1);

				Assert.IsTrue (jobs.OfType<ReferencePersistenceJob> ().First ().FieldId == Druid.Parse ("[L0AD1]"));
				Assert.IsTrue (jobs.OfType<ReferencePersistenceJob> ().First ().LocalEntityId == Druid.Parse ("[L0AM]"));
				Assert.IsTrue (jobs.OfType<ReferencePersistenceJob> ().First ().Target == target);

				Assert.IsTrue (jobs.OfType<CollectionPersistenceJob> ().Count () == 1);
				Assert.IsTrue (jobs.OfType<CollectionPersistenceJob> ().First ().FieldId == Druid.Parse ("[L0AS]"));
				Assert.IsTrue (jobs.OfType<CollectionPersistenceJob> ().First ().LocalEntityId == Druid.Parse ("[L0AM]"));
				Assert.IsTrue (jobs.OfType<CollectionPersistenceJob> ().First ().Targets.SequenceEqual (targets));
			}
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void UpdateEntityTest1()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobGenerator generator = new PersistenceJobGenerator (dataContext);

				generator.CreateUpdateJobs (null);
			}
		}


		[TestMethod]
		public void UpdateEntityTest2()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobGenerator generator = new PersistenceJobGenerator (dataContext);
				
				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				entity.Lastname = "new last name";

				List<AbstractPersistenceJob> jobs = generator.CreateUpdateJobs (entity).ToList ();

				Assert.IsTrue (jobs.Count == 1);
				Assert.IsTrue (jobs[0] is ValuePersistenceJob);
				Assert.IsTrue (jobs[0].Entity == entity);
				Assert.IsTrue (((ValuePersistenceJob) jobs[0]).JobType == PersistenceJobType.Update);
				Assert.IsFalse (((ValuePersistenceJob) jobs[0]).IsRootTypeJob);
				Assert.IsTrue (((ValuePersistenceJob) jobs[0]).LocalEntityId == Druid.Parse ("[L0AN]"));
				Assert.IsTrue (((ValuePersistenceJob) jobs[0]).GetFieldIdsWithValues ().Count () == 1);
				Assert.IsTrue (((ValuePersistenceJob) jobs[0]).GetFieldIdsWithValues ().First ().Key == Druid.Parse ("[L0A01]"));
				Assert.IsTrue (((string) ((ValuePersistenceJob) jobs[0]).GetFieldIdsWithValues ().First ().Value) == "new last name");
			}
		}


		[TestMethod]
		public void UpdateEntityTest3()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobGenerator generator = new PersistenceJobGenerator (dataContext);
				
				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				PersonTitleEntity target = dataContext.ResolveEntity<PersonTitleEntity> (new DbKey (new DbId (1)));
				entity.Title = target;

				List<AbstractPersistenceJob> jobs = generator.CreateUpdateJobs (entity).ToList ();

				Assert.IsTrue (jobs.Count == 1);
				Assert.IsTrue (jobs[0] is ReferencePersistenceJob);
				Assert.IsTrue (jobs[0].Entity == entity);
				Assert.IsTrue (((ReferencePersistenceJob) jobs[0]).JobType == PersistenceJobType.Update);
				Assert.IsTrue (((ReferencePersistenceJob) jobs[0]).LocalEntityId == Druid.Parse ("[L0AN]"));
				Assert.IsTrue (((ReferencePersistenceJob) jobs[0]).FieldId == Druid.Parse ("[L0AU]"));
				Assert.IsTrue (((ReferencePersistenceJob) jobs[0]).Target == target);
			}
		}


		[TestMethod]
		public void UpdateEntityTest4()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobGenerator generator = new PersistenceJobGenerator (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				List<AbstractContactEntity> targets = new List<AbstractContactEntity> ()
				{
					dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1))),
					dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (2))),
					dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (3))),
				};

				entity.Contacts.Add (targets.Last ());

				List<AbstractPersistenceJob> jobs = generator.CreateUpdateJobs (entity).ToList ();

				Assert.IsTrue (jobs.Count == 1);
				Assert.IsTrue (jobs[0] is CollectionPersistenceJob);
				Assert.IsTrue (jobs[0].Entity == entity);
				Assert.IsTrue (((CollectionPersistenceJob) jobs[0]).JobType == PersistenceJobType.Update);
				Assert.IsTrue (((CollectionPersistenceJob) jobs[0]).LocalEntityId == Druid.Parse ("[L0AM]"));
				Assert.IsTrue (((CollectionPersistenceJob) jobs[0]).FieldId == Druid.Parse ("[L0AS]"));
				Assert.IsTrue (((CollectionPersistenceJob) jobs[0]).Targets.SequenceEqual (targets));
			}
		}


		[TestMethod]
		public void UpdateEntityTest5()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobGenerator generator = new PersistenceJobGenerator (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				entity.BirthDate = null;

				entity.Gender = null;

				List<AbstractContactEntity> targets = new List<AbstractContactEntity> ()
				{
					dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1))),
				};

				entity.Contacts.RemoveAt (1);

				List<AbstractPersistenceJob> jobs = generator.CreateUpdateJobs (entity).ToList ();

				Assert.IsTrue (jobs.Count == 3);
				Assert.IsTrue (jobs.All (j => j.Entity == entity));
				Assert.IsTrue (jobs.All (j => ((AbstractFieldPersistenceJob)j).JobType == PersistenceJobType.Update));

				Assert.IsTrue (jobs.OfType<ValuePersistenceJob> ().Count () == 1);
				Assert.IsTrue (jobs.OfType<ValuePersistenceJob> ().First ().IsRootTypeJob == false);
				Assert.IsTrue (jobs.OfType<ValuePersistenceJob> ().First ().LocalEntityId == Druid.Parse ("[L0AN]"));
				Assert.IsTrue (jobs.OfType<ValuePersistenceJob> ().First ().GetFieldIdsWithValues ().Count () == 1);
				Assert.IsTrue (jobs.OfType<ValuePersistenceJob> ().First ().GetFieldIdsWithValues ().First ().Key == Druid.Parse ("[L0A61]"));
				Assert.IsTrue (jobs.OfType<ValuePersistenceJob> ().First ().GetFieldIdsWithValues ().First ().Value == null);

				Assert.IsTrue (jobs.OfType<ReferencePersistenceJob> ().Count () == 1);
				Assert.IsTrue (jobs.OfType<ReferencePersistenceJob> ().First ().LocalEntityId == Druid.Parse ("[L0AN]"));
				Assert.IsTrue (jobs.OfType<ReferencePersistenceJob> ().First ().FieldId == Druid.Parse ("[L0A11]"));
				Assert.IsTrue (jobs.OfType<ReferencePersistenceJob> ().First ().Target == null);

				Assert.IsTrue (jobs.OfType<CollectionPersistenceJob> ().Count () == 1);
				Assert.IsTrue (jobs.OfType<CollectionPersistenceJob> ().First().LocalEntityId == Druid.Parse ("[L0AM]"));
				Assert.IsTrue (jobs.OfType<CollectionPersistenceJob> ().First ().FieldId == Druid.Parse ("[L0AS]"));
				Assert.IsTrue (jobs.OfType<CollectionPersistenceJob> ().First ().Targets.SequenceEqual (targets));
			}
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void DeleteEntityTest1()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobGenerator generator = new PersistenceJobGenerator (dataContext);

				generator.CreateDeletionJob (null);
			}
		}


		[TestMethod]
		public void DeleteEntityTest2()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobGenerator generator = new PersistenceJobGenerator (dataContext);
				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				AbstractPersistenceJob job = generator.CreateDeletionJob (entity);

				Assert.IsInstanceOfType (job, typeof (DeletePersistenceJob));
				Assert.AreEqual (entity, job.Entity);
			}
		}


	}


}
