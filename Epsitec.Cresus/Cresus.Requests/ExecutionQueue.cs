//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// La classe ExecutionQueue permet de représenter la queue des requêtes
	/// en attente d'exécution.
	/// </summary>
	public class ExecutionQueue : IAttachable, IPersistable
	{
		public ExecutionQueue(DbInfrastructure infrastructure)
		{
			this.Setup (infrastructure);
		}
		
		
		
		public System.Data.DataRowCollection	Rows
		{
			get
			{
				return this.queue_data_set.Tables[Tags.TableRequestQueue].Rows;
			}
		}
		
		
		public System.Data.DataRow AddRequest(Requests.Base request)
		{
			byte[] buffer = Requests.Base.SerializeToMemory (request);
			int    length = buffer.Length;
			
			System.Diagnostics.Debug.Assert (length > 0);
			
			System.Data.DataRow row;
			
			this.queue_command.CreateNewRow (Tags.TableRequestQueue, out row);
			
			row.BeginEdit ();
			row[Tags.ColumnQueueData] = buffer;
			row.EndEdit ();
			
			return row;
		}
		
		public Requests.Base GetRequest(System.Data.DataRow row)
		{
			if (row != null)
			{
				byte[] buffer = row[Tags.ColumnQueueData] as byte[];
				return Requests.Base.DeserializeFromMemory (buffer);
			}
			
			return null;
		}
		
		
		
		#region IAttachable Members
		public void Attach(DbInfrastructure infrastructure, DbTable table)
		{
			this.infrastructure = infrastructure;
			this.queue_db_table = table;
		}
		
		public void Detach()
		{
			this.infrastructure = null;
			this.queue_db_table = null;
		}
		#endregion
		
		#region IPersistable Members
		public void RestoreFromBase(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (this.infrastructure != null);
			
			this.queue_command  = DbRichCommand.CreateFromTable (this.infrastructure, transaction, this.queue_db_table, DbSelectRevision.LiveActive);
			this.queue_data_set = this.queue_command.DataSet;
		}
		
		public void SerializeToBase(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (this.queue_command != null);
			
			this.queue_command.UpdateRealIds (transaction);
			this.queue_command.UpdateTables (transaction);
			
			this.queue_command.AcceptChanges ();
		}
		#endregion
		
		private void Setup(DbInfrastructure infrastructure)
		{
			using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbTable table = infrastructure.ResolveDbTable (transaction, Tags.TableRequestQueue);
				
				this.Attach (infrastructure, table);
				this.RestoreFromBase (transaction);
				
				transaction.Commit ();
			}
		}
		
		
		private DbInfrastructure				infrastructure;
		private DbTable							queue_db_table;
		private DbRichCommand					queue_command;
		private System.Data.DataSet				queue_data_set;
	}
}
