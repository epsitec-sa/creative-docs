//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTransaction encapsule les véritables transactions afin de
	/// permettre un meilleur contrôle sur leur création et leur durée de vie.
	/// Cela permet par exemple à DbInfrastructure de vérifier que l'appelant
	/// ne crée pas de transactions imbriquées.
	/// </summary>
	public sealed class DbTransaction : System.IDisposable, System.Data.IDbTransaction, Epsitec.Common.Types.IReadOnly
	{
		internal DbTransaction(System.Data.IDbTransaction transaction, IDbAbstraction database, DbInfrastructure infrastructure, DbTransactionMode mode)
		{
			this.transaction    = transaction;
			this.database       = database;
			this.infrastructure = infrastructure;
			this.mode           = mode;
			
			this.infrastructure.NotifyBeginTransaction (this);
		}
		
		
		public System.Data.IDbTransaction		Transaction
		{
			get
			{
				return this.transaction;
			}
		}
		
		public IDbAbstraction					Database
		{
			get
			{
				return this.database;
			}
		}
		
		public DbInfrastructure					Infrastructure
		{
			get
			{
				return this.infrastructure;
			}
		}
		
		public bool								IsReadOnly
		{
			get
			{
				return this.mode == DbTransactionMode.ReadOnly;
			}
		}
		
		public bool								IsReadWrite
		{
			get
			{
				return this.mode == DbTransactionMode.ReadWrite;
			}
		}
		
		
		#region IDbTransaction Members
		public void Rollback()
		{
			this.transaction.Rollback ();
			this.infrastructure.NotifyEndTransaction (this);
			this.infrastructure = null;
			this.OnReleased ();
		}

		public void Commit()
		{
			this.transaction.Commit ();
			this.infrastructure.NotifyEndTransaction (this);
			this.infrastructure = null;
			this.OnReleased ();
		}

		
		public System.Data.IDbConnection		Connection
		{
			get
			{
				return this.transaction.Connection;
			}
		}

		public System.Data.IsolationLevel		IsolationLevel
		{
			get
			{
				return this.transaction.IsolationLevel;
			}
		}
		#endregion
		
		#region IDisposable Members
		public void Dispose()
		{
			if (this.infrastructure != null)
			{
				this.infrastructure.NotifyEndTransaction (this);
				this.infrastructure = null;
				this.OnReleased ();
			}
			if (this.transaction != null)
			{
				this.transaction.Dispose ();
				this.transaction = null;
			}
			
			this.database = null;
		}
		#endregion
		
		private void OnReleased()
		{
			if (this.Released != null)
			{
				this.Released (this);
			}
		}
		
		
		public event EventHandler				Released;
		
		private System.Data.IDbTransaction		transaction;
		private IDbAbstraction					database;
		private DbInfrastructure				infrastructure;
		private readonly DbTransactionMode		mode;
	}
}
