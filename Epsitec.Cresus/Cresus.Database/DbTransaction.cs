//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTransaction encapsule les v�ritables transactions afin de
	/// permettre un meilleur contr�le sur leur cr�ation et leur dur�e de vie.
	/// Cela permet par exemple � DbInfrastructure de v�rifier que l'appelant
	/// ne cr�e pas de transactions imbriqu�es.
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
		}
		#endregion
		
		protected virtual void OnReleased()
		{
			if (this.Released != null)
			{
				this.Released (this);
			}
		}
		
		public event EventHandler				Released;
		
		protected System.Data.IDbTransaction	transaction;
		protected DbInfrastructure				infrastructure;
	}
}
