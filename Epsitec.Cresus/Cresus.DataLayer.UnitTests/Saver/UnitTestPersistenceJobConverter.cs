using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Saver;
using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.Saver
{


	[TestClass]
	public class UnitTestPersistenceJobConverter
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
		public void PersistenceJobConverterConstructor()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
            {
            	new PersistenceJobConverter(dataContext);
            }
		}


		[TestMethod]
		public void PersistenceJobConverterConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new PersistenceJobConverter (null)
			);
		}


		[TestMethod]
		public void ConvertDeleteJobArgumentCheck()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => converter.Convert ((DeletePersistenceJob) null)
				);
			}
		}


		[TestMethod]
		public void ConvertDeleteJobTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				DeletePersistenceJob job1 = new DeletePersistenceJob (entity);
				var job2 = converter.Convert (job1).ToList ();

				Assert.IsTrue (job2.Count == 1);
				Assert.IsTrue (job2[0] is DeleteSynchronizationJob);
				Assert.AreEqual (dataContext.UniqueId, job2[0].DataContextId);
				Assert.AreEqual (dataContext.GetNormalizedEntityKey (entity), job2[0].EntityKey);
			}
		}


		[TestMethod]
		public void ConvertValueJobArgumentCheck()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => converter.Convert ((ValuePersistenceJob) null)
				);
			}
		}


		[TestMethod]
		public void ConvertValueJobTest1()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid localEntityId = Druid.Parse ("[L0AN]");
				Dictionary<Druid, object> fieldIdsWithValues = new Dictionary<Druid, object> ()
				{
					{ Druid.Parse("[L0AV]"), "prénom" },
					{ Druid.Parse("[L0A01]"), "nom de famille" },
				};
				bool isRootType = false;
				PersistenceJobType jobType = PersistenceJobType.Insert;

				ValuePersistenceJob job1 = new ValuePersistenceJob (entity, localEntityId, fieldIdsWithValues, isRootType, jobType);
				var job2 = converter.Convert (job1).ToList ();

				Assert.IsTrue (job2.Count == 0);
			}
		}


		[TestMethod]
		public void ConvertValueJobTest2()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid localEntityId = Druid.Parse ("[L0AN]");
				Dictionary<Druid, object> fieldIdsWithValues = new Dictionary<Druid, object> ()
				{
					{ Druid.Parse("[L0AV]"), "prénom" },
					{ Druid.Parse("[L0A01]"), "nom de famille" },
					{ Druid.Parse("[L0A61]"), null }
				};
				bool isRootType = false;
				PersistenceJobType jobType = PersistenceJobType.Update;

				ValuePersistenceJob job1 = new ValuePersistenceJob (entity, localEntityId, fieldIdsWithValues, isRootType, jobType);
				var jobs2 = converter.Convert (job1).ToList ();

				Assert.IsTrue (jobs2.Count == 3);
				Assert.IsTrue (jobs2.All (j => j.DataContextId == dataContext.UniqueId));
				Assert.IsTrue (jobs2.All (j => j.EntityKey == dataContext.GetNormalizedEntityKey (entity)));
				Assert.IsTrue (jobs2.All (j => j is ValueSynchronizationJob));

				foreach (Druid fieldId in fieldIdsWithValues.Keys)
				{
					Assert.IsTrue (jobs2.Any (j => ((ValueSynchronizationJob) j).FieldId == fieldId));
					Assert.IsTrue (jobs2.Any (j => ((ValueSynchronizationJob) j).NewValue == fieldIdsWithValues[fieldId]));
				}
			}
		}


		[TestMethod]
		public void ConvertReferenceJobArgumentCheck()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => converter.Convert ((ReferencePersistenceJob) null)
				);
			}
		}


		[TestMethod]
		public void ConvertReferenceJobTest1()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid localEntityId = Druid.Parse ("[L0AN]");
				Druid fieldId = Druid.Parse ("[L0A11]");
				PersonGenderEntity target = entity.Gender;
				PersistenceJobType jobType = PersistenceJobType.Insert;


				ReferencePersistenceJob job1 = new ReferencePersistenceJob (entity, localEntityId, fieldId, target, jobType);
				var job2 = converter.Convert (job1).ToList ();

				Assert.IsTrue (job2.Count == 0);
			}
		}


		[TestMethod]
		public void ConvertReferenceJobTest2()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid localEntityId = Druid.Parse ("[L0AN]");
				Druid fieldId = Druid.Parse ("[L0A11]");
				PersonGenderEntity target = entity.Gender;
				PersistenceJobType jobType = PersistenceJobType.Update;


				ReferencePersistenceJob job1 = new ReferencePersistenceJob (entity, localEntityId, fieldId, target, jobType);
				var job2 = converter.Convert (job1).ToList ();

				Assert.IsTrue (job2.Count == 1);
				Assert.IsTrue (job2[0] is ReferenceSynchronizationJob);
				Assert.AreEqual (dataContext.UniqueId, job2[0].DataContextId);
				Assert.AreEqual (dataContext.GetNormalizedEntityKey (entity), job2[0].EntityKey);
				Assert.AreEqual (dataContext.GetNormalizedEntityKey (target), ((ReferenceSynchronizationJob) job2[0]).NewTargetKey);
				Assert.AreEqual (fieldId, ((ReferenceSynchronizationJob) job2[0]).FieldId);
			}
		}


		[TestMethod]
		public void ConvertReferenceJobTest3()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid localEntityId = Druid.Parse ("[L0AN]");
				Druid fieldId = Druid.Parse ("[L0A11]");
				PersonGenderEntity target = null;
				PersistenceJobType jobType = PersistenceJobType.Update;


				ReferencePersistenceJob job1 = new ReferencePersistenceJob (entity, localEntityId, fieldId, target, jobType);
				var job2 = converter.Convert (job1).ToList ();

				Assert.IsTrue (job2.Count == 1);
				Assert.IsTrue (job2[0] is ReferenceSynchronizationJob);
				Assert.AreEqual (dataContext.UniqueId, job2[0].DataContextId);
				Assert.AreEqual (dataContext.GetNormalizedEntityKey (entity), job2[0].EntityKey);
				Assert.IsFalse (((ReferenceSynchronizationJob) job2[0]).NewTargetKey.HasValue);
				Assert.AreEqual (fieldId, ((ReferenceSynchronizationJob) job2[0]).FieldId);
			}
		}


		[TestMethod]
		public void ConvertCollectionJobArgumentCheck()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => converter.Convert ((CollectionPersistenceJob) null)
				);
			}
		}


		[TestMethod]
		public void ConvertCollectionJobTest1()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid localEntityId = Druid.Parse ("[L0AN]");
				Druid fieldId = Druid.Parse ("[L0AS]");
				List<AbstractEntity> targets = new List<AbstractEntity> ()
				{
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (2))),
				};
				PersistenceJobType jobType = PersistenceJobType.Insert;

				CollectionPersistenceJob job1 = new CollectionPersistenceJob (entity, localEntityId, fieldId, targets, jobType);
				var job2 = converter.Convert (job1).ToList ();

				Assert.IsTrue (job2.Count == 0);
			}
		}


		[TestMethod]
		public void ConvertCollectionJobTest2()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid localEntityId = Druid.Parse ("[L0AN]");
				Druid fieldId = Druid.Parse ("[L0AS]");
				List<AbstractEntity> targets = new List<AbstractEntity> ()
				{
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (2))),
				};
				PersistenceJobType jobType = PersistenceJobType.Update;

				CollectionPersistenceJob job1 = new CollectionPersistenceJob (entity, localEntityId, fieldId, targets, jobType);
				var job2 = converter.Convert (job1).ToList ();

				Assert.IsTrue (job2.Count == 1);
				Assert.IsTrue (job2[0] is CollectionSynchronizationJob);
				Assert.AreEqual (dataContext.UniqueId, job2[0].DataContextId);
				Assert.AreEqual (dataContext.GetNormalizedEntityKey (entity), job2[0].EntityKey);
				Assert.AreEqual (fieldId, ((CollectionSynchronizationJob) job2[0]).FieldId);
				Assert.IsTrue
				(
					targets
					.Select (t => dataContext.GetNormalizedEntityKey (t).Value)
					.SequenceEqual (((CollectionSynchronizationJob) job2[0]).NewTargetKeys)
				);
			}
		}


	}


}
