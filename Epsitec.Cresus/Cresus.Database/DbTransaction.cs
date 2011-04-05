//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbTransaction</c> class encapsulates real ADO.NET transactions
	/// in order to better track them. <c>DbInfrastructure</c> can use this
	/// class to avoid creating nested transactions.
	/// </summary>
	public sealed class DbTransaction : System.IDisposable, System.Data.IDbTransaction, Epsitec.Common.Types.IReadOnly
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbTransaction"/> class.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="database">The database.</param>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="mode">The transaction mode.</param>
		internal DbTransaction(System.Data.IDbTransaction transaction, IDbAbstraction database, DbInfrastructure infrastructure, DbTransactionMode mode)
		{
			this.transaction    = transaction;
			this.database       = database;
			this.infrastructure = infrastructure;
			this.mode           = mode;
			
			this.infrastructure.NotifyBeginTransaction (this);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbTransaction"/> class.
		/// </summary>
		/// <param name="liveTransaction">The live transaction from which to inherit.</param>
		internal DbTransaction(DbTransaction liveTransaction)
		{
			this.transaction    = liveTransaction.transaction;
			this.database       = liveTransaction.database;
			this.infrastructure = liveTransaction.infrastructure;
			this.mode           = liveTransaction.mode;

			this.inheritFromTransaction = liveTransaction;

			System.Threading.Interlocked.Increment (ref this.inheritFromTransaction.inheritanceCount);

		}

		/// <summary>
		/// Gets the ADO.NET transaction.
		/// </summary>
		/// <value>The ADO.NET transaction.</value>
		public System.Data.IDbTransaction		Transaction
		{
			get
			{
				return this.transaction;
			}
		}

		/// <summary>
		/// Gets the database.
		/// </summary>
		/// <value>The database.</value>
		public IDbAbstraction					Database
		{
			get
			{
				return this.database;
			}
		}

		/// <summary>
		/// Gets the SQL builder.
		/// </summary>
		/// <value>The SQL builder.</value>
		public ISqlBuilder						SqlBuilder
		{
			get
			{
				return this.database.SqlBuilder;
			}
		}

		/// <summary>
		/// Gets the infrastructure.
		/// </summary>
		/// <value>The infrastructure.</value>
		public DbInfrastructure					Infrastructure
		{
			get
			{
				return this.infrastructure;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this transaction is read only.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this transaction is read only; otherwise, <c>false</c>.
		/// </value>
		public bool								IsReadOnly
		{
			get
			{
				return this.mode == DbTransactionMode.ReadOnly;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this transaction is read/write.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this transaction is read/write; otherwise, <c>false</c>.
		/// </value>
		public bool								IsReadWrite
		{
			get
			{
				return this.mode == DbTransactionMode.ReadWrite;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this transaction is inherited.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this transaction is inherited; otherwise, <c>false</c>.
		/// </value>
		public bool								IsInherited
		{
			get
			{
				return this.inheritFromTransaction != null;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this transaction is currently
		/// active. Committing, rolling back or disposing will render the
		/// transaction inactive.
		/// </summary>
		/// <value><c>true</c> if this transaction is active; otherwise, <c>false</c>.</value>
		public bool								IsActive
		{
			get
			{
				return this.infrastructure != null;
			}
		}
		
		#region IDbTransaction Members
		
		/// <summary>
		/// Rolls back the transaction.
		/// </summary>
		public void Rollback()
		{
			if (this.IsInherited)
			{
				throw new System.InvalidOperationException ("Inherited transaction may not be rolled back");
			}
			else if (this.inheritanceCount > 0)
			{
				throw new System.InvalidOperationException ("Rollback of inherited transaction prohibited");
			}

			this.transaction.Rollback ();
			this.infrastructure.NotifyEndTransaction (this);
			this.infrastructure = null;
		}

		/// <summary>
		/// Commits the transaction.
		/// </summary>
		public void Commit()
		{
			if (this.IsInherited)
			{
				System.Threading.Interlocked.Decrement (ref this.inheritFromTransaction.inheritanceCount);

				this.infrastructure.NotifyEndInheritedTransaction (this);

				this.inheritFromTransaction = null;
				this.infrastructure         = null;
				this.transaction            = null;
			}
			else if (this.inheritanceCount > 0)
			{
				throw new System.InvalidOperationException ("Commit of inherited transaction prohibited");
			}
			else
			{
				this.transaction.Commit ();
				this.infrastructure.NotifyEndTransaction (this);
				this.infrastructure = null;
			}
		}


		/// <summary>
		/// Specifies the <c>Connection</c> object to associate with the transaction.
		/// </summary>
		/// <returns>The <c>Connection</c> object to associate with the transaction.</returns>
		public System.Data.IDbConnection		Connection
		{
			get
			{
				return this.transaction.Connection;
			}
		}

		/// <summary>
		/// Specifies the <see cref="T:System.Data.IsolationLevel"/> for this transaction.
		/// </summary>
		/// <returns>The <see cref="T:System.Data.IsolationLevel"/> for this transaction. The default is <c>ReadCommitted</c>.</returns>
		public System.Data.IsolationLevel		IsolationLevel
		{
			get
			{
				return this.transaction.IsolationLevel;
			}
		}

		#endregion
		
		#region IDisposable Members
		
		/// <summary>
		/// Disposes the transaction.
		/// </summary>
		public void Dispose()
		{
			if (this.inheritanceCount > 0)
			{
				throw new System.InvalidOperationException ("Disposing inherited transaction");
			}

			if (this.IsInherited)
			{
				throw new System.InvalidOperationException ("Inherited transaction may not be rolled back by Dispose");
			}

			if (this.infrastructure != null)
			{
				this.infrastructure.NotifyEndTransaction (this);
				this.infrastructure = null;
			}
			if (this.transaction != null)
			{
				this.transaction.Dispose ();
				this.transaction = null;
			}
			
			this.database = null;
		}

		#endregion

		private System.Data.IDbTransaction		transaction;
		private IDbAbstraction					database;
		private DbInfrastructure				infrastructure;
		private readonly DbTransactionMode		mode;
		private DbTransaction					inheritFromTransaction;
		private int								inheritanceCount;
	}
}
