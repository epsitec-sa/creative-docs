using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestValuePersistenceJob
	{


		[TestMethod]
		public void ValuePersistenceJobConstructor1Test()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			Dictionary<Druid, object> fieldIdsWithValues = this.GetSampleFieldIdsWithValues ();

			foreach (Druid localEntityId in this.GetSampleDruids ())
			{
				foreach (bool isRootType in this.GetSampleBools ())
				{
					foreach (PersistenceJobType jobType in this.GetSamplePersistenceJobTypes ())
					{
						var job = new ValuePersistenceJob (entity, localEntityId, fieldIdsWithValues, isRootType, jobType);

						Assert.AreSame (entity, job.Entity);
						Assert.AreEqual (localEntityId, job.LocalEntityId);
						Assert.AreEqual (isRootType, job.IsRootTypeJob);
						Assert.AreEqual (jobType, job.JobType);
						Assert.AreEqual (fieldIdsWithValues.Count, job.GetFieldIdsWithValues ().Count ());

						foreach (var item in job.GetFieldIdsWithValues ())
						{
							Assert.AreEqual (fieldIdsWithValues[item.Key], item.Value);
						}
					}
				}
			}
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void ValuePersistenceJobConstructor2Test()
		{
			NaturalPersonEntity entity = null;
			Druid localEntityId = Druid.FromLong (1);
			Dictionary<Druid, object> fieldIdsWithValues = new Dictionary<Druid, object> ();
			bool isRootTypeJob = true;
			PersistenceJobType jobType = PersistenceJobType.Insert;

			new ValuePersistenceJob (entity, localEntityId, fieldIdsWithValues, isRootTypeJob, jobType);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void ValuePersistenceJobConstructor3Test()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			Druid localEntityId = Druid.Empty;
			Dictionary<Druid, object> fieldIdsWithValues = new Dictionary<Druid, object> ();
			bool isRootTypeJob = true;
			PersistenceJobType jobType = PersistenceJobType.Insert;

			new ValuePersistenceJob (entity, localEntityId, fieldIdsWithValues, isRootTypeJob, jobType);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void ValuePersistenceJobConstructor4Test()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			Druid localEntityId = Druid.FromLong (1);
			Dictionary<Druid, object> fieldIdsWithValues = null;
			bool isRootTypeJob = true;
			PersistenceJobType jobType = PersistenceJobType.Insert;

			new ValuePersistenceJob (entity, localEntityId, fieldIdsWithValues, isRootTypeJob, jobType);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void ValuePersistenceJobConstructor5Test()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			Druid localEntityId = Druid.FromLong (1);
			Dictionary<Druid, object> fieldIdsWithValues = new Dictionary<Druid, object> ()
			{
				{ Druid.Empty, "test" },
			};
			bool isRootTypeJob = true;
			PersistenceJobType jobType = PersistenceJobType.Insert;

			new ValuePersistenceJob (entity, localEntityId, fieldIdsWithValues, isRootTypeJob, jobType);
		}


		private IEnumerable<Druid> GetSampleDruids()
		{
			for (int i = 1; i < 10; i++)
			{
				yield return Druid.FromLong (i);
			}
		}


		private IEnumerable<bool> GetSampleBools()
		{
			yield return true;
			yield return false;
		}


		private IEnumerable<PersistenceJobType> GetSamplePersistenceJobTypes()
		{
			yield return PersistenceJobType.Insert;
			yield return PersistenceJobType.Update;
		}


		private Dictionary<Druid, object> GetSampleFieldIdsWithValues()
		{
			Dictionary<Druid, object> example = new Dictionary<Druid, object> ()
			{
				{ Druid.FromLong (1), "1" },
				{ Druid.FromLong (2), "2" },
				{ Druid.FromLong (3), "3" },
				{ Druid.FromLong (4), "4" },
				{ Druid.FromLong (5), "5" },
			};
			
			return example;
		}


	}


}
