//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.Replication
{
	/// <summary>
	/// Summary description for ServerEngine.
	/// </summary>
	public class ServerEngine : System.IDisposable
	{
		public ServerEngine(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
			this.database       = this.infrastructure.CreateDatabaseAbstraction ();
			this.queue          = new System.Collections.Queue ();
			
			this.abort_event    = new System.Threading.ManualResetEvent (false);
			this.queue_event    = new System.Threading.AutoResetEvent (false);
			
			this.worker_thread  = new System.Threading.Thread (new System.Threading.ThreadStart (this.WorkerThread));
			
			this.worker_thread.Start ();
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
		
		
		public void Enqueue(Job job)
		{
			lock (this.queue)
			{
				this.queue.Enqueue (job);
			}
			
			this.queue_event.Set ();
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected void WorkerThread()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine ("Replication.ServerEngine Worker Thread launched.");
				
				for (;;)
				{
					int handle_index = Common.Support.Sync.Wait (this.queue_event, this.abort_event);
					
					//	Tout événement autre que celui lié à la queue provoque l'interruption
					//	du processus :
					
					if (handle_index != 0)
					{
						this.ProcessShutdown ();
						break;
					}
					
					this.ProcessQueue ();
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
		
		
		protected void ProcessQueue()
		{
			for (;;)
			{
				Job job = null;
				
				lock (this.queue)
				{
					if (this.queue.Count > 0)
					{
						job = this.queue.Dequeue () as Job;
					}
				}
				
				if (job == null)
				{
					break;
				}
				
				if (job.ProgressStatus == Remoting.ProgressStatus.Cancelled)
				{
					//	Saute un éventuel job qui aurait été marqué comme annulé.
					
					continue;
				}
				
				try
				{
					job.SignalStartedProcessing ();
					
					this.ProcessQueueEntry (job);
				}
				catch (System.Exception ex)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Replication: ServerEngine.ProcessQueue failed job for client {0}; {1}", job.Client.ToString (), ex.Message));
					
					job.SignalError (ex.ToString ());
				}
				finally
				{
					System.Diagnostics.Debug.Assert (job.ProgressStatus != Remoting.ProgressStatus.Running);
				}
			}
		}
		
		
		protected virtual void ProcessQueueEntry(Job job)
		{
			int client_id = job.Client.ClientId;
			
			DbId sync_start = job.SyncStartId;
			DbId sync_end   = job.SyncEndId;
			
			Job.PullArgsCollection pull = job.PullArgs;
			
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadOnly, this.database))
			{
				DataCruncher cruncher = new DataCruncher (this.infrastructure, transaction);
				System.Collections.ArrayList tables = new System.Collections.ArrayList ();
				
				tables.AddRange (this.infrastructure.FindDbTables (transaction, DbElementCat.Any));
				
				System.Diagnostics.Debug.WriteLine (string.Format ("Found {0} tables. Extracting from {1} to {2}.", tables.Count, sync_start, sync_end));
				
				DbTable def_table_table   = ServerEngine.FindTable (tables, Tags.TableTableDef);
				DbTable def_column_table  = ServerEngine.FindTable (tables, Tags.TableColumnDef);
				DbTable def_type_table    = ServerEngine.FindTable (tables, Tags.TableTypeDef);
				DbTable def_enumval_table = ServerEngine.FindTable (tables, Tags.TableEnumValDef);
				DbTable log_table         = ServerEngine.FindTable (tables, Tags.TableLog);
				
				//	Ces tables sont répliquées de manière un peu particulière, alors on les retire
				//	de la liste des tables à répliquer automatiquement :
				
				tables.Remove (def_table_table);
				tables.Remove (def_column_table);
				tables.Remove (def_type_table);
				tables.Remove (def_enumval_table);
				tables.Remove (log_table);
				
				ReplicationData data = new ReplicationData ();
				
				this.ProcessTable (cruncher, pull, sync_start, sync_end, def_table_table, data);
				this.ProcessTable (cruncher, pull, sync_start, sync_end, def_column_table, data);
				this.ProcessTable (cruncher, pull, sync_start, sync_end, def_type_table, data);
				this.ProcessTable (cruncher, pull, sync_start, sync_end, def_enumval_table, data);
				
				this.ProcessLogTable (cruncher, sync_start, sync_end, log_table, data);
				
				ServerEngine.RemoveTables (tables, DbReplicationMode.None);
				
				System.Diagnostics.Debug.WriteLine (string.Format ("Scrubbed, remaining {0} tables.", tables.Count));
				
				for (int i = 0; i < tables.Count; i++)
				{
					DbTable table = tables[i] as DbTable;
					
					System.Diagnostics.Debug.Assert (table.ReplicationMode == DbReplicationMode.Automatic);
					
					this.ProcessTable (cruncher, pull, sync_start, sync_end, table, data);
				}
				
				//	Sérialise et comprime le "delta" produit par les appels à la méthode
				//	ProcessTable. On va faire transiter sur le réseau un nombre minimal de
				//	données :
				
				job.SignalFinishedProcessing (Common.IO.Serialization.SerializeAndCompressToMemory (data, Common.IO.Compressor.DeflateCompact));
				
				transaction.Commit ();
			}
		}
		
		protected virtual void ProcessTable(DataCruncher cruncher, Job.PullArgsCollection pull, DbId sync_start, DbId sync_end, DbTable table, ReplicationData data)
		{
			System.Data.DataTable data_table = cruncher.ExtractDataUsingLogIds (table, sync_start, sync_end);
					
			if ((pull != null) &&
				(pull.Contains (table)))
			{
				long[] ids = DataCruncher.FindUnknownRowIds (data_table, pull[table].RowIds);
				
				if (ids.Length > 0)
				{
					//	L'auteur de la demande de réplication aimerait aussi obtenir les lignes
					//	spécifiées par 'ids'.
					
					System.Data.DataTable data_merge = cruncher.ExtractDataUsingIds (table, ids);
					
					data_table.BeginLoadData ();
					
					foreach (System.Data.DataRow row in data_merge.Rows)
					{
						data_table.LoadDataRow (row.ItemArray, false);
					}
					
					data_table.EndLoadData ();
				}
			}
			
			if (data_table.Rows.Count > 0)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Table {0} contains {1} rows to replicate.", data_table.TableName, data_table.Rows.Count));
				
				data.Add (PackedTableData.CreateFromTable (table, data_table));
			}
		}
		
		protected virtual void ProcessLogTable(DataCruncher cruncher, DbId sync_start, DbId sync_end, DbTable table, ReplicationData data)
		{
			System.Data.DataTable data_table = cruncher.ExtractDataUsingIds (table, sync_start, sync_end);
					
			if (data_table.Rows.Count > 0)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Table {0} contains {1} rows to replicate.", data_table.TableName, data_table.Rows.Count));
				
				data.Add (PackedTableData.CreateFromTable (table, data_table));
			}
		}
		
		protected virtual void ProcessShutdown()
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
		
		private static void RemoveTables(System.Collections.ArrayList tables, DbReplicationMode mode)
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
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.abort_event.Set ();
				this.worker_thread.Join ();
				
				this.database.Dispose ();
				
				this.database = null;
			}
		}
		
		
		protected DbInfrastructure				infrastructure;
		protected IDbAbstraction				database;
		
		System.Threading.Thread					worker_thread;
		System.Threading.ManualResetEvent		abort_event;
		System.Collections.Queue				queue;
		System.Threading.AutoResetEvent			queue_event;
	}
}
