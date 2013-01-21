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
			using (DB db = DB.ConnectToTestDatabase ())
			{
				System.DateTime t1 = db.DataInfrastructure.GetDatabaseTime ();
				System.DateTime t2 = db.DataInfrastructure.DbInfrastructure.GetDatabaseTime ();

				Assert.IsTrue (t1 - t2 <= System.TimeSpan.FromMilliseconds (100));
			}
		}


		[TestMethod]
		public void AddQueryLogArgumentCheck()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => db.DataInfrastructure.AddQueryLog (null)
				);
			}
		}


		[TestMethod]
		public void RemoveQueryLogArgumentCheck()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => db.DataInfrastructure.RemoveQueryLog (null)
				);
			}
		}


		[TestMethod]
		public void AddAndRemoveQueryLog()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				List<AbstractLog> logs = new List<AbstractLog> ();

				for (int i = 0; i < 10; i++)
				{
					MemoryLog log = new MemoryLog (10);

					logs.Add (log);
					db.DataInfrastructure.AddQueryLog (log);

					Assert.IsTrue (logs.SetEquals (db.DataInfrastructure.DbInfrastructure.QueryLogs));
				}

				while (logs.Any ())
				{
					AbstractLog log = logs[0];

					logs.RemoveAt (0);
					db.DataInfrastructure.RemoveQueryLog (log);

					Assert.IsTrue (logs.SetEquals (db.DataInfrastructure.DbInfrastructure.QueryLogs));
				}
			}
		}


		[TestMethod]
		public void ConnectionSimpleCase1()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();
			
			using (DbInfrastructure dbInfrastructure = DbInfrastructure.Connect (access))
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure, entityEngine))
				{
					Assert.IsNull (dataInfrastructure.Connection);

					dataInfrastructure.OpenConnection ("connexion");

					Assert.AreEqual (ConnectionStatus.Open, dataInfrastructure.Connection.Status);
					Assert.AreEqual ("connexion", dataInfrastructure.Connection.Identity);

					dataInfrastructure.CloseConnection ();

					Assert.AreEqual (ConnectionStatus.Closed, dataInfrastructure.Connection.Status);
				}
			}
		}


		[TestMethod]
		public void ConnectionSimpleCase2()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();
			
			using (DbInfrastructure dbInfrastructure = DbInfrastructure.Connect (access))
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				DbId cId;

				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure, entityEngine))
				{
					Assert.IsNull (dataInfrastructure.Connection);

					dataInfrastructure.OpenConnection ("connexion");

					Assert.AreEqual (ConnectionStatus.Open, dataInfrastructure.Connection.Status);
					Assert.AreEqual ("connexion", dataInfrastructure.Connection.Identity);

					cId = dataInfrastructure.Connection.Id;
				}

				ConnectionManager manager = new ConnectionManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);

				Assert.AreEqual (ConnectionStatus.Closed, manager.GetConnection (cId).Status);
			}
		}


		[TestMethod]
		public void ConnectionSimpleCase3()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();

			using (DbInfrastructure dbInfrastructure = DbInfrastructure.Connect (access))
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure, entityEngine))
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
		}


		[TestMethod]
		public void ConnectionSimpleCase4()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();

			using (DbInfrastructure dbInfrastructure = DbInfrastructure.Connect (access))
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure, entityEngine))
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
		}


		[TestMethod]
		public void ConnectionInterruptionCase()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();

			using (DbInfrastructure dbInfrastructure = DbInfrastructure.Connect (access))
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure, entityEngine))
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
		}


		[TestMethod]
		public void ConnectionInvalidBehavior()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				db.DataInfrastructure.CloseConnection ();

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => db.DataInfrastructure.KeepConnectionAlive ()
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => db.DataInfrastructure.KillDeadConnections (System.TimeSpan.FromSeconds (2))
				);
			}
		}


		[TestMethod]
		public void AreAllLocksAvailableArgumentCheck()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();

			using (DbInfrastructure dbInfrastructure = DbInfrastructure.Connect (access))
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure, entityEngine))
				{
					dataInfrastructure.OpenConnection ("id");

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => dataInfrastructure.AreLocksAvailable (null)
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataInfrastructure.AreLocksAvailable (new List<string> ()
						{
						})
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
		}


		[TestMethod]
		public void CreateLockTransactionArgumentCheck()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();

			using (DbInfrastructure dbInfrastructure = DbInfrastructure.Connect (access))
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure, entityEngine))
				{
					dataInfrastructure.OpenConnection ("id");

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => dataInfrastructure.CreateLockTransaction (null)
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataInfrastructure.CreateLockTransaction (new List<string> ()
						{
						})
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
		}


		[TestMethod]
		public void CreateLockTransactionAndAreLocksAvailableInvalidBehavior()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				db.DataInfrastructure.CloseConnection ();

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => db.DataInfrastructure.AreLocksAvailable (new List<string> () { "lock" })
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => db.DataInfrastructure.CreateLockTransaction (new List<string> () { "lock" })
				);
			}
		}


		[TestMethod]
		public void CreateLockTransactionAndAreLocksAvailable()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();

			using (DbInfrastructure dbInfrastructure1 = DbInfrastructure.Connect (access))
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructure.Connect (access))
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure1);

				using (DataInfrastructure dataInfrastructure1 = new DataInfrastructure (dbInfrastructure1, entityEngine))
				using (DataInfrastructure dataInfrastructure2 = new DataInfrastructure (dbInfrastructure2, entityEngine))
				{
					dataInfrastructure1.OpenConnection ("c1");
					dataInfrastructure2.OpenConnection ("c2");

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
		}


		[TestMethod]
		public void CleanInactiveLocks()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();

			using (DbInfrastructure dbInfrastructure1 = DbInfrastructure.Connect (access))
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructure.Connect (access))
			using (DbInfrastructure dbInfrastructure3 = DbInfrastructure.Connect (access))
			using (DbInfrastructure dbInfrastructure4 = DbInfrastructure.Connect (access))
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure1);

				using (DataInfrastructure dataInfrastructure1 = new DataInfrastructure (dbInfrastructure1, entityEngine))
				using (DataInfrastructure dataInfrastructure2 = new DataInfrastructure (dbInfrastructure2, entityEngine))
				using (DataInfrastructure dataInfrastructure3 = new DataInfrastructure (dbInfrastructure3, entityEngine))
				using (DataInfrastructure dataInfrastructure4 = new DataInfrastructure (dbInfrastructure4, entityEngine))
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
		}


		[TestMethod]
		public void GetDatabaseInfoArgumentCheck()
		{			
			using (DB db = DB.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => db.DataInfrastructure.GetDatabaseInfo (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => db.DataInfrastructure.GetDatabaseInfo ("")
				);
			}
		}


		[TestMethod]
		public void SetDatabaseInfoArgumentCheck()
		{		
			using (DB db = DB.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => db.DataInfrastructure.SetDatabaseInfo (null, "test")
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => db.DataInfrastructure.SetDatabaseInfo ("", "test")
				);
			}
		}


		[TestMethod]
		public void GetSetAndExistsInfoInvalidBehavior()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				db.DataInfrastructure.CloseConnection ();

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => db.DataInfrastructure.GetDatabaseInfo ("key")
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => db.DataInfrastructure.SetDatabaseInfo ("key", "value")
				);
			}
		}


		[TestMethod]
		public void GetAndSetInfo()
		{			
			using (DB db = DB.ConnectToTestDatabase ())
			{
				Dictionary<string, string> info = new Dictionary<string, string> ();

				for (int i = 0; i < 10; i++)
				{
					info[this.GetRandomString ()] = this.GetRandomString ();
				}

				foreach (string key in info.Keys)
				{
					Assert.IsNull (db.DataInfrastructure.GetDatabaseInfo (key));
				}

				for (int i = 0; i < 10; i++)
				{
					foreach (string key in info.Keys.ToList ())
					{
						db.DataInfrastructure.SetDatabaseInfo (key, info[key]);

						Assert.AreEqual (info[key], db.DataInfrastructure.GetDatabaseInfo (key));

						info[key] = this.GetRandomString ();
					}
				}

				foreach (string key in info.Keys)
				{
					db.DataInfrastructure.SetDatabaseInfo (key, null);
				}

				foreach (string key in info.Keys)
				{
					Assert.IsNull (db.DataInfrastructure.GetDatabaseInfo (key));
				}
			}
		}


		[TestMethod]
		public void UidGeneratorInvalidBehaviorTest()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				db.DataInfrastructure.CloseConnection ();

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => db.DataInfrastructure.GetUidGenerator ("name")
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => db.DataInfrastructure.DeleteUidGenerator ("name")
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => db.DataInfrastructure.DoesUidGeneratorExists ("name")
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => db.DataInfrastructure.CreateUidGenerator ("name", new List<UidSlot> () { new UidSlot (1, 2) })
				);
			}
		}


		[TestMethod]
		public void CreateAndExistsUidGeneratorTest()
		{
			using (DB db = DB.ConnectToTestDatabase ())
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

					Assert.IsFalse (db.DataInfrastructure.DoesUidGeneratorExists (name));

					var generator = db.DataInfrastructure.CreateUidGenerator (name, slots);

					Assert.AreEqual ("myCounter" + i, generator.Name);
					Assert.AreEqual (3, generator.Slots.Count);

					Assert.AreEqual (0, generator.Slots[0].MinValue);
					Assert.AreEqual (9, generator.Slots[0].MaxValue);
					Assert.AreEqual (10, generator.Slots[1].MinValue);
					Assert.AreEqual (19, generator.Slots[1].MaxValue);
					Assert.AreEqual (20, generator.Slots[2].MinValue);
					Assert.AreEqual (29, generator.Slots[2].MaxValue);

					Assert.IsTrue (db.DataInfrastructure.DoesUidGeneratorExists (name));
				}
			}
		}


		[TestMethod]
		public void DeleteAndExistsUidGeneratorTest()
		{
			using (DB db = DB.ConnectToTestDatabase ())
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

					db.DataInfrastructure.CreateUidGenerator (name + i, slots);

					countersCreated.Add (i);
					countersRemoved.Remove (i);

					foreach (int j in countersCreated)
					{
						Assert.IsTrue (db.DataInfrastructure.DoesUidGeneratorExists (name + j));
					}

					foreach (int j in countersRemoved)
					{
						Assert.IsFalse (db.DataInfrastructure.DoesUidGeneratorExists (name + j));
					}
				}

				for (int i = 0; i < 10; i++)
				{
					string name = "myCounter";

					db.DataInfrastructure.DeleteUidGenerator (name + i);

					countersCreated.Remove (i);
					countersRemoved.Add (i);

					foreach (int j in countersCreated)
					{
						Assert.IsTrue (db.DataInfrastructure.DoesUidGeneratorExists (name + j));
					}

					foreach (int j in countersRemoved)
					{
						Assert.IsFalse (db.DataInfrastructure.DoesUidGeneratorExists (name + j));
					}
				}
			}
		}


		[TestMethod]
		public void GetUidGeneratorTest()
		{
			using (DB db = DB.ConnectToTestDatabase ())
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

					db.DataInfrastructure.CreateUidGenerator (name, slots);
				}

				for (int i = 0; i < 10; i++)
				{
					string name = "myCounter" + i;

					UidGenerator generator = db.DataInfrastructure.GetUidGenerator (name);

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
			using (DB db = DB.ConnectToTestDatabase ())
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

					db.DataInfrastructure.CreateUidGenerator (name, slots);
				}

				for (int i = 0; i < 3; i++)
				{
					string name = "myCounter" + i;

					UidGenerator generator = db.DataInfrastructure.GetUidGenerator (name);

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
			using (DB db = DB.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => db.DataInfrastructure.CreateEntityDeletionEntry (DbId.Empty, Druid.FromLong (1), new DbId (1))
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => db.DataInfrastructure.CreateEntityDeletionEntry (new DbId (1), Druid.Empty, new DbId (1))
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => db.DataInfrastructure.CreateEntityDeletionEntry (new DbId (1), Druid.FromLong (1), DbId.Empty)
				);
			}
		}


		[TestMethod]
		public void GetEntityDeletionEntriesNewerThanArgumentCheck()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => db.DataInfrastructure.GetEntityDeletionEntriesNewerThan (DbId.Empty)
				);
			}
		}


		[TestMethod]
		public void EntityDeletionEntryInvalidBehavior()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				db.DataInfrastructure.CloseConnection ();

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => db.DataInfrastructure.CreateEntityDeletionEntry (new DbId (1), Druid.FromLong (1), new DbId (1))
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => db.DataInfrastructure.GetEntityDeletionEntriesNewerThan (new DbId (1))
				);
			}
		}


		[TestMethod]
		public void EntityDeletionEntryTest()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				List<EntityDeletionEntry> entries = new List<EntityDeletionEntry> ();

				for (int i = 1; i < 10; i++)
				{
					var entry = db.DataInfrastructure.CreateEntityDeletionEntry (new DbId (i), Druid.FromLong (i + 1), new DbId (i + 2));

					entries.Add (entry);

					for (int j = 1; j <= i; j++)
					{
						var data = db.DataInfrastructure.GetEntityDeletionEntriesNewerThan (new DbId (j));

						CollectionAssert.AreEqual (entries.Skip (j - 1).Select (e => e.EntryId).ToList (), data.Select (e => e.EntryId).ToList ());
					}
				}
			}
		}


		[TestMethod]
		public void EntityModificationEntryInvalidBehavior()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				db.DataInfrastructure.CloseConnection ();

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => db.DataInfrastructure.CreateEntityModificationEntry ()
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => db.DataInfrastructure.GetLatestEntityModificationEntry ()
				);
			}
		}


		[TestMethod]
		public void EntityModificationEntryTest()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				for (int i = 1; i < 10; i++)
				{
					var entry = db.DataInfrastructure.CreateEntityModificationEntry ();

					var data = db.DataInfrastructure.GetLatestEntityModificationEntry ();

					Assert.AreEqual (entry.EntryId, data.EntryId);
				}
			}
		}


		[TestMethod]
		public void DeleteDataContextArgumentCheck()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => db.DataInfrastructure.DeleteDataContext (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => db.DataInfrastructure.DeleteDataContext (new DataContext (db.DataInfrastructure))
				);
			}
		}


		[TestMethod]
		public void ContainsDataContextArgumentCheck()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => db.DataInfrastructure.ContainsDataContext (null)
				);
			}
		}


		[TestMethod]
		public void DataContextTests()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				List<DataContext> dataContexts = new List<DataContext> ();

				for (int i = 0; i < 10; i++)
				{
					DataContext dataContext = db.DataInfrastructure.CreateDataContext ();

					dataContexts.Add (dataContext);

					foreach (DataContext d in dataContexts)
					{
						Assert.IsTrue (db.DataInfrastructure.ContainsDataContext (d));
					}
				}

				for (int i = 0; i < dataContexts.Count; i++)
				{
					DataContext dataContext = dataContexts[0];

					Assert.IsTrue (db.DataInfrastructure.ContainsDataContext (dataContext));
					Assert.IsFalse (dataContext.IsDisposed);

					dataContexts.Remove (dataContext);
					db.DataInfrastructure.DeleteDataContext (dataContext);

					Assert.IsFalse (db.DataInfrastructure.ContainsDataContext (dataContext));
					Assert.IsTrue (dataContext.IsDisposed);

					foreach (DataContext d in dataContexts)
					{
						Assert.IsTrue (db.DataInfrastructure.ContainsDataContext (d));
					}
				}
			}
		}


		[TestMethod]
		public void ThreadSafetyTest()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				var dice = new System.Random (System.Threading.Thread.CurrentThread.ManagedThreadId);
				var time = System.DateTime.Now;
				var duration = System.TimeSpan.FromSeconds (15);
				List<System.Threading.Thread> threads = new List<System.Threading.Thread> ();

				threads.Add (new System.Threading.Thread (() =>
				{
					Dictionary<string, string> data = new Dictionary<string, string> ();
					List<LockTransaction> lockTransactions = new List<LockTransaction> ();

					while (System.DateTime.Now - time <= duration)
					{
						var key = System.Guid.NewGuid ().ToString ();
						var value = System.Guid.NewGuid ().ToString ();

						if (db.DataInfrastructure.Connection != null)
						{
							db.DataInfrastructure.RefreshConnectionData ();
						}

						if (db.DataInfrastructure.Connection != null && db.DataInfrastructure.Connection.Status == ConnectionStatus.Open)
						{
							if (dice.NextDouble () > 0.999)
							{
								try
								{
									db.DataInfrastructure.CloseConnection ();
								}
								catch (System.InvalidOperationException)
								{
									// dataInfrastructure is probably deconnected. Let's ignore that exception
									// Marc
								}
							}
						}

						if (db.DataInfrastructure.Connection == null || db.DataInfrastructure.Connection.Status != ConnectionStatus.Open)
						{
							try
							{
								db.DataInfrastructure.OpenConnection ("myId");
							}
							catch (System.InvalidOperationException)
							{
								// dataInfrastructure is probably deconnected. Let's ignore that exception
								// Marc
							}
						}

						try
						{
							var lockNames = new List<string> ()
						        {
						            System.Guid.NewGuid().ToString (),
						            System.Guid.NewGuid().ToString (),
						            System.Guid.NewGuid().ToString (),
						        };

							LockTransaction lockTransaction = db.DataInfrastructure.CreateLockTransaction (lockNames);

							lockTransactions.Add (lockTransaction);

							lockTransaction.Lock ();
						}
						catch (System.InvalidOperationException)
						{
							// dataInfrastructure is probably deconnected. Let's ignore that exception
							// Marc
						}

						try
						{
							if (dice.NextDouble () > 0.75 && lockTransactions.Count > 0)
							{
								int index = dice.Next (0, lockTransactions.Count);

								LockTransaction lockTransaction = lockTransactions[index];
								lockTransactions.RemoveAt (index);

								lockTransaction.Dispose ();
							}
						}
						catch (System.InvalidOperationException)
						{
							// dataInfrastructure is probably deconnected. Let's ignore that exception
							// Marc
						}

						try
						{
							db.DataInfrastructure.SetDatabaseInfo (key, value);
							data[key] = value;

							Assert.AreEqual (value, db.DataInfrastructure.GetDatabaseInfo (key));
						}
						catch (System.InvalidOperationException)
						{
							// dataInfrastructure is probably deconnected. Let's ignore that exception
							// Marc
						}

						if (data.Count > 15)
						{
							var k = data.Keys.ElementAt (dice.Next (0, data.Count));
							var v = data[k];

							try
							{
								db.DataInfrastructure.SetDatabaseInfo (k, null);
								data.Remove (k);

								Assert.IsNull (db.DataInfrastructure.GetDatabaseInfo (k));
							}
							catch (System.InvalidOperationException)
							{
								// dataInfrastructure is probably deconnected. Let's ignore that exception
								// Marc
							}
						}
					}

					foreach (var lockTransaction in lockTransactions)
					{
						lockTransaction.Dispose ();
					}
				}));

				threads.Add (new System.Threading.Thread (() =>
				{
					var startTime = System.DateTime.Now;
					var stopTime = System.DateTime.Now + System.TimeSpan.FromSeconds (1);

					while (System.DateTime.Now - time <= duration)
					{
						if (startTime < System.DateTime.Now && System.DateTime.Now < stopTime)
						{
							if (db.DataInfrastructure.Connection != null && db.DataInfrastructure.Connection.Status == ConnectionStatus.Open)
							{
								try
								{
									db.DataInfrastructure.KeepConnectionAlive ();
								}
								catch (System.InvalidOperationException)
								{
								}
							}
						}
						else if (startTime <= System.DateTime.Now && stopTime <= System.DateTime.Now)
						{
							startTime = System.DateTime.Now + System.TimeSpan.FromSeconds (2);
							stopTime = System.DateTime.Now + System.TimeSpan.FromSeconds (3);
						}
						else
						{
							System.Threading.Thread.Sleep (50);
						}
					}
				}));

				threads.Add (new System.Threading.Thread (() =>
				{
					while (System.DateTime.Now - time <= duration)
					{
						if (db.DataInfrastructure.Connection != null && db.DataInfrastructure.Connection.Status == ConnectionStatus.Open)
						{
							try
							{
								db.DataInfrastructure.KillDeadConnections (System.TimeSpan.FromSeconds (1));
							}
							catch (System.InvalidOperationException)
							{
							}
						}
					}
				}));

				foreach (var thread in threads)
				{
					thread.Start ();
				}

				foreach (var thread in threads)
				{
					thread.Join ();
				}
			}
		}


		[TestMethod]
		public void ConcurrencyTest()
		{
			var dbs = Enumerable
				.Range (0, 10)
				.Select (e => DB.ConnectToTestDatabase ())
				.ToList ();

			foreach (var db in dbs)
			{
				db.DataInfrastructure.CloseConnection ();
			}

			try
			{
				var time = System.DateTime.Now;

				var threads = dbs.Select (i => new System.Threading.Thread (() =>
				{
					while (System.DateTime.Now - time <= System.TimeSpan.FromMilliseconds (15000))
					{
						i.DataInfrastructure.OpenConnection ("id");

						var lockManager = new LockManager (i.DbInfrastructure, i.DataInfrastructure.EntityEngine.ServiceSchemaEngine);

						lockManager.RequestLocks (i.DataInfrastructure.Connection.Id, new List<string> ()
						{
							this.GetRandomString(),
							this.GetRandomString(),
							this.GetRandomString()
						});

						i.DataInfrastructure.KillDeadConnections (System.TimeSpan.FromSeconds (5));

						i.DataInfrastructure.CloseConnection ();
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
				foreach (var db in dbs)
				{
					db.DataInfrastructure.Dispose ();
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
