using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Logging;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Infrastructure
{


	[TestClass]
	public sealed class UnitTestDataInfrastructure
	{


		// TODO Add test about import/export
		// Marc


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
		public void GetDatabaseTime()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				System.DateTime t1 = dataInfrastructure.GetDatabaseTime ();
				System.DateTime t2 = dataInfrastructure.DbInfrastructure.GetDatabaseTime ();

				Assert.IsTrue (t1 - t2 <= System.TimeSpan.FromMilliseconds (100));
			}
		}


		[TestMethod]
		public void AddQueryLogArgumentCheck()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => dataInfrastructure.AddQueryLog (null)
				);
			}
		}


		[TestMethod]
		public void RemoveQueryLogArgumentCheck()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => dataInfrastructure.RemoveQueryLog (null)
				);
			}
		}


		[TestMethod]
		public void AddAndRemoveQueryLog()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				List<AbstractLog> logs = new List<AbstractLog> ();

				for (int i = 0; i < 10; i++)
				{
					MemoryLog log = new MemoryLog (10);

					logs.Add (log);
					dataInfrastructure.AddQueryLog (log);

					Assert.IsTrue (logs.SetEquals (dataInfrastructure.DbInfrastructure.QueryLogs));
				}

				while (logs.Any ())
				{
					AbstractLog log = logs[0];

					logs.RemoveAt (0);
					dataInfrastructure.RemoveQueryLog (log);

					Assert.IsTrue (logs.SetEquals (dataInfrastructure.DbInfrastructure.QueryLogs));
				}
			}
		}
		

		[TestMethod]
		public void ConnectionSimpleCase1()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();
			EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (access, entityEngine))
			{
				Assert.IsNull (dataInfrastructure.Connection);

				dataInfrastructure.OpenConnection ("connexion");

				Assert.AreEqual (ConnectionStatus.Open, dataInfrastructure.Connection.Status);
				Assert.AreEqual ("connexion", dataInfrastructure.Connection.Identity);

				dataInfrastructure.CloseConnection ();

				Assert.AreEqual (ConnectionStatus.Closed, dataInfrastructure.Connection.Status);
			}
		}


		[TestMethod]
		public void ConnectionSimpleCase2()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();
			EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

			DbId cId;

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (access, entityEngine))
			{
				Assert.IsNull (dataInfrastructure.Connection);

				dataInfrastructure.OpenConnection ("connexion");

				Assert.AreEqual (ConnectionStatus.Open, dataInfrastructure.Connection.Status);
				Assert.AreEqual ("connexion", dataInfrastructure.Connection.Identity);

				cId = dataInfrastructure.Connection.Id;
			}

			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				ConnectionManager manager = new ConnectionManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);

				Assert.AreEqual (ConnectionStatus.Closed, manager.GetConnection (cId).Status);
			}
		}


		[TestMethod]
		public void ConnectionSimpleCase3()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();
			EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (access, entityEngine))
			{
				Assert.IsNull (dataInfrastructure.Connection);

				dataInfrastructure.OpenConnection ("connexion");

				Assert.AreEqual (ConnectionStatus.Open, dataInfrastructure.Connection.Status);
				Assert.AreEqual ("connexion", dataInfrastructure.Connection.Identity);

				DbId cId = dataInfrastructure.Connection.Id;

				ConnectionManager manager = new ConnectionManager (dataInfrastructure.DbInfrastructure, entityEngine.ServiceSchemaEngine);

				manager.CloseConnection (cId);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => dataInfrastructure.CloseConnection ()
				);

				dataInfrastructure.Dispose ();
			}
		}


		[TestMethod]
		public void ConnectionSimpleCase4()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();
			EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (access, entityEngine))
			{
				Assert.IsNull (dataInfrastructure.Connection);

				for (int i = 0; i < 10; i++)
				{
					dataInfrastructure.OpenConnection ("connexion" + i);

					Assert.AreEqual (ConnectionStatus.Open, dataInfrastructure.Connection.Status);
					Assert.AreEqual ("connexion" + i, dataInfrastructure.Connection.Identity);

					dataInfrastructure.CloseConnection ();
				}
			}
		}


		[TestMethod]
		public void ConnectionInterruptionCase()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();
			EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (access, entityEngine))
			{
				Assert.IsNull (dataInfrastructure.Connection);

				dataInfrastructure.OpenConnection ("connexion");

				Assert.AreEqual (ConnectionStatus.Open, dataInfrastructure.Connection.Status);

				System.Threading.Thread.Sleep (3000);

				dataInfrastructure.RefreshConnectionData ();

				Assert.AreEqual (ConnectionStatus.Open, dataInfrastructure.Connection.Status);

				dataInfrastructure.KillDeadConnections (System.TimeSpan.FromSeconds (2));
				Assert.AreEqual (ConnectionStatus.Open, dataInfrastructure.Connection.Status);

				dataInfrastructure.RefreshConnectionData ();
				Assert.AreEqual (ConnectionStatus.Interrupted, dataInfrastructure.Connection.Status);
			}
		}


		[TestMethod]
		public void ConnectionInvalidBehavior()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase())
			{
				dataInfrastructure.CloseConnection ();

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => dataInfrastructure.KeepConnectionAlive ()
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => dataInfrastructure.KillDeadConnections (System.TimeSpan.FromSeconds(2))
				);
			}
		}


		[TestMethod]
		public void AreAllLocksAvailableArgumentCheck()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();
			EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (access, entityEngine))
			{
				dataInfrastructure.OpenConnection ("id");

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => dataInfrastructure.AreLocksAvailable (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.AreLocksAvailable (new List<string> () { })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.AreLocksAvailable (new List<string> () { "" })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.AreLocksAvailable (new List<string> () { null })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.AreLocksAvailable (new List<string> () { "l1", "l1" })
				);
			}
		}


		[TestMethod]
		public void CreateLockTransactionArgumentCheck()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();
			EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (access, entityEngine))
			{
				dataInfrastructure.OpenConnection ("id");

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => dataInfrastructure.CreateLockTransaction (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.CreateLockTransaction (new List<string> () { })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.CreateLockTransaction (new List<string> () { "" })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.CreateLockTransaction (new List<string> () { null })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.CreateLockTransaction (new List<string> () { "l1", "l1" })
				);
			}
		}


		[TestMethod]
		public void CreateLockTransactionAndAreLocksAvailableInvalidBehavior()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				dataInfrastructure.CloseConnection ();

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => dataInfrastructure.AreLocksAvailable (new List<string> () { "lock" })
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => dataInfrastructure.CreateLockTransaction (new List<string> () { "lock" })
				);
			}
		}


		[TestMethod]
		public void CreateLockTransactionAndAreLocksAvailable()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();
			EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

			using (DataInfrastructure dataInfrastructure1 = new DataInfrastructure (access, entityEngine))
			using (DataInfrastructure dataInfrastructure2 = new DataInfrastructure (access, entityEngine))
			{
				dataInfrastructure1.OpenConnection ("c1");
				dataInfrastructure2.OpenConnection("c2");

				Assert.IsTrue (dataInfrastructure1.AreLocksAvailable (new List<string> () { "l1", "l2" }));
				Assert.IsTrue (dataInfrastructure2.AreLocksAvailable (new List<string> () { "l1", "l2" }));

				using (LockTransaction l1 = dataInfrastructure1.CreateLockTransaction (new List<string> () { "l1" }))
				using (LockTransaction l2 = dataInfrastructure2.CreateLockTransaction (new List<string> () { "l2" }))
				{
					l1.Lock ();

					Assert.IsTrue (dataInfrastructure1.AreLocksAvailable (new List<string> () { "l1", "l2" }));
					Assert.IsFalse (dataInfrastructure2.AreLocksAvailable (new List<string> () { "l1", "l2" }));
					Assert.IsTrue (dataInfrastructure2.AreLocksAvailable (new List<string> () { "l2" }));

					l2.Lock ();

					Assert.IsFalse (dataInfrastructure1.AreLocksAvailable (new List<string> () { "l1", "l2" }));
					Assert.IsFalse (dataInfrastructure2.AreLocksAvailable (new List<string> () { "l1", "l2" }));
					Assert.IsTrue (dataInfrastructure1.AreLocksAvailable (new List<string> () { "l1" }));
					Assert.IsTrue (dataInfrastructure2.AreLocksAvailable (new List<string> () { "l2" }));

					l1.Release ();

					Assert.IsFalse (dataInfrastructure1.AreLocksAvailable (new List<string> () { "l1", "l2" }));
					Assert.IsTrue (dataInfrastructure2.AreLocksAvailable (new List<string> () { "l1", "l2" }));
					Assert.IsTrue (dataInfrastructure1.AreLocksAvailable (new List<string> () { "l1" }));

					l2.Release ();

					Assert.IsTrue (dataInfrastructure1.AreLocksAvailable (new List<string> () { "l1", "l2" }));
					Assert.IsTrue (dataInfrastructure2.AreLocksAvailable (new List<string> () { "l1", "l2" }));
				}
			}
		}


		[TestMethod]
		public void CleanInactiveLocks()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();
			EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

			using (DataInfrastructure dataInfrastructure1 = new DataInfrastructure (access, entityEngine))
			using (DataInfrastructure dataInfrastructure2 = new DataInfrastructure (access, entityEngine))
			using (DataInfrastructure dataInfrastructure3 = new DataInfrastructure (access, entityEngine))
			using (DataInfrastructure dataInfrastructure4 = new DataInfrastructure (access, entityEngine))
			{
				dataInfrastructure1.OpenConnection ("1");
				dataInfrastructure2.OpenConnection ("2");
				dataInfrastructure3.OpenConnection ("3");
				dataInfrastructure4.OpenConnection ("4");

				Assert.IsTrue (dataInfrastructure4.AreLocksAvailable (new List<string> { "1" }));
				Assert.IsTrue (dataInfrastructure4.AreLocksAvailable (new List<string> { "2" }));
				Assert.IsTrue (dataInfrastructure4.AreLocksAvailable (new List<string> { "3" }));

				using (LockTransaction t1 = dataInfrastructure1.CreateLockTransaction (new List<string> { "1" }))
				using (LockTransaction t2 = dataInfrastructure2.CreateLockTransaction (new List<string> { "2" }))
				using (LockTransaction t3 = dataInfrastructure3.CreateLockTransaction (new List<string> { "3" }))
				{

					t1.Lock ();
					t2.Lock ();
					t3.Lock ();

					Assert.IsFalse (dataInfrastructure4.AreLocksAvailable (new List<string> { "1" }));
					Assert.IsFalse (dataInfrastructure4.AreLocksAvailable (new List<string> { "2" }));
					Assert.IsFalse (dataInfrastructure4.AreLocksAvailable (new List<string> { "3" }));

					dataInfrastructure2.CloseConnection ();

					Assert.IsFalse (dataInfrastructure4.AreLocksAvailable (new List<string> { "1" }));
					Assert.IsFalse (dataInfrastructure4.AreLocksAvailable (new List<string> { "2" }));
					Assert.IsFalse (dataInfrastructure4.AreLocksAvailable (new List<string> { "3" }));

					for (int i = 0; i < 3; i++)
					{
						dataInfrastructure1.KeepConnectionAlive ();
						dataInfrastructure4.KeepConnectionAlive ();
						dataInfrastructure4.KillDeadConnections (System.TimeSpan.FromSeconds (3));

						System.Threading.Thread.Sleep (1000);
					}

					dataInfrastructure4.KeepConnectionAlive ();
					dataInfrastructure4.KillDeadConnections (System.TimeSpan.FromSeconds (3));

					Assert.IsFalse (dataInfrastructure4.AreLocksAvailable (new List<string> { "1" }));
					Assert.IsTrue (dataInfrastructure4.AreLocksAvailable (new List<string> { "2" }));
					Assert.IsTrue (dataInfrastructure4.AreLocksAvailable (new List<string> { "3" }));

					dataInfrastructure1.CloseConnection ();

					System.Threading.Thread.Sleep (3000);

					dataInfrastructure4.KeepConnectionAlive ();
					dataInfrastructure4.KillDeadConnections (System.TimeSpan.FromSeconds (3));

					Assert.IsTrue (dataInfrastructure4.AreLocksAvailable (new List<string> { "1" }));
					Assert.IsTrue (dataInfrastructure4.AreLocksAvailable (new List<string> { "2" }));
					Assert.IsTrue (dataInfrastructure4.AreLocksAvailable (new List<string> { "3" }));

					dataInfrastructure4.CloseConnection ();
				}
			}
		}


		[TestMethod]
		public void GetDatabaseInfoArgumentCheck()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.GetDatabaseInfo (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.GetDatabaseInfo ("")
				);
			}
		}


		[TestMethod]
		public void SetDatabaseInfoArgumentCheck()
		{		
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.SetDatabaseInfo (null, "test")
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.SetDatabaseInfo ("", "test")
				);
			}
		}


		[TestMethod]
		public void GetSetAndExistsInfoInvalidBehavior()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				dataInfrastructure.CloseConnection ();

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => dataInfrastructure.GetDatabaseInfo ("key")
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => dataInfrastructure.SetDatabaseInfo ("key", "value")
				);
			}
		}


		[TestMethod]
		public void GetAndSetInfo()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				Dictionary<string, string> info = new Dictionary<string, string> ();

				for (int i = 0; i < 10; i++)
				{
					info[this.GetRandomString ()] = this.GetRandomString ();
				}

				foreach (string key in info.Keys)
				{
					Assert.IsNull (dataInfrastructure.GetDatabaseInfo (key));
				}

				for (int i = 0; i < 10; i++)
				{
					foreach (string key in info.Keys.ToList ())
					{
						dataInfrastructure.SetDatabaseInfo (key, info[key]);

						Assert.AreEqual (info[key], dataInfrastructure.GetDatabaseInfo (key));

						info[key] = this.GetRandomString ();
					}
				}

				foreach (string key in info.Keys)
				{
					dataInfrastructure.SetDatabaseInfo (key, null);
				}

				foreach (string key in info.Keys)
				{
					Assert.IsNull (dataInfrastructure.GetDatabaseInfo (key));
				}
			}
		}


		[TestMethod]
		public void UidGeneratorInvalidBehaviorTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				dataInfrastructure.CloseConnection ();

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => dataInfrastructure.GetUidGenerator ("name")
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => dataInfrastructure.DeleteUidGenerator ("name")
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => dataInfrastructure.DoesUidGeneratorExists ("name")
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => dataInfrastructure.CreateUidGenerator ("name", new List<UidSlot> () { new UidSlot (1, 2) })
				);
			}
		}


		[TestMethod]
		public void CreateAndExistsUidGeneratorTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				for (int i = 0; i < 2; i++)
				{
					string name = "myCounter" + i;

					List<UidSlot> slots = new List<UidSlot> ()
					{
						new UidSlot (10, 19),
						new UidSlot (20, 29),
						new UidSlot (0, 9),
					};

					Assert.IsFalse (dataInfrastructure.DoesUidGeneratorExists (name));

					var generator = dataInfrastructure.CreateUidGenerator (name, slots);

					Assert.AreEqual ("myCounter" + i, generator.Name);
					Assert.AreEqual (3, generator.Slots.Count);

					Assert.AreEqual (0, generator.Slots[0].MinValue);
					Assert.AreEqual (9, generator.Slots[0].MaxValue);
					Assert.AreEqual (10, generator.Slots[1].MinValue);
					Assert.AreEqual (19, generator.Slots[1].MaxValue);
					Assert.AreEqual (20, generator.Slots[2].MinValue);
					Assert.AreEqual (29, generator.Slots[2].MaxValue);

					Assert.IsTrue (dataInfrastructure.DoesUidGeneratorExists (name));
				}
			}
		}


		[TestMethod]
		public void DeleteAndExistsUidGeneratorTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				List<int> countersRemoved = new List<int> ();
				List<int> countersCreated = new List<int> ();

				for (int i = 0; i < 10; i++)
				{
					countersRemoved.Add (i);
				}

				for (int i = 0; i < 10; i++)
				{
					string name = "myCounter";

					List<UidSlot> slots = new List<UidSlot> ()
		            {
		                new UidSlot (0, 9),
		                new UidSlot (20, 29),
		                new UidSlot (10, 19),
		            };

					dataInfrastructure.CreateUidGenerator (name + i, slots);

					countersCreated.Add (i);
					countersRemoved.Remove (i);

					foreach (int j in countersCreated)
					{
						Assert.IsTrue (dataInfrastructure.DoesUidGeneratorExists (name + j));
					}

					foreach (int j in countersRemoved)
					{
						Assert.IsFalse (dataInfrastructure.DoesUidGeneratorExists (name + j));
					}
				}

				for (int i = 0; i < 10; i++)
				{
					string name = "myCounter";

					dataInfrastructure.DeleteUidGenerator (name + i);

					countersCreated.Remove (i);
					countersRemoved.Add (i);

					foreach (int j in countersCreated)
					{
						Assert.IsTrue (dataInfrastructure.DoesUidGeneratorExists(name + j));
					}

					foreach (int j in countersRemoved)
					{
						Assert.IsFalse (dataInfrastructure.DoesUidGeneratorExists (name + j));
					}
				}
			}
		}


		[TestMethod]
		public void GetUidGeneratorTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				for (int i = 0; i < 10; i++)
				{
					string name = "myCounter" + i;

					List<UidSlot> slots = new List<UidSlot> ()
		            {
		                new UidSlot (20, 29),
		                new UidSlot (0, 9),
		                new UidSlot (10, 19),
		            };

					dataInfrastructure.CreateUidGenerator (name, slots);
				}

				for (int i = 0; i < 10; i++)
				{
					string name = "myCounter" + i;

					UidGenerator generator = dataInfrastructure.GetUidGenerator (name);

					Assert.IsNotNull (generator);
					Assert.AreEqual (generator.Name, name);
					Assert.AreEqual (generator.Slots.Count (), 3);
					Assert.AreEqual (0, generator.Slots.ElementAt (0).MinValue);
					Assert.AreEqual (9, generator.Slots.ElementAt (0).MaxValue);
					Assert.AreEqual (10, generator.Slots.ElementAt (1).MinValue);
					Assert.AreEqual (19, generator.Slots.ElementAt (1).MaxValue);
					Assert.AreEqual (20, generator.Slots.ElementAt (2).MinValue);
					Assert.AreEqual (29, generator.Slots.ElementAt (2).MaxValue);
				}
			}
		}


		[TestMethod]
		public void GetUidGeneratorNextUidTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				for (int i = 0; i < 3; i++)
				{
					string name = "myCounter" + i;

					List<UidSlot> slots = new List<UidSlot> ()
		            {
		                new UidSlot (20, 29),
		                new UidSlot (0, 9),
		                new UidSlot (10, 19),
		            };

					dataInfrastructure.CreateUidGenerator (name, slots);
				}

				for (int i = 0; i < 3; i++)
				{
					string name = "myCounter" + i;

					UidGenerator generator = dataInfrastructure.GetUidGenerator (name);

					for (int j = 0; j < 30; j++)
					{
						Assert.AreEqual (j, generator.GetNextUid ());
					}

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => generator.GetNextUid ()
					);
				}
			}
		}


		[TestMethod]
		public void CreateEntityDeletionEntryArgumentCheck()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.CreateEntityDeletionEntry (DbId.Empty, Druid.FromLong (1), new DbId (1))
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.CreateEntityDeletionEntry (new DbId(1), Druid.Empty, new DbId (1))
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.CreateEntityDeletionEntry (new DbId (1), Druid.FromLong (1), DbId.Empty)
				);
			}
		}


		[TestMethod]
		public void GetEntityDeletionEntriesNewerThanArgumentCheck()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.GetEntityDeletionEntriesNewerThan (DbId.Empty)
				);
			}
		}


		[TestMethod]
		public void EntityDeletionEntryInvalidBehavior()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				dataInfrastructure.CloseConnection ();

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => dataInfrastructure.CreateEntityDeletionEntry (new DbId (1), Druid.FromLong (1), new DbId (1))
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => dataInfrastructure.GetEntityDeletionEntriesNewerThan (new DbId (1))
				);
			}
		}


		[TestMethod]
		public void EntityDeletionEntryTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				List<EntityDeletionEntry> entries = new List<EntityDeletionEntry> ();

				for (int i = 1; i < 10; i++)
				{
					var entry = dataInfrastructure.CreateEntityDeletionEntry (new DbId (i), Druid.FromLong (i + 1), new DbId (i + 2));

					entries.Add (entry);

					for (int j = 1; j <= i; j++)
					{
						var data = dataInfrastructure.GetEntityDeletionEntriesNewerThan (new DbId (j));

						CollectionAssert.AreEqual (entries.Skip (j - 1).Select (e => e.EntryId).ToList (), data.Select (e => e.EntryId).ToList ());
					}
				}
			}
		}


		[TestMethod]
		public void EntityModificationEntryInvalidBehavior()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				dataInfrastructure.CloseConnection ();

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => dataInfrastructure.CreateEntityModificationEntry ()
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => dataInfrastructure.GetLatestEntityModificationEntry ()
				);
			}
		}


		[TestMethod]
		public void EntityModificationEntryTest()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				for (int i = 1; i < 10; i++)
				{
					var entry = dataInfrastructure.CreateEntityModificationEntry ();

					var data = dataInfrastructure.GetLatestEntityModificationEntry ();

					Assert.AreEqual (entry.EntryId, data.EntryId);
				}
			}
		}


		[TestMethod]
		public void DeleteDataContextArgumentCheck()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => dataInfrastructure.DeleteDataContext (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.DeleteDataContext (new DataContext (dataInfrastructure))
				);
			}
		}


		[TestMethod]
		public void ContainsDataContextArgumentCheck()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => dataInfrastructure.ContainsDataContext (null)
				);
			}
		}


		[TestMethod]
		public void DataContextTests()
		{
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				List<DataContext> dataContexts = new List<DataContext> ();

				for (int i = 0; i < 10; i++)
				{
					DataContext dataContext = dataInfrastructure.CreateDataContext ();

					dataContexts.Add (dataContext);

					foreach (DataContext d in dataContexts)
					{
						Assert.IsTrue (dataInfrastructure.ContainsDataContext (d));
					}
				}

				for (int i = 0; i < dataContexts.Count; i++)
				{
					DataContext dataContext = dataContexts[0];

					Assert.IsTrue (dataInfrastructure.ContainsDataContext (dataContext));
					Assert.IsFalse (dataContext.IsDisposed);

					dataContexts.Remove (dataContext);
					dataInfrastructure.DeleteDataContext (dataContext);

					Assert.IsFalse (dataInfrastructure.ContainsDataContext (dataContext));
					Assert.IsTrue (dataContext.IsDisposed);

					foreach (DataContext d in dataContexts)
					{
						Assert.IsTrue (dataInfrastructure.ContainsDataContext (d));
					}
				}
			}
		}


		private string GetRandomString()
		{
			return this.dice.Next ().ToString ();
		}


		private System.Random dice = new System.Random ();


	}


}
