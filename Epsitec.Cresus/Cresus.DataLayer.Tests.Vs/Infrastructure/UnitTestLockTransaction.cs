using Epsitec.Common.Support.Extensions;

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
	public sealed class UnitTestLockTransaction
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
		public void LockTransactionConstructorArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				LockManager manager = new LockManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);

				List<string> lockNames = new List<string> ()
				{
					"myLock1",
					"myLock2",
					"myLock3",
				};
				DbId connectionId = new DbId (1);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new LockTransaction (null, connectionId, lockNames)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new LockTransaction (manager, DbId.Empty, lockNames)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new LockTransaction (manager, connectionId, null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new LockTransaction (manager, connectionId, new List<string> () { })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new LockTransaction (manager, connectionId, new List<string> () { "" })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new LockTransaction (manager, connectionId, new List<string> () { null })
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => new LockTransaction (manager, connectionId, new List<string> () { "l1", "l1" })
				);
			}
		}


		[TestMethod]
		public void StateMachine1()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				ConnectionManager connectionManager = new ConnectionManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);
				LockManager lockManager = new LockManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);

				Connection c1 = connectionManager.OpenConnection ("c1");
				Connection c2 = connectionManager.OpenConnection ("c2");

				using (LockTransaction l1 = new LockTransaction (lockManager, c1.Id, new List<string> () { "lock1" }))
				{
					Assert.AreEqual (LockState.Idle, l1.State);
					l1.Dispose ();
					Assert.AreEqual (LockState.Disposed, l1.State);
				}

				using (LockTransaction l2 = new LockTransaction (lockManager, c1.Id, new List<string> () { "lock2" }))
				{
					Assert.AreEqual (LockState.Idle, l2.State);
					Assert.IsTrue (l2.Lock ().Item1);
					Assert.AreEqual (LockState.Locked, l2.State);
					l2.Release ();
					Assert.AreEqual (LockState.Disposed, l2.State);
					l2.Dispose ();
					Assert.AreEqual (LockState.Disposed, l2.State);
				}

				using (LockTransaction l3 = new LockTransaction (lockManager, c1.Id, new List<string> () { "lock3" }))
				{
					Assert.AreEqual (LockState.Idle, l3.State);
					Assert.IsTrue (l3.Lock ().Item1);
					Assert.AreEqual (LockState.Locked, l3.State);
					l3.Dispose ();
					Assert.AreEqual (LockState.Disposed, l3.State);
				}

				using (LockTransaction l4a = new LockTransaction (lockManager, c1.Id, new List<string> () { "lock4" }))
				using (LockTransaction l4b = new LockTransaction (lockManager, c2.Id, new List<string> () { "lock4" }))
				{
					Assert.AreEqual (LockState.Idle, l4a.State);
					Assert.IsTrue (l4a.Lock ().Item1);
					Assert.AreEqual (LockState.Locked, l4a.State);
					Assert.IsFalse (l4b.Lock ().Item1);
					Assert.AreEqual (LockState.Idle, l4b.State);
					l4a.Release ();
					Assert.AreEqual (LockState.Disposed, l4a.State);
					Assert.IsTrue (l4b.Lock ().Item1);
					Assert.AreEqual (LockState.Locked, l4b.State);
					l4b.Release ();
					Assert.AreEqual (LockState.Disposed, l4b.State);
				}

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "lock1", "lock2", "lock3", "lock4" }).Count ());
			}
		}


		[TestMethod]
		public void StateMachine2()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				ConnectionManager connectionManager = new ConnectionManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);
				LockManager lockManager = new LockManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);

				Connection c = connectionManager.OpenConnection ("c");

				using (LockTransaction l1 = new LockTransaction (lockManager, c.Id, new List<string> () { "lock1" }))
				{
					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => l1.Release ()
					);
				}

				using (LockTransaction l2 = new LockTransaction (lockManager, c.Id, new List<string> () { "lock2" }))
				{
					Assert.IsTrue (l2.Lock ().Item1);

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => l2.Lock ()
					);
				}

				using (LockTransaction l3 = new LockTransaction (lockManager, c.Id, new List<string> () { "lock3" }))
				{
					Assert.IsTrue (l3.Lock ().Item1);
					l3.Release ();

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => l3.Lock ()
					);
				}

				using (LockTransaction l4 = new LockTransaction (lockManager, c.Id, new List<string> () { "lock4" }))
				{
					l4.Dispose ();

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => l4.Lock ()
					);
				}

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "lock1", "lock2", "lock3", "lock4" }).Count ());
			}
		}


		[TestMethod]
		public void SimpleCase()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				ConnectionManager connectionManager = new ConnectionManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);
				LockManager lockManager = new LockManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);

				Connection c1 = connectionManager.OpenConnection ("c1");
				Connection c2 = connectionManager.OpenConnection ("c2");

				using (LockTransaction la = new LockTransaction (lockManager, c1.Id, new List<string> () { "lock" }))
				using (LockTransaction lb = new LockTransaction (lockManager, c2.Id, new List<string> () { "lock" }))
				{
					var result1 = la.Lock ();
					System.DateTime t = System.DateTime.Now;

					Assert.IsTrue (result1.Item1);
					Assert.IsTrue (result1.Item2.IsEmpty ());

					var result2 = lb.Lock ();
					Assert.IsFalse (result2.Item1);
					this.CheckLock (result2.Item2.Single (), c1, "lock", t);

					la.Release ();

					var result3 = lb.Lock ();
					Assert.IsTrue (result3.Item1);
					Assert.IsTrue (result3.Item2.IsEmpty ());

					lb.Release ();
				}

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "lock" }).Count ());
			}
		}


		[TestMethod]
		public void ComplexeCase()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				ConnectionManager connectionManager = new ConnectionManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);
				LockManager lockManager = new LockManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);

				Connection c1 = connectionManager.OpenConnection ("c1");
				Connection c2 = connectionManager.OpenConnection ("c2");
				Connection c3 = connectionManager.OpenConnection ("c3");
				Connection c4 = connectionManager.OpenConnection ("c4");

				using (LockTransaction la = new LockTransaction (lockManager, c1.Id, new List<string> () { "lock1" }))
				using (LockTransaction lb = new LockTransaction (lockManager, c1.Id, new List<string> () { "lock1", "lock2", "lock3" }))
				using (LockTransaction lc = new LockTransaction (lockManager, c2.Id, new List<string> () { "lock2", "lock4", "lock5" }))
				using (LockTransaction ld = new LockTransaction (lockManager, c3.Id, new List<string> () { "lock4" }))
				using (LockTransaction le = new LockTransaction (lockManager, c4.Id, new List<string> () { "lock1" }))
				{
					Assert.IsTrue (la.Lock ().Item1);
					Assert.IsFalse (le.Lock ().Item1);

					Assert.IsTrue (lc.Lock ().Item1);
					Assert.IsFalse (lb.Lock ().Item1);
					Assert.IsFalse (ld.Lock ().Item1);
					Assert.IsFalse (le.Lock ().Item1);

					lc.Release ();

					Assert.IsTrue (lb.Lock ().Item1);
					Assert.IsFalse (le.Lock ().Item1);

					Assert.IsTrue (ld.Lock ().Item1);

					la.Release ();

					Assert.IsFalse (le.Lock ().Item1);

					ld.Release ();

					Assert.IsFalse (le.Lock ().Item1);

					lb.Release ();

					Assert.IsTrue (le.Lock ().Item1);

					le.Release ();
				}

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "lock1", "lock2", "lock3", "lock4", "lock5" }).Count ());
			}
		}


		[TestMethod]
		public void ReEntrency1()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				ConnectionManager connectionManager = new ConnectionManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);
				LockManager lockManager = new LockManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);

				Connection c1 = connectionManager.OpenConnection ("c1");
				Connection c2 = connectionManager.OpenConnection ("c2");

				using (LockTransaction la = new LockTransaction (lockManager, c1.Id, new List<string> () { "lock" }))
				using (LockTransaction lb = new LockTransaction (lockManager, c1.Id, new List<string> () { "lock" }))
				using (LockTransaction lc = new LockTransaction (lockManager, c2.Id, new List<string> () { "lock" }))
				{
					Assert.IsTrue (la.Lock ().Item1);
					Assert.IsTrue (lb.Lock ().Item1);
					Assert.IsFalse (lc.Lock ().Item1);

					la.Release ();

					Assert.IsFalse (lc.Lock ().Item1);

					lb.Release ();

					Assert.IsTrue (lc.Lock ().Item1);

					lc.Release ();
				}

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "lock" }).Count ());
			}
		}


		[TestMethod]
		public void ReEntrency2()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				ConnectionManager connectionManager = new ConnectionManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);
				LockManager lockManager = new LockManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);

				Connection c1 = connectionManager.OpenConnection ("c1");
				Connection c2 = connectionManager.OpenConnection ("c2");

				using (LockTransaction la = new LockTransaction (lockManager, c1.Id, new List<string> () { "lock1", "lock0" }))
				using (LockTransaction lb = new LockTransaction (lockManager, c1.Id, new List<string> () { "lock1", }))
				using (LockTransaction lc = new LockTransaction (lockManager, c2.Id, new List<string> () { "lock2", "lock0" }))
				{
					Assert.IsTrue (la.Lock ().Item1);
					Assert.IsTrue (lb.Lock ().Item1);
					Assert.IsFalse (lc.Lock ().Item1);

					lb.Release ();

					Assert.IsFalse (lc.Lock ().Item1);

					la.Release ();

					Assert.IsTrue (lc.Lock ().Item1);

					lc.Release ();
				}

				Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "lock1", "lock2", "lock3", "lock4" }).Count ());
			}
		}


		[TestMethod]
		public void GetLockOwnersTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				ConnectionManager connectionManager = new ConnectionManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);
				LockManager lockManager = new LockManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);

				Connection c1 = connectionManager.OpenConnection ("c1");
				Connection c2 = connectionManager.OpenConnection ("c2");
				Connection c3 = connectionManager.OpenConnection ("c2");

				using (LockTransaction lt1 = new LockTransaction (lockManager, c1.Id, new List<string> () { "lock1", "lock2" }))
				using (LockTransaction lt2 = new LockTransaction (lockManager, c2.Id, new List<string> () { "lock3", }))
				using (LockTransaction lt3 = new LockTransaction (lockManager, c3.Id, new List<string> () { "lock1", "lock2", "lock3", "lock4" }))
				{
					var result1 = lt1.Lock ();
					System.DateTime t1 = System.DateTime.Now;

					var result2 = lt2.Lock ();
					System.DateTime t2 = System.DateTime.Now;

					var result3 = lt3.Lock ();

					Assert.IsTrue (result1.Item1);
					Assert.IsTrue (result1.Item2.IsEmpty ());

					Assert.IsTrue (result2.Item1);
					Assert.IsTrue (result2.Item2.IsEmpty ());

					Assert.IsFalse (result3.Item1);

					var locks = result3.Item2.ToDictionary (l => l.Name);

					Assert.AreEqual (3, locks.Count);

					this.CheckLock (locks["lock1"], c1, "lock1", t1);
					this.CheckLock (locks["lock2"], c1, "lock2", t1);
					this.CheckLock (locks["lock3"], c2, "lock3", t2);
				}
			}
		}


		[TestMethod]
		public void DisposeTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				ConnectionManager connectionManager = new ConnectionManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);
				LockManager lockManager = new LockManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);

				Connection c1 = connectionManager.OpenConnection ("c1");
				Connection c2 = connectionManager.OpenConnection ("c2");

				using (LockTransaction la = new LockTransaction (lockManager, c1.Id, new List<string> () { "lock1", "lock2", "lock3" }))
				using (LockTransaction lb = new LockTransaction (lockManager, c1.Id, new List<string> () { "lock1", }))
				{
					Assert.IsTrue (la.Lock ().Item1);

					lockManager.ReleaseLocks (c1.Id, new List<string> () { "lock1" });
					lockManager.ReleaseLocks (c1.Id, new List<string> () { "lock2" });

					Assert.IsTrue (lb.Lock ().Item1);

					Assert.AreEqual (c1.Id, lockManager.GetLocks (new List<string> () { "lock1" }).Single ().Owner.Id);
					Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "lock2" }).Count ());
					Assert.AreEqual (c1.Id, lockManager.GetLocks (new List<string> () { "lock3" }).Single ().Owner.Id);

					la.Dispose ();
					lb.Dispose ();

					Assert.AreEqual (0, lockManager.GetLocks (new List<string> () { "lock1", "lock2", "lock3" }).Count ());
				}
			}
		}


		private void CheckLock(Lock l, Connection connection, string name, System.DateTime time)
		{
			Assert.AreEqual (name, l.Name);
			Assert.IsTrue (System.Math.Abs ((time - l.CreationTime).TotalMilliseconds) < 100);
			Assert.AreEqual (connection.Id, l.Owner.Id);
			Assert.AreEqual (connection.Identity, l.Owner.Identity);
			Assert.AreEqual (connection.Status, l.Owner.Status);
			Assert.AreEqual (connection.EstablishmentTime, l.Owner.EstablishmentTime);
			Assert.AreEqual (connection.RefreshTime, l.Owner.RefreshTime);
		}


	}


}
