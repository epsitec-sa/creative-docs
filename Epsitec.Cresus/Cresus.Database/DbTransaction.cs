//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 01/12/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTransaction encapsule les véritables transactions afin de
	/// permettre un meilleur contrôle sur leur création et leur durée de vie.
	/// Cela permet par exemple à DbInfrastructure de vérifier que l'appelant
	/// ne crée pas de transactions imbriquées.
	/// </summary>
	public class DbTransaction : System.IDisposable, System.Data.IDbTransaction
	{
		internal DbTransaction(System.Data.IDbTransaction transaction, DbInfrastructure infrastructure)
		{
			this.transaction    = transaction;
			this.infrastructure = infrastructure;
			this.infrastructure.NotifyBeginTransaction (this);
		}
		
		
		public System.Data.IDbTransaction		Transaction
		{
			get { return this.transaction; }
		}
		
		public DbInfrastructure					Infrastructure
		{
			get { return this.infrastructure; }
		}
		
		
		#region IDbTransaction Members
		public void Rollback()
		{
			this.transaction.Rollback ();
			this.infrastructure.NotifyEndTransaction (this);
			this.infrastructure = null;
		}

		public void Commit()
		{
			this.transaction.Commit ();
			this.infrastructure.NotifyEndTransaction (this);
			this.infrastructure = null;
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
			}
			if (this.transaction != null)
			{
				this.transaction.Dispose ();
				this.transaction = null;
			}
		}
		#endregion
		
		protected System.Data.IDbTransaction	transaction;
		protected DbInfrastructure				infrastructure;
	}
}
