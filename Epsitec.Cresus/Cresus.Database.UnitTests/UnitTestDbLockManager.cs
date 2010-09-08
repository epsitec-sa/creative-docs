using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Cresus.Database.UnitTests
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

				Assert.IsFalse (manager.IsLockOwned ("myLock1"));
				
				manager.ReleaseLock ("myLock", 0);
				manager.ReleaseLock ("myLock", 1);

				Assert.IsFalse (manager.IsLockOwned ("myLock1"));

				manager.RequestLock ("myLock", 0);

				Assert.IsTrue (manager.IsLockOwned ("myLock"));
				Assert.AreEqual (0, manager.GetLockConnexionId ("myLock"));

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.RequestLock ("myLock", 1)
				);

				Assert.IsTrue (manager.IsLockOwned ("myLock"));
				Assert.AreEqual (0, manager.GetLockConnexionId ("myLock"));

				manager.ReleaseLock ("myLock", 0);

				Assert.IsFalse (manager.IsLockOwned ("myLock"));

				manager.RequestLock ("myLock", 1);

				Assert.IsTrue (manager.IsLockOwned ("myLock"));
				Assert.AreEqual (1, manager.GetLockConnexionId ("myLock"));

				manager.ReleaseLock ("myLock", 1);

				Assert.IsFalse (manager.IsLockOwned ("myLock"));

				manager.ReleaseLock ("myLock", 1);

				Assert.IsFalse (manager.IsLockOwned ("myLock"));	
			}
		}


		[TestMethod]
		public void GetLockOwnerArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetLockConnexionId (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetLockConnexionId ("")
				);
			}
		}


		[TestMethod]
		public void GetLockOwner()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				Assert.IsNull (manager.GetLockConnexionId ("myLock1"));
				Assert.IsNull (manager.GetLockConnexionId ("myLock2"));
				Assert.IsNull (manager.GetLockConnexionId ("myLock3"));

				manager.RequestLock ("myLock2", 0);

				Assert.IsNull (manager.GetLockConnexionId ("myLock1"));
				Assert.AreEqual (0, manager.GetLockConnexionId ("myLock2"));
				Assert.IsNull (manager.GetLockConnexionId ("myLock3"));

				manager.RequestLock ("myLock1", 0);
				manager.RequestLock ("myLock2", 0);
				manager.RequestLock ("myLock3", 1);

				Assert.AreEqual (0, manager.GetLockConnexionId ("myLock1"));
				Assert.AreEqual (0, manager.GetLockConnexionId ("myLock2"));
				Assert.AreEqual (1, manager.GetLockConnexionId ("myLock3"));

				manager.ReleaseLock ("myLock1", 0);
				manager.ReleaseLock ("myLock2", 0);
				manager.ReleaseLock ("myLock3", 1);

				Assert.IsNull (manager.GetLockConnexionId ("myLock1"));
				Assert.AreEqual (0, manager.GetLockConnexionId ("myLock2"));
				Assert.IsNull (manager.GetLockConnexionId ("myLock3"));

				manager.ReleaseLock ("myLock2", 0);

				Assert.IsNull (manager.GetLockConnexionId ("myLock1"));
				Assert.IsNull (manager.GetLockConnexionId ("myLock2"));
				Assert.IsNull (manager.GetLockConnexionId ("myLock3"));
			}
		}


	}


}
