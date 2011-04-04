using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Infrastructure
{


	[TestClass]
	public sealed class UnitTestEntityModificationEntry
	{


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new EntityModificationEntry (DbId.Empty, new DbId (1), System.DateTime.Now)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new EntityModificationEntry (new DbId (1), DbId.Empty, System.DateTime.Now)
			);
		}


		[TestMethod]
		public void ConstructorAndPropertiesTest()
		{
			var entryId = new DbId (5432);
			var connectionId = new DbId (54245);
			var time = System.DateTime.Now;

			var entry = new EntityModificationEntry (entryId, connectionId, time);

			Assert.AreEqual (entryId, entry.EntryId);
			Assert.AreEqual (connectionId, entry.ConnectionId);
			Assert.AreEqual (time, entry.Time);
		}


	}


}
