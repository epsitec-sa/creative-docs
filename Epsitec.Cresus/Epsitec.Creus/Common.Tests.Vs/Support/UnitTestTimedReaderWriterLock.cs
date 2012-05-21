using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Threading;


namespace Epsitec.Common.Tests.Vs.Support
{


	[TestClass]
	public sealed class UnitTestTimedReaderWriterLock
	{


		[TestMethod]
		public void LockReadArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => TimedReaderWriterLock.LockRead (null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => TimedReaderWriterLock.LockRead (null, System.TimeSpan.FromSeconds (1))
			);

			using (ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => TimedReaderWriterLock.LockRead (rwLock, System.TimeSpan.FromSeconds (-1))
				);
			}
		}


		[TestMethod]
		public void LockWriteArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => TimedReaderWriterLock.LockWrite (null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => TimedReaderWriterLock.LockWrite (null, System.TimeSpan.FromSeconds (1))
			);

			using (ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => TimedReaderWriterLock.LockWrite (rwLock, System.TimeSpan.FromSeconds (-1))
				);
			}
		}


		[TestMethod]
		public void SimpleTestRead()
		{
			this.SimpleTestReadImplementation (null);
			this.SimpleTestReadImplementation (System.TimeSpan.FromSeconds (1));
		}


		private void SimpleTestReadImplementation(System.TimeSpan? timeOut)
		{
			using (ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim (LockRecursionPolicy.SupportsRecursion))
			{
				Assert.IsFalse (rwLock.IsReadLockHeld);
				Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
				Assert.IsFalse (rwLock.IsWriteLockHeld);
				
				using (var l = TimedReaderWriterLock.LockRead (rwLock, timeOut))
				{
					Assert.IsNotNull (l);
					Assert.IsTrue (rwLock.IsReadLockHeld);
					Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
					Assert.IsFalse (rwLock.IsWriteLockHeld);
				}
				
				Assert.IsFalse (rwLock.IsReadLockHeld);
				Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
				Assert.IsFalse (rwLock.IsWriteLockHeld);
			}
		}


		[TestMethod]
		public void SimpleTestWrite()
		{
			this.SimpleTestWriteImplementation (null);
			this.SimpleTestWriteImplementation (System.TimeSpan.FromSeconds (1));
		}


		public void SimpleTestWriteImplementation(System.TimeSpan? timeOut)
		{
			using (ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim (LockRecursionPolicy.SupportsRecursion))
			{
				Assert.IsFalse (rwLock.IsReadLockHeld);
				Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
				Assert.IsFalse (rwLock.IsWriteLockHeld);
				
				using (var l = TimedReaderWriterLock.LockWrite (rwLock, timeOut))
				{
					Assert.IsNotNull (l);
					Assert.IsFalse (rwLock.IsReadLockHeld);
					Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
					Assert.IsTrue (rwLock.IsWriteLockHeld);
				}
				
				Assert.IsFalse (rwLock.IsReadLockHeld);
				Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
				Assert.IsFalse (rwLock.IsWriteLockHeld);
			}
		}


		[TestMethod]
		public void ReentrencyTestRead()
		{
			this.ReentrencyTestReadImplementation (null);
			this.ReentrencyTestReadImplementation (System.TimeSpan.FromSeconds (1));
		}


		public void ReentrencyTestReadImplementation(System.TimeSpan? timeOut)
		{
			using (ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim (LockRecursionPolicy.SupportsRecursion))
			{
				List<System.IDisposable> locks = new List<System.IDisposable> ();

				Assert.IsFalse (rwLock.IsReadLockHeld);
				Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
				Assert.IsFalse (rwLock.IsWriteLockHeld);

				for (int i = 0; i < 10; i++)
				{
					locks.Add (TimedReaderWriterLock.LockRead (rwLock, timeOut));
				}

				Assert.IsTrue (rwLock.IsReadLockHeld);
				Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
				Assert.IsFalse (rwLock.IsWriteLockHeld);

				for (int i = 9; i >= 0; i--)
				{
					locks[i].Dispose ();
				}

				Assert.IsFalse (rwLock.IsReadLockHeld);
				Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
				Assert.IsFalse (rwLock.IsWriteLockHeld);
			}
		}


		[TestMethod]
		public void ReentrencyTestWrite()
		{
			this.ReentrencyTestWriteImplementation (null);
			this.ReentrencyTestWriteImplementation (System.TimeSpan.FromSeconds (1));
		}


		public void ReentrencyTestWriteImplementation(System.TimeSpan? timeOut)
		{
			using (ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim (LockRecursionPolicy.SupportsRecursion))
			{
				List<System.IDisposable> locks = new List<System.IDisposable> ();

				Assert.IsFalse (rwLock.IsReadLockHeld);
				Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
				Assert.IsFalse (rwLock.IsWriteLockHeld);

				for (int i = 0; i < 10; i++)
				{
					locks.Add (TimedReaderWriterLock.LockWrite (rwLock, timeOut));
				}

				Assert.IsFalse (rwLock.IsReadLockHeld);
				Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
				Assert.IsTrue (rwLock.IsWriteLockHeld);

				for (int i = 9; i >= 0; i--)
				{
					locks[i].Dispose ();
				}

				Assert.IsFalse (rwLock.IsReadLockHeld);
				Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
				Assert.IsFalse (rwLock.IsWriteLockHeld);
			}
		}


		[TestMethod]
		public void ReentrencyTestDownGrade()
		{
			this.ReentrencyTestDownGradeImplementation (null);
			this.ReentrencyTestDownGradeImplementation (System.TimeSpan.FromSeconds (1));
		}


		public void ReentrencyTestDownGradeImplementation(System.TimeSpan? timeOut)
		{
			using (ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim (LockRecursionPolicy.SupportsRecursion))
			{
				Assert.IsFalse (rwLock.IsReadLockHeld);
				Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
				Assert.IsFalse (rwLock.IsWriteLockHeld);

				using (var l1 = TimedReaderWriterLock.LockWrite (rwLock, timeOut))
				{
					Assert.IsNotNull (l1);

					Assert.IsFalse (rwLock.IsReadLockHeld);
					Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
					Assert.IsTrue (rwLock.IsWriteLockHeld);

					using (var l2 = TimedReaderWriterLock.LockRead (rwLock, timeOut))
					{
						Assert.IsNotNull (l2);

						Assert.IsTrue (rwLock.IsReadLockHeld);
						Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
						Assert.IsTrue (rwLock.IsWriteLockHeld);
					}

					Assert.IsFalse (rwLock.IsReadLockHeld);
					Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
					Assert.IsTrue (rwLock.IsWriteLockHeld);
				}

				Assert.IsFalse (rwLock.IsReadLockHeld);
				Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
				Assert.IsFalse (rwLock.IsWriteLockHeld);
			}
		}


		[TestMethod]
		public void ReentrencyTestUpGrade()
		{
			this.ReentrencyTestUpGradeImplementation (null);
			this.ReentrencyTestUpGradeImplementation (System.TimeSpan.FromSeconds (1));
		}


		public void ReentrencyTestUpGradeImplementation(System.TimeSpan? timeOut)
		{
			using (ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim (LockRecursionPolicy.SupportsRecursion))
			{
				Assert.IsFalse (rwLock.IsReadLockHeld);
				Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
				Assert.IsFalse (rwLock.IsWriteLockHeld);

				using (var l1 = TimedReaderWriterLock.LockRead (rwLock, timeOut))
				{
					Assert.IsNotNull (l1);

					Assert.IsTrue (rwLock.IsReadLockHeld);
					Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
					Assert.IsFalse (rwLock.IsWriteLockHeld);

					ExceptionAssert.Throw<LockRecursionException>
					(
						() => TimedReaderWriterLock.LockWrite (rwLock, timeOut)
					);

					Assert.IsTrue (rwLock.IsReadLockHeld);
					Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
					Assert.IsFalse (rwLock.IsWriteLockHeld);
				}

				Assert.IsFalse (rwLock.IsReadLockHeld);
				Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
				Assert.IsFalse (rwLock.IsWriteLockHeld);
			}
		}


		[TestMethod]
		public void TimeOutReadTest1()
		{
			using (ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim (LockRecursionPolicy.SupportsRecursion))
			{
				Thread t1 = new Thread (() =>
				{
					rwLock.EnterWriteLock ();

					Thread.Sleep (1000);

					rwLock.ExitWriteLock ();
				});

				Thread t2 = new Thread (() =>
				{
					Thread.Sleep (500);

					Assert.IsFalse (rwLock.IsReadLockHeld);
					Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
					Assert.IsFalse (rwLock.IsWriteLockHeld);

					using (TimedReaderWriterLock.LockRead (rwLock, System.TimeSpan.FromMilliseconds (1000)))
					{
						Assert.IsTrue (rwLock.IsReadLockHeld);
						Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
						Assert.IsFalse (rwLock.IsWriteLockHeld);
					}

					Assert.IsFalse (rwLock.IsReadLockHeld);
					Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
					Assert.IsFalse (rwLock.IsWriteLockHeld);
				});

				t1.Start ();
				t2.Start ();

				t1.Join ();
				t2.Join ();
			}
		}


		[TestMethod]
		public void TimeOutReadTest2()
		{
			using (ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim (LockRecursionPolicy.SupportsRecursion))
			{
				Thread t1 = new Thread (() =>
				{
					rwLock.EnterWriteLock ();

					Thread.Sleep (1000);

					rwLock.ExitWriteLock ();
				});

				Thread t2 = new Thread (() =>
				{
					Thread.Sleep (500);

					Assert.IsFalse (rwLock.IsReadLockHeld);
					Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
					Assert.IsFalse (rwLock.IsWriteLockHeld);

					ExceptionAssert.Throw<LockTimeoutException>
					(
						() => TimedReaderWriterLock.LockRead (rwLock, System.TimeSpan.FromMilliseconds (100))
					);

					Assert.IsFalse (rwLock.IsReadLockHeld);
					Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
					Assert.IsFalse (rwLock.IsWriteLockHeld);
				});

				t1.Start ();
				t2.Start ();

				t1.Join ();
				t2.Join ();
			}
		}


		[TestMethod]
		public void TimeOutWriteTest1()
		{
			using (ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim (LockRecursionPolicy.SupportsRecursion))
			{
				Thread t1 = new Thread (() =>
				{
					rwLock.EnterWriteLock ();

					Thread.Sleep (1000);

					rwLock.ExitWriteLock ();
				});

				Thread t2 = new Thread (() =>
				{
					Thread.Sleep (500);

					Assert.IsFalse (rwLock.IsReadLockHeld);
					Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
					Assert.IsFalse (rwLock.IsWriteLockHeld);

					using (TimedReaderWriterLock.LockWrite (rwLock, System.TimeSpan.FromMilliseconds (1000)))
					{
						Assert.IsFalse (rwLock.IsReadLockHeld);
						Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
						Assert.IsTrue (rwLock.IsWriteLockHeld);
					}

					Assert.IsFalse (rwLock.IsReadLockHeld);
					Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
					Assert.IsFalse (rwLock.IsWriteLockHeld);
				});

				t1.Start ();
				t2.Start ();

				t1.Join ();
				t2.Join ();
			}
		}


		[TestMethod]
		public void TimeOutWriteTest2()
		{
			using (ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim (LockRecursionPolicy.SupportsRecursion))
			{
				Thread t1 = new Thread (() =>
				{
					rwLock.EnterWriteLock ();

					Thread.Sleep (1000);

					rwLock.ExitWriteLock ();
				});

				Thread t2 = new Thread (() =>
				{
					Thread.Sleep (500);

					Assert.IsFalse (rwLock.IsReadLockHeld);
					Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
					Assert.IsFalse (rwLock.IsWriteLockHeld);

					ExceptionAssert.Throw<LockTimeoutException>
					(
						() => TimedReaderWriterLock.LockWrite (rwLock, System.TimeSpan.FromMilliseconds (100))
					);

					Assert.IsFalse (rwLock.IsReadLockHeld);
					Assert.IsFalse (rwLock.IsUpgradeableReadLockHeld);
					Assert.IsFalse (rwLock.IsWriteLockHeld);
				});

				t1.Start ();
				t2.Start ();

				t1.Join ();
				t2.Join ();
			}
		}
	
	
	}


}
