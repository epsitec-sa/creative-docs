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
			this.enqueue_event = new System.Threading.AutoResetEvent (false);
			
			this.Setup (infrastructure);
		}
		
		
		public System.Data.DataRowCollection	Rows
		{
			get
			{
				return this.queue_data_set.Tables[Tags.TableRequestQueue].Rows;
			}
		}
		
		public System.Threading.AutoResetEvent	EnqueueEvent
		{
			get
			{
				return this.enqueue_event;
			}
		}
		
		
		public void Enqueue(AbstractRequest request)
		{
			this.AddRequest (request);
			this.enqueue_event.Set ();
		}
		
		
		public void WaitOnEnqueueEvent(System.TimeSpan timeout)
		{
			this.enqueue_event.WaitOne (timeout, true);
		}
		
		
		public System.Data.DataRow AddRequest(AbstractRequest request)
		{
			byte[] buffer = AbstractRequest.SerializeToMemory (request);
			int    length = buffer.Length;
			
			System.Diagnostics.Debug.Assert (length > 0);
			
			System.Data.DataRow row;
			
			this.queue_command.CreateNewRow (Tags.TableRequestQueue, out row);
			
			row.BeginEdit ();
			row[Tags.ColumnReqData]    = buffer;
			row[Tags.ColumnReqExState] = (int) ExecutionState.Pending;
			row.EndEdit ();
			
			return row;
		}
		
		public AbstractRequest GetRequest(System.Data.DataRow row)
		{
			if (row != null)
			{
				byte[] buffer = row[Tags.ColumnReqData] as byte[];
				return AbstractRequest.DeserializeFromMemory (buffer);
			}
			
			return null;
		}
		
		public ExecutionState GetRequestExecutionState(System.Data.DataRow row)
		{
			this.CheckRow (row);
			
			System.Enum state;
			
			if (Epsitec.Common.Types.Converter.Convert (row[Tags.ColumnReqExState], typeof (ExecutionState), out state))
			{
				return (ExecutionState) state;
			}
			
			throw new System.InvalidCastException ("Invalid ExecutionState in row.");
		}
		
		public void SetRequestExecutionState(System.Data.DataRow row, ExecutionState new_state)
		{
			ExecutionState current_state = this.GetRequestExecutionState (row);
			
			if (StateMachine.Check (current_state, new_state) == false)
			{
				throw new System.InvalidOperationException (string.Format ("Cannot change from state {0} to {1}.", current_state, new_state));
			}
			
			row[Tags.ColumnReqExState] = (short) new_state;
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
		
		protected void CheckRow(System.Data.DataRow row)
		{
			if (row == null)
			{
				throw new System.ArgumentNullException ("Row is null.");
			}
			if (row.Table.DataSet != this.queue_data_set)
			{
				throw new System.ArgumentException ("Invalid row specified.");
			}
		}
		
		
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
		
		private System.Threading.AutoResetEvent	enqueue_event;
	}
}
