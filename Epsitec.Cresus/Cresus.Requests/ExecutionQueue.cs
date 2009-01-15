//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Extensions;

using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// The <c>ExecutionQueue</c> class manages the queue of requests waiting for
	/// execution.
	/// </summary>
	public sealed class ExecutionQueue : IPersistable, System.IDisposable
	{
		public ExecutionQueue(DbInfrastructure infrastructure, IDbAbstraction database)
		{
			this.infrastructure      = infrastructure;
			this.database            = (database == null) ? this.infrastructure.DefaultDbAbstraction : database;
			this.enqueueEvent        = new AutoResetEvent (false);
			this.executionStateEvent = new AutoResetEvent (false);
			this.isServer            = this.infrastructure.LocalSettings.IsServer;
			
			int n = (int) ExecutionState.Count;
			this.stateCount = new int[n];
			
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly, this.database))
			{
				DbTable table = this.infrastructure.ResolveDbTable (transaction, Tags.TableRequestQueue);

				this.queueDbTable = table;
				this.LoadFromBase (transaction);
				
				transaction.Commit ();
			}
		}
		
		
		public System.Data.DataRow[]			Rows
		{
			get
			{
				lock (this.exclusion)
				{
					return this.queueDataTable.Rows.ToArray ();
				}
			}
		}
		
		internal AutoResetEvent					EnqueueWaitEvent
		{
			get
			{
				return this.enqueueEvent;
			}
		}
		
		internal AutoResetEvent					ExecutionStateWaitEvent
		{
			get
			{
				return this.executionStateEvent;
			}
		}
		
		internal System.Data.DataTable			QueueDataTable
		{
			get
			{
				return this.queueDataTable;
			}
		}
		
		
		public bool								IsRunningAsServer
		{
			get
			{
				return this.isServer;
			}
		}
		
		
		public bool								HasPending
		{
			get
			{
				lock (this.exclusion)
				{
					return this.stateCount[(int) ExecutionState.Pending] > 0;
				}
			}
		}
		
		public bool								HasConflicting
		{
			get
			{
				lock (this.exclusion)
				{
					return this.stateCount[(int) ExecutionState.Conflicting] > 0;
				}
			}
		}
		
		public bool								HasConflictResolved
		{
			get
			{
				lock (this.exclusion)
				{
					return this.stateCount[(int) ExecutionState.ConflictResolved] > 0;
				}
			}
		}
		
		public bool								HasExecutedByClient
		{
			get
			{
				lock (this.exclusion)
				{
					return this.stateCount[(int) ExecutionState.ExecutedByClient] > 0;
				}
			}
		}
		
		public bool								HasSentToServer
		{
			get
			{
				lock (this.exclusion)
				{
					return this.stateCount[(int) ExecutionState.SentToServer] > 0;
				}
			}
		}
		
		public bool								HasExecutedByServer
		{
			get
			{
				lock (this.exclusion)
				{
					return this.stateCount[(int) ExecutionState.ExecutedByServer] > 0;
				}
			}
		}
		
		public bool								HasConflictingOnServer
		{
			get
			{
				lock (this.exclusion)
				{
					return this.stateCount[(int) ExecutionState.ConflictingOnServer] > 0;
				}
			}
		}


		/// <summary>
		/// Gets the data rows in the queue, sorted by their date and time.
		/// </summary>
		/// <returns>The sorted data rows.</returns>
		public System.Data.DataRow[] GetDateTimeSortedRows()
		{
			lock (this.exclusion)
			{
				var rows = from row in DbRichCommand.GetLiveRows (this.queueDataTable.Rows)
						   let dateX = (System.DateTime) row[Tags.ColumnDateTime]
						   orderby dateX
						   select row;

				return rows.ToArray ();
			}
		}


		/// <summary>
		/// Adds the specified request into the queue.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="request">The request.</param>
		public void Enqueue(Database.DbTransaction transaction, AbstractRequest request)
		{
			if (transaction == null)
			{
				using (transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					this.Enqueue (transaction, request);
					transaction.Commit ();
					return;
				}
			}
			
			System.Data.DataRow row = this.AddRequest (transaction, request);

			lock (this.exclusion)
			{
				this.queueDataTable.Rows.Add (row);
				this.UpdateCounts ();
			}
			
			this.enqueueEvent.Set ();
		}

		/// <summary>
		/// Adds the specified requests into the queue. The requests are provided in a serialized
		/// format.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="serializedRequests">The serialized requests.</param>
		/// <param name="ids">The client ids of the requests.</param>
		public void Enqueue(Database.DbTransaction transaction, byte[][] serializedRequests, DbId[] ids)
		{
			if (transaction == null)
			{
				using (transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
				{
					this.Enqueue (transaction, serializedRequests, ids);
					transaction.Commit ();
					return;
				}
			}
			
			System.Diagnostics.Debug.Assert (serializedRequests.Length == ids.Length);

			lock (this.exclusion)
			{
				for (int i = 0; i < serializedRequests.Length; i++)
				{
					//	Create one row for every serialized request; its timestamp is set using the
					//	local time :
					
					System.Data.DataRow row  = this.AddRequest (transaction, serializedRequests[i]);
					System.Data.DataRow find = DbRichCommand.FindRow (this.queueDataTable, ids[i]);

					//	The caller assigned the IDs for the requests, we have to use them instead
					//	of the IDs provided by the AddRequest method.
					
					DbKey.SetRowId (row, ids[i]);
					
					if ((find != null) &&
						(find.RowState != System.Data.DataRowState.Deleted))
					{
						//	If the table already contains a row with the same ID, this obviously
						//	means that we have a problem, maybe a conflict which might have been
						//	resolved since the last attempt.
						
						ExecutionState oldState = this.GetRequestExecutionState (find);
						
						System.Diagnostics.Debug.WriteLine (string.Format ("Overwrite request {0}, old state was {1}.", ids[i].Value, oldState));
						
						if (oldState == ExecutionState.Conflicting)
						{
							find.BeginEdit ();
							find.ItemArray = row.ItemArray;		//	replace the conflicting request
							find.EndEdit ();
						}
						else
						{
							System.Diagnostics.Debug.WriteLine (string.Format ("Dropped request {0}.", ids[i].Value));
						}
					}
					else
					{
						this.queueDataTable.Rows.Add (row);
					}
				}
				
				this.UpdateCounts ();
			}
			
			System.Diagnostics.Debug.WriteLine ("Enqueued " + serializedRequests.Length + " serialized requests.");
			
			this.enqueueEvent.Set ();
		}


		/// <summary>
		/// Clears the queue.
		/// </summary>
		public void ClearQueue()
		{
			lock (this.exclusion)
			{
				System.Data.DataRow[] rows = this.Rows;
				
				for (int i = 0; i < rows.Length; i++)
				{
					DbRichCommand.KillRow (rows[i]);
				}
				
				this.UpdateCounts ();
			}
			
			this.enqueueEvent.Set ();
		}
		
		
		private System.Data.DataRow AddRequest(Database.DbTransaction transaction, AbstractRequest request)
		{
			return this.AddRequest (transaction, AbstractRequest.SerializeToMemory (request));
		}
		
		private System.Data.DataRow AddRequest(Database.DbTransaction transaction, byte[] data)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (data != null);

			int length = data.Length;
			
			System.Diagnostics.Debug.Assert (length > 0);

			System.Data.DataRow row = DbRichCommand.CreateRow (this.queueDataTable, this.CreateLogId (transaction));
			
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
			lock (this.exclusion)
			{
				Database.DbRichCommand.KillRow (row);
				this.UpdateCounts ();
			}
			this.enqueueEvent.Set ();
		}
		
		public void RemoveRequests(IEnumerable<System.Data.DataRow> rows)
		{
			lock (this.exclusion)
			{
				foreach (var row in rows)
				{
					Database.DbRichCommand.KillRow (row);
				}
				
				this.UpdateCounts ();
			}
			this.enqueueEvent.Set ();
		}
		
		public AbstractRequest GetRequest(System.Data.DataRow row)
		{
			lock (this.exclusion)
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
			lock (this.exclusion)
			{
				this.CheckRow (row);
				return ExecutionQueue.ConvertToExecutionState (row[Tags.ColumnReqExState]);
			}
		}
		
		public void SetRequestExecutionState(System.Data.DataRow row, ExecutionState new_state)
		{
			lock (this.exclusion)
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
				
				this.stateCount[(int) old_state]--;
				this.stateCount[(int) new_state]++;
			}
			
			this.executionStateEvent.Set ();
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		#endregion

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.enqueueEvent.Close ();
				this.executionStateEvent.Close ();
			}
		}

		#region IPersistable Members
		
		public void LoadFromBase(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (this.infrastructure != null);

			lock (this.exclusion)
			{
				this.queueCommand   = DbRichCommand.CreateFromTable (this.infrastructure, transaction, this.queueDbTable, DbSelectRevision.LiveActive);
				this.queueDataSet   = this.queueCommand.DataSet;
				this.queueDataTable = this.queueDataSet.Tables[Tags.TableRequestQueue];
				
				this.UpdateCounts ();
			}
		}
		
		public void PersistToBase(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (this.queueCommand != null);

			lock (this.exclusion)
			{
				this.queueCommand.AssignRealRowIds (transaction);
				this.queueCommand.UpdateTables (transaction);
				this.queueCommand.AcceptChanges (transaction);
			}
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
		
		
		DbId CreateLogId(Database.DbTransaction transaction)
		{
			if (this.isServer)
			{
				return this.infrastructure.Logger.CreatePermanentEntry (transaction);
			}
			else
			{
				return this.infrastructure.Logger.CreateTemporaryEntry (transaction);
			}
		}
		
		void CheckRow(System.Data.DataRow row)
		{
			if (row == null)
			{
				throw new System.ArgumentNullException ("Row is null.");
			}
			if (DbRichCommand.IsRowDeleted (row))
			{
				throw new System.ArgumentException ("Row has been deleted.");
			}
			
			if (row.Table.DataSet != this.queueDataSet)
			{
				throw new System.ArgumentException ("Invalid row specified.");
			}
		}
		
		void UpdateCounts()
		{
			int   n     = (int) ExecutionState.Count;
			int[] count = new int[n];
			
			foreach (System.Data.DataRow row in this.queueDataTable.Rows)
			{
				if (DbRichCommand.IsRowDeleted (row) == false)
				{
					short state = (short) ExecutionQueue.ConvertToExecutionState (row[Tags.ColumnReqExState]);
					count[state]++;
				}
			}
			
			this.stateCount = count;
		}


		readonly object							exclusion = new object ();
		
		readonly DbInfrastructure				infrastructure;
		readonly IDbAbstraction					database;
		readonly DbTable						queueDbTable;
		private DbRichCommand					queueCommand;
		private System.Data.DataSet				queueDataSet;
		private System.Data.DataTable			queueDataTable;

		readonly AutoResetEvent					enqueueEvent;
		readonly AutoResetEvent					executionStateEvent;
		readonly bool							isServer;
		private int[]							stateCount;
	}
}
