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
			this.database       = this.infrastructure.CreateDbAbstraction ();
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
				
				try
				{
					this.ProcessQueueEntry (job);
				}
				catch (System.Exception ex)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Replication: ServerEngine.ProcessQueue failed job for client {0}; {1}", job.Client.ToString (), ex.Message));
				}
			}
		}
		
		protected virtual void ProcessQueueEntry(Job job)
		{
			//	TODO: traiter la requête...
			
			job.SignalReady ();
		}
		
		
		protected virtual void ProcessShutdown()
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("Processing shutdown."));
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
