//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// La classe Orchestrator g�re l'arriv�e de requ�tes, leur mise en queue et
	/// leur traitement.
	/// </summary>
	public class Orchestrator : System.IDisposable
	{
		public Orchestrator(DbInfrastructure infrastructure)
		{
			this.infrastructure   = infrastructure;
			this.database         = this.infrastructure.CreateDbAbstraction ();
			this.execution_engine = new ExecutionEngine (this.infrastructure);
			this.execution_queue  = new ExecutionQueue (this.infrastructure, this.database);
			
			this.thread_abort_event = new System.Threading.ManualResetEvent (false);
			this.worker_thread = new System.Threading.Thread (new System.Threading.ThreadStart (this.WorkerThread));
			
			this.worker_thread.Start ();
		}
		
		
		public ExecutionQueue					ExecutionQueue
		{
			get
			{
				return this.execution_queue;
			}
		}
		
		public DbInfrastructure					Infrastructure
		{
			get
			{
				return this.infrastructure;
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
			
			handles[0] = this.execution_queue.EnqueueEvent;
			handles[1] = Common.Support.Globals.AbortEvent;
			handles[2] = this.thread_abort_event;
			
			try
			{
				System.Diagnostics.Debug.WriteLine ("Requests.Orchestrator Worker Thread launched.");
				
				for (;;)
				{
					int handle_index = System.Threading.WaitHandle.WaitAny (handles);
					
					//	G�re le cas particulier d�crit dans la documentation o� l'index peut �tre
					//	incorrect dans certains cas :
					
					if (handle_index >= 128)
					{
						handle_index -= 128;
					}
					
					//	Tout �v�nement autre que celui li� � la queue provoque l'interruption
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
				System.Diagnostics.Debug.WriteLine ("Requests.Orchestrator Worker Thread terminated.");
			}
		}
		
		protected void ProcessQueue()
		{
			//	Passe en revue la queue � la recherche de requ�tes en attente d'ex�cution.
			
			System.Data.DataRow[] rows = this.execution_queue.Rows;
			
			//	Prend note du nombre de lignes dans la queue; si des nouvelles lignes sont
			//	rajout�es pendant notre ex�cution, on les ignore. Elles seront trait�es au
			//	prochain tour.
			
			int n = rows.Length;
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Queue contains {0} {1}.", n, (n == 1) ? "request" : "requests"));
			
			for (int i = 0; i < n; i++)
			{
				System.Data.DataRow row = rows[i];
				
				if (row.RowState == System.Data.DataRowState.Deleted)
				{
					continue;
				}
				
				ExecutionState state = this.execution_queue.GetRequestExecutionState (row);
				
				System.Diagnostics.Debug.WriteLine (string.Format (" {0} --> {1}", i, state));
				
				AbstractRequest request = null;;
				
				switch (state)
				{
					case ExecutionState.Pending:
					case ExecutionState.ConflictResolved:
						request = this.execution_queue.GetRequest (row);
						this.ProcessPendingRequest (row, request);
						break;
					
					default:
						break;
				}
			}
		}
		
		
		protected virtual void ProcessPendingRequest(System.Data.DataRow row, AbstractRequest request)
		{
			//	Traite une requ�te dans l'�tat ExecuctionState.Pending ou ConflictResolved;
			//	son ex�cution en local peut la faire passer dans l'�tat ExecutedByClient ou
			//	Conflicting, en fonction de son succ�s ou non.
			
			DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Processing request ({0}).", request.RequestType));
			
			bool conflict_detected = false;
			
			try
			{
				this.execution_engine.Execute (transaction, request);
				this.execution_queue.SetRequestExecutionState (row, ExecutionState.ExecutedByClient);
				this.execution_queue.SerializeToBase (transaction);
				
				transaction.Commit ();
			}
			catch (System.Exception exception)
			{
				System.Diagnostics.Debug.WriteLine (exception.Message);
				
				conflict_detected = true;
			}
			finally
			{
				transaction.Dispose ();
			}
			
			if (conflict_detected)
			{
				this.ProcessDetectedConflict (row, request);
			}
		}
		
		protected virtual void ProcessDetectedConflict(System.Data.DataRow row, AbstractRequest request)
		{
			//	Un conflit a �t� d�tect� lors de la tentative de mise � jour de la
			//	requ�te. Note que la requ�te est en "conflit" sans pour autant mettre
			//	� jour la base de donn�es; en effet, si on perd maintenant l'information
			//	qui nous indique le conflit, on risque tout au plus de re-ex�cuter la
			//	requ�te (la derni�re ex�cution n'a eu aucun effet de bord).
			
			this.execution_queue.SetRequestExecutionState (row, ExecutionState.Conflicting);
		}
		
		protected virtual void ProcessShutdown()
		{
			//	Avant l'arr�t planifi� du processus, on s'empresse encore de mettre � jour
			//	l'�tat de la queue dans la base de donn�es (la queue poss�de un cache en
			//	m�moire).
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Processing shutdown."));
			
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database))
			{
				this.execution_queue.SerializeToBase (transaction);
				transaction.Commit ();
			}
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
		protected ExecutionQueue				execution_queue;
		protected ExecutionEngine				execution_engine;
		protected IDbAbstraction				database;
		
		System.Threading.Thread					worker_thread;
		System.Threading.ManualResetEvent		thread_abort_event;
	}
}
