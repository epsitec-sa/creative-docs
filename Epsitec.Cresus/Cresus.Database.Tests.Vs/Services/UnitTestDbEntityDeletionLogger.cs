using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.Services;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database.UnitTests.Services
{


	[TestClass]
	public class UnitTestDbEntityDeletionLogger
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
				() => new DbEntityDeletionLogger (null)
			);
		}


		[TestMethod]
		public void CreateAndGetEntityDeletionEntry()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbEntityDeletionLogger logger = dbInfrastructure.ServiceManager.EntityDeletionLogger;

				var samples = this.GetSamples ().ToList ();

				for (int i = 0; i < samples.Count; i++)
				{
					DbId entryId = new DbId (i + 1);
					DbId logEntryId = samples[i].Item1;
					long instanceType = samples[i].Item2;
					DbId entityId = samples[i].Item3;

					DbEntityDeletionLogEntry entry = logger.CreateEntityDeletionLogEntry (logEntryId, instanceType, entityId);

					Assert.AreEqual (entryId, entry.EntryId);
					Assert.AreEqual (logEntryId, entry.LogEntryId);
					Assert.AreEqual (instanceType, entry.InstanceType);
					Assert.AreEqual (entityId, entry.EntityId);
				}

				for (int i = 0; i < samples.Count; i++)
				{
					var entries = logger.GetEntityDeletionLogEntries (samples[i].Item1).ToList ();

					Assert.AreEqual (samples.Count - i, entries.Count);

					for (int j = 0; j < entries.Count; j++)
					{
						DbId entryId = new DbId (i + j + 1);
						DbId logEntryId = samples[i + j].Item1;
						long instanceType = samples[i + j].Item2;
						DbId entityId = samples[i + j].Item3;

						DbEntityDeletionLogEntry entry = entries[j];

						Assert.AreEqual (entryId, entry.EntryId);
						Assert.AreEqual (logEntryId, entry.LogEntryId);
						Assert.AreEqual (instanceType, entry.InstanceType);
						Assert.AreEqual (entityId, entry.EntityId);
					}
				}
			}
		}


		private IEnumerable<System.Tuple<DbId, long, DbId>> GetSamples()
		{
			for (int i = 0; i < 10; i++)
			{
				DbId logEntryId = new DbId (i * 3 + 0);
				long instanceType = (i * 3 + 1);
				DbId entityId = new DbId (i * 3 + 2);

				yield return System.Tuple.Create (logEntryId, instanceType, entityId);
			}

		}


	}


}
