using Epsitec.Common.Support.Extensions;

using System;

using System.Threading;


namespace Epsitec.Common.Support
{


	/// <summary>
	/// The <see cref="TimedReaderWriterLock"/> class provides a way to wraps lock/unlock calls to
	/// instances of <see cref="ReaderWriterLockSlim"/> within a C# using(...) statement.
	/// </summary>
	/// <remarks>
	/// This class is very similar to <see cref="TimedLock"/>. The main difference is that
	/// this one manages reader writer locks instead of regular exclusive locks.
	/// </remarks>
	public static class TimedReaderWriterLock
	{


		/// <summary>
		/// Tries to acquire the lock defined by <paramref name="rwLock"/> in shared read mode.
		/// </summary>
		/// <param name="rwLock">The lock to acquire.</param>
		/// <param name="timeOut">The maximum time to wait before a <see cref="LockTimeoutException"/> is thrown.</param>
		/// <returns>An instance of <see cref="System.IDisposable"/> that must be disposed in order to release the lock.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="rwLock"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="timeOut"/> is negative.</exception>
		/// <exception cref="LockTimeoutException">If the lock could not be acquired before the given delay ran out.</exception>
		public static IDisposable LockRead(ReaderWriterLockSlim rwLock, TimeSpan? timeOut = null)
		{
			rwLock.ThrowIfNull ("rwLock");			
			timeOut.ThrowIf (t => t.HasValue && t.Value.Ticks < 0, "negative timeout");

			TimedReaderWriterLock.AcquireLockRead (rwLock, timeOut);

			Action releaseAction = () => rwLock.ExitReadLock ();

			return DisposableWrapper.Get (releaseAction);
		}


		/// <summary>
		/// Tries to acquire the lock defined by <paramref name="rwLock"/> in exclusive write mode.
		/// </summary>
		/// <param name="rwLock">The lock to acquire.</param>
		/// <param name="timeOut">The maximum time to wait before a <see cref="LockTimeoutException"/> is thrown.</param>
		/// <returns>An instance of <see cref="System.IDisposable"/> that must be disposed in order to release the lock.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="rwLock"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="timeOut"/> is negative.</exception>
		/// <exception cref="LockTimeoutException">If the lock could not be acquired before the given delay ran out.</exception>
		public static IDisposable LockWrite(ReaderWriterLockSlim rwLock, TimeSpan? timeOut = null)
		{
			rwLock.ThrowIfNull ("rwLock");
			timeOut.ThrowIf (t => t.HasValue && t.Value.Ticks < 0, "negative timeout");

			TimedReaderWriterLock.AcquireLockWrite (rwLock, timeOut);

			Action releaseAction = () => rwLock.ExitWriteLock ();

			return DisposableWrapper.Get (releaseAction);
		}


		private static void AcquireLockRead(ReaderWriterLockSlim rwLock, TimeSpan? timeOut)
		{
			if (timeOut.HasValue)
			{
				bool locked = rwLock.TryEnterReadLock (timeOut.Value);

				if (!locked)
				{
					throw new LockTimeoutException ();
				}
			}
			else
			{
				rwLock.EnterReadLock ();
			}
		}


		private static void AcquireLockWrite(ReaderWriterLockSlim rwLock, TimeSpan? timeOut)
		{
			if (timeOut.HasValue)
			{
				bool locked = rwLock.TryEnterWriteLock (timeOut.Value);

				if (!locked)
				{
					throw new LockTimeoutException ();
				}
			}
			else
			{
				rwLock.EnterWriteLock ();
			}
		}


	}


}
