﻿using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.Infrastructure
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
			DatabaseHelper.CreateAndConnectToDatabase ();
		}


		[TestCleanup]
		public static void TestCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestMethod]
		public void LockTransactionConstructorArgumentCheck()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;
			List<string> lockNames = new List<string> ()
		    {
		        "myLock1",
		        "myLock2",
		        "myLock3",
		    };
			long connectionId = 0;

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new LockTransaction (null, connectionId, lockNames)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new LockTransaction (dbInfrastructure, connectionId, null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new LockTransaction (dbInfrastructure, connectionId, new List<string> () { "l1", "" })
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new LockTransaction (dbInfrastructure, connectionId, new List<string> () { "l1", null })
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new LockTransaction (dbInfrastructure, connectionId, new List<string> () { "l1", "l2", "l1" })
			);
		}


		[TestMethod]
		public void StateMachine1()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;

			using (LockTransaction l1 = new LockTransaction (dbInfrastructure, 0, new List<string> () { "lock1" }))
			{
				Assert.AreEqual (LockState.Idle, l1.State);
				l1.Dispose ();
				Assert.AreEqual (LockState.Disposed, l1.State);
			}

			using (LockTransaction l2 = new LockTransaction (dbInfrastructure, 0, new List<string> () { "lock2" }))
			{
				Assert.AreEqual (LockState.Idle, l2.State);
				Assert.IsTrue (l2.Lock ());
				Assert.AreEqual (LockState.Locked, l2.State);
				l2.Release ();
				Assert.AreEqual (LockState.Disposed, l2.State);
				l2.Dispose ();
				Assert.AreEqual (LockState.Disposed, l2.State);
			}

			using (LockTransaction l3 = new LockTransaction (dbInfrastructure, 0, new List<string> () { "lock3" }))
			{
				Assert.AreEqual (LockState.Idle, l3.State);
				Assert.IsTrue (l3.Lock ());
				Assert.AreEqual (LockState.Locked, l3.State);
				l3.Dispose ();
				Assert.AreEqual (LockState.Disposed, l3.State);
			}

			using (LockTransaction l4a = new LockTransaction (dbInfrastructure, 0, new List<string> () { "lock4" }))
			using (LockTransaction l4b = new LockTransaction (dbInfrastructure, 1, new List<string> () { "lock4" }))
			{
				Assert.AreEqual (LockState.Idle, l4a.State);
				Assert.IsTrue (l4a.Lock ());
				Assert.AreEqual (LockState.Locked, l4a.State);
				Assert.IsFalse (l4b.Lock ());
				Assert.AreEqual (LockState.Idle, l4b.State);
				l4a.Release ();
				Assert.AreEqual (LockState.Disposed, l4a.State);
				Assert.IsTrue (l4b.Lock ());
				Assert.AreEqual (LockState.Locked, l4b.State);
				l4b.Release ();
				Assert.AreEqual (LockState.Disposed, l4b.State);
			}

			using (LockTransaction l1 = new LockTransaction (dbInfrastructure, 1, new List<string> () { "lock1" }))
			using (LockTransaction l2 = new LockTransaction (dbInfrastructure, 1, new List<string> () { "lock2" }))
			using (LockTransaction l3 = new LockTransaction (dbInfrastructure, 1, new List<string> () { "lock3" }))
			using (LockTransaction l4 = new LockTransaction (dbInfrastructure, 1, new List<string> () { "lock4" }))
			{
				Assert.IsTrue (l1.Lock ());
				Assert.IsTrue (l2.Lock ());
				Assert.IsTrue (l3.Lock ());
				Assert.IsTrue (l4.Lock ());
			}
		}


		[TestMethod]
		public void StateMachine2()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;

			using (LockTransaction l1 = new LockTransaction (dbInfrastructure, 0, new List<string> () { "lock1" }))
			{
				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => l1.Release ()
				);
			}

			using (LockTransaction l2 = new LockTransaction (dbInfrastructure, 0, new List<string> () { "lock2" }))
			{
				Assert.IsTrue (l2.Lock ());

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => l2.Lock ()
				);
			}

			using (LockTransaction l3 = new LockTransaction (dbInfrastructure, 0, new List<string> () { "lock3" }))
			{
				Assert.IsTrue (l3.Lock ());
				l3.Release ();

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => l3.Lock ()
				);
			}

			using (LockTransaction l4 = new LockTransaction (dbInfrastructure, 0, new List<string> () { "lock4" }))
			{
				l4.Dispose ();

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => l4.Lock ()
				);
			}

			using (LockTransaction l1 = new LockTransaction (dbInfrastructure, 1, new List<string> () { "lock1" }))
			using (LockTransaction l2 = new LockTransaction (dbInfrastructure, 1, new List<string> () { "lock2" }))
			using (LockTransaction l3 = new LockTransaction (dbInfrastructure, 1, new List<string> () { "lock3" }))
			using (LockTransaction l4 = new LockTransaction (dbInfrastructure, 1, new List<string> () { "lock4" }))
			{
				Assert.IsTrue (l1.Lock ());
				Assert.IsTrue (l2.Lock ());
				Assert.IsTrue (l3.Lock ());
				Assert.IsTrue (l4.Lock ());
			}
		}


		[TestMethod]
		public void SimpleCase1()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;

			using (LockTransaction la = new LockTransaction (dbInfrastructure, 0, new List<string> () { "lock" }))
			using (LockTransaction lb = new LockTransaction (dbInfrastructure, 1, new List<string> () { "lock" }))
			{
				Assert.IsTrue (la.Lock ());
				Assert.IsFalse (lb.Lock ());

				la.Release ();

				Assert.IsTrue (lb.Lock ());

				lb.Release ();
			}

			using (LockTransaction l = new LockTransaction (dbInfrastructure, 2, new List<string> () { "lock" }))
			{
				Assert.IsTrue (l.Lock ());
			}
		}


		[TestMethod]
		public void SimpleCase2()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;

			using (LockTransaction la = new LockTransaction (dbInfrastructure, 0, new List<string> () { "lock" }))
			using (LockTransaction lb = new LockTransaction (dbInfrastructure, 1, new List<string> () { "lock" }))
			{
				Assert.IsTrue (la.Lock ());
				Assert.IsFalse (lb.Lock ());
			}

			using (LockTransaction l = new LockTransaction (dbInfrastructure, 2, new List<string> () { "lock" }))
			{
				Assert.IsTrue (l.Lock ());
			}
		}


		[TestMethod]
		public void ReEntrency1()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;

			using (LockTransaction la = new LockTransaction (dbInfrastructure, 0, new List<string> () { "lock" }))
			using (LockTransaction lb = new LockTransaction (dbInfrastructure, 0, new List<string> () { "lock" }))
			using (LockTransaction lc = new LockTransaction (dbInfrastructure, 1, new List<string> () { "lock" }))
			{
				Assert.IsTrue (la.Lock ());
				Assert.IsTrue (lb.Lock ());
				Assert.IsFalse (lc.Lock ());

				la.Release ();

				Assert.IsFalse (lc.Lock ());

				lb.Release ();

				Assert.IsTrue (lc.Lock ());

				lc.Release ();
			}

			using (LockTransaction ld = new LockTransaction (dbInfrastructure, 1, new List<string> () { "lock" }))
			{
				Assert.IsTrue (ld.Lock ());
			}
		}


		[TestMethod]
		public void ReEntrency2()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;

			using (LockTransaction la = new LockTransaction (dbInfrastructure, 0, new List<string> () { "lock1", "lock0" }))
			using (LockTransaction lb = new LockTransaction (dbInfrastructure, 0, new List<string> () { "lock1", "lock0" }))
			using (LockTransaction lc = new LockTransaction (dbInfrastructure, 1, new List<string> () { "lock3", "lock0" }))
			{
				Assert.IsTrue (la.Lock ());
				Assert.IsTrue (lb.Lock ());
				Assert.IsFalse (lc.Lock ());

				lb.Release ();

				Assert.IsFalse (lc.Lock ());

				la.Release ();

				Assert.IsTrue (lc.Lock ());

				lc.Release ();
			}

			using (LockTransaction l0 = new LockTransaction (dbInfrastructure, 1, new List<string> () { "lock0" }))
			using (LockTransaction l1 = new LockTransaction (dbInfrastructure, 1, new List<string> () { "lock1" }))
			using (LockTransaction l2 = new LockTransaction (dbInfrastructure, 1, new List<string> () { "lock2" }))
			using (LockTransaction l3 = new LockTransaction (dbInfrastructure, 1, new List<string> () { "lock3" }))
			{
				Assert.IsTrue (l0.Lock ());
				Assert.IsTrue (l1.Lock ());
				Assert.IsTrue (l2.Lock ());
				Assert.IsTrue (l3.Lock ());
			}
		}


		[TestMethod]
		public void AreAllLocksAvailableArgumentCheck()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;
			List<string> lockNames = new List<string> ()
		    {
		        "myLock1",
		        "myLock2",
		        "myLock3",
		    };
			long connectionId = 0;

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => LockTransaction.AreAllLocksAvailable (null, connectionId, lockNames)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => LockTransaction.AreAllLocksAvailable (dbInfrastructure, connectionId, null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => LockTransaction.AreAllLocksAvailable (dbInfrastructure, connectionId, new List<string> () { "l1", "" })
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => LockTransaction.AreAllLocksAvailable (dbInfrastructure, connectionId, new List<string> () { "l1", null })
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => LockTransaction.AreAllLocksAvailable (dbInfrastructure, connectionId, new List<string> () { "l1", "l2", "l1" })
			);
		}


		[TestMethod]
		public void AreAllLocksAvailable()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;

			Assert.IsTrue (LockTransaction.AreAllLocksAvailable (dbInfrastructure, 0, new List<string> () { "l1", "l2" }));
			Assert.IsTrue (LockTransaction.AreAllLocksAvailable (dbInfrastructure, 1, new List<string> () { "l1", "l2" }));

			using (LockTransaction l1 = new LockTransaction (dbInfrastructure, 0, new List<string> () { "l1" }))
			using (LockTransaction l2 = new LockTransaction (dbInfrastructure, 1, new List<string> () { "l2" }))
			{
				l1.Lock ();

				Assert.IsTrue (LockTransaction.AreAllLocksAvailable (dbInfrastructure, 0, new List<string> () { "l1", "l2" }));
				Assert.IsFalse (LockTransaction.AreAllLocksAvailable (dbInfrastructure, 1, new List<string> () { "l1", "l2" }));
				Assert.IsTrue (LockTransaction.AreAllLocksAvailable (dbInfrastructure, 1, new List<string> () { "l2" }));

				l2.Lock ();

				Assert.IsFalse (LockTransaction.AreAllLocksAvailable (dbInfrastructure, 0, new List<string> () { "l1", "l2" }));
				Assert.IsFalse (LockTransaction.AreAllLocksAvailable (dbInfrastructure, 1, new List<string> () { "l1", "l2" }));
				Assert.IsTrue (LockTransaction.AreAllLocksAvailable (dbInfrastructure, 0, new List<string> () { "l1" }));
				Assert.IsTrue (LockTransaction.AreAllLocksAvailable (dbInfrastructure, 1, new List<string> () { "l2" }));

				l1.Release ();

				Assert.IsFalse (LockTransaction.AreAllLocksAvailable (dbInfrastructure, 0, new List<string> () { "l1", "l2" }));
				Assert.IsTrue (LockTransaction.AreAllLocksAvailable (dbInfrastructure, 1, new List<string> () { "l1", "l2" }));
				Assert.IsTrue (LockTransaction.AreAllLocksAvailable (dbInfrastructure, 0, new List<string> () { "l1" }));

				l2.Release ();

				Assert.IsTrue (LockTransaction.AreAllLocksAvailable (dbInfrastructure, 0, new List<string> () { "l1", "l2" }));
				Assert.IsTrue (LockTransaction.AreAllLocksAvailable (dbInfrastructure, 1, new List<string> () { "l1", "l2" }));
			}
		}


	}


}
