//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Remoting;

namespace Epsitec.Cresus.Requests
{
	using EventHandler = Common.Support.EventHandler;
	
	/// <summary>
	/// La classe Orchestrator gère l'arrivée de requêtes, leur mise en queue et
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
			
			this.abort_event   = new System.Threading.ManualResetEvent (false);
			this.worker_thread = new System.Threading.Thread (new System.Threading.ThreadStart (this.WorkerThread));
			this.waiter_thread = new System.Threading.Thread (new System.Threading.ThreadStart (this.WaiterThread));
			
			this.worker_thread.Start ();
		}
		
		
		public ExecutionQueue					ExecutionQueue
		{
			get
			{
				return this.execution_queue;
			}
		}
		
		public ExecutionEngine					ExecutionEngine
		{
			get
			{
				return this.execution_engine;
			}
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
		
		public IRequestExecutionService			RemotingService
		{
			get
			{
				return this.service;
			}
		}
		
		public ClientIdentity					ClientIdentity
		{
			get
			{
				return this.client;
			}
		}
		
		public OrchestratorState				State
		{
			get
			{
				return this.state;
			}
		}
		
		
		public void DefineRemotingService(IRequestExecutionService service, ClientIdentity client)
		{
			this.service = service;
			this.client  = client;
			
			if (this.service != null)
			{
				this.waiter_thread.Start ();
			}
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected void WaiterThread()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine ("Requests.Orchestrator Waiter Thread launched.");
				
				int change_id = -1;
				
				for (;;)
				{
					RequestState[] states;
					
					this.service.QueryRequestStates (this.client, ref change_id, System.TimeSpan.FromSeconds (30.0), out states);
					
					System.Collections.ArrayList list = new System.Collections.ArrayList ();
					System.Data.DataRow[]        rows = DbRichCommand.CopyLiveRows (this.execution_queue.DateTimeSortedRows);
					
					bool conflict_detected = false;
					
					//	Passe en revue l'état de toutes les requêtes retournées par le
					//	serveur :
					
					for (int i = 0; i < states.Length; i++)
					{
						lock (this.execution_queue)
						{
							System.Data.DataRow row = DbRichCommand.FindRow (rows, states[i].Identifier);
							
							if ((row == null) ||
								(DbRichCommand.IsRowDeleted (row)))
							{
								list.Add (states[i]);
								continue;
							}
							
							//	La requête est contenue dans la queue d'exécution. En fonction de
							//	l'état de celle-ci, il va falloir soit la valider définitivement,
							//	soit la marquer comme étant invalide.
							
							switch ((ExecutionState) states[i].State)
							{
								case ExecutionState.ExecutedByServer:
									this.ProcessServerExecuted (row);
									list.Add (states[i]);
									break;
								
								case ExecutionState.Conflicting:
									this.ProcessServerConflict (row);
									conflict_detected = true;
									break;
							}
						}
					}
					
					if (conflict_detected)
					{
						//	Le serveur a détecté un conflit pour notre client.
					}
					
					if (list.Count > 0)
					{
						states = new RequestState[list.Count];
						list.CopyTo (states);
						this.service.ClearRequestStates (this.client, states);
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
				System.Diagnostics.Debug.WriteLine ("Requests.Orchestrator Waiter Thread terminated.");
			}
		}
		
		protected void ProcessServerExecuted(System.Data.DataRow row)
		{
			System.Diagnostics.Debug.WriteLine ("Requests.Orchestrator: server executed request " + row[Tags.ColumnId]);
			this.execution_queue.RemoveRequest (row);
		}
		
		protected void ProcessServerConflict(System.Data.DataRow row)
		{
			System.Diagnostics.Debug.WriteLine ("Requests.Orchestrator: server detected conflict on request " + row[Tags.ColumnId]);
			this.execution_queue.SetRequestExecutionState (row, ExecutionState.Conflicting);
		}
		
		
		protected void WorkerThread()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine ("Requests.Orchestrator Worker Thread launched.");
				
				System.Threading.WaitHandle[] wait_events = new System.Threading.WaitHandle[3];
				
				wait_events[0] = this.execution_queue.EnqueueWaitEvent;
				wait_events[1] = this.execution_queue.ExecutionStateWaitEvent;
				wait_events[2] = this.abort_event;
				
				for (;;)
				{
					this.ChangeToState (this.execution_queue.HasConflicting ? OrchestratorState.Conflicting : OrchestratorState.Ready);
					
					int handle_index = Common.Support.Sync.Wait (wait_events);
					
					//	Tout événement autre que ceux liés à la queue provoque l'interruption
					//	du processus :
					
					if ((handle_index < 0) ||
						(handle_index > 1))
					{
						this.ProcessShutdown ();
						break;
					}
					
					//	Analyse le contenu de la queue.
					
					if (this.execution_queue.HasConflicting)
					{
						//	La queue contient des éléments en conflit.
						
						this.ChangeToState (OrchestratorState.Conflicting);
					}
					else
					{
						this.ChangeToState (OrchestratorState.Processing);
						
						if ((this.execution_queue.HasConflictResolved) ||
							(this.execution_queue.HasPending))
						{
							this.ProcessReadyInQueue ();
							continue;
						}
						
						if ((this.service != null) &&
							(this.execution_queue.IsRunningAsServer == false))
						{
							if (this.execution_queue.HasExecutedByClient)
							{
								//	Le client possède une série de requêtes exécutées localement que
								//	l'on peut maintenant tenter d'envoyer au serveur.
								
								this.ProcessSendToServer ();
							}
						}
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
				System.Diagnostics.Debug.WriteLine ("Requests.Orchestrator Worker Thread terminated.");
			}
		}
		
		
		protected void ProcessReadyInQueue()
		{
			//	Passe en revue la queue à la recherche de requêtes en attente d'exécution.
			
			System.Data.DataRow[] rows = DbRichCommand.CopyLiveRows (this.execution_queue.DateTimeSortedRows);
			
			//	Prend note du nombre de lignes dans la queue; si des nouvelles lignes sont
			//	rajoutées pendant notre exécution, on les ignore. Elles seront traitées au
			//	prochain tour.
			
			int n = rows.Length;
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Queue contains {0} {1}.", n, (n == 1) ? "request" : "requests"));
			
			for (int i = 0; i < n; i++)
			{
				if (this.execution_queue.HasConflicting)
				{
					break;
				}
				
				System.Data.DataRow row = rows[i];
				
				if (DbRichCommand.IsRowDeleted (row))
				{
					continue;
				}
				
				ExecutionState state = this.execution_queue.GetRequestExecutionState (row);
				
				System.DateTime time = (System.DateTime) row[Tags.ColumnDateTime];
				string precise_time = string.Format ("{0:00}:{1:00}:{2:00}.{3:000}", time.Hour, time.Minute, time.Second, time.Millisecond);
				System.Diagnostics.Debug.WriteLine (string.Format (" {0} --> {1} - {2}", i, state, precise_time));
				
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
		
		protected void ProcessPendingRequest(System.Data.DataRow row, AbstractRequest request)
		{
			//	Traite une requête dans l'état ExecuctionState.Pending ou ConflictResolved;
			//	son exécution en local peut la faire passer dans l'état ExecutedByClient ou
			//	Conflicting, en fonction de son succès ou non.
			
			DbKey  request_key = new DbKey (row);
			DbId   request_id  = request_key.Id;
			
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
			
			this.OnRequestExecuted (request_id);
			
			if (conflict_detected)
			{
				this.ProcessDetectedConflict (row, request);
			}
		}
		
		protected void ProcessDetectedConflict(System.Data.DataRow row, AbstractRequest request)
		{
			//	Un conflit a été détecté lors de la tentative de mise à jour de la
			//	requête. Note que la requête est en "conflit" sans pour autant mettre
			//	à jour la base de données; en effet, si on perd maintenant l'information
			//	qui nous indique le conflit, on risque tout au plus de re-exécuter la
			//	requête (la dernière exécution n'a eu aucun effet de bord).
			
			this.execution_queue.SetRequestExecutionState (row, ExecutionState.Conflicting);
		}
		
		protected void ProcessSendToServer()
		{
			//	Passe en revue la queue à la recherche de requêtes prêtes à être envoyées
			//	au serveur. Dans l'implémentation actuelle, on envoie une requête à la
			//	fois pour simplifier la détection des conflits.
			
			System.Data.DataRow[] rows = DbRichCommand.CopyLiveRows (this.execution_queue.DateTimeSortedRows);
			
			//	Prend note du nombre de lignes dans la queue; si des nouvelles lignes sont
			//	rajoutées pendant notre exécution, on les ignore. Elles seront traitées au
			//	prochain tour.
			
			int n = rows.Length;
			
			for (int i = 0; i < n; i++)
			{
				if (this.execution_queue.HasConflicting)
				{
					break;
				}
				
				System.Data.DataRow row = rows[i];
				
				if (DbRichCommand.IsRowDeleted (row))
				{
					continue;
				}
				
				ExecutionState  state   = this.execution_queue.GetRequestExecutionState (row);
				AbstractRequest request = null;;
				
				switch (state)
				{
					case ExecutionState.ExecutedByClient:
						request = this.execution_queue.GetRequest (row);
						this.ProcessSendToServer (row, request);
						break;
					
					default:
						break;
				}
			}
		}
		
		protected void ProcessSendToServer(System.Data.DataRow row, AbstractRequest request)
		{
			SerializedRequest[] requests = new SerializedRequest[1];
			
			DbKey  request_key = new DbKey (row);
			DbId   request_id  = request_key.Id;
			byte[] serialized  = Requests.AbstractRequest.SerializeToMemory (request);
			
			requests[0] = new Remoting.SerializedRequest (request_id, serialized);
			
			this.service.EnqueueRequest (this.client, requests);
			
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database))
			{
				this.execution_queue.SetRequestExecutionState (row, ExecutionState.SentToServer);
				this.execution_queue.SerializeToBase (transaction);
				transaction.Commit ();
			}
		}
		
		protected void ProcessShutdown()
		{
			//	Avant l'arrêt planifié du processus, on s'empresse encore de mettre à jour
			//	l'état de la queue dans la base de données (la queue possède un cache en
			//	mémoire).
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Processing shutdown."));
			
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database))
			{
				this.execution_queue.SerializeToBase (transaction);
				transaction.Commit ();
			}
		}
		
		
		protected void ChangeToState(OrchestratorState state)
		{
			if (this.state != state)
			{
				this.state = state;
				this.OnStateChanged ();
			}
		}
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.abort_event.Set ();
				this.worker_thread.Join ();
				
				if (this.waiter_thread.IsAlive)
				{
					this.waiter_thread.Interrupt ();
					this.waiter_thread.Join ();
				}
				
				this.database.Dispose ();
				
				System.Diagnostics.Debug.WriteLine ("Disposed Orchestrator.");
				
				this.database = null;
			}
		}
		
		
		protected virtual void OnStateChanged()
		{
			if (this.StateChanged != null)
			{
				this.StateChanged (this);
			}
		}
		
		protected virtual void OnRequestExecuted(DbId request_id)
		{
			if (this.RequestExecuted != null)
			{
				this.RequestExecuted (this, request_id);
			}
		}
		
		public delegate void RequestExecutedCallback(Orchestrator sender, DbId request_id);
		
		public event EventHandler				StateChanged;
		public event RequestExecutedCallback	RequestExecuted;
		
		
		protected DbInfrastructure				infrastructure;
		protected ExecutionQueue				execution_queue;
		protected ExecutionEngine				execution_engine;
		protected IDbAbstraction				database;
		protected OrchestratorState				state;
		
		System.Threading.Thread					worker_thread;
		System.Threading.Thread					waiter_thread;
		System.Threading.ManualResetEvent		abort_event;
		
		protected IRequestExecutionService		service;
		protected ClientIdentity				client;
	}
}
