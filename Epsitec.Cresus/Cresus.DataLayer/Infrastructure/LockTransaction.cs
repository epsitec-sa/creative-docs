//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using System.Collections.Generic;

using System.Linq;

namespace Epsitec.Cresus.DataLayer.Infrastructure
{
	public sealed class LockTransaction : System.IDisposable
	{
		// TODO Comment this class and related stuff.
		// Marc

		internal LockTransaction(DbInfrastructure dbInfrastructure, long connectionId, IEnumerable<string> lockNames)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			lockNames.ThrowIfNull ("lockNames");
			lockNames.ThrowIf (names => names.Any (n => string.IsNullOrEmpty (n)), "lock names cannot be null or empty.");
			lockNames.ThrowIf (names => names.Count () != names.Distinct ().Count (), "lockNames cannot contain duplicates.");
			
			this.State = LockState.Idle;

			this.dbInfrastructure = dbInfrastructure;
			this.lockNames = lockNames.ToList ();
			this.connectionId = connectionId;
		}

		~LockTransaction()
		{
			this.Dispose (false);
		}


		public LockState State
		{
			get;
			private set;
		}

		private DbLockManager DbLockManager
		{
			get
			{
				return this.dbInfrastructure.LockManager;
			}
		}

		
		#region IDisposable Members

		
		public void Dispose()
		{
			this.Dispose (true);	
		}
			
		
		#endregion
		

		public bool Lock()
		{
			if (this.State != LockState.Idle)
			{
				throw new System.InvalidOperationException ("Lock operation cannot be performed while in " + this.State + " state");
			}

			bool isLocked = this.InternalLock ();

			if (isLocked)
			{
				this.State = LockState.Locked;
			}

			return isLocked;
		}

		public void Release()
		{
			if (this.State != LockState.Locked)
			{
				throw new System.InvalidOperationException ("Release operation cannot be performed while in " + this.State + " state");
			}

			this.InternalRelease();

			this.State = LockState.Disposed;
		}


		private bool InternalLock()
		{
			using (DbTransaction transaction = this.CreateWriteTransaction ())
			{
				DbLockManager lockManager = dbInfrastructure.LockManager;

				bool canLock = lockNames.All (lockName =>
				{
					return lockManager.IsLockOwned (lockName)
						|| lockManager.GetLockOwner (lockName) == this.connectionId + "";
				});

				if (canLock)
				{
					foreach (string lockName in lockNames)
					{
						lockManager.RequestLock (lockName, this.connectionId + "");
					}
				}

				transaction.Commit ();

				return canLock;
			}
		}

		private void InternalRelease()
		{
			using (DbTransaction transaction = this.CreateWriteTransaction ())
			{
				foreach (string lockName in this.lockNames)
				{
					this.DbLockManager.ReleaseLock (lockName, this.connectionId + "");
				}

				transaction.Commit ();
			}
		}
		
		
		private void Dispose(bool disposing)
		{
			if (this.State != LockState.Disposed)
			{
				if (this.State == LockState.Locked)
				{
					this.Release ();
				}
				
				this.State = LockState.Disposed;
			}
		}


		private DbTransaction CreateWriteTransaction()
		{
			List<DbTable> tablesToLock = new List<DbTable> ()
			{
				this.dbInfrastructure.ResolveDbTable (Tags.TableLock),
			};

			return this.dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite, tablesToLock);
		}


		private IEnumerable<string> lockNames;

		private long connectionId;

		private DbInfrastructure dbInfrastructure;
	}
}
