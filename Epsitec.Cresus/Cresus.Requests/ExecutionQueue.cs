//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// La classe ExecutionQueue permet de repr�senter la queue des requ�tes
	/// en attente d'ex�cution.
	/// </summary>
	public class ExecutionQueue : IAttachable, IPersistable
	{
		public ExecutionQueue(DbInfrastructure infrastructure, IDbAbstraction database)
		{
			this.infrastructure = infrastructure;
			this.database       = (database == null) ? this.infrastructure.DefaultDbAbstraction : database;
			this.enqueue_event  = new System.Threading.AutoResetEvent (false);
			this.exstate_event  = new System.Threading.AutoResetEvent (false);
			this.is_server      = this.infrastructure.LocalSettings.IsServer;
			
			int n = (int) ExecutionState.Count;
			this.state_count = new int[n];
			
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
		
		public System.Data.DataRow[]			DateTimeSortedRows
		{
			get
			{
				System.Data.DataRow[] rows = this.Rows;
				System.Array.Sort (rows, new DateTimeRowComparer ());
				return rows;
			}
		}
		
		
		public System.Threading.AutoResetEvent	EnqueueWaitEvent
		{
			get
			{
				return this.enqueue_event;
			}
		}
		
		public System.Threading.AutoResetEvent	ExecutionStateWaitEvent
		{
			get
			{
				return this.exstate_event;
			}
		}
		
		
		public bool								IsRunningAsServer
		{
			get
			{
				return this.is_server;
			}
		}
		
		
		public bool								HasPending
		{
			get
			{
				lock (this)
				{
					return this.state_count[(int) ExecutionState.Pending] > 0;
				}
			}
		}
		
		public bool								HasConflicting
		{
			get
			{
				lock (this)
				{
					return this.state_count[(int) ExecutionState.Conflicting] > 0;
				}
			}
		}
		
		public bool								HasConflictResolved
		{
			get
			{
				lock (this)
				{
					return this.state_count[(int) ExecutionState.ConflictResolved] > 0;
				}
			}
		}
		
		public bool								HasExecutedByClient
		{
			get
			{
				lock (this)
				{
					return this.state_count[(int) ExecutionState.ExecutedByClient] > 0;
				}
			}
		}
		
		public bool								HasSentToServer
		{
			get
			{
				lock (this)
				{
					return this.state_count[(int) ExecutionState.SentToServer] > 0;
				}
			}
		}
		
		public bool								HasExecutedByServer
		{
			get
			{
				lock (this)
				{
					return this.state_count[(int) ExecutionState.ExecutedByServer] > 0;
				}
			}
		}
		
		
		public void Enqueue(AbstractRequest request)
		{
			System.Data.DataRow row = this.AddRequest (request);
			
			lock (this.queue_data_table)
			{
				this.queue_data_table.Rows.Add (row);
			}
			
			this.UpdateCounts ();
			this.enqueue_event.Set ();
		}
		
		public void Enqueue(byte[][] serialized_requests, DbId[] ids)
		{
			//	Ajoute une s�rie de requ�tes qui sont d�j� sous forme s�rialis�e et leur
			//	attribue un ID particulier (cet ID d�pend du client et doit �tre associ�
			//	� la requ�te pour permettre de dialoguer correctement avec le client).
			
			System.Diagnostics.Debug.Assert (serialized_requests.Length == ids.Length);
			
			lock (this.queue_data_table)
			{
				for (int i = 0; i < serialized_requests.Length; i++)
				{
					System.Data.DataRow row  = this.AddRequest (serialized_requests[i]);
					System.Data.DataRow find = DbRichCommand.FindRow (this.queue_data_table, ids[i]);
					
					//	L'appelant force des IDs pour les diverses requ�tes (parce qu'elles proviennent
					//	d'un client distant qui a d�j� attribu� les IDs).
					
					row[Tags.ColumnId] = ids[i].Value;
					
					if ((find != null) &&
						(find.RowState != System.Data.DataRowState.Deleted))
					{
						//	Si la table contient d�j� une requ�te avec cet ID, on va simplement la
						//	remplacer par la nouvelle requ�te :
						
						find.BeginEdit ();
						find.ItemArray = row.ItemArray;
						find.EndEdit ();
						
						System.Diagnostics.Debug.WriteLine ("Server: Replaced request " + ids[i].Value);
					}
					else
					{
						this.queue_data_table.Rows.Add (row);
						
						System.Diagnostics.Debug.WriteLine ("Server: Inserted request " + ids[i].Value);
					}
				}
			}
			
			System.Diagnostics.Debug.WriteLine ("Enqueued " + serialized_requests.Length + " serialized requests.");
			
			this.UpdateCounts ();
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
			row[Tags.ColumnDateTime]   = System.DateTime.UtcNow;
			row.EndEdit ();
			
			return row;
		}
		
		public void RemoveRequest(System.Data.DataRow row)
		{
			Database.DbRichCommand.KillRow (row);
			this.UpdateCounts ();
			this.enqueue_event.Set ();
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
			ExecutionState old_state = this.GetRequestExecutionState (row);
			
			if (old_state == new_state)
			{
				return;
			}
			
			if (StateMachine.Check (old_state, new_state) == false)
			{
				throw new System.InvalidOperationException (string.Format ("Cannot change from state {0} to {1}.", old_state, new_state));
			}
			
			row[Tags.ColumnReqExState] = (short) new_state;
			
			lock (this)
			{
				this.state_count[(int) old_state]--;
				this.state_count[(int) new_state]++;
			}
			
			this.exstate_event.Set ();
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
			
			this.UpdateCounts ();
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
		
		#region DateTimeRowComparer Class
		public class DateTimeRowComparer : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				System.Data.DataRow row_x = x as System.Data.DataRow;
				System.Data.DataRow row_y = y as System.Data.DataRow;
				
				System.DateTime date_x = (System.DateTime) row_x[Tags.ColumnDateTime];
				System.DateTime date_y = (System.DateTime) row_y[Tags.ColumnDateTime];
				
				return date_x.CompareTo (date_y);
			}
			#endregion
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
		
		protected void UpdateCounts()
		{
			int   n     = (int) ExecutionState.Count;
			int[] count = new int[n];
			
			foreach (System.Data.DataRow row in this.queue_data_table.Rows)
			{
				if (DbRichCommand.IsRowDeleted (row) == false)
				{
					short state = (short) row[Tags.ColumnReqExState];
					count[state]++;
				}
			}
			
			lock (this)
			{
				this.state_count = count;
			}
		}
		
		
		private DbInfrastructure				infrastructure;
		private IDbAbstraction					database;
		private DbTable							queue_db_table;
		private DbRichCommand					queue_command;
		private System.Data.DataSet				queue_data_set;
		private System.Data.DataTable			queue_data_table;
		
		private System.Threading.AutoResetEvent	enqueue_event;
		private System.Threading.AutoResetEvent	exstate_event;
		private bool							is_server;
		private int[]							state_count;
	}
}
