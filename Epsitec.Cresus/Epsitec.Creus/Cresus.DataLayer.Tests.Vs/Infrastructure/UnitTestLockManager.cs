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
	public sealed class UnitTestLockManager
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
					() => new LockManager (null, entityEngine.ServiceSchemaEngine)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new LockManager (dbinfrastructure, null)
				);
			}
		}


		[TestMethod]
		public void RequestLocksArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				LockManager manager = new LockManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.RequestLocks (DbId.Empty, new List<string>()  { "name" })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.RequestLocks (new DbId(1), null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.RequestLocks (new DbId (1), new List<string> () { })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.RequestLocks (new DbId (1), new List<string> () { "" })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.RequestLocks (new DbId (1), new List<string> () { null })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.RequestLocks (new DbId (1), new List<string> () { "name", "name" })
				);
			}
		}


		[TestMethod]
		public void ReleaseLocksArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				LockManager manager = new LockManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ReleaseLocks (DbId.Empty, new List<string>()  { "name" })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ReleaseLocks (new DbId(1), null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ReleaseLocks (new DbId (1), new List<string> () { })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ReleaseLocks (new DbId (1), new List<string> () { "" })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ReleaseLocks (new DbId (1), new List<string> () { null })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ReleaseLocks (new DbId (1), new List<string> () { "name", "name" })
				);
			}
		}


		[TestMethod]
		public void GetLocksArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				LockManager manager = new LockManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetLocks (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetLocks (new List<string> () { })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetLocks (new List<string> () { "" })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetLocks (new List<string> () { null })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetLocks (new List<string> () { "name", "name" })
				);
			}
		}


		[TestMethod]
		public void RequestReleaseLocks1()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ConnectionManager connectionManager = new ConnectionManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);
				LockManager lockManager = new LockManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				Connection c1 = connectionManager.OpenConnection ("c1");
				Connection c2 = connectionManager.OpenConnection ("c2");

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock1" }).Count ());
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock2" }).Count ());
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock3" }).Count ());

				lockManager.RequestLocks (c1.Id, new List<string> () { "myLock2" });

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock1" }).Count ());
				Assert.AreEqual (c1.Id, lockManager.GetLocks (new List<string> () { "myLock2" }).Single ().Owner.Id);
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock3" }).Count ());

				lockManager.RequestLocks (c1.Id, new List<string> () { "myLock1" });
				lockManager.RequestLocks (c1.Id, new List<string> () { "myLock2" });
				lockManager.RequestLocks (c2.Id, new List<string> () { "myLock3" });

				Assert.AreEqual (c1.Id, lockManager.GetLocks (new List<string> () { "myLock1" }).Single ().Owner.Id);
				Assert.AreEqual (c1.Id, lockManager.GetLocks (new List<string> () { "myLock2" }).Single ().Owner.Id);
				Assert.AreEqual (c2.Id, lockManager.GetLocks (new List<string> () { "myLock3" }).Single ().Owner.Id);

				lockManager.ReleaseLocks (c1.Id, new List<string> () { "myLock1" });
				lockManager.ReleaseLocks (c1.Id, new List<string> () { "myLock2" });
				lockManager.ReleaseLocks (c2.Id, new List<string> () { "myLock3" });

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock1" }).Count ());
				Assert.AreEqual (c1.Id, lockManager.GetLocks (new List<string> () { "myLock2" }).Single ().Owner.Id);
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock3" }).Count ());

				lockManager.ReleaseLocks (c1.Id, new List<string> () { "myLock2" });

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock1" }).Count ());
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock2" }).Count ());
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock3" }).Count ());
			}
		}


		[TestMethod]
		public void RequestReleaseLocks2()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ConnectionManager connectionManager = new ConnectionManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);
				LockManager lockManager = new LockManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				Connection c1 = connectionManager.OpenConnection ("c1");
				Connection c2 = connectionManager.OpenConnection ("c2");

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock" }).Count ());

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => lockManager.ReleaseLocks (c1.Id, new List<string> () { "myLock" })
				);

				lockManager.RequestLocks (c1.Id, new List<string> () { "myLock" });

				Assert.AreEqual (c1.Id, lockManager.GetLocks (new List<string> () { "myLock" }).Single ().Owner.Id);

				var result = lockManager.RequestLocks (c2.Id, new List<string> () { "myLock" });

				Assert.IsFalse (result.Item1);
				Assert.AreEqual (c1.Id, result.Item2.Single ().Owner.Id);

				Assert.AreEqual (c1.Id, lockManager.GetLocks (new List<string> () { "myLock" }).Single ().Owner.Id);

				lockManager.ReleaseLocks (c1.Id, new List<string> () { "myLock" });

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock" }).Count ());

				lockManager.RequestLocks (c2.Id, new List<string> () { "myLock" });

				Assert.AreEqual (c2.Id, lockManager.GetLocks (new List<string> () { "myLock" }).Single ().Owner.Id);

				lockManager.ReleaseLocks (c2.Id, new List<string> () { "myLock" });

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock" }).Count ());
			}
		}


		[TestMethod]
		public void ReleaseOwnedLocks1()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ConnectionManager connectionManager = new ConnectionManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);
				LockManager lockManager = new LockManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				Connection c = connectionManager.OpenConnection ("c");

				Assert.IsTrue (lockManager.RequestLocks (c.Id, new List<string> () { "lock2" }).Item1);

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "lock1" }).Count ());
				Assert.AreEqual (c.Id, lockManager.GetLocks (new List<string> () { "lock2" }).Single ().Owner.Id);

				lockManager.ReleaseOwnedLocks (c.Id, new List<string> () { "lock1", "lock2" });

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "lock1" }).Count ());
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "lock2" }).Count ());
			}
		}


		[TestMethod]
		public void ReleaseOwnedLocks2()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ConnectionManager connectionManager = new ConnectionManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);
				LockManager lockManager = new LockManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				Connection c1 = connectionManager.OpenConnection ("c1");
				Connection c2 = connectionManager.OpenConnection ("c2");

				Assert.IsTrue (lockManager.RequestLocks (c1.Id, new List<string> () { "lock1" }).Item1);
				Assert.IsTrue (lockManager.RequestLocks (c2.Id, new List<string> () { "lock2" }).Item1);

				Assert.AreEqual (c1.Id, lockManager.GetLocks (new List<string> () { "lock1" }).Single ().Owner.Id);
				Assert.AreEqual (c2.Id, lockManager.GetLocks (new List<string> () { "lock2" }).Single ().Owner.Id);
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "lock3" }).Count ());
				
				lockManager.ReleaseOwnedLocks (c1.Id, new List<string> () { "lock1", "lock2", "lock3" });

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "lock1" }).Count ());
				Assert.AreEqual (c2.Id, lockManager.GetLocks (new List<string> () { "lock2" }).Single ().Owner.Id);
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "lock3" }).Count ());

				lockManager.ReleaseOwnedLocks (c2.Id, new List<string> () { "lock1", "lock2", "lock3" });
				
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "lock1" }).Count ());
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "lock2" }).Count ());
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "lock3" }).Count ());
			}
		}


		[TestMethod]
		public void GetLocks1()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ConnectionManager connectionManager = new ConnectionManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);
				LockManager lockManager = new LockManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				Connection c1 = connectionManager.OpenConnection ("c1");
				Connection c2 = connectionManager.OpenConnection ("c2");

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock1" }).Count ());
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock2" }).Count ());
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock3" }).Count ());

				lockManager.RequestLocks (c1.Id, new List<string> () { "myLock2" });
				System.DateTime t2 = System.DateTime.Now;
				
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock1" }).Count ());
				Lock l2 = lockManager.GetLocks (new List<string> () { "myLock2" }).Single ();
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock3" }).Count ());

				this.CheckLock (l2, c1, "myLock2", t2);

				lockManager.RequestLocks (c1.Id, new List<string> () { "myLock1" });
				System.DateTime t1 = System.DateTime.Now;
				lockManager.RequestLocks (c1.Id, new List<string> () { "myLock2" });
				lockManager.RequestLocks (c2.Id, new List<string> () { "myLock3" });
				System.DateTime t3 = System.DateTime.Now;

				Lock l1 = lockManager.GetLocks (new List<string> () { "myLock1" }).Single ();
				l2 = lockManager.GetLocks (new List<string> () { "myLock2" }).Single ();
				Lock l3 = lockManager.GetLocks (new List<string> () { "myLock3" }).Single ();

				this.CheckLock (l1, c1, "myLock1", t1);
				this.CheckLock (l2, c1, "myLock2", t2);
				this.CheckLock (l3, c2, "myLock3", t3);

				lockManager.ReleaseLocks (c1.Id, new List<string> () { "myLock1" });
				lockManager.ReleaseLocks (c1.Id, new List<string> () { "myLock2" });
				lockManager.ReleaseLocks (c2.Id, new List<string> () { "myLock3" });

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock1" }).Count ());
				l2 = lockManager.GetLocks (new List<string> () { "myLock2" }).Single ();
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock3" }).Count ());

				this.CheckLock (l2, c1, "myLock2", t2);

				lockManager.ReleaseLocks (c1.Id, new List<string> () { "myLock2" });

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock1" }).Count ());
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock2" }).Count ());
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock3" }).Count ());
			}
		}


		[TestMethod]
		public void GetLocksTest2()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ConnectionManager connectionManager = new ConnectionManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);
				LockManager lockManager = new LockManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				Connection c1 = connectionManager.OpenConnection ("c1");
				Connection c2 = connectionManager.OpenConnection ("c2");
				Connection c3 = connectionManager.OpenConnection ("c3");

				lockManager.RequestLocks (c1.Id, new List<string> () { "lock1", "lock2" });
				System.DateTime t1 = System.DateTime.Now;

				lockManager.RequestLocks (c2.Id, new List<string> () { "lock3" });
				System.DateTime t2 = System.DateTime.Now;

				lockManager.RequestLocks (c2.Id, new List<string> () { "lock3" });

				lockManager.RequestLocks (c3.Id, new List<string> () { "lock4" });
				System.DateTime t3 = System.DateTime.Now;

				var locks1 = lockManager.GetLocks (new List<string> () { "lock1", "lock2", "lock3", "lock4", }).ToDictionary (l => l.Name);

				Assert.AreEqual (4, locks1.Count);
				this.CheckLock (locks1["lock1"], c1, "lock1", t1);
				this.CheckLock (locks1["lock2"], c1, "lock2", t1);
				this.CheckLock (locks1["lock3"], c2, "lock3", t2);
				this.CheckLock (locks1["lock4"], c3, "lock4", t3);

				var locks2 = lockManager.GetLocks (new List<string> () { "lock1", "lock2", }).ToDictionary (l => l.Name);

				Assert.AreEqual (2, locks2.Count);
				this.CheckLock (locks2["lock1"], c1, "lock1", t1);
				this.CheckLock (locks2["lock2"], c1, "lock2", t1);

				var locks3 = lockManager.GetLocks (new List<string> () { "lock3", }).ToDictionary (l => l.Name);
				
				Assert.AreEqual (1, locks3.Count);
				this.CheckLock (locks3["lock3"], c2, "lock3", t2);

				var locks4 = lockManager.GetLocks (new List<string> () { "lock4", }).ToDictionary (l => l.Name);

				Assert.AreEqual (1, locks4.Count);
				this.CheckLock (locks4["lock4"], c3, "lock4", t3);
				
				var locks5 = lockManager.GetLocks (new List<string> () { "lock5", }).ToDictionary (l => l.Name);

				Assert.AreEqual (0, locks5.Count);
			}
		}


		[TestMethod]
		public void GetLocksTest3()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ConnectionManager connectionManager = new ConnectionManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);
				LockManager lockManager = new LockManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				Connection c1 = connectionManager.OpenConnection ("c1");
				Connection c2 = connectionManager.OpenConnection ("c2");
				Connection c3 = connectionManager.OpenConnection ("c3");
				Connection c4 = connectionManager.OpenConnection ("c3");

				Assert.IsTrue (lockManager.RequestLocks (c1.Id, new List<string> () { "lock1", "lock2" }).Item1);
				Assert.IsTrue (lockManager.RequestLocks (c1.Id, new List<string> () { "lock1", "lock3" }).Item1);
				Assert.IsTrue (lockManager.RequestLocks (c2.Id, new List<string> () { "lock4", "lock5", "lock6" }).Item1);
				Assert.IsTrue (lockManager.RequestLocks (c2.Id, new List<string> () { "lock5", "lock6" }).Item1);

				Assert.IsFalse(lockManager.RequestLocks (c1.Id, new List<string> () { "lock4", "lock7" }).Item1);
				Assert.IsFalse(lockManager.RequestLocks (c3.Id, new List<string> () { "lock1", "lock7" }).Item1);
				Assert.IsFalse (lockManager.RequestLocks (c4.Id, new List<string> () { "lock3" }).Item1);
				Assert.IsFalse (lockManager.RequestLocks (c4.Id, new List<string> () { "lock6" }).Item1);
				
				lockManager.ReleaseLocks (c2.Id, new List<string> () { "lock4", "lock6" });

				Assert.IsTrue (lockManager.RequestLocks (c1.Id, new List<string> () { "lock4", "lock7" }).Item1);
				Assert.IsFalse (lockManager.RequestLocks (c3.Id, new List<string> () { "lock1", "lock7" }).Item1);
				Assert.IsFalse (lockManager.RequestLocks (c4.Id, new List<string> () { "lock3" }).Item1);
				Assert.IsFalse (lockManager.RequestLocks (c4.Id, new List<string> () { "lock6" }).Item1);

				lockManager.ReleaseLocks (c1.Id, new List<string> () { "lock1", "lock2", "lock3" });

				Assert.IsFalse (lockManager.RequestLocks (c3.Id, new List<string> () { "lock1", "lock7" }).Item1);
				Assert.IsTrue (lockManager.RequestLocks (c4.Id, new List<string> () { "lock3" }).Item1);
				Assert.IsFalse (lockManager.RequestLocks (c4.Id, new List<string> () { "lock6" }).Item1);

				lockManager.ReleaseLocks (c2.Id, new List<string> () { "lock5", "lock6" });

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => lockManager.ReleaseLocks(c3.Id, new List<string>() { "lock1", "lock2" })
				);

				Assert.IsTrue (lockManager.RequestLocks (c4.Id, new List<string> () { "lock6" }).Item1);
				Assert.IsTrue (lockManager.RequestLocks (c1.Id, new List<string> () { "lock1" }).Item1);
				Assert.IsTrue (lockManager.RequestLocks (c1.Id, new List<string> () { "lock4" }).Item1);
				Assert.IsTrue (lockManager.RequestLocks (c1.Id, new List<string> () { "lock7" }).Item1);

				Assert.IsTrue (lockManager.RequestLocks (c1.Id, new List<string> () { "lock1", "lock7" }).Item1);

				lockManager.ReleaseLocks (c1.Id, new List<string> () { "lock1", "lock4", "lock7" });

				Assert.IsTrue (lockManager.RequestLocks (c1.Id, new List<string> () { "lock1", "lock4" }).Item1);
				
				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => lockManager.ReleaseLocks (c1.Id, new List<string> () { "lock1", "lock8" })
				);

				lockManager.ReleaseLocks (c1.Id, new List<string> () { "lock1" });

				lockManager.ReleaseLocks (c1.Id, new List<string> () { "lock4", "lock7" });

				lockManager.ReleaseLocks (c1.Id, new List<string> () { "lock1" });

				lockManager.ReleaseLocks (c1.Id, new List<string> () { "lock1", "lock4", "lock7" });
				lockManager.ReleaseLocks (c2.Id, new List<string> () { "lock5" });

				Assert.IsTrue (lockManager.RequestLocks (c3.Id, new List<string> () { "lock1", "lock7" }).Item1);

				lockManager.ReleaseLocks (c3.Id, new List<string> () { "lock1", "lock7" });
				lockManager.ReleaseLocks (c4.Id, new List<string> () { "lock3", "lock6" });
				
				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => lockManager.ReleaseLocks (c2.Id, new List<string> () { "lock5", })
				);

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "lock1", "locks2", "locks3", "locks4", "locks5", "locks6", "locks7" }).Count ());
			}
		}


		[TestMethod]
		public void RemoveInactiveLocks()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ConnectionManager connectionManager = new ConnectionManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);
				LockManager lockManager = new LockManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				Connection c1 = connectionManager.OpenConnection ("c1");
				Connection c2 = connectionManager.OpenConnection ("c2");
				Connection c3 = connectionManager.OpenConnection ("c3");

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock1" }).Count ());
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock2" }).Count ());
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock3" }).Count ());

				lockManager.RequestLocks (c1.Id, new List<string> () { "myLock1" });
				lockManager.RequestLocks (c2.Id, new List<string> () { "myLock2" });
				lockManager.RequestLocks (c3.Id, new List<string> () { "myLock3" });

				Lock l1 = lockManager.GetLocks (new List<string> () { "myLock1" }).Single ();
				System.DateTime t1 = System.DateTime.Now;
				Lock l2 = lockManager.GetLocks (new List<string> () { "myLock2" }).Single ();
				System.DateTime t2 = System.DateTime.Now;
				Lock l3 = lockManager.GetLocks (new List<string> () { "myLock3" }).Single ();
				System.DateTime t3 = System.DateTime.Now;

				this.CheckLock (l1, c1, "myLock1", t1);
				this.CheckLock (l2, c2, "myLock2", t2);
				this.CheckLock (l3, c3, "myLock3", t3);

				connectionManager.CloseConnection (c2.Id);

				for (int i = 0; i < 3; i++)
				{
					connectionManager.KeepConnectionAlive (c1.Id);

					System.Threading.Thread.Sleep (1000);
				}

				connectionManager.KillDeadConnections (System.TimeSpan.FromSeconds (3));

				c1 = connectionManager.GetConnection (c1.Id);
				c2 = connectionManager.GetConnection (c2.Id);
				c3 = connectionManager.GetConnection (c3.Id);

				Assert.AreEqual (ConnectionStatus.Open, c1.Status);
				Assert.AreEqual (ConnectionStatus.Closed, c2.Status);
				Assert.AreEqual (ConnectionStatus.Interrupted, c3.Status);

				l1 = lockManager.GetLocks (new List<string> () { "myLock1" }).Single ();
				l2 = lockManager.GetLocks (new List<string> () { "myLock2" }).Single ();
				l3 = lockManager.GetLocks (new List<string> () { "myLock3" }).Single ();

				this.CheckLock (l1, c1, "myLock1", t1);
				this.CheckLock (l2, c2, "myLock2", t2);
				this.CheckLock (l3, c3, "myLock3", t3);

				lockManager.KillDeadLocks ();

				l1 = lockManager.GetLocks (new List<string> () { "myLock1" }).Single ();
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock2" }).Count ());
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock3" }).Count ());

				this.CheckLock (l1, c1, "myLock1", t1);

				lockManager.ReleaseLocks (c1.Id, new List<string> () { "myLock1" });

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock1" }).Count ());
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock2" }).Count ());
				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "myLock3" }).Count ());
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

					var lockManager = new LockManager (d, entityEngine.ServiceSchemaEngine);

					DbId connectionId = new DbId (System.Threading.Thread.CurrentThread.ManagedThreadId);

					List<List<string>> locks = new List<List<string>> ();

					while (System.DateTime.Now - time <= System.TimeSpan.FromSeconds (15))
					{
						List<string> lockNames = new List<string> ();

						locks.Add(lockNames);

						do
                        {
							lockNames.Add (System.Guid.NewGuid ().ToString ());
                        } while (dice.NextDouble() > 0.4);

						lockManager.RequestLocks (connectionId, lockNames);

						lockManager.GetLocks (locks[dice.Next (0, locks.Count)]);

						if (dice.NextDouble () > 0.8 && locks.Count > 0)
						{
							int index = dice.Next (0, locks.Count);

							lockManager.ReleaseOwnedLocks (connectionId, locks[index]);

							locks.RemoveAt (index);
						}

						if (dice.NextDouble () > 0.8 && locks.Count > 0)
						{
							int index = dice.Next (0, locks.Count);

							try
							{
								lockManager.ReleaseLocks (connectionId, locks[index]);
							}
							catch (System.InvalidOperationException)
							{
								// The locks have been killed. Let's ignore the exception.
								// Marc
							}
							finally
							{
								locks.RemoveAt (index);
							}
						}

						if (dice.NextDouble () > 0.995)
						{
							lockManager.KillDeadLocks ();
						}
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


		private void CheckLock(Lock l, Connection connection, string name, System.DateTime time)
		{
			Assert.AreEqual (name, l.Name);
			Assert.IsTrue (System.Math.Abs ((time - l.CreationTime).TotalMilliseconds) < 150);
			Assert.AreEqual (connection.Id, l.Owner.Id);
			Assert.AreEqual (connection.Identity, l.Owner.Identity);
			Assert.AreEqual (connection.Status, l.Owner.Status);
			Assert.AreEqual (connection.EstablishmentTime, l.Owner.EstablishmentTime);
			Assert.AreEqual (connection.RefreshTime, l.Owner.RefreshTime);
		}


	}


}
