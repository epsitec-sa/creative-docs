//	Copyright � 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Remoting;

namespace Epsitec.Cresus.Requests
{
	using EventHandler = Common.Support.EventHandler;
	
	public delegate void RequestExecutedCallback(Orchestrator sender, DbId request_id);
	
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
			
			this.abort_event   = new System.Threading.ManualResetEvent (false);
			this.server_event  = new System.Threading.AutoResetEvent (false);
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
			
			if ((this.service != null) &&
				(this.waiter_thread.IsAlive == false))
			{
				//	D�marre le processus de synchronisation entre le serveur et le
				//	client.
				
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
		
		#region WaiterThread Implementation
		protected void WaiterThread()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine ("Requests.Orchestrator Waiter Thread launched.");
				
				int change_id = -1;
				
				for (;;)
				{
					RequestState[] states;
					
					//	Charge l'�tat de nos requ�tes sur le serveur. Cet appel est bloquant
					//	si rien n'a chang� depuis le dernier appel :
					
					this.service.QueryRequestStates (this.client, ref change_id, System.TimeSpan.FromSeconds (60.0), out states);
					
					lock (this)
					{
						this.server_request_states = states;
					}
					
					this.server_event.Set ();
				}
			}
			catch (System.Threading.ThreadInterruptedException)
			{
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
		#endregion
		
		#region WorkerThread Implementation
		protected void WorkerThread()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine ("Requests.Orchestrator Worker Thread launched.");
				
				System.Threading.WaitHandle[] wait_events = new System.Threading.WaitHandle[4];
				
				wait_events[0] = this.execution_queue.EnqueueWaitEvent;
				wait_events[1] = this.execution_queue.ExecutionStateWaitEvent;
				wait_events[2] = this.server_event;
				wait_events[3] = this.abort_event;
				
				for (;;)
				{
					this.ChangeToState (this.execution_queue.HasConflicting ? OrchestratorState.Conflicting : OrchestratorState.Ready);
					
					int handle_index = Common.Support.Sync.Wait (wait_events);
					
					
					if ((handle_index < 0) ||
						(handle_index > 2))
					{
						//	L'�v�nement re�u ne fait pas partie de ceux qui requi�rent du
						//	travail; cela signifie qu'il faut s'arr�ter :
						
						this.ProcessShutdown ();
						break;
					}
					
					if (handle_index == 2)
					{
						//	L'�tat des requ�tes dans la queue du serveur a chang�. Il faut
						//	traiter ces modifications :
						
						this.ChangeToState (OrchestratorState.Processing);
						this.ProcessServerChanges ();
					}
					
					//	Analyse le contenu de la queue locale.
					
					if (this.execution_queue.HasConflicting)
					{
						//	La queue contient des requ�tes en conflit. On cesse tout travail
						//	en attendant que le conflit ait �t� r�solu...
						
						this.ChangeToState (OrchestratorState.Conflicting);
					}
					else
					{
						//	La queue contient peut-�tre des requ�tes pr�tes � �tre trait�es.
						
						this.ChangeToState (OrchestratorState.Processing);
						
						if ((this.execution_queue.HasConflictResolved) ||
							(this.execution_queue.HasPending))
						{
							this.ProcessReadyInQueue ();
							continue;
						}
						
						//	Regarde encore si nous avons une connexion avec un serveur,
						//	auquel cas nous devons assurer la synchronisation de la queue
						//	du serveur avec la n�tre.
						
						if ((this.service != null) &&
							(this.execution_queue.IsRunningAsServer == false))
						{
							if (this.execution_queue.HasExecutedByClient)
							{
								//	Le client poss�de une s�rie de requ�tes ex�cut�es
								//	localement que l'on peut maintenant tenter d'envoyer
								//	au serveur.
								
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
		#endregion
		
		protected void ProcessReadyInQueue()
		{
			//	Passe en revue la queue � la recherche de requ�tes en attente d'ex�cution.
			
			System.Data.DataRow[] rows = DbRichCommand.CopyLiveRows (this.execution_queue.DateTimeSortedRows);
			
			//	Prend note du nombre de lignes dans la queue; si des nouvelles lignes sont
			//	rajout�es pendant notre ex�cution, on les ignore. Elles seront trait�es au
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
			//	Traite une requ�te dans l'�tat ExecuctionState.Pending ou ConflictResolved;
			//	son ex�cution en local peut la faire passer dans l'�tat ExecutedByClient ou
			//	Conflicting, en fonction de son succ�s ou non.
			
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
			//	Un conflit a �t� d�tect� lors de la tentative de mise � jour de la
			//	requ�te. Passe l'�tat � 'Conflicting' et persiste la queue dans la
			//	base de donn�es.
			
			this.SwitchToConflictingLocally (row);
		}
		
		protected void ProcessSendToServer()
		{
			//	Passe en revue la queue � la recherche de requ�tes pr�tes � �tre envoy�es
			//	au serveur. Dans l'impl�mentation actuelle, on envoie une requ�te � la
			//	fois pour simplifier la d�tection des conflits.
			
			System.Data.DataRow[] rows = DbRichCommand.CopyLiveRows (this.execution_queue.DateTimeSortedRows);
			
			//	Prend note du nombre de lignes dans la queue; si des nouvelles lignes sont
			//	rajout�es pendant notre ex�cution, on les ignore. Elles seront trait�es au
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
		
		protected void ProcessServerChanges()
		{
			//	Met � jour la queue locale en fonction de l'�tat des requ�tes dans la
			//	queue du serveur (dont nous avons une copie, gr�ce � WaiterThread).
			
			RequestState[] states;
			
			lock (this)
			{
				states = (RequestState[]) this.server_request_states.Clone ();
			}
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			System.Data.DataRow[]        rows = this.execution_queue.DateTimeSortedRows;
			
			for (int i = 0; i < states.Length; i++)
			{
				System.Data.DataRow row = DbRichCommand.FindRow (rows, states[i].Identifier);
				
				if (row == null)
				{
					//	La requ�te n'existe plus dans la queue locale; ceci implique que
					//	celle stock�e sur le serveur est caduque et peut �tre supprim�e :
					
					System.Diagnostics.Debug.WriteLine (string.Format ("Warning: server still knows request {0} !", states[i].Identifier));
					
					list.Add (states[i]);
					continue;
				}
				
				//	La requ�te existe dans la queue d'ex�cution locale. D�termine s'il faut
				//	modifier son �tat :
				
				ExecutionState remote_state = ExecutionQueue.ConvertToExecutionState (states[i].State);
				ExecutionState local_state  = this.execution_queue.GetRequestExecutionState (row);
				
				if (local_state == ExecutionState.ExecutedByClient)
				{
					//	La requ�te ne "peut" pas encore avoir �t� envoy�e au serveur (en fait,
					//	elle a certainement �t� envoy�e juste avant un crash du client); il
					//	faut donc ignorer celle-ci en attendant qu'elle passe � l'�tat "envoy�e".
					
					continue;
				}
				
				if ((local_state != ExecutionState.SentToServer) &&
					(local_state != ExecutionState.ExecutedByServer) &&
					(local_state != ExecutionState.ConflictingOnServer))
				{
					//	La requ�te doit �tre dans l'un des �tats suivants :
					//
					//	(1) SentToServeur, cas normal d'une requ�te envoy�e au serveur.
					//
					//	(2) ExecutedByServer, au cas o� le client aurait red�marr� avant la
					//		suppression de la requ�te des queues du serveur et du client.
					//
					//	(3) ConflictingOnServer, au cas o� le client aurait red�marr� avant
					//		la suppression de la requ�te de la queue du serveur et son
					//		passage en local � l'�tat Conflicting.
					
					System.Diagnostics.Debug.WriteLine (string.Format ("Warning: request {0} local state is {1}; should be SentToServer.", states[i].Identifier, local_state));
				}
				
				switch (remote_state)
				{
					case ExecutionState.Pending:
						break;
					
					case ExecutionState.ExecutedByServer:
						
						//	La requ�te a �t� ex�cut�e sur le serveur. Il faut mettre � jour
						//	l'�tat dans la queue locale :
						
						this.SwitchToExecutedByServer (states[i], row);
						break;
					
					case ExecutionState.ConflictingOnServer:
						
						//	La requ�te a �t� rejet�e par le serveur (elle g�n�re un conflit).
						//	Il faut mettre � jour l'�tat dans la queue locale :
						
						this.SwitchToConflictingOnServer (row);
						break;
					
					default:
						throw new System.InvalidOperationException (string.Format ("Request ExecutionState set to {0} on server.", remote_state));
				}
			}
			
			if (list.Count > 0)
			{
				//	Il y a des requ�tes "mortes" qui peuvent �tre supprim�es de la queue
				//	du serveur :
				
				states = new RequestState[list.Count];
				list.CopyTo (states);
				this.service.RemoveRequestStates (this.client, states);
			}
			
			if (this.execution_queue.HasConflictingOnServer)
			{
				//	La queue locale contient des requ�tes marqu�es comme �tant en
				//	conflit sur le serveur; ceci n'est pas possible dans un fonc-
				//	tionnement normal.
				//
				//	C'est un �tat transitoire possible ici uniquement si la m�thode
				//	SwitchToConflictingOnServer a �t� quitt�e pr�matur�ment par une
				//	exception, caus�e par une perte de connexion avec le serveur,
				//	par exemple.
				
				System.Diagnostics.Debug.WriteLine ("Warning: found requests said to be conflicting on server, but there are none.");
				
				foreach (System.Data.DataRow row in rows)
				{
					if (this.execution_queue.GetRequestExecutionState (row) == ExecutionState.ConflictingOnServer)
					{
						this.SwitchToConflictingOnServer (row);
					}
				}
				
				System.Diagnostics.Debug.Assert (this.execution_queue.HasConflictingOnServer == false);
			}
			
			if (this.execution_queue.HasExecutedByServer)
			{
				//	La queue locale contient des requ�tes marqu�es comme ayant �t�
				//	ex�cut�es sur le serveur.
				//	
				//	Ces requ�tes n'ont plus aucun int�r�t maintenant, car elles ont
				//	d�j� �t� supprim�es de la queue du serveur � ce point.
				
				System.Diagnostics.Debug.WriteLine ("Removing ExecutedByServer local states.");
				
				foreach (System.Data.DataRow row in rows)
				{
					if (this.execution_queue.GetRequestExecutionState (row) == ExecutionState.ExecutedByServer)
					{
						this.execution_queue.RemoveRequest (row);
					}
				}
				
				System.Diagnostics.Debug.Assert (this.execution_queue.HasExecutedByServer == false);
			}
		}
		
		protected void ProcessShutdown()
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
		
		
		protected void SwitchToExecutedByServer(RequestState server_state, System.Data.DataRow row)
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database))
			{
				this.execution_queue.SetRequestExecutionState (row, ExecutionState.ExecutedByServer);
				this.execution_queue.SerializeToBase (transaction);
				transaction.Commit ();
			}
			
			//	Supprime la requ�te de la queue du serveur. En cas de perte de connexion
			//	ici, ce n'est pas autrement catastrophique : la requ�te locale est dans
			//	l'�tat ExecutedByServer et on va recevoir une nouvelle notification de
			//	la part du serveur (-> ExecutedByServer) et repasser par ici...
			
			RequestState[] states = new RequestState[] { server_state };
			
			this.service.RemoveRequestStates (this.client, states);
		}
		
		protected void SwitchToConflictingOnServer(System.Data.DataRow row)
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database))
			{
				this.execution_queue.SetRequestExecutionState (row, ExecutionState.ConflictingOnServer);
				this.execution_queue.SerializeToBase (transaction);
				transaction.Commit ();
			}
			
			//	Supprime la requ�te de la queue du serveur. En cas de perte de connexion
			//	ici, ce n'est pas autrement catastrophique : la requ�te locale est dans
			//	l'�tat ConflictingOnServer et on va recevoir une nouvelle notification
			//	de la part du serveur (-> ConflictingOnServer) et repasser par ici...
			
			this.service.RemoveAllRequestStates (this.client);
			
			//	Maintenant que le serveur n'a plus aucune trace de nos requ�tes, on peut
			//	marquer la requ�te actuelle comme Conflicting.
			
			//	-------------------------------------------------------------------------
			//	TODO: demander une r�plication "Pull" pour se remettre dans l'�tat avant
			//	l'ex�cution de la requ�te en local.
			//
			//	Il faut :
			//	
			//	  - Trouver toutes les lignes de toutes les tables qui ont �t� modifi�es
			//		depuis l'ex�cution en local de la requ�te (cf ReplicationTest et
			//		DataCruncher.ExtractRowSetsUsingLogId)
			//
			//	  - Supprimer toutes ces lignes. Le code pour une suppression multiple
			//		doit encore �tre �crit (on peut utiliser DbSelectCondition).
			//
			//	  - Demander la r�plication via PullReplication, en sp�cifiant � la fois
			//		la fourchette � r�pliquer (LOG_ID de .. � ..) et la liste des CR_ID.
			//	
			//	-------------------------------------------------------------------------
			
			//	Si cette op�ration �choue, on se retrouve avec une requ�te dans l'�tat
			//	ConflictingOnServer et aucune requ�tes sur le serveur; �a peut �tre
			//	une indication qu'il faut passer � Conflicting.
			
			this.SwitchToConflictingLocally (row);
		}
		
		protected void SwitchToConflictingLocally(System.Data.DataRow row)
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database))
			{
				this.execution_queue.SetRequestExecutionState (row, ExecutionState.Conflicting);
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
		
		
		public event EventHandler				StateChanged;
		public event RequestExecutedCallback	RequestExecuted;
		
		
		protected DbInfrastructure				infrastructure;
		protected ExecutionQueue				execution_queue;
		protected ExecutionEngine				execution_engine;
		protected IDbAbstraction				database;
		protected OrchestratorState				state;
		protected volatile RequestState[]		server_request_states;
		
		System.Threading.Thread					worker_thread;
		System.Threading.Thread					waiter_thread;
		System.Threading.ManualResetEvent		abort_event;
		System.Threading.AutoResetEvent			server_event;
		
		protected IRequestExecutionService		service;
		protected ClientIdentity				client;
	}
}
