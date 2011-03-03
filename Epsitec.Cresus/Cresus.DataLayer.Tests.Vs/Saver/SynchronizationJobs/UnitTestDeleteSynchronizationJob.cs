using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;

using Epsitec.Cresus.DataLayer.Context;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Saver.SynchronizationJobs
{


	[TestClass]
	public sealed class UnitTestDeleteSynchronizationJob
	{


		[TestMethod]
		public void DeleteSynchronizationJobConstructorTest()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1000000001)));

			var job = new DeleteSynchronizationJob (dataContextId, entityKey);

			Assert.AreEqual (dataContextId, job.DataContextId);
			Assert.AreEqual (entityKey, job.EntityKey);
		}


		[TestMethod]
		public void DeleteSynchronizationJobConstructorArgumentCheck()
		{
			int dataContextId = 0;
			EntityKey entityKey = EntityKey.Empty;

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new DeleteSynchronizationJob (dataContextId, entityKey)
			);
		}


		[TestMethod]
		public void SynchronizeArgumentCheck()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1000000001)));

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new DeleteSynchronizationJob (dataContextId, entityKey).Synchronize (null)
			);
		}


	}


}
