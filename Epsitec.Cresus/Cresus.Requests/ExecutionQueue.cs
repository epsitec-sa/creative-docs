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
		public ExecutionQueue(DbInfrastructure infrastructure, IDbAbstraction database)
		{
			this.infrastructure = infrastructure;
			this.database       = (database == null) ? this.infrastructure.DefaultDbAbstraction : database;
			this.enqueue_event  = new System.Threading.AutoResetEvent (false);
			this.is_server      = this.infrastructure.LocalSettings.IsServer;
			
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly, this.database))
			{
				DbTable table = this.infrastructure.ResolveDbTable (transaction, Tags.TableRequestQueue);
				
				this.Attach (infrastructure, table);
				this.RestoreFromBase (transaction);
				
				transaction.Commit ();
			}

		}
		
		
		public System.Data.DataRow[]			Rows
		{
			get
			{
				lock (this.queue_data_table)
				{
					System.Data.DataRow[] rows = new System.Data.DataRow[this.queue_data_table.Rows.Count];
					
					this.queue_data_table.Rows.CopyTo (rows, 0);
					
					return rows;
				}
			}
		}
		
		public System.Threading.AutoResetEvent	EnqueueEvent
		{
			get
			{
				return this.enqueue_event;
			}
		}
		
		public bool								IsRunningAsServer
		{
			get
			{
				return this.is_server;
			}
		}
		
		
		public void Enqueue(AbstractRequest request)
		{
			System.Data.DataRow row = this.AddRequest (request);
			
			lock (this.queue_data_table)
			{
				this.queue_data_table.Rows.Add (row);
			}
			
			this.enqueue_event.Set ();
		}
		
		public void Enqueue(byte[][] serialized_requests, DbId[] ids)
		{
			//	Ajoute une série de requêtes qui sont déjà sous forme sérialisée et leur
			//	attribue un ID particulier (cet ID dépend du client et doit être associé
			//	à la requête pour permettre de dialoguer correctement avec le client).
			
			System.Diagnostics.Debug.Assert (serialized_requests.Length == ids.Length);
			
			lock (this.queue_data_table)
			{
				for (int i = 0; i < serialized_requests.Length; i++)
				{
					System.Data.DataRow row = this.AddRequest (serialized_requests[i]);
					
					//	L'appelant force des IDs pour les diverses requêtes (parce qu'elles proviennent
					//	d'un client distant qui a déjà attribués les IDs).
					
					row[Tags.ColumnId] = ids[i].Value;
					
					this.queue_data_table.Rows.Add (row);
				}
			}
			
			System.Diagnostics.Debug.WriteLine ("Enqueued " + serialized_requests.Length + " serialized requests.");
			
			this.enqueue_event.Set ();
		}
		
		public void ClearQueue()
		{
			System.Data.DataRow[] rows = this.Rows;
			
			for (int i = 0; i < rows.Length; i++)
			{
				DbRichCommand.KillRow (rows[i]);
			}
		}
		
		public void WaitOnEnqueueEvent(System.TimeSpan timeout)
		{
			this.enqueue_event.WaitOne (timeout, true);
		}
		
		
		public System.Data.DataRow AddRequest(AbstractRequest request)
		{
			return this.AddRequest (AbstractRequest.SerializeToMemory (request));
		}
		
		public System.Data.DataRow AddRequest(byte[] data)
		{
			int length = data.Length;
			
			System.Diagnostics.Debug.Assert (length > 0);
			
			System.Data.DataRow row;
			
			DbRichCommand.CreateRow (this.queue_data_table, out row);
			
			row.BeginEdit ();
			row[Tags.ColumnReqData]    = data;
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
			if (this.infrastructure == infrastructure)
			{
				System.Diagnostics.Debug.Assert (this.database != null);
				
				this.queue_db_table = table;
			}
			else
			{
				this.infrastructure = infrastructure;
				this.database       = this.infrastructure.DefaultDbAbstraction;
				this.queue_db_table = table;
			}
		}
		
		public void Detach()
		{
			this.infrastructure = null;
			this.database       = null;
			this.queue_db_table = null;
		}
		#endregion
		
		#region IPersistable Members
		public void RestoreFromBase(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (this.infrastructure != null);
			
			this.queue_command    = DbRichCommand.CreateFromTable (this.infrastructure, transaction, this.queue_db_table, DbSelectRevision.LiveActive);
			this.queue_data_set   = this.queue_command.DataSet;
			this.queue_data_table = this.queue_data_set.Tables[Tags.TableRequestQueue];
		}
		
		public void SerializeToBase(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (this.queue_command != null);
			
			lock (this.queue_data_table)
			{
				this.queue_command.UpdateRealIds (transaction);
				this.queue_command.UpdateTables (transaction);
				
				this.queue_command.AcceptChanges ();
			}
		}
		#endregion
		
		protected void CheckRow(System.Data.DataRow row)
		{
			if (row == null)
			{
				throw new System.ArgumentNullException ("Row is null.");
			}
			if (DbRichCommand.IsRowDeleted (row))
			{
				throw new System.ArgumentException ("Row has been deleted.");
			}
			
			if (row.Table.DataSet != this.queue_data_set)
			{
				throw new System.ArgumentException ("Invalid row specified.");
			}
		}
		
		
		private DbInfrastructure				infrastructure;
		private IDbAbstraction					database;
		private DbTable							queue_db_table;
		private DbRichCommand					queue_command;
		private System.Data.DataSet				queue_data_set;
		private System.Data.DataTable			queue_data_table;
		
		private System.Threading.AutoResetEvent	enqueue_event;
		private bool							is_server;
	}
}
