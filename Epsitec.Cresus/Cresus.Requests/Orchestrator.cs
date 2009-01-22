//	Copyright � 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Remoting;

using System.Threading;
using System.Collections.Generic;

namespace Epsitec.Cresus.Requests
{

	/// <summary>
	/// The <c>Orchestrator</c> class receives requests, puts them into an execution queue
	/// and then processes them sequentially.
	/// </summary>
	public sealed class Orchestrator : System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Orchestrator"/> class.
		/// </summary>
		/// <param name="infrastructure">The database infrastructure.</param>
		public Orchestrator(DbInfrastructure infrastructure)
		{
			this.infrastructure   = infrastructure;
			
			this.database                      = this.infrastructure.CreateDatabaseAbstraction ();
			this.database.SqlBuilder.AutoClear = true;

			this.executionEngine = new ExecutionEngine (this.infrastructure);
			this.executionQueue  = new ExecutionQueue (this.infrastructure, this.database);
			
			this.abortEvent   = new System.Threading.ManualResetEvent (false);
			this.serverEvent  = new System.Threading.AutoResetEvent (false);
			this.workerThread = new System.Threading.Thread (this.WorkerThread);
			this.waiterThread = new System.Threading.Thread (this.WaiterThread);

			this.workerThread.Name = "Requests.Orchestrator worker";
			this.waiterThread.Name = "Requests.Orchestrator waiter";
			
			this.workerThread.Start ();
		}
		
		
		public ExecutionQueue					ExecutionQueue
		{
			get
			{
				return this.executionQueue;
			}
		}
		
		public ExecutionEngine					ExecutionEngine
		{
			get
			{
				return this.executionEngine;
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


		/// <summary>
		/// Defines the remoting service; this method gets called by the <see cref="Epsitec.Cresus.Services.Engine"/>
		/// class.
		/// </summary>
		/// <param name="service">The request execution service.</param>
		/// <param name="client">The client identity.</param>
		public void DefineRemotingService(IRequestExecutionService service, ClientIdentity client)
		{
			this.service = service;
			this.client  = client;
			
			if ((this.service != null) &&
				(this.waiterThread.IsAlive == false))
			{
				this.waiterThread.Start ();
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

		/// <summary>
		/// The waiter thread waits for the arrival of requests on the request execution
		/// service; whenever requests arrive, the worker thread will be awoken.
		/// </summary>
		void WaiterThread()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine ("Requests.Orchestrator Waiter Thread launched.");

				int changeId = -1;
				
				while (!this.isThreadAbortRequested)
				{
					RequestState[] states;
					
					//	Load the states of the requests on our server instance; block until something
					//	changes or we time out.
					
					int newChangeId = this.service.QueryRequestStatesUsingFilter (this.client, changeId, out states);

					if (changeId != newChangeId)
					{
						changeId = newChangeId;
						
						lock (this.exclusion)
						{
							this.serverRequestStates = states;
						}

						this.serverEvent.Set ();
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

		#endregion
		
		#region WorkerThread Implementation
		
		void WorkerThread()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine ("Requests.Orchestrator Worker Thread launched.");
				
				System.Threading.WaitHandle[] wait_events = new System.Threading.WaitHandle[4];
				
				wait_events[0] = this.executionQueue.QueueChangedWaitEvent;
				wait_events[1] = this.executionQueue.ExecutionStateWaitEvent;
				wait_events[2] = this.serverEvent;
				wait_events[3] = this.abortEvent;
				
				for (;;)
				{
					this.ChangeToState (this.executionQueue.HasConflicting ? OrchestratorState.Conflicting : OrchestratorState.Ready);
					
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
					
					if (this.executionQueue.HasConflicting)
					{
						//	La queue contient des requ�tes en conflit. On cesse tout travail
						//	en attendant que le conflit ait �t� r�solu...
						
						this.ChangeToState (OrchestratorState.Conflicting);
					}
					else
					{
						//	La queue contient peut-�tre des requ�tes pr�tes � �tre trait�es.
						
						this.ChangeToState (OrchestratorState.Processing);
						
						if ((this.executionQueue.HasConflictResolved) ||
							(this.executionQueue.HasPending))
						{
							this.ProcessReadyInQueue ();
							continue;
						}
						
						//	Regarde encore si nous avons une connexion avec un serveur,
						//	auquel cas nous devons assurer la synchronisation de la queue
						//	du serveur avec la n�tre.
						
						if ((this.service != null) &&
							(this.executionQueue.IsRunningAsServer == false))
						{
							if (this.executionQueue.HasExecutedByClient)
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
		
		void ProcessReadyInQueue()
		{
			//	Passe en revue la queue � la recherche de requ�tes en attente d'ex�cution.
			
			List<System.Data.DataRow> rows = new List<System.Data.DataRow> (DbRichCommand.GetLiveRows (this.executionQueue.GetDateTimeSortedRows ()));
			
			//	Prend note du nombre de lignes dans la queue; si des nouvelles lignes sont
			//	rajout�es pendant notre ex�cution, on les ignore. Elles seront trait�es au
			//	prochain tour.
			
			int n = rows.Count;
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Queue contains {0} {1}.", n, (n == 1) ? "request" : "requests"));
			
			for (int i = 0; i < n; i++)
			{
				if (this.executionQueue.HasConflicting)
				{
					break;
				}
				
				System.Data.DataRow row = rows[i];
				
				if (DbRichCommand.IsRowDeleted (row))
				{
					continue;
				}
				
				ExecutionState state = this.executionQueue.GetRequestExecutionState (row);
				
				System.DateTime time = (System.DateTime) row[Tags.ColumnDateTime];
				string precise_time = string.Format ("{0:00}:{1:00}:{2:00}.{3:000}", time.Hour, time.Minute, time.Second, time.Millisecond);
				System.Diagnostics.Debug.WriteLine (string.Format (" {0} --> {1} - {2}", i, state, precise_time));
				
				AbstractRequest request = null;;
				
				switch (state)
				{
					case ExecutionState.Pending:
					case ExecutionState.ConflictResolved:
						request = this.executionQueue.GetRequest (row);
						this.ProcessPendingRequest (row, request);
						break;
					
					default:
						break;
				}
			}
		}
		
		void ProcessPendingRequest(System.Data.DataRow row, AbstractRequest request)
		{
			//	Traite une requ�te dans l'�tat ExecuctionState.Pending ou ConflictResolved;
			//	son ex�cution en local peut la faire passer dans l'�tat ExecutedByClient ou
			//	Conflicting, en fonction de son succ�s ou non.
			
			DbKey  request_key = new DbKey (row);
			DbId   request_id  = request_key.Id;
			
			DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Processing request ({0}).", request.GetType ().Name));
			
			bool conflict_detected = false;
			
			try
			{
				this.executionEngine.Execute (transaction, request);
				this.executionQueue.SetRequestExecutionState (row, ExecutionState.ExecutedByClient);
				this.executionQueue.PersistToBase (transaction);
				
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
		
		void ProcessDetectedConflict(System.Data.DataRow row, AbstractRequest request)
		{
			//	Un conflit a �t� d�tect� lors de la tentative de mise � jour de la
			//	requ�te. Passe l'�tat � 'Conflicting' et persiste la queue dans la
			//	base de donn�es.
			
			this.SwitchToConflictingLocally (row);
		}
		
		void ProcessSendToServer()
		{
			//	Passe en revue la queue � la recherche de requ�tes pr�tes � �tre envoy�es
			//	au serveur. Dans l'impl�mentation actuelle, on envoie une requ�te � la
			//	fois pour simplifier la d�tection des conflits.
			
			List<System.Data.DataRow> rows = new List<System.Data.DataRow> (DbRichCommand.GetLiveRows (this.executionQueue.GetDateTimeSortedRows ()));
			
			//	Prend note du nombre de lignes dans la queue; si des nouvelles lignes sont
			//	rajout�es pendant notre ex�cution, on les ignore. Elles seront trait�es au
			//	prochain tour.
			
			int n = rows.Count;
			
			for (int i = 0; i < n; i++)
			{
				if (this.executionQueue.HasConflicting)
				{
					break;
				}
				
				System.Data.DataRow row = rows[i];
				
				if (DbRichCommand.IsRowDeleted (row))
				{
					continue;
				}
				
				ExecutionState  state   = this.executionQueue.GetRequestExecutionState (row);
				AbstractRequest request = null;;
				
				switch (state)
				{
					case ExecutionState.ExecutedByClient:
						request = this.executionQueue.GetRequest (row);
						this.ProcessSendToServer (row, request);
						break;
					
					default:
						break;
				}
			}
		}
		
		void ProcessSendToServer(System.Data.DataRow row, AbstractRequest request)
		{
			SerializedRequest[] requests = new SerializedRequest[1];
			
			DbKey  request_key = new DbKey (row);
			DbId   request_id  = request_key.Id;
			byte[] serialized  = Epsitec.Common.IO.Serialization.SerializeToMemory (request);
			
			requests[0] = new Remoting.SerializedRequest (request_id, serialized);
			
			this.service.EnqueueRequest (this.client, requests);
			
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database))
			{
				this.executionQueue.SetRequestExecutionState (row, ExecutionState.SentToServer);
				this.executionQueue.PersistToBase (transaction);
				transaction.Commit ();
			}
		}
		
		void ProcessServerChanges()
		{
			//	Met � jour la queue locale en fonction de l'�tat des requ�tes dans la
			//	queue du serveur (dont nous avons une copie, gr�ce � WaiterThread).
			
			RequestState[] states;

			lock (this.exclusion)
			{
				states = (RequestState[]) this.serverRequestStates.Clone ();
			}
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			System.Data.DataRow[]        rows = this.executionQueue.GetDateTimeSortedRows ();
			
			for (int i = 0; i < states.Length; i++)
			{
				System.Data.DataRow row = DbRichCommand.FindRow (this.executionQueue.QueueDataTable, rows, states[i].RequestId);
				
				if (row == null)
				{
					//	La requ�te n'existe plus dans la queue locale; ceci implique que
					//	celle stock�e sur le serveur est caduque et peut �tre supprim�e :
					
					System.Diagnostics.Debug.WriteLine (string.Format ("Warning: server still knows request {0} !", states[i].RequestId));
					
					list.Add (states[i]);
					continue;
				}
				
				//	La requ�te existe dans la queue d'ex�cution locale. D�termine s'il faut
				//	modifier son �tat :
				
				ExecutionState remote_state = ExecutionQueue.ConvertToExecutionState (states[i].State);
				ExecutionState local_state  = this.executionQueue.GetRequestExecutionState (row);
				
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
					
					System.Diagnostics.Debug.WriteLine (string.Format ("Warning: request {0} local state is {1}; should be SentToServer.", states[i].RequestId, local_state));
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
			
			if (this.executionQueue.HasConflictingOnServer)
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
					if (this.executionQueue.GetRequestExecutionState (row) == ExecutionState.ConflictingOnServer)
					{
						this.SwitchToConflictingOnServer (row);
					}
				}
				
				System.Diagnostics.Debug.Assert (this.executionQueue.HasConflictingOnServer == false);
			}
			
			if (this.executionQueue.HasExecutedByServer)
			{
				//	La queue locale contient des requ�tes marqu�es comme ayant �t�
				//	ex�cut�es sur le serveur.
				//	
				//	Ces requ�tes n'ont plus aucun int�r�t maintenant, car elles ont
				//	d�j� �t� supprim�es de la queue du serveur � ce point.
				
				System.Diagnostics.Debug.WriteLine ("Removing ExecutedByServer local states.");
				
				foreach (System.Data.DataRow row in rows)
				{
					if (this.executionQueue.GetRequestExecutionState (row) == ExecutionState.ExecutedByServer)
					{
						this.executionQueue.RemoveRequestRow (row);
					}
				}
				
				System.Diagnostics.Debug.Assert (this.executionQueue.HasExecutedByServer == false);
			}
		}
		
		void ProcessShutdown()
		{
			//	Avant l'arr�t planifi� du processus, on s'empresse encore de mettre � jour
			//	l'�tat de la queue dans la base de donn�es (la queue poss�de un cache en
			//	m�moire).
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Processing shutdown."));
			
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database))
			{
				this.executionQueue.PersistToBase (transaction);
				transaction.Commit ();
			}
		}
		
		
		void SwitchToExecutedByServer(RequestState server_state, System.Data.DataRow row)
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database))
			{
				this.executionQueue.SetRequestExecutionState (row, ExecutionState.ExecutedByServer);
				this.executionQueue.PersistToBase (transaction);
				transaction.Commit ();
			}
			
			//	Supprime la requ�te de la queue du serveur. En cas de perte de connexion
			//	ici, ce n'est pas autrement catastrophique : la requ�te locale est dans
			//	l'�tat ExecutedByServer et on va recevoir une nouvelle notification de
			//	la part du serveur (-> ExecutedByServer) et repasser par ici...
			
			RequestState[] states = new RequestState[] { server_state };
			
			this.service.RemoveRequestStates (this.client, states);
		}
		
		void SwitchToConflictingOnServer(System.Data.DataRow row)
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database))
			{
				this.executionQueue.SetRequestExecutionState (row, ExecutionState.ConflictingOnServer);
				this.executionQueue.PersistToBase (transaction);
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
		
		void SwitchToConflictingLocally(System.Data.DataRow row)
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database))
			{
				this.executionQueue.SetRequestExecutionState (row, ExecutionState.Conflicting);
				this.executionQueue.PersistToBase (transaction);
				transaction.Commit ();
			}
		}
		
		
		void ChangeToState(OrchestratorState newState)
		{
			if (this.state != newState)
			{
				OrchestratorState oldState = this.state;
				
				this.state = newState;
				this.OnStateChanged (new DependencyPropertyChangedEventArgs ("State", oldState, newState));
			}
		}
		
		
		void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.isThreadAbortRequested = true;

				this.abortEvent.Set ();
				this.workerThread.Join ();
				this.waiterThread.Join ();

				this.abortEvent.Close ();
				this.serverEvent.Close ();
				
				this.database.Dispose ();
				
				System.Diagnostics.Debug.WriteLine ("Disposed Orchestrator.");
			}
		}
		
		
		void OnStateChanged(DependencyPropertyChangedEventArgs e)
		{
			if (this.StateChanged != null)
			{
				this.StateChanged (this, e);
			}
		}
		
		void OnRequestExecuted(DbId request_id)
		{
			if (this.RequestExecuted != null)
			{
				this.RequestExecuted (this, request_id);
			}
		}
		
		
		public event EventHandler<DependencyPropertyChangedEventArgs>	StateChanged;
		public event System.Action<Orchestrator, DbId>					RequestExecuted;

		readonly object							exclusion = new object ();
		
		readonly DbInfrastructure				infrastructure;
		readonly ExecutionQueue					executionQueue;
		readonly ExecutionEngine				executionEngine;
		readonly IDbAbstraction					database;

		OrchestratorState						state;
		volatile RequestState[]					serverRequestStates;
		volatile bool							isThreadAbortRequested;
		
		readonly Thread							workerThread;
		readonly Thread							waiterThread;
		readonly ManualResetEvent				abortEvent;
		readonly AutoResetEvent					serverEvent;
		
		IRequestExecutionService				service;
		ClientIdentity							client;
	}
}
