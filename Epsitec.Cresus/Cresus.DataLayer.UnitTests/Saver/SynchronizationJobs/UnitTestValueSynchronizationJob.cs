using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;

using Epsitec.Cresus.DataLayer.Context;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestValueSynchronizationJob
	{

		
		[TestMethod]
		public void ValueSynchronizationJobConstructorTest1()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1)));
			Druid fieldId = Druid.FromLong (1);
			object value = "value";

			var job = new ValueSynchronizationJob (dataContextId, entityKey, fieldId, value);

			Assert.AreEqual (dataContextId, job.DataContextId);
			Assert.AreEqual (entityKey, job.EntityKey);
			Assert.AreEqual (fieldId, job.FieldId);
			Assert.AreEqual (value, job.NewValue);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void ValueSynchronizationJobConstructorTest2()
		{
			int dataContextId = 0;
			EntityKey entityKey = EntityKey.Empty;
			Druid fieldId = Druid.FromLong (1);
			object value = "value";

			var job = new ValueSynchronizationJob (dataContextId, entityKey, fieldId, value);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void ValueSynchronizationJobConstructorTest3()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1)));
			Druid fieldId = Druid.Empty;
			object value = "value";

			var job = new ValueSynchronizationJob (dataContextId, entityKey, fieldId, value);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void SynchronizeTest()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1)));
			Druid fieldId = Druid.FromLong (1);
			object value = "value";

			new ValueSynchronizationJob (dataContextId, entityKey, fieldId, value).Synchronize (null);
		}


	}


}
