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
		/// <summary>
		/// Initializes a new instance of the <see cref="ExecutionQueue"/> class.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="database">The database.</param>
		public ExecutionQueue(DbInfrastructure infrastructure, IDbAbstraction database)
		{
			this.infrastructure      = infrastructure;
			this.database            = database ?? this.infrastructure.DefaultDbAbstraction;
			this.queueChangedEvent   = new AutoResetEvent (false);
			this.stateChangedEvent   = new AutoResetEvent (false);
			this.isServer            = this.infrastructure.LocalSettings.IsServer;
			
			int n = (int) ExecutionState.Count;
			this.stateCountCache = new int[n];
			
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly, this.database))
			{
				DbTable table = this.infrastructure.ResolveDbTable (transaction, Tags.TableRequestQueue);

				this.queueDbTable = table;
				this.LoadFromBase (transaction);
				
				transaction.Commit ();
			}
		}


		/// <summary>
		/// Gets the queue changed wait event. This must be used by a single thread,
		/// or else, events will be lost.
		/// </summary>
		/// <value>The queue changed wait event.</value>
		internal AutoResetEvent					QueueChangedWaitEvent
		{
			get
			{
				return this.queueChangedEvent;
			}
		}

		/// <summary>
		/// Gets the execution state wait event. This must be used by a single thread,
		/// or else, events will be lost.
		/// </summary>
		/// <value>The execution state wait event.</value>
		internal AutoResetEvent					ExecutionStateWaitEvent
		{
			get
			{
				return this.stateChangedEvent;
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

		public bool								IsRunningAsClient
		{
			get
			{
				return !this.isServer;
			}
		}
		
		
		public bool								HasPending
		{
			get
			{
				lock (this.exclusion)
				{
					return this.stateCountCache[(int) ExecutionState.Pending] > 0;
				}
			}
		}
		
		public bool								HasConflicting
		{
			get
			{
				lock (this.exclusion)
				{
					return this.stateCountCache[(int) ExecutionState.Conflicting] > 0;
				}
			}
		}
		
		public bool								HasConflictResolved
		{
			get
			{
				lock (this.exclusion)
				{
					return this.stateCountCache[(int) ExecutionState.ConflictResolved] > 0;
				}
			}
		}
		
		public bool								HasExecutedByClient
		{
			get
			{
				lock (this.exclusion)
				{
					return this.stateCountCache[(int) ExecutionState.ExecutedByClient] > 0;
				}
			}
		}
		
		public bool								HasSentToServer
		{
			get
			{
				lock (this.exclusion)
				{
					return this.stateCountCache[(int) ExecutionState.SentToServer] > 0;
				}
			}
		}
		
		public bool								HasExecutedByServer
		{
			get
			{
				lock (this.exclusion)
				{
					return this.stateCountCache[(int) ExecutionState.ExecutedByServer] > 0;
				}
			}
		}
		
		public bool								HasConflictingOnServer
		{
			get
			{
				lock (this.exclusion)
				{
					return this.stateCountCache[(int) ExecutionState.ConflictingOnServer] > 0;
				}
			}
		}

		public int								QueueChangeCounter
		{
			get
			{
				lock (this.queueChangeMonitor)
				{
					return this.queueChangeCounter;
				}
			}
		}


		/// <summary>
		/// Gets the rows currently defined in the database, which hold the requests
		/// of the execution queue.
		/// </summary>
		/// <value>The rows.</value>
		public System.Data.DataRow[] GetRows()
		{
			lock (this.exclusion)
			{
				return this.queueDataTable.Rows.ToArray ();
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
		/// Allows the caller to execute an atomic check on the queue.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns>The value returned by the predicate.</returns>
		public bool AtomicCheck(System.Predicate<ExecutionQueue> predicate)
		{
			lock (this.queueChangeMonitor)
			{
				return predicate (this);
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

			DbId logId = this.CreateLogId (transaction);
			System.Data.DataRow row = this.CreateRequestRow (transaction, request, logId);

			lock (this.exclusion)
			{
				this.queueDataTable.Rows.Add (row);
				this.UpdateCounts ();
			}

			this.SignalQueueChanged ();
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
				DbId logId = this.CreateLogId (transaction);

				for (int i = 0; i < serializedRequests.Length; i++)
				{
					//	Create one row for every serialized request; its timestamp is set using the
					//	local time :
					
					System.Data.DataRow row  = this.CreateRequestRow (transaction, serializedRequests[i], logId);
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

			this.SignalQueueChanged ();
		}


		/// <summary>
		/// Clears the queue.
		/// </summary>
		public void ClearQueue()
		{
			lock (this.exclusion)
			{
				System.Data.DataRow[] rows = this.GetRows ();
				
				for (int i = 0; i < rows.Length; i++)
				{
					DbRichCommand.KillRow (rows[i]);
				}
				
				this.UpdateCounts ();
			}

			this.SignalQueueChanged ();
		}


		/// <summary>
		/// Waits for the queue to change.
		/// </summary>
		/// <param name="waitPredicate">The wait predicate which must return <c>true</c> to block on the internal monitor.</param>
		/// <param name="timeout">The timeout.</param>
		/// <returns><c>true</c> if the wait succeeded or if the predicate returned <c>false</c>.</returns>
		public bool WaitForQueueChange(System.Predicate<ExecutionQueue> waitPredicate)
		{
			return this.WaitForQueueChange (waitPredicate, System.TimeSpan.FromMilliseconds (-1));
		}

		/// <summary>
		/// Waits for the queue to change.
		/// </summary>
		/// <param name="waitPredicate">The wait predicate which must return <c>true</c> to block on the internal monitor.</param>
		/// <param name="timeout">The timeout.</param>
		/// <returns><c>true</c> if the wait succeeded or if the predicate returned <c>false</c>.</returns>
		public bool WaitForQueueChange(System.Predicate<ExecutionQueue> waitPredicate, System.TimeSpan timeout)
		{
			lock (this.queueChangeMonitor)
			{
				if (waitPredicate (this))
				{
					return Monitor.Wait (this.queueChangeMonitor, timeout);
				}
				else
				{
					return true;
				}
			}
		}


		/// <summary>
		/// Removes the request row.
		/// </summary>
		/// <param name="row">The row.</param>
		public void RemoveRequestRow(System.Data.DataRow row)
		{
			lock (this.exclusion)
			{
				Database.DbRichCommand.KillRow (row);
				this.UpdateCounts ();
			}

			this.SignalQueueChanged ();
		}

		/// <summary>
		/// Removes the request rows.
		/// </summary>
		/// <param name="rows">The rows.</param>
		public void RemoveRequestRows(IEnumerable<System.Data.DataRow> rows)
		{
			lock (this.exclusion)
			{
				foreach (var row in rows)
				{
					Database.DbRichCommand.KillRow (row);
				}
				
				this.UpdateCounts ();
			}

			this.SignalQueueChanged ();
		}

		/// <summary>
		/// Gets the request from a row. The request gets deserialized from the row
		/// data.
		/// </summary>
		/// <param name="row">The row.</param>
		/// <returns>The request or <c>null</c>.</returns>
		public AbstractRequest GetRequest(System.Data.DataRow row)
		{
			byte[] buffer = null;

			lock (this.exclusion)
			{
				if ((row != null) &&
					(DbRichCommand.IsRowDeleted (row) == false))
				{
					buffer = row[Tags.ColumnReqData] as byte[];
				}
			}

			return Epsitec.Common.IO.Serialization.DeserializeFromMemory<AbstractRequest> (buffer);
		}

		/// <summary>
		/// Gets the execution state of the request, based on a row.
		/// </summary>
		/// <param name="row">The row.</param>
		/// <returns>The execution state.</returns>
		public ExecutionState GetRequestExecutionState(System.Data.DataRow row)
		{
			lock (this.exclusion)
			{
				this.CheckRow (row);
				return ExecutionQueue.ConvertToExecutionState (row[Tags.ColumnReqExState]);
			}
		}

		/// <summary>
		/// Sets the execution state of the request, based on a row. This method
		/// may only be called by the <see cref="Orchestrator"/> worker thread.
		/// </summary>
		/// <param name="row">The row for the request.</param>
		/// <param name="newState">The new state.</param>
		internal void SetRequestExecutionState(System.Data.DataRow row, ExecutionState newState)
		{
			System.Diagnostics.Debug.Assert (this.orchestratorWorkerThread == Thread.CurrentThread);

			lock (this.exclusion)
			{
				ExecutionState oldState = this.GetRequestExecutionState (row);
				
				if (oldState == newState)
				{
					return;
				}
				
				if (StateMachine.Check (oldState, newState) == false)
				{
					throw new System.InvalidOperationException (string.Format ("Cannot change from state {0} to {1}.", oldState, newState));
				}
				
				System.Diagnostics.Debug.WriteLine (string.Format ("Request {0} changed from {1} to {2}.", row[0], oldState, newState));
				
				row[Tags.ColumnReqExState] = ExecutionQueue.ConvertFromExecutionState (newState);
				
				this.stateCountCache[(int) oldState]--;
				this.stateCountCache[(int) newState]++;
			}

			this.SignalStateChanged ();
		}

		/// <summary>
		/// Sets the orchestrator worker thread; this is used to make sure that the
		/// <see cref="SetRequestExecutionState"/> method only gets called by the
		/// correct thread.
		/// </summary>
		/// <param name="thread">The thread.</param>
		internal void SetOrchestratorWorkerThread(Thread thread)
		{
			this.orchestratorWorkerThread = thread;
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		#endregion

		private void SignalQueueChanged()
		{
			this.queueChangedEvent.Set ();

			lock (this.queueChangeMonitor)
			{
				this.queueChangeCounter++;
				Monitor.PulseAll (this.queueChangeMonitor);
			}
		}

		private void SignalStateChanged()
		{
			this.stateChangedEvent.Set ();

			lock (this.queueChangeMonitor)
			{
				this.queueChangeCounter++;
				Monitor.PulseAll (this.queueChangeMonitor);
			}
		}


		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.queueChangedEvent.Close ();
				this.stateChangedEvent.Close ();
			}
		}

		#region IPersistable Members

		/// <summary>
		/// Loads the instance data from the database.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
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

		/// <summary>
		/// Persists the instance data to the database.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
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
		
		
		internal static ExecutionState ConvertToExecutionState(int value)
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


		/// <summary>
		/// Creates the request row.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="request">The request.</param>
		/// <returns>The new data row.</returns>
		System.Data.DataRow CreateRequestRow(Database.DbTransaction transaction, AbstractRequest request, DbId logId)
		{
			return this.CreateRequestRow (transaction, Epsitec.Common.IO.Serialization.SerializeToMemory (request), logId);
		}

		/// <summary>
		/// Creates the request row.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="data">The serialized data of the request.</param>
		/// <returns>The new data row.</returns>
		System.Data.DataRow CreateRequestRow(Database.DbTransaction transaction, byte[] data, DbId logId)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (data != null);

			int length = data.Length;

			System.Diagnostics.Debug.Assert (length > 0);

			System.Data.DataRow row = DbRichCommand.CreateRow (this.queueDataTable, logId);

			row.BeginEdit ();
			row[Tags.ColumnReqData]    = data;
			row[Tags.ColumnReqExState] = ExecutionQueue.ConvertFromExecutionState (ExecutionState.Pending);
			row[Tags.ColumnDateTime]   = System.DateTime.UtcNow;
			row.EndEdit ();

			System.Diagnostics.Debug.WriteLine (string.Format ("Request {0} added, initial state {1}.", row[0], ExecutionQueue.ConvertToExecutionState (row[Tags.ColumnReqExState])));

			return row;
		}


		/// <summary>
		/// Creates a new log id. If we are running on the server, create a permanent
		/// entry, otherwise create only a temporary entry.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <returns>The new log id.</returns>
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

		/// <summary>
		/// Checks that the row is valid.
		/// </summary>
		/// <param name="row">The row.</param>
		void CheckRow(System.Data.DataRow row)
		{
			if (row == null)
			{
				throw new System.ArgumentNullException ("row", "Row is null.");
			}
			if (DbRichCommand.IsRowDeleted (row))
			{
				throw new System.ArgumentException ("Row has been deleted.", "row");
			}
			
			if (row.Table.DataSet != this.queueDataSet)
			{
				throw new System.ArgumentException ("Invalid row specified.", "row");
			}
		}

		/// <summary>
		/// Updates the counts for each state.
		/// </summary>
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

			if (Epsitec.Common.Types.Comparer.EqualValues (this.stateCountCache, count))
			{
				return;
			}
			
			this.stateCountCache = count;
			this.SignalStateChanged ();
		}


		readonly object							exclusion = new object ();
		readonly object							queueChangeMonitor = new object ();
		
		readonly DbInfrastructure				infrastructure;
		readonly IDbAbstraction					database;
		readonly DbTable						queueDbTable;

		readonly AutoResetEvent					queueChangedEvent;
		readonly AutoResetEvent					stateChangedEvent;
		readonly bool							isServer;
		
		private DbRichCommand					queueCommand;
		private System.Data.DataSet				queueDataSet;
		private System.Data.DataTable			queueDataTable;
		private int								queueChangeCounter;

		private Thread							orchestratorWorkerThread;
		
		private int[]							stateCountCache;
	}
}
