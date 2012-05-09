using Epsitec.Common.Support;

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
    public class UnitTestEntityDeletionLog
    {


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

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
					() => new EntityDeletionLog (null, entityEngine.ServiceSchemaEngine)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new EntityDeletionLog (dbinfrastructure, null)
				);
			}
		}


		[TestMethod]
		public void CreateEntryArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				EntityDeletionLog log = new EntityDeletionLog (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => log.CreateEntry (DbId.Empty, Druid.FromLong (1), new DbId (1))
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => log.CreateEntry (new DbId (1), Druid.Empty, new DbId (1))
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => log.CreateEntry (new DbId (1), Druid.FromLong (1), DbId.Empty)
				);
			}
		}


		[TestMethod]
		public void GetEntriesNewerThanArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				EntityDeletionLog log = new EntityDeletionLog (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => log.GetEntriesNewerThan (DbId.Empty)
				);
			}
		}


		[TestMethod]
		public void CreateAndGetEntriesNewerThan()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				EntityDeletionLog log = new EntityDeletionLog (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				var samples = this.GetSamples ().ToList ();

				for (int i = 0; i < samples.Count; i++)
				{
					DbId entryId = new DbId (i + 1);
					DbId entityModificationEntryId = samples[i].Item1;
					Druid entityTypeId = samples[i].Item2;
					DbId entityId = samples[i].Item3;

					EntityDeletionEntry entry = log.CreateEntry (entityModificationEntryId, entityTypeId, entityId);

					Assert.AreEqual (entryId, entry.EntryId);
					Assert.AreEqual (entityModificationEntryId, entry.EntityModificationEntryId);
					Assert.AreEqual (entityTypeId, entry.EntityTypeId);
					Assert.AreEqual (entityId, entry.EntityId);
				}

				for (int i = 0; i < samples.Count; i++)
				{
					var entries = log.GetEntriesNewerThan (samples[i].Item1).ToList ();

					Assert.AreEqual (samples.Count - i, entries.Count);

					for (int j = 0; j < entries.Count; j++)
					{
						DbId entryId = new DbId (i + j + 1);
						DbId entityModificationEntryId = samples[i + j].Item1;
						Druid entityTypeId = samples[i + j].Item2;
						DbId entityId = samples[i + j].Item3;

						EntityDeletionEntry entry = entries[j];

						Assert.AreEqual (entryId, entry.EntryId);
						Assert.AreEqual (entityModificationEntryId, entry.EntityModificationEntryId);
						Assert.AreEqual (entityTypeId, entry.EntityTypeId);
						Assert.AreEqual (entityId, entry.EntityId);
					}
				}
			}
		}


		[TestMethod]
		public void Concurrency()
		{
			int nbThreads = 100;

			var entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

			var dbInfrastructures = Enumerable.Range (0, nbThreads)
				.Select (i => DbInfrastructureHelper.ConnectToTestDatabase ())
				.ToList ();

			try
			{
				System.DateTime time = System.DateTime.Now;

				var threads = dbInfrastructures.Select (d => new System.Threading.Thread (() =>
				{
					var dice = new System.Random (System.Threading.Thread.CurrentThread.ManagedThreadId);

					var log = new EntityDeletionLog (d, entityEngine.ServiceSchemaEngine);

					while (System.DateTime.Now - time <= System.TimeSpan.FromSeconds (15))
					{
						var entry = log.CreateEntry (new DbId (dice.Next ()), Druid.FromLong (dice.Next ()), new DbId (dice.Next ()));

						var id = System.Math.Max (entry.EntryId.Value - 10, 1);

						var entries = log.GetEntriesNewerThan (new DbId (id));
					}
				})).ToList ();

				foreach (var thread in threads)
				{
					thread.Start ();
				}

				foreach (var thread in threads)
				{
					thread.Join ();
				}
			}
			finally
			{
				foreach (var dbInfrastructure in dbInfrastructures)
				{
					dbInfrastructure.Dispose ();
				}
			}
		}


		private IEnumerable<System.Tuple<DbId, Druid, DbId>> GetSamples()
		{
			for (int i = 1; i < 10; i++)
			{
				DbId entityModificationEntryId = new DbId (i * 3 + 0);
				Druid entityTypeId = Druid.FromLong (i * 3 + 1);
				DbId entityId = new DbId (i * 3 + 2);

				yield return System.Tuple.Create (entityModificationEntryId, entityTypeId, entityId);
			}

		}


	}


}
