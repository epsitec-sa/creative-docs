using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Infrastructure
{


	[TestClass]
	public class UnitTestEntityModificationLog
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseCreator2.ResetEmptyTestDatabase ();
		}


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new EntityModificationLog (null, entityEngine.ServiceSchemaEngine)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new EntityModificationLog (dbinfrastructure, null)
				);
			}
		}


		[TestMethod]
		public void CreateEntryArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				EntityModificationLog log = new EntityModificationLog (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => log.CreateEntry (DbId.Empty)
				);
			}
		}


		[TestMethod]
		public void GetEntryArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				EntityModificationLog log = new EntityModificationLog (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => log.GetEntry (DbId.Empty)
				);
			}
		}


		[TestMethod]
		public void DeleteEntryArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				EntityModificationLog log = new EntityModificationLog (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => log.DeleteEntry (DbId.Empty)
				);
			}
		}


		[TestMethod]
		public void DoesEntryExistsArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				EntityModificationLog log = new EntityModificationLog (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => log.DoesEntryExists (DbId.Empty)
				);
			}
		}


		[TestMethod]
		public void CreateExistsGetDeleteLogEntry()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				EntityModificationLog log = new EntityModificationLog (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				foreach (DbId dbId in this.GetDbIdSamples ())
				{
					Assert.IsFalse (log.DoesEntryExists (dbId));

					EntityModificationEntry logEntry1 = log.CreateEntry (dbId);

					Assert.IsTrue (log.DoesEntryExists (dbId));

					EntityModificationEntry logEntry2 = log.GetEntry (logEntry1.EntryId);

					Assert.AreEqual (dbId, logEntry1.ConnectionId);
					Assert.AreEqual (logEntry1.EntryId, logEntry2.EntryId);
					Assert.AreEqual (logEntry1.ConnectionId, logEntry2.ConnectionId);
					Assert.AreEqual (logEntry1.Time, logEntry2.Time);

					log.DeleteEntry (dbId);

					Assert.IsFalse (log.DoesEntryExists (dbId));
				}
			}
		}


		[TestMethod]
		public void GetLatestLogEntry()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				EntityModificationLog log = new EntityModificationLog (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				List<DbId> sampleIds = this.GetDbIdSamples ().ToList ();

				Assert.IsNull (log.GetLatestEntry ());

				for (int i = 0; i < sampleIds.Count; i++)
				{
					DbId dbId = sampleIds[i];

					EntityModificationEntry logEntry1 = log.CreateEntry (dbId);
					EntityModificationEntry logEntry2 = log.GetLatestEntry ();

					Assert.AreEqual (dbId, logEntry1.ConnectionId);
					Assert.AreEqual (logEntry1.EntryId, logEntry2.EntryId);
					Assert.AreEqual (logEntry1.ConnectionId, logEntry2.ConnectionId);
					Assert.AreEqual (logEntry1.Time, logEntry2.Time);
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
