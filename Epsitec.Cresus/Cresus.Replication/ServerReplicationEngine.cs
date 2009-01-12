//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database;

using System.Collections.Generic;
using System.Threading;

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
		
		
		public DbInfrastructure					Infrastructure
		{
			get
			{
				return this.infrastructure;
			}
		}
		
		public IDbAbstraction					Database
		{
			get
			{
				return this.database;
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
						job = this.queue.Dequeue () as ReplicationJob;
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
		/// Processes the replication job.
		/// </summary>
		/// <param name="job">The replication job.</param>
		void ProcessReplicationJob(ReplicationJob job)
		{
			int clientId = job.Client.Id;
			
			DbId syncStart = job.SyncStartId;
			DbId syncEnd   = job.SyncEndId;

			PullArguments pull = job.PullArguments;
			
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly, this.database))
			{
				DataCruncher cruncher = new DataCruncher (this.infrastructure, transaction);
				List<DbTable> tables = new List<DbTable> ();
				
				tables.AddRange (this.infrastructure.FindDbTables (transaction, DbElementCat.Any));
				
				System.Diagnostics.Debug.WriteLine (string.Format ("Found {0} tables. Extracting from {1} to {2}.", tables.Count, syncStart, syncEnd));
				
				DbTable defTableTable  = ServerReplicationEngine.FindTable (tables, Tags.TableTableDef);
				DbTable defColumnTable = ServerReplicationEngine.FindTable (tables, Tags.TableColumnDef);
				DbTable defTypeTable   = ServerReplicationEngine.FindTable (tables, Tags.TableTypeDef);
				DbTable logTable       = ServerReplicationEngine.FindTable (tables, Tags.TableLog);
				
				//	We don't replicate these tables through the standard replication mechanism;
				//	remove them from the list of tables :
				
				tables.Remove (defTableTable);
				tables.Remove (defColumnTable);
				tables.Remove (defTypeTable);
				tables.Remove (logTable);
				
				ReplicationData data = new ReplicationData ();

				//	Process the special schema information tables first, before we process the real
				//	data tables :
				
				this.ProcessTable (cruncher, pull, syncStart, syncEnd, defTableTable, data);
				this.ProcessTable (cruncher, pull, syncStart, syncEnd, defColumnTable, data);
				this.ProcessTable (cruncher, pull, syncStart, syncEnd, defTypeTable, data);
				
				this.ProcessLogTable (cruncher, syncStart, syncEnd, logTable, data);
				
				ServerReplicationEngine.RemoveTables (tables, DbReplicationMode.None);
				
				System.Diagnostics.Debug.WriteLine (string.Format ("Scrubbed, remaining {0} tables.", tables.Count));
				
				foreach  (DbTable table in tables)
				{
					System.Diagnostics.Debug.Assert (table.ReplicationMode == DbReplicationMode.Automatic);
					
					this.ProcessTable (cruncher, pull, syncStart, syncEnd, table, data);
				}
				
				//	Serialize and manually compress the delta produced by the calls to
				//	method ProcessTable; we want to minimize the amount of data which
				//	goes down the wire :
				
				job.NotifyDoneProcessing (Common.IO.Serialization.SerializeAndCompressToMemory (data, Common.IO.Compressor.DeflateCompact));
				
				transaction.Commit ();
			}
		}

		void ProcessTable(DataCruncher cruncher, PullArguments pull, DbId syncStart, DbId syncEnd, DbTable table, ReplicationData data)
		{
			System.Data.DataTable dataTable = cruncher.ExtractDataUsingLogIds (table, syncStart, syncEnd);
					
			if ((pull != null) &&
				(pull.Contains (table)))
			{
				long[] ids = DataCruncher.FindUnknownRowIds (dataTable, pull[table].RowIds);
				
				if (ids.Length > 0)
				{
					//	L'auteur de la demande de réplication aimerait aussi obtenir les lignes
					//	spécifiées par 'ids'.
					
					System.Data.DataTable dataMerge = cruncher.ExtractDataUsingIds (table, ids);
					
					dataTable.BeginLoadData ();
					
					foreach (System.Data.DataRow row in dataMerge.Rows)
					{
						dataTable.LoadDataRow (row.ItemArray, false);
					}
					
					dataTable.EndLoadData ();
				}
			}
			
			if (dataTable.Rows.Count > 0)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Table {0} contains {1} rows to replicate.", dataTable.TableName, dataTable.Rows.Count));
				
				data.Add (PackedTableData.CreateFromTable (table, dataTable));
			}
		}
		
		void ProcessLogTable(DataCruncher cruncher, DbId syncStart, DbId syncEnd, DbTable table, ReplicationData data)
		{
			System.Data.DataTable dataTable = cruncher.ExtractDataUsingIds (table, syncStart, syncEnd);
					
			if (dataTable.Rows.Count > 0)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Table {0} contains {1} rows to replicate.", dataTable.TableName, dataTable.Rows.Count));
				
				data.Add (PackedTableData.CreateFromTable (table, dataTable));
			}
		}
		
		void ProcessShutdown()
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("Processing shutdown."));
		}
		
		
		private static DbTable FindTable(System.Collections.IEnumerable tables, string name)
		{
			foreach (DbTable table in tables)
			{
				if (table.Name == name)
				{
					return table;
				}
			}
			
			return null;
		}
		
		private static void RemoveTables(List<DbTable> tables, DbReplicationMode mode)
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
