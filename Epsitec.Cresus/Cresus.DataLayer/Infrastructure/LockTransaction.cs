//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Services;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{


	/// <summary>
	/// The <c>LockTransaction</c> class provides the tools required to manage the high level
	/// transactions.
	/// A <c>LockTransaction</c> can contain several locks and it must be used in the appropriate
	/// way: once it has been locked, it must absolutely be unlocked or disposed.
	/// </summary>
	public sealed class LockTransaction : System.IDisposable
	{
		
		
		/// <summary>
		/// Builds a new <see cref="LockTransaction"/> for a given set of locks.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> used to communicate with the database.</param>
		/// <param name="connectionId">The id of the connection to use with the transaction.</param>
		/// <param name="lockNames">The names of the locks that must be acquired.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dbInfrastructure"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="connectionId"/> is lower than zero.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="lockNames"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="lockNames"/> contains <c>null</c> or empty elements.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="lockNames"/> contains duplicates.</exception>
		internal LockTransaction(DbInfrastructure dbInfrastructure, long connectionId, IEnumerable<string> lockNames)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			connectionId.ThrowIf (c => c < 0, "connectionId cannot be lower than zero");
			lockNames.ThrowIfNull ("lockNames");
			lockNames.ThrowIf (names => names.Any (n => string.IsNullOrEmpty (n)), "lock names cannot be null or empty.");
			lockNames.ThrowIf (names => names.Count () != names.Distinct ().Count (), "lockNames cannot contain duplicates.");
			
			this.State = LockState.Idle;

			this.dbInfrastructure = dbInfrastructure;
			this.lockNames = lockNames.ToList ();
			this.connectionId = connectionId;
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
		/// The <see cref="DbLockManager"/> used to access the low level lock functionnalities.
		/// </summary>
		private DbLockManager DbLockManager
		{
			get
			{
				return this.dbInfrastructure.ServiceManager.LockManager;
			}
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
		/// Requests all the locks within the transaction. The success or failure is indicated by
		/// the return value.
		/// </summary>
		/// <param name="lockOwners">An optional list which will be filled with the foreign lock owners, if any.</param>
		/// <returns>
		/// 	<c>true</c> if the locks have been acquired, <c>false</c> if they have not.
		/// </returns>
		/// <exception cref="System.InvalidOperationException">If the state of the transaction is not <see cref="LockState.Idle"/>.</exception>
		public bool Lock(IList<LockOwner> lockOwners = null)
		{
			if (this.State != LockState.Idle)
			{
				throw new System.InvalidOperationException ("Lock operation cannot be performed while in " + this.State + " state");
			}

			if (lockOwners != null)
			{
				lockOwners.Clear ();
			}

			using (DbTransaction transaction = LockTransaction.CreateWriteTransaction (this.dbInfrastructure))
			{
				bool isLocked = this.InternalLock ();

				if (isLocked)
				{
					this.State = LockState.Locked;
				}
				else
				{
					if (lockOwners != null)
					{
						lockOwners.AddRange (this.GetLockOwners ());
					}
				}

				transaction.Commit ();

				return isLocked;
			}
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

			using (DbTransaction transaction = LockTransaction.CreateWriteTransaction (this.dbInfrastructure))
			{
				this.InternalRelease ();

				transaction.Commit ();
			}

			this.State = LockState.Disposed;
		}


		/// <summary>
		/// For each lock defined in this instance, finds its name, the time at which it has been
		/// acquired and by whom it has been acquired.
		/// </summary>
		/// <returns>The identity of the locks owner and the locks name and creation time for this instance.</returns>
		public IEnumerable<LockOwner> GetLockOwners()
		{
			if (this.State == LockState.Disposed)
			{
				throw new System.InvalidOperationException ("This operation cannot be performed while in disposed state");
			}

			using (DbTransaction transaction = LockTransaction.CreateReadTransaction (dbInfrastructure))
			{
				var data = this.InternalGetLockOwners ();

				transaction.Commit ();

				return data;
			}
		}


		/// <summary>
		/// Requests all the locks. This method does not uses any transaction, so its call must be
		/// included in one to ensure atomicity.
		/// </summary>
		/// <returns><c>true</c> if the locks have been acquired, <c>false</c> if they have not.</returns>
		private bool InternalLock()
		{
			bool canLock = LockTransaction.AreAllLocksAvailable (this.DbLockManager, this.connectionId, this.lockNames);

			if (canLock)
			{
				foreach (string lockName in lockNames)
				{
					this.DbLockManager.RequestLock (lockName, this.connectionId);
				}
			}

			return canLock;
		}


		/// <summary>
		/// Releases all the locks. This method does not uses any transaction, so its call must be
		/// included in one to ensure atomicity.
		/// </summary>
		private void InternalRelease()
		{
			foreach (string lockName in this.lockNames)
			{
				this.DbLockManager.ReleaseLock (lockName, this.connectionId);
			}
		}


		/// <summary>
		/// Gets the data about who owns the locks of this instance. This method does not uses any
		/// transaction, so its call must be included in one to ensure atomicity.
		/// </summary>
		/// <returns>The identity of the locks owner and the locks name and creation time for this instance.</returns>
		private IEnumerable<LockOwner> InternalGetLockOwners()
		{
			var data = this.dbInfrastructure.ServiceManager.ConnectionManager.GetLockOwners (this.lockNames);

			return data.Select (x => new LockOwner (x)).ToList ();
		}

		
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
					this.Release ();
				}
				
				this.State = LockState.Disposed;
			}
		}


		/// <summary>
		/// Checks whether a set of locks is available for a given connection id.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> object used to communicate with the database.</param>
		/// <param name="connectionId">The id of the connection used for the lock.</param>
		/// <param name="lockNames">The name of the locks whose state to check.</param>
		/// <returns><c>true</c> if fall locks coule be acquired, <c>false</c> if at least one of them could not.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dbInfrastructure"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="connectionId"/> is lower than zero.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="lockNames"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="lockNames"/> contains <c>null</c> or empty elements.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="lockNames"/> contains duplicates.</exception>
		internal static bool AreAllLocksAvailable(DbInfrastructure dbInfrastructure, long connectionId, IEnumerable<string> lockNames)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			connectionId.ThrowIf (c => c < 0, "connectionId cannot be lower than zero");
			lockNames.ThrowIfNull ("lockNames");
			lockNames.ThrowIf (names => names.Any (n => string.IsNullOrEmpty (n)), "lock names cannot be null or empty.");
			lockNames.ThrowIf (names => names.Count () != names.Distinct ().Count (), "lockNames cannot contain duplicates.");

			DbLockManager lockManager = dbInfrastructure.ServiceManager.LockManager;

			using (DbTransaction transaction = LockTransaction.CreateReadTransaction (dbInfrastructure))
			{
				bool available = LockTransaction.AreAllLocksAvailable (lockManager, connectionId, lockNames);

				transaction.Commit ();

				return available;
			}
		}


		/// <summary>
		/// Removes the locks associated with an inactive connection.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> object used to communicate with the database.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dbInfrastructure"/> is <c>null</c>.</exception>
		internal static void RemoveInactiveLocks(DbInfrastructure dbInfrastructure)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");

			DbLockManager lockManager = dbInfrastructure.ServiceManager.LockManager;

			lockManager.RemoveInactiveLocks ();
		}

		
		/// <summary>
		/// Checks whether a set of locks is available for a given connection id.
		/// </summary>
		/// <param name="lockManager">The <see cref="DbLockManager"/> object used to communicate with the database.</param>
		/// <param name="connectionId">The id of the connection used for the lock.</param>
		/// <param name="lockNames">The name of the locks whose state to check.</param>
		/// <returns><c>true</c> if fall locks coule be acquired, <c>false</c> if at least one of them could not.</returns>
		private static bool AreAllLocksAvailable(DbLockManager lockManager, long connectionId, IEnumerable<string> lockNames)
		{
			return lockNames.All (lockName =>
			{
				return !lockManager.IsLockOwned (lockName)
                	|| lockManager.GetLock (lockName).ConnectionId == connectionId;
			});
		}


		/// <summary>
		/// Creates the <see cref="DbTransaction"/> object used for read only transactions.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> object used to communicate with the database.</param>
		/// <returns>The <see cref="DbTransaction"/> object</returns>
		private static DbTransaction CreateReadTransaction(DbInfrastructure dbInfrastructure)
		{
			return dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly);
		}


		/// <summary>
		/// Creates the <see cref="DbTransaction"/> object used for read write transactions.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> object used to communicate with the database.</param>
		/// <returns>The <see cref="DbTransaction"/> object</returns>
		private static DbTransaction CreateWriteTransaction(DbInfrastructure dbInfrastructure)
		{
			List<DbTable> tablesToLock = new List<DbTable> ()
			{
				dbInfrastructure.ResolveDbTable (Tags.TableLock),
			};

			return dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite, tablesToLock);
		}


		/// <summary>
		/// The names of the locks contained in the transaction.
		/// </summary>
		private IEnumerable<string> lockNames;


		/// <summary>
		/// The id of the connection used to acquire the locks.
		/// </summary>
		private long connectionId;


		/// <summary>
		/// The <see cref="DbInfrastructure"/> object used to communicate with the database.
		/// </summary>
		private DbInfrastructure dbInfrastructure;


	}

}
