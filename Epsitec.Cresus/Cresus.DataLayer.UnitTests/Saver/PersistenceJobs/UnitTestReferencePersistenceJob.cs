using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public class UnitTestReferencePersistenceJob
	{


		[TestMethod]
		public void ReferencePersistenceJobConstructor1Test()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			NaturalPersonEntity target = new NaturalPersonEntity ();

			foreach (Druid localEntityId in this.GetSampleDruids ())
			{
				foreach (PersistenceJobType jobType in this.GetSamplePersistenceJobTypes ())
				{
					foreach (Druid fieldId in this.GetSampleDruids ())
					{
						var job = new ReferencePersistenceJob (entity, localEntityId, fieldId, target, jobType);

						Assert.AreSame (entity, job.Entity);
						Assert.AreEqual (localEntityId, job.LocalEntityId);
						Assert.AreEqual (fieldId, job.FieldId);
						Assert.AreSame (target, job.Target);
						Assert.AreEqual (jobType, job.JobType);
					}
				}
			}
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void ReferencePersistenceJobConstructor2Test()
		{
			NaturalPersonEntity entity = null;
			Druid localEntityId = Druid.FromLong (1);
			Druid fieldId = Druid.FromLong (2);
			NaturalPersonEntity target = new NaturalPersonEntity ();
			PersistenceJobType jobType = PersistenceJobType.Insert;

			new ReferencePersistenceJob (entity, localEntityId, fieldId, target, jobType);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void ReferencePersistenceJobConstructor3Test()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			Druid localEntityId = Druid.Empty;
			Druid fieldId = Druid.FromLong (2);
			NaturalPersonEntity target = new NaturalPersonEntity ();
			PersistenceJobType jobType = PersistenceJobType.Insert;

			new ReferencePersistenceJob (entity, localEntityId, fieldId, target, jobType);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void ReferencePersistenceJobConstructor4Test()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			Druid localEntityId = Druid.FromLong (1);
			Druid fieldId = Druid.Empty;
			NaturalPersonEntity target = new NaturalPersonEntity ();
			PersistenceJobType jobType = PersistenceJobType.Insert;

			new ReferencePersistenceJob (entity, localEntityId, fieldId, target, jobType);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void ConvertTest()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			Druid localEntityId = Druid.FromLong (1);
			Druid fieldId = Druid.FromLong (2);
			NaturalPersonEntity target = new NaturalPersonEntity ();
			PersistenceJobType jobType = PersistenceJobType.Insert;

			new ReferencePersistenceJob (entity, localEntityId, fieldId, target, jobType).Convert (null);
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


	}


}
