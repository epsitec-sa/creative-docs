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
			this.infrastructure   = infrastructure;
			this.database         = this.infrastructure.CreateDbAbstraction ();
			
			this.thread_abort_event = new System.Threading.ManualResetEvent (false);
			this.worker_thread = new System.Threading.Thread (new System.Threading.ThreadStart (this.WorkerThread));
			
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
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected void WorkerThread()
		{
			System.Threading.WaitHandle[] handles = new System.Threading.WaitHandle[3];
			
//			handles[0] = ...;
			handles[1] = Common.Support.Globals.AbortEvent;
			handles[2] = this.thread_abort_event;
			
			try
			{
				System.Diagnostics.Debug.WriteLine ("Replication.ServerEngine Worker Thread launched.");
				
				for (;;)
				{
					int handle_index = System.Threading.WaitHandle.WaitAny (handles);
					
					//	Gère le cas particulier décrit dans la documentation où l'index peut être
					//	incorrect dans certains cas :
					
					if (handle_index >= 128)
					{
						handle_index -= 128;
					}
					
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
		}
		
		
		protected virtual void ProcessShutdown()
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("Processing shutdown."));
		}
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.thread_abort_event.Set ();
				this.worker_thread.Join ();
				
				this.database.Dispose ();
				
				this.database = null;
			}
		}
		
		
		protected DbInfrastructure				infrastructure;
		protected IDbAbstraction				database;
		
		System.Threading.Thread					worker_thread;
		System.Threading.ManualResetEvent		thread_abort_event;
	}
}
