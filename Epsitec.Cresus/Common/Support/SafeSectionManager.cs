using System;

using System.Threading;


namespace Epsitec.Common.Support
{


	/// <summary>
	/// The <c>SafeSectionManager</c> class provides an easy way to create safe section that will
	/// ensure that the Dispose method will block until the end of their lifetime and that when the
	/// Dispose method of that is called, any further attempt to create a safe section will fail
	/// with an Exception.
	/// </summary>
	public sealed class SafeSectionManager : IDisposable
	{


		// NOTE The implementation of the synchronization between the Enter, the Exit, the Close and
		// the Wait methods can probably be improved. However, this kind of multi threaded stuff is
		// really tricky so I preferred to have a working solution instead of a clever but maybe not
		// working solution. Only if this code ever becomes a performance bottleneck, it would be a
		// good idea to improve it.


		/// <summary>
		/// Creates a new SafeSectionManager.
		/// </summary>
		public SafeSectionManager()
		{
			this.exclusion = new object ();
		}

		
		/// <summary>
		/// Creates a new safe section. If this method succeeds, it is guaranteed that any call to
		/// the Dispose method will block until this safe section is exited.
		/// </summary>
		/// <exception cref="ObjectDisposedException">If a call to Dispose has been made</exception>
		/// <returns>An object that must be disposed to exit the safe section.</returns>
		public IDisposable Create()
		{
			var safeSection = this.TryCreate ();

			if (safeSection == null)
			{
				throw new ObjectDisposedException ("This instance is being or has been disposed");
			}

			return safeSection;
		}


		/// <summary>
		/// Tries to create a new safe section. If this method succeeds, it is guaranteed that any
		/// call to the Dispose method will block until this safe section is exited. If a call to
		/// the Dispose method has been made, this method returns null.
		/// </summary>
		/// <returns>
		/// An object that must be disposed to exit the safe section, or null if the safe section
		/// could not be created.
		/// </returns>
		public IDisposable TryCreate()
		{
			var success = this.Enter ();

			if (!success)
			{
				return null;
			}

			return DisposableWrapper.CreateDisposable (this.Exit);
		}


		/// <summary>
		/// Enter a new safe section. This method will fail if the Dispose method as already been
		/// called.
		/// </summary>
		/// <returns>True if the creation was successful, false if it was not.</returns>
		private bool Enter()
		{
			lock (this.exclusion)
			{
				// First we check that the Dispose method has not been called. If it has been, we
				// don't accept to enter in a safe section at this point and return false.

				if (this.isDisposing)
				{
					return false;
				}

				// We increment the count variable to notify that we have entered in a safe section
				// and that Dispose must wait for our exit to proceed.

				this.safeSectionCount++;
			}

			// The safe section has been successfully created, so we return true.
			return true;
		}


		/// <summary>
		/// Exit from a safe section.
		/// </summary>
		private void Exit()
		{
			bool wakeUpDisposeThread;

			lock (this.exclusion)
			{
				// We decrement the count variable to notify that we have exited the safe section
				// and that Dispose don't need to wait on us to proceed.

				this.safeSectionCount--;

				// Check whether there is a thread waiting to dispose this instance and if we should
				// wake it up.

				wakeUpDisposeThread = this.isDisposing && this.safeSectionCount == 0;
			}

			// Wake up the thread waiting to dispose if we should do it.

			if (wakeUpDisposeThread)
			{
				this.disposeAlarm.Set ();
			}
		}


		/// <summary>
		/// Closes this object. This means that from now on, any attempt to create new safe sections
		/// will fail.
		/// </summary>
		private void Close()
		{
			lock (this.exclusion)
			{
				// First we let the Enter method know that Dispose has been called so that it won't
				// allow any new safe sections to be entered.

				this.isDisposing = true;

				// We create the alarm so that threads that are within a safe section can wake us up
				// when they exit.

				this.disposeAlarm = new AutoResetEvent (false);
			}
		}


		/// <summary>
		/// Waits until all safe sections have been exited.
		/// </summary>
		private void Wait()
		{
			bool shouldWait;

			// Checks whether there are threads executing a safe section.

			lock (this.exclusion)
			{
				shouldWait = this.safeSectionCount > 0;
			}

			// If there are threads that are executing a safe section, we wait for the last one to
			// wake us up.

			if (shouldWait)
			{
				this.disposeAlarm.WaitOne ();
			}
		}


		#region IDisposable Members


		/// <summary>
		/// Disposes this instance. A call to this method forbids further call to the Create method
		/// and will then block until all safe sections are exited.
		/// </summary>
		public void Dispose()
		{
			this.Close ();
			this.Wait ();

			this.disposeAlarm.Dispose ();
		}


		#endregion


		/// <summary>
		/// The object used to ensure mutual exclusion in critical sections of this class.
		/// </summary>
		private readonly object exclusion;


		/// <summary>
		/// The EventWaitHandle that is used by the Exit method to tell the Dispose method that it's
		/// time to wake up.
		/// </summary>
		private AutoResetEvent disposeAlarm;


		/// <summary>
		/// Indicates whether this instance is being disposed.
		/// </summary>
		private bool isDisposing;


		/// <summary>
		/// Monitors the current number of safe sections.
		/// </summary>
		private int safeSectionCount;


	}


}
