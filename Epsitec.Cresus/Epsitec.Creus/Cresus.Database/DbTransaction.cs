//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System;

using System.Data;

using System.Threading;


namespace Epsitec.Cresus.Database
{
	
	
	/// <summary>
	/// The <c>DbTransaction</c> class encapsulates real ADO.NET transactions
	/// in order to better track them. <c>DbInfrastructure</c> can use this
	/// class to avoid creating nested transactions.
	/// </summary>
	public sealed class DbTransaction : IDbTransaction, IReadOnly, IDisposed, IIsDisposed
	{
		
		
		/// <summary>
		/// Initializes a new instance of the <see cref="DbTransaction"/> class.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="database">The database.</param>
		/// <param name="mode">The transaction mode.</param>
		internal DbTransaction(IDbTransaction transaction, IDbAbstraction database, DbTransactionMode mode)
		{
			this.transaction = transaction;
			this.database = database;
			this.mode = mode;
			
			this.isInheritor = false;
			this.inheritFromTransaction = null;
			this.nbInheritors = 0;

			this.isDisposed = false;
		}

		
		/// <summary>
		/// Initializes a new instance of the <see cref="DbTransaction"/> class.
		/// </summary>
		/// <param name="liveTransaction">The live transaction from which to inherit.</param>
		internal DbTransaction(DbTransaction liveTransaction)
		{
			this.transaction = liveTransaction.transaction;
			this.database = liveTransaction.database;
			this.mode = liveTransaction.mode;
			
			this.isInheritor = true;
			this.inheritFromTransaction = liveTransaction;
			this.nbInheritors = 0;

			this.isDisposed = false;

			Interlocked.Increment (ref this.inheritFromTransaction.nbInheritors);
		}

		
		/// <summary>
		/// Gets the ADO.NET transaction.
		/// </summary>
		/// <value>The ADO.NET transaction.</value>
		public IDbTransaction Transaction
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
		public IDbAbstraction Database
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
		public ISqlBuilder SqlBuilder
		{
			get
			{
				return this.database.SqlBuilder;
			}
		}

		
		/// <summary>
		/// Gets a value indicating whether this transaction is read only.
		/// </summary>
		/// <value><c>true</c> if this transaction is read only; otherwise, <c>false</c>.</value>
		public bool IsReadOnly
		{
			get
			{
				return this.mode == DbTransactionMode.ReadOnly;
			}
		}

		
		/// <summary>
		/// Gets a value indicating whether this transaction is read/write.
		/// </summary>
		/// <value><c>true</c> if this transaction is read/write; otherwise, <c>false</c>.</value>
		public bool IsReadWrite
		{
			get
			{
				return this.mode == DbTransactionMode.ReadWrite;
			}
		}

		
		/// <summary>
		/// Gets a value indicating whether this transaction inherits from another transaction.
		/// </summary>
		/// <value><c>true</c> if this transaction is an inheritor; otherwise, <c>false</c>.</value>
		public bool IsInheritor
		{
			get
			{
				return this.isInheritor;
			}
		}


		private void ThrowIfHasInheritors()
		{
			if (this.nbInheritors > 0)
			{
				var message = "Operation forbidden because this instance has inheritors";

				throw new InvalidOperationException (message);
			}
		}


		private void ThrowIfIsInheritor()
		{
			if (this.IsInheritor)
			{
				var message = "Operation forbidden because this instance inherits from another";

				throw new InvalidOperationException (message);
			}
		}

		
		#region IDbTransaction Members
		
		
		/// <summary>
		/// Rolls back the transaction.
		/// </summary>
		/// <exception cref="InvalidOperationException">If this instance inherits from another transaction.</exception>
		/// <exception cref="InvalidOperationException">If this instance currently has one or more inheritor.</exception>
		public void Rollback()
		{
			this.ThrowIfIsInheritor ();
			this.ThrowIfHasInheritors ();

			this.transaction.Rollback ();
		}


		/// <summary>
		/// Commits the transaction.
		/// </summary>
		/// <exception cref="InvalidOperationException">If this instance currently has one or more inheritors.</exception>
		public void Commit()
		{
			this.ThrowIfHasInheritors ();

			if (!this.isInheritor)
			{
				this.transaction.Commit ();
			}
		}


		/// <summary>
		/// Specifies the <c>Connection</c> object to associate with the transaction.
		/// </summary>
		/// <returns>The <c>Connection</c> object to associate with the transaction.</returns>
		public IDbConnection Connection
		{
			get
			{
				return this.transaction.Connection;
			}
		}

		
		/// <summary>
		/// Specifies the <see cref="IsolationLevel"/> for this transaction.
		/// </summary>
		/// <returns>The <see cref="IsolationLevel"/> for this transaction. The default is <c>ReadCommitted</c>.</returns>
		public IsolationLevel IsolationLevel
		{
			get
			{
				return this.transaction.IsolationLevel;
			}
		}


		#endregion



		#region IIsDisposed Members


		public bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}


		#endregion
		
		
		#region IDisposable Members
		
		
		/// <summary>
		/// Disposes the transaction.
		/// </summary>
		/// <exception cref="InvalidOperationException">If this instance currently has one or more inheritors.</exception>
		public void Dispose()
		{
			this.ThrowIfHasInheritors ();

			if (!this.isDisposed)
			{
				if (this.IsInheritor)
				{
					Interlocked.Decrement (ref this.inheritFromTransaction.nbInheritors);
				}
				else
				{
					this.transaction.Dispose ();
				}

				this.Disposed.Raise (this);
				this.isDisposed = true;
			}
		}

		
		#endregion

		
		#region IDisposed Members

		
		public event Epsitec.Common.Support.EventHandler Disposed;

		
		#endregion

		
		private readonly IDbTransaction transaction;
		private readonly IDbAbstraction database;
		private readonly DbTransactionMode mode;
		
		
		private readonly bool isInheritor;
		private readonly DbTransaction inheritFromTransaction;
		private int nbInheritors;


		private bool isDisposed;


	}


}
