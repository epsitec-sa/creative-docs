﻿using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;
using Epsitec.Cresus.DataLayer.Context;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.Saver.SynchronizationJobs
{


	[TestClass]
	public sealed class UnitTestCollectionSynchronizationJob
	{


		[TestMethod]
		public void CollectionSynchronizationJobConstructorTest()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1)));
			Druid fieldId = Druid.FromLong (1);
			List<EntityKey> targetKeys = new List<EntityKey> ()
			{
				new EntityKey (Druid.FromLong (2), new DbKey (new DbId (1))),
				new EntityKey (Druid.FromLong (3), new DbKey (new DbId (2))),
				new EntityKey (Druid.FromLong (4), new DbKey (new DbId (3))),
			};

			var job = new CollectionSynchronizationJob (dataContextId, entityKey, fieldId, targetKeys);

			Assert.AreEqual (dataContextId, job.DataContextId);
			Assert.AreEqual (entityKey, job.EntityKey);
			Assert.AreEqual (fieldId, job.FieldId);
			CollectionAssert.AreEquivalent (targetKeys, job.NewTargetKeys.ToList ());
		}


		[TestMethod]
		public void CollectionSynchronizationJobConstructorArgumentCheck()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1)));
			Druid fieldId = Druid.FromLong (1);
			List<EntityKey> targetKeys = new List<EntityKey> ()
			{
				new EntityKey (Druid.FromLong (2), new DbKey (new DbId (1))),
				new EntityKey (Druid.FromLong (3), new DbKey (new DbId (2))),
				new EntityKey (Druid.FromLong (4), new DbKey (new DbId (3))),
			};

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new CollectionSynchronizationJob (dataContextId, EntityKey.Empty, fieldId, targetKeys)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new CollectionSynchronizationJob (dataContextId, entityKey, fieldId, null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new CollectionSynchronizationJob (dataContextId, entityKey, Druid.Empty, targetKeys)
			);
		}


		[TestMethod]
		public void SynchronizeTest()
		{
			int dataContextId = 0;
			EntityKey entityKey = new EntityKey (Druid.FromLong (1), new DbKey (new DbId (1)));
			Druid fieldId = Druid.FromLong (1);
			List<EntityKey> targetKeys = new List<EntityKey> ()
			{
				new EntityKey (Druid.FromLong (2), new DbKey (new DbId (1))),
				new EntityKey (Druid.FromLong (3), new DbKey (new DbId (2))),
				new EntityKey (Druid.FromLong (4), new DbKey (new DbId (3))),
			};

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new CollectionSynchronizationJob (dataContextId, entityKey, fieldId, targetKeys).Synchronize (null)
			);
		}


	}


}
