using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
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

			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void PersistenceJobConverterConstructor()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				new PersistenceJobConverter (dataContext);
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				Druid localEntityId = Druid.Parse ("[J1AJ1]");
				Dictionary<Druid, object> fieldIdsWithValues = new Dictionary<Druid, object> ()
					{
						{ Druid.Parse("[J1AL1]"), "prénom" },
						{ Druid.Parse("[J1AM1]"), "nom de famille" },
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				Druid localEntityId = Druid.Parse ("[J1AJ1]");
				Dictionary<Druid, object> fieldIdsWithValues = new Dictionary<Druid, object> ()
					{
						{ Druid.Parse("[J1AL1]"), "prénom" },
						{ Druid.Parse("[J1AM1]"), "nom de famille" },
						{ Druid.Parse("[J1AO1]"), null }
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				Druid localEntityId = Druid.Parse ("[J1AJ1]");
				Druid fieldId = Druid.Parse ("[J1AN1]");
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				Druid localEntityId = Druid.Parse ("[J1AJ1]");
				Druid fieldId = Druid.Parse ("[J1AN1]");
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				Druid localEntityId = Druid.Parse ("[J1AJ1]");
				Druid fieldId = Druid.Parse ("[J1AN1]");
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				Druid localEntityId = Druid.Parse ("[J1AJ1]");
				Druid fieldId = Druid.Parse ("[J1AC1]");
				List<AbstractEntity> targets = new List<AbstractEntity> ()
					{
						dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001))),
						dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000002))),
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				PersistenceJobConverter converter = new PersistenceJobConverter (dataContext);

				NaturalPersonEntity entity = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				Druid localEntityId = Druid.Parse ("[J1AJ1]");
				Druid fieldId = Druid.Parse ("[J1AC1]");
				List<AbstractEntity> targets = new List<AbstractEntity> ()
					{
						dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001))),
						dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000002))),
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
