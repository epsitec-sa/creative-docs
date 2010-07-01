//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			
			this.abortEvent   = new ManualResetEvent (false);
			this.serverEvent  = new AutoResetEvent (false);
			this.workerThread = new Thread (this.WorkerThread);
			this.waiterThread = new Thread (this.WaiterThread);

			this.workerThread.Name = "Requests.Orchestrator worker";
			this.waiterThread.Name = "Requests.Orchestrator waiter";
			
			this.workerThread.Start ();
		}


		/// <summary>
		/// Gets the execution queue.
		/// </summary>
		/// <value>The execution queue.</value>
		public ExecutionQueue ExecutionQueue
		{
			get
			{
				return this.executionQueue;
			}
		}


		/// <summary>
		/// Gets the execution engine.
		/// </summary>
		/// <value>The execution engine.</value>
		public ExecutionEngine ExecutionEngine
		{
			get
			{
				return this.executionEngine;
			}
		}


		/// <summary>
		/// Gets the database infrastructure.
		/// </summary>
		/// <value>The database infrastructure.</value>
		public DbInfrastructure Infrastructure
		{
			get
			{
				return this.infrastructure;
			}
		}


		/// <summary>
		/// Gets the database abstraction.
		/// </summary>
		/// <value>The database abstraction.</value>
		public IDbAbstraction Database
		{
			get
			{
				return this.database;
			}
		}


		/// <summary>
		/// Gets the remoting service.
		/// </summary>
		/// <value>The remoting service.</value>
		public IRequestExecutionService RemotingService
		{
			get
			{
				return this.service;
			}
		}


		/// <summary>
		/// Gets the client identity.
		/// </summary>
		/// <value>The client identity.</value>
		public ClientIdentity ClientIdentity
		{
			get
			{
				return this.client;
			}
		}


		/// <summary>
		/// Gets the orchestrator state.
		/// </summary>
		/// <value>The orchestrator state.</value>
		public OrchestratorState State
		{
			get
			{
				return this.state;
			}
		}


		/// <summary>
		/// Defines the remoting service; this method gets called by the
		/// <see cref="Epsitec.Cresus.Services.Engine"/> class.
		/// </summary>
		/// <param name="service">The request execution service.</param>
		/// <param name="client">The client identity.</param>
		public void DefineRemotingService(IRequestExecutionService service, ClientIdentity client)
		{
			//	The service can be of two types :
			//
			//	(1) local, accessed through an AppDomain crossing .NET remoting proxy;
			//		this is the case when the orchestrator is running inside the server,
			//		where the remoting service is locally available.
			//	
			//	(2) remote, accessed through a WCF proxy; this is the case when the
			//		orchestrator is running inside a client and needs to access the
			//		server through a network interface.

			System.Diagnostics.Debug.Assert (this.waiterThread.IsAlive == false);
			System.Diagnostics.Debug.Assert (this.service == null);
			
			this.service = service;
			this.client  = client;

			System.Diagnostics.Debug.Assert (this.service != null);

			this.waiterThread.Start ();
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
		/// service, i.e. on the server request queue; whenever requests arrive, their
		/// state gets recorded and the worker thread is started.
		/// </summary>
		private void WaiterThread()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine ("Requests.Orchestrator Waiter Thread launched.");

				int changeId = -1;
				
				while (!this.isThreadAbortRequested)
				{
					RequestState[] states;
					
					//	Load the states of the requests on the server queue; block until something
					//	changes or we time out.
					
					int newChangeId = this.service.QueryRequestStatesUsingFilter (this.client, changeId, out states);

					if ((changeId != newChangeId) &&
						(!this.isThreadAbortRequested))
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


		private static class WaitEvents
		{
			public const int QueueChanged = 0;
			public const int StateChanged = 1;
			public const int ServerChanged = 2;
			public const int Abort = 3;
		}

		/// <summary>
		/// The worker thread walks through all pending requests and executes them
		/// sequentially.
		/// </summary>
		private void WorkerThread()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine ("Requests.Orchestrator Worker Thread launched.");

				this.executionQueue.SetOrchestratorWorkerThread (Thread.CurrentThread);
				
				WaitHandle[] waitEvents = new WaitHandle[4];
				
				waitEvents[WaitEvents.QueueChanged]  = this.executionQueue.QueueChangedWaitEvent;
				waitEvents[WaitEvents.StateChanged]  = this.executionQueue.ExecutionStateWaitEvent;
				waitEvents[WaitEvents.ServerChanged] = this.serverEvent;
				waitEvents[WaitEvents.Abort]         = this.abortEvent;
				
				while (!this.isThreadAbortRequested)
				{
					this.ChangeToState (this.executionQueue.HasConflicting ? OrchestratorState.Conflicting : OrchestratorState.Ready);
					
					int handleIndex = Common.Support.Sync.Wait (waitEvents);
					
					if (handleIndex < 0 || handleIndex >= WaitEvents.Abort)
					{
						//	The event does not belong to those which require work; this means
						//	we have to abort the worker thread.
						
						break;
					}
					
					if (handleIndex == WaitEvents.ServerChanged)
					{
						//	The states of the requests on the server have changed; process all
						//	changes found on the server and copies them into the local queue.
						
						this.ChangeToState (OrchestratorState.Processing);
						this.UpdateLocalRequestsBasedOnServerStates ();
					}
					
					//	Process the contents of the local queue too :
					
					if (this.executionQueue.HasConflicting)
					{
						//	As long as there is a conflict in the local queue, we cannot make any
						//	kind of progress; we have to wait until the conflict has been resolved.

						continue;
					}
					else
					{
						//	There might be requests ready in the local queue :
						
						this.ChangeToState (OrchestratorState.Processing);

						if ((this.executionQueue.HasConflictResolved) ||
							(this.executionQueue.HasPending))
						{
							this.ProcessRequestsReadyInLocalQueue ();
						}
						else
						{
							//	If we have a connection to the server, we have to synchronize the
							//	queues (unless we are the server), which means we will try to send
							//	the requests to the server :

							if ((this.service != null) &&
								(this.executionQueue.IsRunningAsClient) &&
								(this.executionQueue.HasExecutedByClient))
							{
								this.SendLocalRequestsToServer ();
							}
						}
					}
				}

				this.ProcessShutdown ();
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


		/// <summary>
		/// Processes the requests which are ready in the local queue.
		/// </summary>
		void ProcessRequestsReadyInLocalQueue()
		{
			List<System.Data.DataRow> rows = new List<System.Data.DataRow> (DbRichCommand.GetLiveRows (this.executionQueue.GetDateTimeSortedRows ()));
			
			int n = rows.Count;
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Queue contains {0} {1}.", n, (n == 1) ? "request" : "requests"));
			
			for (int i = 0; i < n; i++)
			{
				if (this.executionQueue.HasConflicting)
				{
					//	Stop as soon as we detect a conflict in the request queue !
					
					break;
				}
				
				System.Data.DataRow row = rows[i];
				
				if (DbRichCommand.IsRowDeleted (row))
				{
					//	Skip rows which are marked as deleted.

					continue;
				}
				
				ExecutionState state = this.executionQueue.GetRequestExecutionState (row);
				
#if DEBUG
				System.DateTime time = (System.DateTime) row[Tags.ColumnDateTime];
				string formattedTime = string.Format (System.Globalization.CultureInfo.InvariantCulture, "{0:00}:{1:00}:{2:00}.{3:000}", time.Hour, time.Minute, time.Second, time.Millisecond);
				System.Diagnostics.Debug.WriteLine (string.Format (" {0} --> {1} - {2}", i, state, formattedTime));
#endif
				
				switch (state)
				{
					case ExecutionState.Pending:
					case ExecutionState.ConflictResolved:
						this.ProcessLocalRequest (row);
						break;
					
					default:
						break;
				}
			}
		}


		/// <summary>
		/// Processes one local request, based on its serialized representation.
		/// </summary>
		/// <param name="row">The row containing the local request.</param>
		void ProcessLocalRequest(System.Data.DataRow row)
		{
			AbstractRequest request = this.executionQueue.GetRequest (row);

			//	The request has either the Pending or ConflictResolved state.
			//	Its execution will change its state to either ExecutedByClient or Conflicting.
			
			DbKey requestKey = new DbKey (row);
			DbId  requestId  = requestKey.Id;

			bool conflictDetected = false;

			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database))
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Processing request ({0}).", request.GetType ().Name));

				try
				{
					this.executionEngine.Execute (transaction, request);

					this.SwitchToExecutedByClient (row, transaction);

					transaction.Commit ();
				}
				catch (System.Exception exception)
				{
					//	TODO: don't swallow all exceptions here, but just the conflicting !
					
					System.Diagnostics.Debug.WriteLine (exception.Message);
					conflictDetected = true;
				}
			}
			
			this.OnRequestExecuted (requestId);
			
			if (conflictDetected)
			{
				this.HandleLocalConflictDetected (row, request);
			}
		}

		/// <summary>
		/// Handle a conflict detected while executing the specified request.
		/// </summary>
		/// <param name="row">The row containing the local request.</param>
		/// <param name="request">The request.</param>
		void HandleLocalConflictDetected(System.Data.DataRow row, AbstractRequest request)
		{
			this.SwitchToConflictingLocally (row);
		}

		
		/// <summary>
		/// Sends the local requests to server and mark them all as <c>SentToServer</c>.
		/// If some requests are in conflict, doesn't do anything.
		/// </summary>
		void SendLocalRequestsToServer()
		{
			List<System.Data.DataRow> rows = new List<System.Data.DataRow> (DbRichCommand.GetLiveRows (this.executionQueue.GetDateTimeSortedRows ()));
			
			foreach (var row in rows)
			{
				if (this.executionQueue.HasConflicting)
				{
					break;
				}
				
				if (DbRichCommand.IsRowDeleted (row))
				{
					continue;
				}
				
				ExecutionState state = this.executionQueue.GetRequestExecutionState (row);
				
				switch (state)
				{
					case ExecutionState.ExecutedByClient:
						this.SendLocalRequestToServer (row);
						break;
					
					default:
						break;
				}
			}
		}

		/// <summary>
		/// Sends the local request to the server and mark the request as <c>SentToServer</c>.
		/// </summary>
		/// <param name="row">The row for the specified request.</param>
		void SendLocalRequestToServer(System.Data.DataRow row)
		{
			AbstractRequest request = this.executionQueue.GetRequest (row);
			
			DbKey  requestKey = new DbKey (row);
			DbId   requestId  = requestKey.Id;
			byte[] serialized = Epsitec.Common.IO.Serialization.SerializeToMemory (request);
			
			//	Send the request to the server :

			this.service.EnqueueRequest (this.client, new SerializedRequest[] { new Remoting.SerializedRequest (requestId, serialized) });
			
			//	Remember that this request was sent to the server; if we happen to crash
			//	before we commit the transaction, we will send the request twice to the
			//	server, but this is taken care of on the server queue :

			this.SwitchToSentToServer (row);
		}



		/// <summary>
		/// Updates the local requests based on the server states. The request
		/// states in the execution queue will be changed.
		/// </summary>
		void UpdateLocalRequestsBasedOnServerStates()
		{
			RequestState[] states;

			lock (this.exclusion)
			{
				states = (RequestState[]) this.serverRequestStates.Clone ();
			}

			List<RequestState>    deadRequests = new List<RequestState> ();
			System.Data.DataRow[] rows         = this.executionQueue.GetDateTimeSortedRows ();
			
			foreach (RequestState requestState in states)
			{
				System.Data.DataRow row = DbRichCommand.FindRow (this.executionQueue.QueueDataTable, rows, requestState.RequestId);
				
				if (row == null)
				{
					//	The request cannot be found in our local execution queue; this means that
					//	the server still references a dead request :

					System.Diagnostics.Debug.WriteLine (string.Format ("Warning: server references dead request {0} !", requestState.RequestId));

					deadRequests.Add (requestState);
					continue;
				}
				
				//	The request exists in our local execution queue.

				ExecutionState remoteState = ExecutionQueue.ConvertToExecutionState (requestState.State);
				ExecutionState localState  = this.executionQueue.GetRequestExecutionState (row);
				
				if (localState == ExecutionState.ExecutedByClient)
				{
					//	We never sent the request to the server (yet, there it is, which most probably
					//	means that we crashed in SendLocalRequestToServer, before being able to mark
					//	the request as sent); skip the request -- it will be sent again later.

					continue;
				}
				
				if ((localState != ExecutionState.SentToServer) &&
					(localState != ExecutionState.ExecutedByServer) &&
					(localState != ExecutionState.ConflictingOnServer))
				{
					//	The state of the request in the local queue is not valide; we support only
					//	one of the following valid states :
					//	
					//	(1) SentToServeur, the request was successfully sent to the server.
					//
					//	(2) ExecutedByServer, the client was restarted before the request
					//		could be removed from the server and client queues.
					//
					//	(3) ConflictingOnServer, the client was restarted before the request
					//		could be removed from the server and before the local queue could
					//		be updated to Conflicting.

					System.Diagnostics.Debug.WriteLine (string.Format ("Warning: request {0} local state is {1}; should be SentToServer.", requestState.RequestId, localState));
				}
				
				switch (remoteState)
				{
					case ExecutionState.Pending:
						//	Server has not processed the request yet.
						break;
					
					case ExecutionState.ExecutedByServer:
						//	Server has successfully processed the request. Remove the request
						//	from the server and mark the local request as executed.

						this.SwitchToExecutedByServer (requestState, row);
						break;
					
					case ExecutionState.ConflictingOnServer:
						//	Server has detected a conflict on the request. Remove all requests
						//	still known on the server and mark the local request as conflicting
						//	on the server.

						this.SwitchToConflictingOnServer (row);
						break;
					
					default:
						throw new System.InvalidOperationException (string.Format ("Request ExecutionState set to {0} on server.", remoteState));
				}
			}

			this.CleanUpDeadRequests (deadRequests);
			this.CleanUpConflictingOnServer (rows);
			this.CleanUpExecutedByServer (rows);
		}

		/// <summary>
		/// Cleans up dead requests.
		/// </summary>
		/// <param name="deadRequests">The dead requests.</param>
		void CleanUpDeadRequests(List<RequestState> deadRequests)
		{
			if (deadRequests.Count > 0)
			{
				this.service.RemoveRequestStates (this.client, deadRequests.ToArray ());
			}

		}

		/// <summary>
		/// Cleans up requests locally marked as conflicting on server.
		/// </summary>
		/// <param name="rows">The rows for the local requests.</param>
		void CleanUpConflictingOnServer(System.Data.DataRow[] rows)
		{
			if (this.executionQueue.HasConflictingOnServer)
			{
				//	We should never reach the HasConflictingOnServer state here,
				//	unless there was a problem while executing method SwitchTo-
				//	ConflictingOnServer.

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
		}

		/// <summary>
		/// Cleans up requests locally marked as executed by server.
		/// </summary>
		/// <param name="rows">The rows for the local requests.</param>
		void CleanUpExecutedByServer(System.Data.DataRow[] rows)
		{
			if (this.executionQueue.HasExecutedByServer)
			{
				//	The local queue contains requests marked as executed on the server;
				//	we no longer need to keep track of them.

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


		/// <summary>
		/// Prepares for shutdown by persisting the contents of the execution queue
		/// to the database.
		/// </summary>
		void ProcessShutdown()
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("Preparing Orchestrator for shutdown."));
			
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database))
			{
				this.executionQueue.PersistToBase (transaction);
				transaction.Commit ();
			}
		}


		/// <summary>
		/// Switches the local request state to <c>ExecutedByServer</c> and removes the
		/// matching state from the server.
		/// </summary>
		/// <param name="serverState">The server state of the request.</param>
		/// <param name="row">The row containg the local request.</param>
		void SwitchToExecutedByServer(RequestState serverState, System.Data.DataRow row)
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database))
			{
				this.executionQueue.SetRequestExecutionState (row, ExecutionState.ExecutedByServer);
				this.executionQueue.PersistToBase (transaction);
				transaction.Commit ();
			}
			
			//	Remove the request from the server queue; if we crash here, this is not
			//	catastrophic, as we will handle a future ExecutedByServer notification
			//	and be called again.
			
			RequestState[] states = new RequestState[] { serverState };
			
			this.service.RemoveRequestStates (this.client, states);
		}

		/// <summary>
		/// Switches the local request state to <c>ConflictingOnServer</c>, removes
		/// all requests from this client on the server and the finally switch to
		/// the final <c>Conflicting</c> state.
		/// </summary>
		/// <param name="row">The row containing the local request.</param>
		void SwitchToConflictingOnServer(System.Data.DataRow row)
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database))
			{
				this.executionQueue.SetRequestExecutionState (row, ExecutionState.ConflictingOnServer);
				this.executionQueue.PersistToBase (transaction);
				transaction.Commit ();
			}

			//	Remove the requests from the server queue; if we crash here, this is not
			//	catastrophic, as we will handle a future ConflictingOnServer notification
			//	and be called again.
			
			this.service.RemoveAllRequestStates (this.client);
			
			//	The local request can now be marked as Conflicting...
			
			//	-------------------------------------------------------------------------
			//	TODO: demander une réplication "Pull" pour se remettre dans l'état avant
			//	l'exécution de la requête en local.
			//
			//	Il faut :
			//	
			//	  - Trouver toutes les lignes de toutes les tables qui ont été modifiées
			//		depuis l'exécution en local de la requête (cf ReplicationTest et
			//		DataCruncher.ExtractRowSetsUsingLogId)
			//
			//	  - Supprimer toutes ces lignes. Le code pour une suppression multiple
			//		doit encore être écrit (on peut utiliser DbSelectCondition).
			//
			//	  - Demander la réplication via PullReplication, en spécifiant à la fois
			//		la fourchette à répliquer (LOG_ID de .. à ..) et la liste des CR_ID.
			//	
			//	-------------------------------------------------------------------------
			
			//	If we here, we will still have a ConflictingOnServer state which will
			//	just cause a new call to SwitchToConflictingOnServer.
			
			this.SwitchToConflictingLocally (row);
		}

		/// <summary>
		/// Switches the local request state to <c>Conflicting</c>.
		/// </summary>
		/// <param name="row">The row containing the local request.</param>
		void SwitchToConflictingLocally(System.Data.DataRow row)
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database))
			{
				this.executionQueue.SetRequestExecutionState (row, ExecutionState.Conflicting);
				this.executionQueue.PersistToBase (transaction);
				transaction.Commit ();
			}
		}

		/// <summary>
		/// Switches the local request state to <c>SentToServer</c>.
		/// </summary>
		/// <param name="row">The row containing the local request.</param>
		void SwitchToSentToServer(System.Data.DataRow row)
		{
			using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, this.database))
			{
				this.executionQueue.SetRequestExecutionState (row, ExecutionState.SentToServer);
				this.executionQueue.PersistToBase (transaction);
				transaction.Commit ();
			}
		}

		/// <summary>
		/// Switches the local request state to <c>ExecutedByClient</c>.
		/// </summary>
		/// <param name="row">The row containing the local request.</param>
		/// <param name="transaction">The active transaction.</param>
		void SwitchToExecutedByClient(System.Data.DataRow row, DbTransaction transaction)
		{
			this.executionQueue.SetRequestExecutionState (row, ExecutionState.ExecutedByClient);
			this.executionQueue.PersistToBase (transaction);
		}
		


		/// <summary>
		/// Changes the orchestrator to the specified state.
		/// </summary>
		/// <param name="newState">The new state.</param>
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
			if ((disposing) &&
				(!this.isThreadAbortRequested))
			{
				this.isThreadAbortRequested = true;
				this.abortEvent.Set ();

				if (this.service != null)
				{
					this.service.WakeUpQueryRequestStatesUsingFilter (this.client);
				}

				if (this.workerThread.IsAlive)
				{
					this.workerThread.Join ();
				}
				
				if (this.waiterThread.IsAlive)
				{
					this.waiterThread.Join ();
				}

				this.abortEvent.Close ();
				this.serverEvent.Close ();

				this.executionQueue.Dispose ();
				this.executionEngine.Dispose ();
				
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
		
		void OnRequestExecuted(DbId requestId)
		{
			if (this.RequestExecuted != null)
			{
				this.RequestExecuted (this, requestId);
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
