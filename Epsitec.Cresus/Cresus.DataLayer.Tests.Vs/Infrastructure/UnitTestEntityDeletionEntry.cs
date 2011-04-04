using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Infrastructure
{


	[TestClass]
	public sealed class UnitTestEntityDeletionEntry
	{


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new EntityDeletionEntry (DbId.Empty, new DbId (1), Druid.FromLong (1), new DbId (1))
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new EntityDeletionEntry (new DbId (1), DbId.Empty, Druid.FromLong (1), new DbId (1))
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new EntityDeletionEntry (new DbId (1), new DbId (1),Druid.Empty, new DbId (1))
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new EntityDeletionEntry (new DbId (1), new DbId (1), Druid.FromLong (1), DbId.Empty)
			);
		}


		[TestMethod]
		public void ConstructorAndPropertiesTest()
		{
			var entryId = new DbId (5432);
			var entityModificationEntryId = new DbId (54245);
			var entityTypeId = Druid.FromLong (543);
			var entityId = new DbId (634);

			var entry = new EntityDeletionEntry (entryId, entityModificationEntryId, entityTypeId, entityId);

			Assert.AreEqual (entryId, entry.EntryId);
			Assert.AreEqual (entityModificationEntryId, entry.EntityModificationEntryId);
			Assert.AreEqual (entityTypeId, entry.EntityTypeId);
			Assert.AreEqual (entityId, entry.EntityId);
		}


	}


}
