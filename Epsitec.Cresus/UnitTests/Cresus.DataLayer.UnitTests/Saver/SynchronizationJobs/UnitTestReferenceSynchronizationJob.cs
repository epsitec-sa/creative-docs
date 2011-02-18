using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;

using Epsitec.Cresus.DataLayer.Context;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests.Saver.SynchronizationJobs
{


	[TestClass]
	public sealed class UnitTestReferenceSynchronizationJob
	{


		[TestMethod]
		public void ReferenceSynchronizationJobConstructorTest()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1000000001)));
			Druid fieldId = Druid.FromLong (1);
			EntityKey targetKey = new EntityKey (Druid.FromLong (2), new DbKey (new DbId (1000000001)));

			var job = new ReferenceSynchronizationJob (dataContextId, entityKey, fieldId, targetKey);

			Assert.AreEqual (dataContextId, job.DataContextId);
			Assert.AreEqual (entityKey, job.EntityKey);
			Assert.AreEqual (fieldId, job.FieldId);
			Assert.AreEqual (targetKey, job.NewTargetKey);
		}


		[TestMethod]
		public void ReferenceSynchronizationJobConstructorArgumentCheck()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1000000001)));
			Druid fieldId = Druid.FromLong (1);
			EntityKey targetKey = new EntityKey (Druid.FromLong (2), new DbKey (new DbId (1000000001)));

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new ReferenceSynchronizationJob (dataContextId, EntityKey.Empty, fieldId, targetKey)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new ReferenceSynchronizationJob (dataContextId, entityKey, Druid.Empty, targetKey)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new ReferenceSynchronizationJob (dataContextId, entityKey, fieldId, EntityKey.Empty)
			);
		}


		[TestMethod]
		public void SynchronizeArgumentCheck()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1000000001)));
			Druid fieldId = Druid.FromLong(1);
			EntityKey targetKey = new EntityKey (Druid.FromLong (2), new DbKey (new DbId (1000000001)));
			
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new ReferenceSynchronizationJob (dataContextId, entityKey, fieldId, targetKey).Synchronize (null)
			);
		}


	}


}
