//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{
	
	
	/// <summary>
	/// The <c>LockTransaction</c> class provides the tools required to manage the high level lock
	/// transactions.
	/// A <c>LockTransaction</c> can contain several locks and it must be used in the appropriate
	/// way: once it has been locked, it must absolutely be unlocked or disposed.
	/// </summary>
	public sealed class LockTransaction : System.IDisposable
	{


		// TODO Comment this class
		// Marc


		/// <summary>
		/// Builds a new <see cref="LockTransaction"/> for a given set of locks.
		/// </summary>
		/// <param name="lockManager">The <see cref="LockManager"/> used to make lock operations in the database.</param>
		/// <param name="connectionId">The id of the connection to use with the transaction.</param>
		/// <param name="lockNames">The names of the locks that must be acquired.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="lockManager"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="connectionId"/> is lower than zero.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="lockNames"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="lockNames"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="lockNames"/> contains <c>null</c> or empty elements.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="lockNames"/> contains duplicates.</exception>
		internal LockTransaction(LockManager lockManager, DbId connectionId, IEnumerable<string> lockNames)
		{
			lockManager.ThrowIfNull ("lockManager");
			connectionId.ThrowIf (cId => cId.IsEmpty, "connectionId cannot be empty");
			lockNames.ThrowIfNull ("lockNames");

			this.State = LockState.Idle;

			this.lockManager = lockManager;
			this.lockNames = lockNames.AsReadOnlyCollection ();
			this.connectionId = connectionId;

			this.lockNames.ThrowIf (names => names.Count == 0, "lockNames is empty.");
			this.lockNames.ThrowIf (names => names.Any (string.IsNullOrEmpty), "lock names cannot be null or empty.");
			this.lockNames.ThrowIf (names => names.Count != names.Distinct ().Count (), "lockNames cannot contain duplicates.");
		}


		/// <summary>
		/// Destructor of the class.
		/// </summary>
		~LockTransaction()
		{
			this.Dispose (false);
		}


		/// <summary>
		/// The current state of the transaction.
		/// </summary>
		/// <remarks>
		/// This state might get out of sync with its real state in the database if the connection
		/// used by the transaction is interrupted. This should never happen while the application is
		/// still running, but you never know.
		/// </remarks>
		public LockState State
		{
			get;
			private set;
		}

		
		/// <summary>
		/// Requests all the locks within the transaction. The success or failure is indicated by
		/// the return value.
		/// </summary>
		/// <returns>A tuple whose first item is a bool that will be <c>true</c> if the locks have
		/// been acquired, and <c>false</c> if they have not. The second item will contain data about
		/// the locks that are not currently available.
		/// </returns>
		/// <exception cref="System.InvalidOperationException">If the state of the transaction is not <see cref="LockState.Idle"/>.</exception>
		public System.Tuple<bool, IEnumerable<Lock>> Lock()
		{
			if (this.State != LockState.Idle)
			{
				throw new System.InvalidOperationException ("Lock operation cannot be performed while in " + this.State + " state");
			}

			var result = this.lockManager.RequestLocks (this.connectionId, this.lockNames);

			var locked = result.Item1;
			var unavailableLocks = result.Item2;

			if (locked)
			{
				this.State = LockState.Locked;
			}

			return System.Tuple.Create (locked, unavailableLocks);
		}


		/// <summary>
		/// Releases all the locks within the transaction. This method must be called if the transaction
		/// has ever been locked.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">If the state of the transaction is not <see cref="LockState.Locked"/>.</exception>
		public void Release()
		{
			if (this.State != LockState.Locked)
			{
				throw new System.InvalidOperationException ("Release operation cannot be performed while in " + this.State + " state");
			}

			this.lockManager.ReleaseLocks (this.connectionId, this.lockNames);

			this.State = LockState.Disposed;
		}


		/// <summary>
		/// For each lock defined in this instance, finds its name, the time at which it has been
		/// acquired and by whom it has been acquired.
		/// </summary>
		/// <returns>The identity of the locks owner and the locks name and creation time for this instance.</returns>
		public IEnumerable<Lock> GetLocks()
		{
			if (this.State == LockState.Disposed)
			{
				throw new System.InvalidOperationException ("This operation cannot be performed while in disposed state");
			}

			return this.lockManager.GetLocks (this.lockNames);
		}


		#region IDisposable Members


		/// <summary>
		/// Disposes the transaction. This method must be called if the transaction has ever been
		/// locked, otherwise, an <see cref="System.InvalidOperationException"/> will be thrown by
		/// the destructor.
		/// </summary>
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
			
		
		#endregion
		
		
		/// <summary>
		/// Disposes the transaction, which release the locks if they haven't been released before.
		/// </summary>
		/// <param name="disposing">Indicates if the method has been called by the Dispose() method or by the destructor.</param>
		private void Dispose(bool disposing)
		{
			if (!disposing && (this.State != LockState.Disposed && this.State != LockState.Idle))
			{
				throw new System.InvalidOperationException ("The method Dispose() has not been called before garbage collection.");
			}

			if (this.State != LockState.Disposed)
			{
				if (this.State == LockState.Locked)
				{
					this.lockManager.ReleaseOwnedLocks (this.connectionId, this.lockNames);
				}
				
				this.State = LockState.Disposed;
			}
		}


		private readonly ReadOnlyCollection<string> lockNames;


		private readonly DbId connectionId;


		private readonly LockManager lockManager;


	}


}
