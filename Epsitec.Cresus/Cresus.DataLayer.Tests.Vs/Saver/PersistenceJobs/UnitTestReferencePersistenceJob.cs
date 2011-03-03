using Epsitec.Common.Support;

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
		public void ReferencePersistenceJobConstructorArgumentCheck()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();;
			Druid localEntityId = Druid.FromLong (1);
			Druid fieldId = Druid.FromLong (2);
			NaturalPersonEntity target = new NaturalPersonEntity ();
			PersistenceJobType jobType = PersistenceJobType.Insert;

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new ReferencePersistenceJob (null, localEntityId, fieldId, target, jobType)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new ReferencePersistenceJob (entity, Druid.Empty, fieldId, target, jobType)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new ReferencePersistenceJob (entity, localEntityId, Druid.Empty, target, jobType)
			);
		}


		[TestMethod]
		public void ConvertArgumentCheck()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			Druid localEntityId = Druid.FromLong (1);
			Druid fieldId = Druid.FromLong (2);
			NaturalPersonEntity target = new NaturalPersonEntity ();
			PersistenceJobType jobType = PersistenceJobType.Insert;

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new ReferencePersistenceJob (entity, localEntityId, fieldId, target, jobType).Convert (null)
			);
		}


		[TestMethod]
		public void GetAffectedTablesArgumentCheck()
		{
			NaturalPersonEntity entity = new NaturalPersonEntity ();
			Druid localEntityId = Druid.FromLong (1);
			Druid fieldId = Druid.FromLong (2);
			NaturalPersonEntity target = new NaturalPersonEntity ();
			PersistenceJobType jobType = PersistenceJobType.Insert;

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new ReferencePersistenceJob (entity, localEntityId, fieldId, target, jobType).GetAffectedTables (null)
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


	}


}
