using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Services;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.Database.UnitTests.Services
{


	[TestClass]
	public class UnitTestDbLogger
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DbInfrastructureHelper.ResetTestDatabase ();
		}


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new DbLogger (null)
			);
		}


		[TestMethod]
		public void CreateExistsGetDeleteLogEntry()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbLogger logger = dbInfrastructure.ServiceManager.Logger;

				foreach (DbId dbId in this.GetDbIdSamples ())
				{
					Assert.IsFalse (logger.LogEntryExists (dbId));

					DbLogEntry logEntry1 = logger.CreateLogEntry (dbId);

					Assert.IsTrue (logger.LogEntryExists (dbId));

					DbLogEntry logEntry2 = logger.GetLogEntry (logEntry1.EntryId);

					Assert.AreEqual (dbId, logEntry1.ConnectionId);
					Assert.AreEqual (dbId.Value, logEntry1.SequenceNumber);
					Assert.AreEqual (logEntry1.EntryId, logEntry2.EntryId);
					Assert.AreEqual (logEntry1.ConnectionId, logEntry2.ConnectionId);
					Assert.AreEqual (logEntry1.DateTime, logEntry2.DateTime);
					Assert.AreEqual (logEntry1.SequenceNumber, logEntry2.SequenceNumber);

					logger.RemoveLogEntry (dbId);

					Assert.IsFalse (logger.LogEntryExists (dbId));
				}
			}
		}


		private IEnumerable<DbId> GetDbIdSamples()
		{
			for (int i = 1; i < 11; i++)
			{
				yield return new DbId (i);
			}
		}


	}


}
