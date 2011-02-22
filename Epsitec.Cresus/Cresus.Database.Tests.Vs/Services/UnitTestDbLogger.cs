using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Services;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


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
					Assert.AreEqual (logEntry1.EntryId, logEntry2.EntryId);
					Assert.AreEqual (logEntry1.ConnectionId, logEntry2.ConnectionId);
					Assert.AreEqual (logEntry1.DateTime, logEntry2.DateTime);

					logger.RemoveLogEntry (dbId);

					Assert.IsFalse (logger.LogEntryExists (dbId));
				}
			}
		}


		[TestMethod]
		public void GetLatestLogEntry()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbLogger logger = dbInfrastructure.ServiceManager.Logger;

				List<DbId> sampleIds = this.GetDbIdSamples ().ToList ();

				Assert.IsNull (logger.GetLatestLogEntry ());

				for (int i = 0; i < sampleIds.Count; i++)
				{
					DbId dbId = sampleIds[i];
	
					DbLogEntry logEntry1 = logger.CreateLogEntry (dbId);
					DbLogEntry logEntry2 = logger.GetLatestLogEntry ();

					Assert.AreEqual (dbId, logEntry1.ConnectionId);
					Assert.AreEqual (logEntry1.EntryId, logEntry2.EntryId);
					Assert.AreEqual (logEntry1.ConnectionId, logEntry2.ConnectionId);
					Assert.AreEqual (logEntry1.DateTime, logEntry2.DateTime);
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
