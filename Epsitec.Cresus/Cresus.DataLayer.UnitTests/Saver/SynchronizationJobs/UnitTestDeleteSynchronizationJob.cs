using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;

using Epsitec.Cresus.DataLayer.Context;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests.Saver.SynchronizationJobs
{


	[TestClass]
	public sealed class UnitTestDeleteSynchronizationJob
	{


		[TestMethod]
		public void DeleteSynchronizationJobConstructorTest1()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1)));

			var job = new DeleteSynchronizationJob (dataContextId, entityKey);

			Assert.AreEqual (dataContextId, job.DataContextId);
			Assert.AreEqual (entityKey, job.EntityKey);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void DeleteSynchronizationJobConstructorTest2()
		{
			int dataContextId = 0;
			EntityKey entityKey = EntityKey.Empty;

			var job = new DeleteSynchronizationJob (dataContextId, entityKey);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void SynchronizeTest()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1)));

			new DeleteSynchronizationJob (dataContextId, entityKey).Synchronize (null);
		}


	}


}
