using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;

using Epsitec.Cresus.DataLayer.Context;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests.Saver.SynchronizationJobs
{


	[TestClass]
	public sealed class UnitTestValueSynchronizationJob
	{

		
		[TestMethod]
		public void ValueSynchronizationJobConstructorTest()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1000000001)));
			Druid fieldId = Druid.FromLong (1);
			object value = "value";

			var job = new ValueSynchronizationJob (dataContextId, entityKey, fieldId, value);

			Assert.AreEqual (dataContextId, job.DataContextId);
			Assert.AreEqual (entityKey, job.EntityKey);
			Assert.AreEqual (fieldId, job.FieldId);
			Assert.AreEqual (value, job.NewValue);
		}


		[TestMethod]
		public void ValueSynchronizationJobConstructorArgumentCheck()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1000000001)));
			Druid fieldId = Druid.FromLong (1);
			object value = "value";

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new ValueSynchronizationJob (dataContextId, EntityKey.Empty, fieldId, value)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new ValueSynchronizationJob (dataContextId, entityKey, Druid.Empty, value)
			);
		}


		[TestMethod]
		public void SynchronizeTest()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1000000001)));
			Druid fieldId = Druid.FromLong (1);
			object value = "value";

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new ValueSynchronizationJob (dataContextId, entityKey, fieldId, value).Synchronize (null)
			);
		}


	}


}
