//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// La classe ExecutionEngine exécute les requêtes qui modifient la base de
	/// données.
	/// </summary>
	public class ExecutionEngine
	{
		public ExecutionEngine(DbInfrastructure infrastructure)
		{
			this.Setup (infrastructure);
		}
		
		
		public DbInfrastructure					Infrastructure
		{
			get
			{
				return this.infrastructure;
			}
		}
		
		public DbId								CurrentLogId
		{
			get
			{
				return this.current_log_id;
			}
		}
		
		public DbTransaction					CurrentTransaction
		{
			get
			{
				return this.current_transaction;
			}
		}
		
		
		public void Execute(DbTransaction transaction, AbstractRequest request)
		{
			try
			{
				this.DefineCurrentLogId ();
				this.DefineCurrentTransaction (transaction);
				
//				request.Execute (this);
			}
			finally
			{
				this.ClearCurrentSettings ();
			}
		}
		
		
		private void Setup(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
		}
		
		
		private void DefineCurrentLogId()
		{
			System.Diagnostics.Debug.Assert (this.current_log_id.Value == 0);
			
			this.current_log_id = this.infrastructure.Logger.CurrentId;
		}
		
		private void DefineCurrentTransaction(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (this.current_transaction == null);
			
			this.current_transaction = transaction;
		}
		
		private void ClearCurrentSettings()
		{
			this.current_log_id      = DbId.Zero;
			this.current_transaction = null;
		}
		
		
		private DbInfrastructure				infrastructure;
		private DbId							current_log_id;
		private DbTransaction					current_transaction;
	}
}
