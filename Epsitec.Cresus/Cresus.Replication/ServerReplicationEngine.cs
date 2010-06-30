//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Data;


namespace Epsitec.Cresus.Replication
{
	/// <summary>
	/// The <c>ServerReplicationEngine</c> class implements the server side replication
	/// engine.
	/// </summary>
	public sealed class ServerReplicationEngine : System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ServerEngine"/> class.
		/// </summary>
		/// <param name="infrastructure">The database infrastructure.</param>
		public ServerReplicationEngine(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
			
			this.database                      = this.infrastructure.CreateDatabaseAbstraction ();
			this.database.SqlBuilder.AutoClear = true;

			this.queue         = new Queue<ReplicationJob> ();
			this.queueEvent    = new AutoResetEvent (false);
			this.shutdownEvent = new ManualResetEvent (false);
			
			this.workerThread  = new Thread (this.WorkerThread);
			this.workerThread.Name = "Replication Engine";
			this.workerThread.Start ();
		}


		/// <summary>
		/// Gets the database infrastructure.
		/// </summary>
		/// <value>The database infrastructure.</value>
		public DbInfrastructure					Infrastructure
		{
			get
			{
				return this.infrastructure;
			}
		}
		
		/// <summary>
		/// Enqueues a new replication job.
		/// </summary>
		/// <param name="job">The replication job.</param>
		public void Enqueue(ReplicationJob job)
		{
			lock (this.queue)
			{
				this.queue.Enqueue (job);
			}

			//	Wake up the worker thread
			
			this.queueEvent.Set ();
		}
		
		
		#region IDisposable Members
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		
		#endregion


		/// <summary>
		/// The worker thread processes the replication jobs which are present in
		/// the queue.
		/// </summary>
		void WorkerThread()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine ("Replication.ServerEngine Worker Thread launched.");

				while (! this.isShutdownPending)
				{
					int handleIndex = Common.Support.Sync.Wait (this.queueEvent, this.shutdownEvent);
					
					switch (handleIndex)
					{
						case 0:
							this.ProcessQueue ();
							break;

						default:
							//	The shutdown event was signalled; exit the thread after shutting down
							//	gracefully.

							System.Diagnostics.Debug.Assert (this.isShutdownPending);

							this.ProcessShutdown ();
							break;
					}
				}
			}
			catch (System.Exception exception)
			{
				System.Diagnostics.Debug.WriteLine (exception.Message);
				System.Diagnostics.Debug.WriteLine (exception.StackTrace);
			}
			finally
			{
				System.Diagnostics.Debug.WriteLine ("Replication.ServerEngine Worker Thread terminated.");
			}
		}


		/// <summary>
		/// Processes the jobs in the queue, until the queue is empty.
		/// </summary>
		void ProcessQueue()
		{
			while (! this.isShutdownPending)
			{
				ReplicationJob job = null;
				
				lock (this.queue)
				{
					if (this.queue.Count > 0)
					{
						job = this.queue.Dequeue ();
					}
				}
				
				if (job == null)
				{
					break;
				}
				
				if (job.ProgressState == Remoting.ProgressState.Canceled)
				{
					//	Skip canceled jobs.

					continue;
				}
				
				try
				{
					job.NotifyStartProcessing ();
					
					this.ProcessReplicationJob (job);
				}
				catch (System.Exception ex)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Replication: ServerEngine.ProcessQueue failed job for client {0}; {1}", job.Client.ToString (), ex.Message));
					
					job.NotifyError (ex.ToString ());
				}
				finally
				{
					System.Diagnostics.Debug.Assert (job.ProgressState != Remoting.ProgressState.Running);
				}
			}
		}
		
		/// <summary>
		/// Cleanup before the thread shuts down.
		/// </summary>
		void ProcessShutdown()
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("Processing shutdown."));
		}

		/// <summary>
		/// Processes the replication job.
		/// </summary>
		/// <param name="job">The replication job.</param>
		void ProcessReplicationJob(ReplicationJob job)
		{
			DbId syncStart = job.SyncStartId;
			DbId syncEnd   = job.SyncEndId;

			PullArguments pull = job.PullArguments;
			
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly, this.database))
			{
				DataExtractor extractor = new DataExtractor (this.infrastructure, transaction);
				List<DbTable> tables = new List<DbTable> ();
				
				tables.AddRange (this.infrastructure.FindDbTables (transaction, DbElementCat.Any));
				
				System.Diagnostics.Debug.WriteLine (string.Format ("Found {0} tables. Extracting from {1} to {2}.", tables.Count, syncStart, syncEnd));

				DbTable defTableTable  = tables.FirstOrDefault (t => t.Name == Tags.TableTableDef);
				DbTable defColumnTable = tables.FirstOrDefault (t => t.Name == Tags.TableColumnDef);
				DbTable defTypeTable   = tables.FirstOrDefault (t => t.Name == Tags.TableTypeDef);
				DbTable logTable       = tables.FirstOrDefault (t => t.Name == Tags.TableLog);
				
				//	We don't replicate these tables through the standard replication mechanism;
				//	remove them from the list of tables :
				
				tables.Remove (defTableTable);
				tables.Remove (defColumnTable);
				tables.Remove (defTypeTable);
				tables.Remove (logTable);
				
				ReplicationData buffer = new ReplicationData ();

				//	Process the special schema information tables first, before we process the real
				//	data tables :
				
				ServerReplicationEngine.ReplicateTable (extractor, pull, syncStart, syncEnd, defTableTable, buffer);
				ServerReplicationEngine.ReplicateTable (extractor, pull, syncStart, syncEnd, defColumnTable, buffer);
				ServerReplicationEngine.ReplicateTable (extractor, pull, syncStart, syncEnd, defTypeTable, buffer);

				ServerReplicationEngine.ReplicateLogTable (extractor, syncStart, syncEnd, logTable, buffer);
				
				ServerReplicationEngine.RemoveAllMatchingTables (tables, DbReplicationMode.None);
				
				System.Diagnostics.Debug.WriteLine (string.Format ("Scrubbed, remaining {0} tables.", tables.Count));
				
				foreach  (DbTable table in tables)
				{
					System.Diagnostics.Debug.Assert (table.ReplicationMode == DbReplicationMode.Automatic);

					ServerReplicationEngine.ReplicateTable (extractor, pull, syncStart, syncEnd, table, buffer);
				}
				
				//	Serialize and manually compress the delta produced by the calls to
				//	method ProcessTable; we want to minimize the amount of data which
				//	goes down the wire :

				byte[] compressedData = Common.IO.Serialization.SerializeAndCompressToMemory (buffer, Common.IO.Compressor.DeflateCompact);

				job.NotifyDoneProcessing (compressedData);
				
				transaction.Commit ();
			}
		}

		/// <summary>
		/// Replicates the specified table.
		/// </summary>
		/// <param name="extractor">The data extractor.</param>
		/// <param name="pull">The pull arguments.</param>
		/// <param name="syncStart">The sync start id.</param>
		/// <param name="syncEnd">The sync end id.</param>
		/// <param name="table">The table definition.</param>
		/// <param name="buffer">The buffer for the replication data.</param>
		static void ReplicateTable(DataExtractor extractor, PullArguments pull, DbId syncStart, DbId syncEnd, DbTable table, ReplicationData buffer)
		{
			//	Get all the data which changed between the two sync ids :

			DataTable dataTable = extractor.ExtractDataUsingLogIds (table, syncStart, syncEnd);

			//	Is more data required ?

			if (pull != null && pull.Contains (table))
			{
				List<long> ids = ServerReplicationEngine.FindUnknownRowIds (dataTable, pull[table].RowIds);

				if (ids.Count > 0)
				{
					//	The author of the replication job also wants some explicit rows of this table to
					//	be included in the replication; include them in the data table :

					DataTable dataMerge = extractor.ExtractDataUsingIds (table, ids);

					dataTable.BeginLoadData ();

					foreach (DataRow row in dataMerge.Rows)
					{
						dataTable.LoadDataRow (row.ItemArray, false);
					}

					dataTable.EndLoadData ();
				}
			}

			//	If the table contains any data, pack it into the replication buffer :

			if (dataTable.Rows.Count > 0)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Table {0} contains {1} rows to replicate.", dataTable.TableName, dataTable.Rows.Count));
				buffer.Add (PackedTableData.CreateFromTable (table, dataTable));
			}
		}

		/// <summary>
		/// Replicates the log table. This is a special case of the <see cref="ReplicateTable"/>
		/// method.
		/// </summary>
		/// <param name="extractor">The data extractor.</param>
		/// <param name="syncStart">The sync start id.</param>
		/// <param name="syncEnd">The sync end id.</param>
		/// <param name="table">The table definition.</param>
		/// <param name="buffer">The buffer for the replication data.</param>
		static void ReplicateLogTable(DataExtractor extractor, DbId syncStart, DbId syncEnd, DbTable table, ReplicationData buffer)
		{
			DataTable dataTable = extractor.ExtractDataUsingIds (table, syncStart, syncEnd);

			if (dataTable.Rows.Count > 0)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Table {0} contains {1} rows to replicate.", dataTable.TableName, dataTable.Rows.Count));
				buffer.Add (PackedTableData.CreateFromTable (table, dataTable));
			}
		}



		/// <summary>
		/// Finds the rows which don't match the collection of ids.
		/// </summary>
		/// <param name="table">The source data table.</param>
		/// <param name="ids">The collection of ids.</param>
		/// <returns>The collection of ids of the rows found in the table which are not found in the collection.</returns>
		static List<long> FindUnknownRowIds(DataTable table, IEnumerable<long> ids)
		{
			HashSet<long> idsCopy = new HashSet<long> (ids);
			HashSet<long> dataTableIds = new HashSet<long> (table.AsEnumerable ().Select (r => (long) r[0]));

			return idsCopy.Except (dataTableIds).ToList ();
		}

		/// <summary>
		/// Removes all tables which match the specified replication mode.
		/// </summary>
		/// <param name="tables">The tables.</param>
		/// <param name="mode">The replication mode.</param>
		static void RemoveAllMatchingTables(List<DbTable> tables, DbReplicationMode mode)
		{
			int i = 0;
			
			while (i < tables.Count)
			{
				DbTable table = tables[i] as DbTable;
				
				if (table.ReplicationMode == mode)
				{
					tables.RemoveAt (i);
				}
				else
				{
					i++;
				}
			}
		}
		
		
		void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.isShutdownPending = true;
				this.shutdownEvent.Set ();
				this.workerThread.Join ();

				this.shutdownEvent.Close ();
				this.queueEvent.Close ();
				this.database.Dispose ();
			}
		}
		
		
		readonly DbInfrastructure				infrastructure;
		readonly IDbAbstraction					database;

		readonly Thread							workerThread;
		readonly ManualResetEvent				shutdownEvent;
		volatile bool							isShutdownPending;
		readonly Queue<ReplicationJob>			queue;
		readonly AutoResetEvent					queueEvent;
	}
}
