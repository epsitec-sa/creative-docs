using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;

using Epsitec.Cresus.DataLayer.Context;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestReferenceSynchronizationJob
	{


		[TestMethod]
		public void ReferenceSynchronizationJobConstructorTest1()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1)));
			Druid fieldId = Druid.FromLong (1);
			EntityKey targetKey = new EntityKey (Druid.FromLong (2), new DbKey (new DbId (1)));

			var job = new ReferenceSynchronizationJob (dataContextId, entityKey, fieldId, targetKey);

			Assert.AreEqual (dataContextId, job.DataContextId);
			Assert.AreEqual (entityKey, job.EntityKey);
			Assert.AreEqual (fieldId, job.FieldId);
			Assert.AreEqual (targetKey, job.NewTargetKey);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void ReferenceSynchronizationJobConstructorTest2()
		{
			int dataContextId = 0;
			EntityKey entityKey = EntityKey.Empty;
			Druid fieldId = Druid.FromLong (1);
			EntityKey targetKey = new EntityKey (Druid.FromLong (2), new DbKey (new DbId (1)));

			var job = new ReferenceSynchronizationJob (dataContextId, entityKey, fieldId, targetKey);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void ReferenceSynchronizationJobConstructorTest3()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1)));
			Druid fieldId = Druid.FromLong (1);
			EntityKey targetKey = EntityKey.Empty;

			var job = new ReferenceSynchronizationJob (dataContextId, entityKey, fieldId, targetKey);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void ReferenceSynchronizationJobConstructorTest4()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1)));
			Druid fieldId = Druid.Empty;
			EntityKey targetKey = new EntityKey (Druid.FromLong (2), new DbKey (new DbId (1)));

			var job = new ReferenceSynchronizationJob (dataContextId, entityKey, fieldId, targetKey);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void SynchronizeTest()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1)));
			Druid fieldId = Druid.FromLong(1);
			EntityKey targetKey = new EntityKey (Druid.FromLong (2), new DbKey (new DbId (1)));

			new ReferenceSynchronizationJob (dataContextId, entityKey, fieldId, targetKey).Synchronize (null);
		}


	}


}
