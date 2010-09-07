using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Cresus.Database.UnitTests
{


	[TestClass]
	public class UnitTestDbLockManager
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
					() => manager.RequestLock (null, "myUser")
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.RequestLock ("", "myUser")
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.RequestLock ("myLock", null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.RequestLock ("myLock", "")
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
					() => manager.ReleaseLock (null, "myUser")
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ReleaseLock ("", "myUser")
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ReleaseLock ("myLock", null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ReleaseLock ("myLock", "")
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

				manager.RequestLock ("myLock2", "myUser1");

				Assert.IsFalse (manager.IsLockOwned ("myLock1"));
				Assert.IsTrue (manager.IsLockOwned ("myLock2"));
				Assert.IsFalse (manager.IsLockOwned ("myLock3"));

				manager.RequestLock ("myLock1", "myUser1");
				manager.RequestLock ("myLock2", "myUser1");
				manager.RequestLock ("myLock3", "myUser2");

				Assert.IsTrue (manager.IsLockOwned ("myLock1"));
				Assert.IsTrue (manager.IsLockOwned ("myLock2"));
				Assert.IsTrue (manager.IsLockOwned ("myLock3"));

				manager.ReleaseLock ("myLock1", "myUser1");
				manager.ReleaseLock ("myLock2", "myUser1");
				manager.ReleaseLock ("myLock3", "myUser2");

				Assert.IsFalse (manager.IsLockOwned ("myLock1"));
				Assert.IsTrue (manager.IsLockOwned ("myLock2"));
				Assert.IsFalse (manager.IsLockOwned ("myLock3"));

				manager.ReleaseLock ("myLock2", "myUser1");

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
				
				manager.ReleaseLock ("myLock", "myUser1");
				manager.ReleaseLock ("myLock", "myUser2");

				Assert.IsFalse (manager.IsLockOwned ("myLock1"));

				manager.RequestLock ("myLock", "myUser1");

				Assert.IsTrue (manager.IsLockOwned ("myLock"));
				Assert.AreEqual ("myUser1", manager.GetLockOwner ("myLock"));

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.RequestLock ("myLock", "myUser2")
				);

				Assert.IsTrue (manager.IsLockOwned ("myLock"));
				Assert.AreEqual ("myUser1", manager.GetLockOwner ("myLock"));

				manager.ReleaseLock ("myLock", "myUser1");

				Assert.IsFalse (manager.IsLockOwned ("myLock"));

				manager.RequestLock ("myLock", "myUser2");

				Assert.IsTrue (manager.IsLockOwned ("myLock"));
				Assert.AreEqual ("myUser2", manager.GetLockOwner ("myLock"));

				manager.ReleaseLock ("myLock", "myUser2");

				Assert.IsFalse (manager.IsLockOwned ("myLock"));

				manager.ReleaseLock ("myLock", "myUser2");

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
					() => manager.GetLockOwner (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetLockOwner ("")
				);
			}
		}


		[TestMethod]
		public void GetLockOwner()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				Assert.IsNull (manager.GetLockOwner ("myLock1"));
				Assert.IsNull (manager.GetLockOwner ("myLock2"));
				Assert.IsNull (manager.GetLockOwner ("myLock3"));

				manager.RequestLock ("myLock2", "myUser1");

				Assert.IsNull (manager.GetLockOwner ("myLock1"));
				Assert.AreEqual ("myUser1", manager.GetLockOwner ("myLock2"));
				Assert.IsNull (manager.GetLockOwner ("myLock3"));

				manager.RequestLock ("myLock1", "myUser1");
				manager.RequestLock ("myLock2", "myUser1");
				manager.RequestLock ("myLock3", "myUser2");
				
				Assert.AreEqual ("myUser1", manager.GetLockOwner ("myLock1"));
				Assert.AreEqual ("myUser1", manager.GetLockOwner ("myLock2"));
				Assert.AreEqual ("myUser2", manager.GetLockOwner ("myLock3"));

				manager.ReleaseLock ("myLock1", "myUser1");
				manager.ReleaseLock ("myLock2", "myUser1");
				manager.ReleaseLock ("myLock3", "myUser2");

				Assert.IsNull (manager.GetLockOwner ("myLock1"));
				Assert.AreEqual ("myUser1", manager.GetLockOwner ("myLock2"));
				Assert.IsNull (manager.GetLockOwner ("myLock3"));

				manager.ReleaseLock ("myLock2", "myUser1");

				Assert.IsNull (manager.GetLockOwner ("myLock1"));
				Assert.IsNull (manager.GetLockOwner ("myLock2"));
				Assert.IsNull (manager.GetLockOwner ("myLock3"));
			}
		}


	}


}
