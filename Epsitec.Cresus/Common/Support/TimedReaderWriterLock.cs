using Epsitec.Common.Support.Extensions;

using System;

using System.Threading;


namespace Epsitec.Common.Support
{


	/// <summary>
	/// The <see cref="TimedReaderWriterLock"/> provides a way to wraps lock/unlock calls to
	/// instances of <see cref="ReaderWriterLockSlim"/> within a C# using(...) statement.
	/// </summary>
	/// <remarks>
	/// This class is very similar to <see cref="TimedLock"/>. The main difference is that
	/// this one manages reader writer locks instead of regular exclusive locks.
	/// </remarks>
	public sealed class TimedReaderWriterLock : IDisposable
	{
		

		private TimedReaderWriterLock(ReaderWriterLockSlim rwLock, Mode mode, TimeSpan? timeOut)
		{
			this.rwLock = rwLock;
			this.mode = mode;
			this.timeOut = timeOut;

			this.locked = false;
		}


		~TimedReaderWriterLock()
		{
			if (this.locked)
			{
				throw new System.InvalidOperationException ("Forgot to call dispose!");
			}
		}
		

		#region IDisposable Members


		public void Dispose()
		{
			this.Unlock ();

			System.GC.SuppressFinalize (this);
		}


		#endregion


		private void Lock()
		{
			switch (this.mode)
			{
				case Mode.Read:
					this.LockRead ();				
					break;

				case Mode.Write:
					this.LockWrite ();
					break;

				default:
					throw new System.NotImplementedException ();
			}

			if (!this.locked)
			{
				throw new LockTimeoutException ();
			}
		}


		private void LockRead()
		{
			if (this.timeOut.HasValue)
			{
				this.locked = this.rwLock.TryEnterReadLock (this.timeOut.Value);
			}
			else
			{
				this.rwLock.EnterReadLock ();

				this.locked = true;
			}
		}


		private void LockWrite()
		{
			if (this.timeOut.HasValue)
			{
				this.locked = this.rwLock.TryEnterWriteLock (this.timeOut.Value);
			}
			else
			{
				this.rwLock.EnterWriteLock ();

				this.locked = true;
			}
		}


		private void Unlock()
		{
			if (this.locked)
			{
				switch (this.mode)
				{
					case Mode.Read:
						this.rwLock.ExitReadLock ();
						break;

					case Mode.Write:
						this.rwLock.ExitWriteLock ();
						break;

					default:
						throw new System.NotImplementedException ();
				}
			}
		}


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
			return TimedReaderWriterLock.Lock (rwLock, Mode.Read, timeOut);
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
			return TimedReaderWriterLock.Lock (rwLock, Mode.Write, timeOut);
		}
		

		private static IDisposable Lock(ReaderWriterLockSlim rwLock, Mode mode, TimeSpan? timeOut)
		{
			rwLock.ThrowIfNull ("rwLock");
			timeOut.ThrowIf (t => t.HasValue && t.Value.Ticks < 0, "negative timeout");

			TimedReaderWriterLock l = new TimedReaderWriterLock (rwLock, mode, timeOut);

			l.Lock ();

			return l;
		}


		private bool locked;


		private readonly Mode mode;


		private readonly ReaderWriterLockSlim rwLock;


		private readonly TimeSpan? timeOut;
		

		private enum Mode { Read, Write };


	}


}
