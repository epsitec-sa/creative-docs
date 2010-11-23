using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Services;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Cresus.Database.UnitTests.Services
{


	[TestClass]
	public sealed class UnitTestDbLockManager
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			TestHelper.DeleteDatabase ();
			TestHelper.CreateDatabase ();
		}


		[TestMethod]
		public void AttachArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new DbLockManager ().Attach (null, new DbTable ())
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new DbLockManager ().Attach (dbInfrastructure, null)
				);
			}
		}


		[TestMethod]
		public void AttachAndDetach()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				Assert.IsNotNull (dbInfrastructure.LockManager);
			}
		}


		[TestMethod]
		public void RequestLockArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.RequestLock (null, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.RequestLock ("", 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.RequestLock ("test", -1)
				);
			}
		}


		[TestMethod]
		public void ReleaseLockArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ReleaseLock (null, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ReleaseLock ("", 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ReleaseLock ("test", -1)
				);
			}
		}


		[TestMethod]
		public void IsLockOwnedArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.IsLockOwned (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.IsLockOwned ("")
				);
			}
		}


		[TestMethod]
		public void RequestReleaseAndIsLockOwned1()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				Assert.IsFalse (manager.IsLockOwned ("myLock1"));
				Assert.IsFalse (manager.IsLockOwned ("myLock2"));
				Assert.IsFalse (manager.IsLockOwned ("myLock3"));

				manager.RequestLock ("myLock2", 0);

				Assert.IsFalse (manager.IsLockOwned ("myLock1"));
				Assert.IsTrue (manager.IsLockOwned ("myLock2"));
				Assert.IsFalse (manager.IsLockOwned ("myLock3"));

				manager.RequestLock ("myLock1", 0);
				manager.RequestLock ("myLock2", 0);
				manager.RequestLock ("myLock3", 1);

				Assert.IsTrue (manager.IsLockOwned ("myLock1"));
				Assert.IsTrue (manager.IsLockOwned ("myLock2"));
				Assert.IsTrue (manager.IsLockOwned ("myLock3"));

				manager.ReleaseLock ("myLock1", 0);
				manager.ReleaseLock ("myLock2", 0);
				manager.ReleaseLock ("myLock3", 1);

				Assert.IsFalse (manager.IsLockOwned ("myLock1"));
				Assert.IsTrue (manager.IsLockOwned ("myLock2"));
				Assert.IsFalse (manager.IsLockOwned ("myLock3"));

				manager.ReleaseLock ("myLock2", 0);

				Assert.IsFalse (manager.IsLockOwned ("myLock1"));
				Assert.IsFalse (manager.IsLockOwned ("myLock2"));
				Assert.IsFalse (manager.IsLockOwned ("myLock3"));
			}
		}


		[TestMethod]
		public void RequestReleaseAndIsLockOwned2()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				Assert.IsFalse (manager.IsLockOwned ("myLock"));

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.ReleaseLock ("myLock", 0)
				);

				manager.RequestLock ("myLock", 0);

				Assert.IsTrue (manager.IsLockOwned ("myLock"));
				Assert.AreEqual (0, manager.GetLock ("myLock").ConnectionId.Value);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.RequestLock ("myLock", 1)
				);

				Assert.IsTrue (manager.IsLockOwned ("myLock"));
				Assert.AreEqual (0, manager.GetLock ("myLock").ConnectionId.Value);

				manager.ReleaseLock ("myLock", 0);

				Assert.IsFalse (manager.IsLockOwned ("myLock"));

				manager.RequestLock ("myLock", 1);

				Assert.IsTrue (manager.IsLockOwned ("myLock"));
				Assert.AreEqual (1, manager.GetLock ("myLock").ConnectionId.Value);

				manager.ReleaseLock ("myLock", 1);

				Assert.IsFalse (manager.IsLockOwned ("myLock"));	
			}
		}


		[TestMethod]
		public void GetLockArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetLock (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetLock ("")
				);
			}
		}


		[TestMethod]
		public void GetLockConnectionId()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				manager.RequestLock ("myLock2", 0);

				Assert.IsFalse (manager.IsLockOwned ("myLock1"));
				Assert.AreEqual (0, manager.GetLock ("myLock2").ConnectionId.Value);
				Assert.IsFalse (manager.IsLockOwned ("myLock3"));

				manager.RequestLock ("myLock1", 0);
				manager.RequestLock ("myLock2", 0);
				manager.RequestLock ("myLock3", 1);

				Assert.AreEqual (0, manager.GetLock ("myLock1").ConnectionId.Value);
				Assert.AreEqual (0, manager.GetLock ("myLock2").ConnectionId.Value);
				Assert.AreEqual (1, manager.GetLock ("myLock3").ConnectionId.Value);

				manager.ReleaseLock ("myLock1", 0);
				manager.ReleaseLock ("myLock2", 0);
				manager.ReleaseLock ("myLock3", 1);

				Assert.IsFalse (manager.IsLockOwned ("myLock1"));
				Assert.AreEqual (0, manager.GetLock ("myLock2").ConnectionId.Value);
				Assert.IsFalse (manager.IsLockOwned ("myLock3"));

				manager.ReleaseLock ("myLock2", 0);

				Assert.IsFalse (manager.IsLockOwned ("myLock1"));
				Assert.IsFalse (manager.IsLockOwned ("myLock2"));
				Assert.IsFalse (manager.IsLockOwned ("myLock3"));
			}
		}


		[TestMethod]
		public void RemoveInactiveLocks()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager lockManager = dbInfrastructure.LockManager;
				DbConnectionManager connectionManager = dbInfrastructure.ConnectionManager;

				DbId connectionId1 = connectionManager.OpenConnection ("myId1").Id;
				DbId connectionId2 = connectionManager.OpenConnection ("myId2").Id;
				DbId connectionId3 = connectionManager.OpenConnection ("myId3").Id;
				
				Assert.IsFalse (lockManager.IsLockOwned ("myLock1"));
				Assert.IsFalse (lockManager.IsLockOwned ("myLock2"));
				Assert.IsFalse (lockManager.IsLockOwned ("myLock3"));

				lockManager.RequestLock ("myLock1", connectionId1);
				lockManager.RequestLock ("myLock2", connectionId2);
				lockManager.RequestLock ("myLock3", connectionId3);

				Assert.IsTrue (lockManager.IsLockOwned ("myLock1"));
				Assert.IsTrue (lockManager.IsLockOwned ("myLock2"));
				Assert.IsTrue (lockManager.IsLockOwned ("myLock3"));

				connectionManager.CloseConnection (connectionId2);

				for (int i = 0; i < 5; i++)
				{
					connectionManager.KeepConnectionAlive (connectionId1);

					System.Threading.Thread.Sleep (1000);
				}

				System.Threading.Thread.Sleep (1000);

				connectionManager.InterruptDeadConnections (System.TimeSpan.FromSeconds (5));

				Assert.IsTrue (lockManager.IsLockOwned ("myLock1"));
				Assert.IsTrue (lockManager.IsLockOwned ("myLock2"));
				Assert.IsTrue (lockManager.IsLockOwned ("myLock3"));

				Assert.AreEqual (DbConnectionStatus.Open, connectionManager.GetConnection (connectionId1).Status);
				Assert.AreEqual (DbConnectionStatus.Closed, connectionManager.GetConnection (connectionId2).Status);
				Assert.AreEqual (DbConnectionStatus.Interrupted, connectionManager.GetConnection (connectionId3).Status);

				lockManager.RemoveInactiveLocks ();

				Assert.IsTrue (lockManager.IsLockOwned ("myLock1"));
				Assert.IsFalse (lockManager.IsLockOwned ("myLock2"));
				Assert.IsFalse (lockManager.IsLockOwned ("myLock3"));

				lockManager.ReleaseLock ("myLock1", connectionId1);
				Assert.IsFalse (lockManager.IsLockOwned ("myLock1"));

				connectionManager.CloseConnection (connectionId1);
			}
		}


	}


}
