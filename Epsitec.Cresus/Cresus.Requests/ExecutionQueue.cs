//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			this.exstate_event  = new System.Threading.AutoResetEvent (false);
			this.is_server      = this.infrastructure.LocalSettings.IsServer;
			
			int n = (int) ExecutionState.Count;
			this.state_count = new int[n];
			
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly, this.database))
			{
				DbTable table = this.infrastructure.ResolveDbTable (transaction, Tags.TableRequestQueue);
				
				this.Attach (infrastructure, table);
				this.LoadFromBase (transaction);
				
				transaction.Commit ();
			}

		}
		
		
		public System.Data.DataRow[]			Rows
		{
			get
			{
				lock (this)
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
				lock (this)
				{
					System.Data.DataRow[] rows = DbRichCommand.GetLiveRows (this.Rows);
					System.Array.Sort (rows, new DateTimeRowComparer ());
					return rows;
				}
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
		
		public bool								HasConflictingOnServer
		{
			get
			{
				lock (this)
				{
					return this.state_count[(int) ExecutionState.ConflictingOnServer] > 0;
				}
			}
		}
		
		
		public void Enqueue(Database.DbTransaction transaction, AbstractRequest request)
		{
			//	Ajoute une requête dans la queue.
			
			if (transaction == null)
			{
				try
				{
					transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite);
					this.Enqueue (transaction, request);
				}
				finally
				{
					transaction.Commit ();
					transaction.Dispose ();
				}
			}
			else
			{
				System.Data.DataRow row = this.AddRequest (transaction, request);
				
				lock (this)
				{
					this.queue_data_table.Rows.Add (row);
					this.UpdateCounts ();
				}
				
				this.enqueue_event.Set ();
			}
		}
		
		public void Enqueue(Database.DbTransaction transaction, byte[][] serialized_requests, DbId[] ids)
		{
			//	Ajoute une série de requêtes qui sont déjà sous forme sérialisée,
			//	en leur attribuant un ID particulier (cet ID dépend du client et
			//	doit être associé à la requête pour permettre un dialogue correct
			//	avec le client).
			
			System.Diagnostics.Debug.Assert (serialized_requests.Length == ids.Length);
			
			lock (this)
			{
				for (int i = 0; i < serialized_requests.Length; i++)
				{
					//	Crée la ligne décrivant la requête sérialisée. La date
					//	courante (locale au processus) est affectée à la ligne :
					
					System.Data.DataRow row  = this.AddRequest (transaction, serialized_requests[i]);
					System.Data.DataRow find = DbRichCommand.FindRow (this.queue_data_table, ids[i]);
					
					//	L'appelant force des IDs pour les diverses requêtes (parce
					//	qu'elles proviennent d'un client distant qui a déjà attribué
					//	des IDs aux requêtes).
					
					row[Tags.ColumnId] = ids[i].Value;
					
					if ((find != null) &&
						(find.RowState != System.Data.DataRowState.Deleted))
					{
						//	La table contient déjà une requête avec cet ID, ce qui est
						//	une indication qu'il y a eu un problème, manifestement.
						
						ExecutionState old_state = this.GetRequestExecutionState (find);
						
						System.Diagnostics.Debug.WriteLine (string.Format ("Overwrite request {0}, old state was {1}.", ids[i].Value, old_state));
						
						if (old_state == ExecutionState.Conflicting)
						{
							//	L'ancienne requête était en conflit. Elle est remplacée
							//	par une requête fraîche -- le conflit va disparaître :
							
							find.BeginEdit ();
							find.ItemArray = row.ItemArray;
							find.EndEdit ();
						}
						else
						{
							System.Diagnostics.Debug.WriteLine (string.Format ("Dropped request {0}.", ids[i].Value));
						}
					}
					else
					{
						this.queue_data_table.Rows.Add (row);
					}
				}
				
				this.UpdateCounts ();
			}
			
			System.Diagnostics.Debug.WriteLine ("Enqueued " + serialized_requests.Length + " serialized requests.");
			
			this.enqueue_event.Set ();
		}
		
		
		public void ClearQueue()
		{
			lock (this)
			{
				System.Data.DataRow[] rows = this.Rows;
				
				for (int i = 0; i < rows.Length; i++)
				{
					DbRichCommand.KillRow (rows[i]);
				}
				
				this.UpdateCounts ();
			}
			
			this.enqueue_event.Set ();
		}
		
		
		public System.Data.DataRow AddRequest(Database.DbTransaction transaction, AbstractRequest request)
		{
			return this.AddRequest (transaction, AbstractRequest.SerializeToMemory (request));
		}
		
		public System.Data.DataRow AddRequest(Database.DbTransaction transaction, byte[] data)
		{
			int length = data.Length;
			
			System.Diagnostics.Debug.Assert (length > 0);

			System.Data.DataRow row = DbRichCommand.CreateRow (this.queue_data_table, this.CreateLogId (transaction));
			
			row.BeginEdit ();
			row[Tags.ColumnReqData]    = data;
			row[Tags.ColumnReqExState] = ExecutionQueue.ConvertFromExecutionState (ExecutionState.Pending);
			row[Tags.ColumnDateTime]   = System.DateTime.UtcNow;
			row.EndEdit ();
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Request {0} added, initial state {1}.", row[0], (ExecutionState)row[Tags.ColumnReqExState]));
			
			return row;
		}
		
		public void RemoveRequest(System.Data.DataRow row)
		{
			lock (this)
			{
				Database.DbRichCommand.KillRow (row);
				this.UpdateCounts ();
			}
			this.enqueue_event.Set ();
		}
		
		public void RemoveRequests(System.Collections.IEnumerable rows)
		{
			lock (this)
			{
				foreach (System.Data.DataRow row in rows)
				{
					Database.DbRichCommand.KillRow (row);
				}
				
				this.UpdateCounts ();
			}
			this.enqueue_event.Set ();
		}
		
		public AbstractRequest GetRequest(System.Data.DataRow row)
		{
			lock (this)
			{
				if ((row != null) &&
					(DbRichCommand.IsRowDeleted (row) == false))
				{
					byte[] buffer = row[Tags.ColumnReqData] as byte[];
					return AbstractRequest.DeserializeFromMemory (buffer);
				}
			}
			
			return null;
		}
		
		public ExecutionState GetRequestExecutionState(System.Data.DataRow row)
		{
			lock (this)
			{
				this.CheckRow (row);
				return ExecutionQueue.ConvertToExecutionState (row[Tags.ColumnReqExState]);
			}
		}
		
		public void SetRequestExecutionState(System.Data.DataRow row, ExecutionState new_state)
		{
			lock (this)
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
				
				System.Diagnostics.Debug.WriteLine (string.Format ("Request {0} changed from {1} to {2}.", row[0], old_state, new_state));
				
				row[Tags.ColumnReqExState] = ExecutionQueue.ConvertFromExecutionState (new_state);
				
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
		
		public void LoadFromBase(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (this.infrastructure != null);
			
			lock (this)
			{
				this.queue_command    = DbRichCommand.CreateFromTable (this.infrastructure, transaction, this.queue_db_table, DbSelectRevision.LiveActive);
				this.queue_data_set   = this.queue_command.DataSet;
				this.queue_data_table = this.queue_data_set.Tables[Tags.TableRequestQueue];
				
				this.UpdateCounts ();
			}
		}
		
		public void PersistToBase(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (this.queue_command != null);
			
			lock (this)
			{
				this.queue_command.AssignRealRowIds (transaction);
				this.queue_command.UpdateTables (transaction);
				this.queue_command.AcceptChanges (transaction);
			}
		}
		
		#endregion
		
		#region DateTimeRowComparer Class
		protected class DateTimeRowComparer : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				System.Data.DataRow row_x = x as System.Data.DataRow;
				System.Data.DataRow row_y = y as System.Data.DataRow;
				
				System.Diagnostics.Debug.Assert (row_x.RowState != System.Data.DataRowState.Deleted);
				System.Diagnostics.Debug.Assert (row_y.RowState != System.Data.DataRowState.Deleted);
				
				System.DateTime date_x = (System.DateTime) row_x[Tags.ColumnDateTime];
				System.DateTime date_y = (System.DateTime) row_y[Tags.ColumnDateTime];
				
				return date_x.CompareTo (date_y);
			}
			#endregion
		}
		#endregion
		
		internal static ExecutionState ConvertToExecutionState(short value)
		{
			return (ExecutionState) value;
		}
		
		internal static ExecutionState ConvertToExecutionState(object value)
		{
			System.Enum state;
				
			if (Common.Types.InvariantConverter.Convert (value, typeof (ExecutionState), out state))
			{
				return (ExecutionState) state;
			}
			
			throw new System.InvalidCastException ("Invalid ExecutionState value.");
		}
		
		internal static object ConvertFromExecutionState(ExecutionState value)
		{
			return (short) value;
		}
		
		
		protected DbId CreateLogId(Database.DbTransaction transaction)
		{
			if (this.is_server)
			{
				return this.infrastructure.Logger.CreatePermanentEntry (transaction);
			}
			else
			{
				return this.infrastructure.Logger.CreateTemporaryEntry (transaction);
			}
		}
		
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
					short state = (short) ExecutionQueue.ConvertToExecutionState (row[Tags.ColumnReqExState]);
					count[state]++;
				}
			}
			
			this.state_count = count;
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
