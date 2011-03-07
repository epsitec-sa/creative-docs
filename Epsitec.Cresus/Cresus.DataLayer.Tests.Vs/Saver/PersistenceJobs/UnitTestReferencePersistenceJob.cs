using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Saver.PersistenceJobs
{


	[TestClass]
	public class UnitTestReferencePersistenceJob
	{


		[TestMethod]
		public void ReferencePersistenceJobConstructorTest()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			Dictionary<Druid, AbstractEntity> fieldIdsWithTargets = this.GetSampleFieldIdsWithTargets ();

			foreach (Druid localEntityId in this.GetSampleDruids ())
			{
				foreach (PersistenceJobType jobType in this.GetSamplePersistenceJobTypes ())
				{
					var job = new ReferencePersistenceJob (entity, localEntityId, fieldIdsWithTargets, jobType);

					Assert.AreSame (entity, job.Entity);
					Assert.AreEqual (localEntityId, job.LocalEntityId);
					Assert.AreEqual (jobType, job.JobType);

					foreach (var item in job.GetFieldIdsWithTargets ())
					{
						Assert.AreEqual (fieldIdsWithTargets[item.Key], item.Value);
					}
				}
			}
		}


		[TestMethod]
		public void ReferencePersistenceJobConstructorArgumentCheck()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			Druid localEntityId = Druid.FromLong (1);
			Dictionary<Druid, AbstractEntity> fieldIdsWithTargets = this.GetSampleFieldIdsWithTargets ();
			PersistenceJobType jobType = PersistenceJobType.Insert;

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new ReferencePersistenceJob (null, localEntityId, fieldIdsWithTargets, jobType)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new ReferencePersistenceJob (entity, Druid.Empty, fieldIdsWithTargets, jobType)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new ReferencePersistenceJob (entity, localEntityId, null, jobType)
			);
		}


		[TestMethod]
		public void ConvertArgumentCheck()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			Druid localEntityId = Druid.FromLong (1);
			Dictionary<Druid, AbstractEntity> fieldIdsWithTargets = this.GetSampleFieldIdsWithTargets ();
			PersistenceJobType jobType = PersistenceJobType.Insert;

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new ReferencePersistenceJob (entity, localEntityId, fieldIdsWithTargets, jobType).Convert (null)
			);
		}


		[TestMethod]
		public void GetAffectedTablesArgumentCheck()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			Druid localEntityId = Druid.FromLong (1);
			Dictionary<Druid, AbstractEntity> fieldIdsWithTargets = this.GetSampleFieldIdsWithTargets ();
			PersistenceJobType jobType = PersistenceJobType.Insert;

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new ReferencePersistenceJob (entity, localEntityId, fieldIdsWithTargets, jobType).GetAffectedTables (null)
			);
		}


		private IEnumerable<Druid> GetSampleDruids()
		{
			for (int i = 1; i < 10; i++)
			{
				yield return Druid.FromLong (i);
			}
		}


		private Dictionary<Druid, AbstractEntity> GetSampleFieldIdsWithTargets()
		{
			Dictionary<Druid, AbstractEntity> example = new Dictionary<Druid, AbstractEntity> ()
			{
				{ Druid.FromLong (1), new NaturalPersonEntity () },
				{ Druid.FromLong (2), new NaturalPersonEntity () },
				{ Druid.FromLong (3), new NaturalPersonEntity () },
				{ Druid.FromLong (4), new NaturalPersonEntity () },
				{ Druid.FromLong (5), new NaturalPersonEntity () },
			};

			return example;
		}


		private IEnumerable<PersistenceJobType> GetSamplePersistenceJobTypes()
		{
			yield return PersistenceJobType.Insert;
			yield return PersistenceJobType.Update;
		}


	}


}
