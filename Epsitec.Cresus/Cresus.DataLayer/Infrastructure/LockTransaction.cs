using Epsitec.Cresus.Database;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{


	public sealed class LockTransaction : System.IDisposable
	{

		// TODO Comment this class and related stuff.
		// Marc


		internal LockTransaction(DbInfrastructure dbInfrastructure, string userName, IEnumerable<string> lockNames)
		{
			this.dbInfrastructure = dbInfrastructure;
			this.lockNames = lockNames.ToList ();
			this.userName = userName;
		}


		~LockTransaction()
		{
			this.Dispose (false);
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


		internal static bool TryCreateLockTransaction(DbInfrastructure dbInfrastructure, IEnumerable<string> lockNames, string userName, out LockTransaction lockTransaction)
		{
			lockTransaction = null;

			using (DbTransaction transaction = LockTransaction.CreateWriteTransaction (dbInfrastructure))
			{
				DbLockManager lockManager = dbInfrastructure.LockManager;

				bool canLock = lockNames.All (lockName =>
				{
					return lockManager.IsLockOwned (lockName)
						|| lockManager.GetLockOwner (lockName) == userName;
				});

				if (canLock)
				{
					foreach (string lockName in lockNames)
					{
						lockManager.RequestLock (lockName, userName);
					}

					lockTransaction = new LockTransaction (dbInfrastructure, userName, lockNames);
				}

				transaction.Commit ();
			}

			return lockTransaction != null;
		}
		

		private void Dispose(bool disposing)
		{
			if (!disposing)
			{
				throw new System.InvalidOperationException ("LockTransaction has not been disposed.");
			}

			using (DbTransaction transaction = LockTransaction.CreateWriteTransaction (this.dbInfrastructure))
			{
				foreach (string lockName in this.lockNames)
				{
					this.DbLockManager.ReleaseLock (lockName, this.userName);
				}

				transaction.Commit ();
			}
		}


		private static DbTransaction CreateWriteTransaction(DbInfrastructure dbInfrastructure)
		{
			List<DbTable> tablesToLock = new List<DbTable> ()
			{
				dbInfrastructure.ResolveDbTable (Tags.TableLock),
			};

			return dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite, tablesToLock);
		}


		private IEnumerable<string> lockNames;


		private string userName;


		private DbInfrastructure dbInfrastructure;

	
	}



}
