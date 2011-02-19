using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.Saver.PersistenceJobs
{


	[TestClass]
	public class UnitTestCollectionPersistenceJob
	{


		[TestMethod]
		public void CollectionPersistenceJobConstructor()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			var targets = this.GetSampleTargets ().ToList ();

			foreach (Druid localEntityId in this.GetSampleDruids ())
			{
				foreach (PersistenceJobType jobType in this.GetSamplePersistenceJobTypes ())
				{
					foreach (Druid fieldId in this.GetSampleDruids ())
					{
						var job = new CollectionPersistenceJob (entity, localEntityId, fieldId, targets, jobType);

						Assert.AreSame (entity, job.Entity);
						Assert.AreEqual (localEntityId, job.LocalEntityId);
						Assert.AreEqual (fieldId, job.FieldId);
						Assert.AreEqual (jobType, job.JobType);

						Assert.AreEqual (targets.Count (), job.Targets.Count ());
						Assert.IsTrue (targets.SequenceEqual (job.Targets));
					}
				}
			}
		}


		[TestMethod]
		public void CollectionPersistenceJobConstructorArgumentCheck()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			Druid localEntityId = Druid.FromLong (1);
			Druid fieldId = Druid.FromLong (2);
			var targets = this.GetSampleTargets ();
			PersistenceJobType jobType = PersistenceJobType.Insert;

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new CollectionPersistenceJob (null, localEntityId, fieldId, targets, jobType)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new CollectionPersistenceJob (entity, Druid.Empty, fieldId, targets, jobType)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new CollectionPersistenceJob (entity, localEntityId, Druid.Empty, targets, jobType)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new CollectionPersistenceJob (entity, localEntityId, fieldId, null, jobType)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new CollectionPersistenceJob (entity, localEntityId, fieldId, new List<AbstractEntity> () { null }, jobType)
			);
		}


		[TestMethod]
		public void ConvertArgumentCheck()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			Druid localEntityId = Druid.FromLong (1);
			Druid fieldId =Druid.FromLong (2);
			var targets = this.GetSampleTargets ();
			PersistenceJobType jobType = PersistenceJobType.Insert;

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new CollectionPersistenceJob (entity, localEntityId, fieldId, targets, jobType).Convert (null)
			);
		}


		[TestMethod]
		public void GetAffectedTablesArgumentCheck()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			Druid localEntityId = Druid.FromLong (1);
			Druid fieldId =Druid.FromLong (2);
			var targets = this.GetSampleTargets ();
			PersistenceJobType jobType = PersistenceJobType.Insert;

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new CollectionPersistenceJob (entity, localEntityId, fieldId, targets, jobType).GetAffectedTables (null)
			);
		}


		private IEnumerable<Druid> GetSampleDruids()
		{
			for (int i = 1; i < 10; i++)
			{
				yield return Druid.FromLong (i);
			}
		}


		private IEnumerable<PersistenceJobType> GetSamplePersistenceJobTypes()
		{
			yield return PersistenceJobType.Insert;
			yield return PersistenceJobType.Update;
		}


		private IEnumerable<AbstractEntity> GetSampleTargets()
		{
			for (int i = 0; i < 10; i++)
			{
				yield return new NaturalPersonEntity ();
			}
		}


	}


}
