//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Remoting;
using Epsitec.Cresus.Requests;

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// La classe RequestExecutionEngine implémente un service de réception de
	/// requêtes via le réseau.
	/// </summary>
	internal sealed class RequestExecutionEngine : AbstractServiceEngine, IRequestExecutionService
	{
		public RequestExecutionEngine(Engine engine) : base (engine, "RequestExecution")
		{
			this.orchestrator    = this.engine.Orchestrator;
			this.execution_queue = this.orchestrator.ExecutionQueue;
			this.client_changes  = new System.Collections.Hashtable ();
			
			this.orchestrator.RequestExecuted += new RequestExecutedCallback (this.HandleOrchestratorRequestExecuted);
			
			System.Diagnostics.Debug.Assert (this.execution_queue.IsRunningAsServer);
		}
		
		
		#region IRequestExecutionService Members
		void IRequestExecutionService.EnqueueRequest(ClientIdentity client, SerializedRequest[] requests)
		{
			//	Place une série de requêtes dans la queue.
			
			int n = requests.Length;
			
			byte[][]        data = new byte[n][];
			Database.DbId[] ids  = new Database.DbId[n];
			
			for (int i = 0; i < requests.Length; i++)
			{
				data[i] = requests[i].Data;
				ids[i]  = new Database.DbId (requests[i].Identifier);
			}
			
			//	Vérifie que tous les IDs proviennent bien du même client. C'est un test
			//	de plausibilité, pour voir si tout s'est bien passé...
			
			if (n > 1)
			{
				int client_id = ids[0].ClientId;
				
				for (int i = 1; i < n; i++)
				{
					if (ids[i].ClientId != client_id)
					{
						throw new System.InvalidOperationException (string.Format ("Request {0} has ID {1}/{2}; client ID should be {3}.", i, ids[i].ClientId, ids[i].LocalId, client_id));
					}
				}
			}
			
			this.execution_queue.Enqueue (data, ids);
		}
		
		void IRequestExecutionService.QueryRequestStates(Remoting.ClientIdentity client, out RequestState[] states)
		{
			this.InternalQueryRequestStates (client, out states);
		}
		
		void IRequestExecutionService.QueryRequestStates(ClientIdentity client, ref int change_id, System.TimeSpan timeout, out RequestState[] states)
		{
			//	Retourne les informations sur les états uniquement en cas de changement
			//	ou si le temps imparti est écoulé.
			
			//	De manière interne, le serveur conserve une table qui fait le lien entre
			//	chaque client avec lequel il a été en contact et le compteur de changement
			//	associé :
			
			ClientChangeInfo info = this.GetClientChangeInfo (client.ClientId);
			
			//	Attend jusqu'à ce que l'état soit différent de 'change_id' (ou que le temps
			//	imparti soit écoulé) :
			
			info.WaitChange (change_id, timeout);
			
			//	L'appelant va être informé de la nouvelle valeur du compteur de changements.
			//	Il faut considérer ce compteur comme une valeur "opaque"; il n'a pas de sens
			//	à l'extérieur du serveur !
			
			change_id = info.ChangeId;
			
			this.InternalQueryRequestStates (client, out states);
		}
		
		void IRequestExecutionService.RemoveRequestStates(ClientIdentity client, RequestState[] states)
		{
			//	Supprime de la queue les requêtes dont l'état correspond à celui
			//	décrit.
			
			lock (this.execution_queue)
			{
				System.Collections.ArrayList list = new System.Collections.ArrayList ();
				System.Data.DataRow[]        rows = this.execution_queue.DateTimeSortedRows;
				
				for (int i = 0; i < rows.Length; i++)
				{
					Database.DbKey row_key   = new Database.DbKey (rows[i]);
					ExecutionState row_state = this.execution_queue.GetRequestExecutionState (rows[i]);
						
					//	Comme l'exécution est faite sur le serveur, il faut ajuster l'état d'exécution
					//	de manière à refléter la réalité :
					
					switch (row_state)
					{
						case Requests.ExecutionState.ExecutedByClient:
							row_state = Requests.ExecutionState.ExecutedByServer;
							break;
						
						case Requests.ExecutionState.Conflicting:
							row_state = Requests.ExecutionState.ConflictingOnServer;
							break;
					}
					
					for (int j = 0; j < states.Length; j++)
					{
						if ((states[j].Identifier == row_key.Id.Value) &&
							(states[j].State == (int)row_state))
						{
							list.Add (rows[i]);
							
							//	Si c'était le seul élément restant à supprimer de la table, on s'arrête
							//	tout de suite.
							
							if (states.Length == 1)
							{
								goto end;
							}
							
							//	Retire l'élément que l'on vient de supprimer de la liste des éléments
							//	à supprimer :
							
							RequestState[] copy = new RequestState[states.Length-1];
							
							System.Array.Copy (states, 0, copy, 0, j);
							System.Array.Copy (states, j+1, copy, j, states.Length-j-1);
							
							states = copy;
							
							break;
						}
					}
				}
			end:
				if (list.Count > 0)
				{
					this.execution_queue.RemoveRequests (list);
				}
			}
		}
		
		void IRequestExecutionService.RemoveAllRequestStates(ClientIdentity client)
		{
			//	TODO: supprimer toutes les requêtes de ce client
		}
		#endregion
		
		private void InternalQueryRequestStates(Remoting.ClientIdentity client, out RequestState[] states)
		{
			//	Détermine l'état de toutes les requêtes soumises par le client
			//	spécifié.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			lock (this.execution_queue)
			{
				System.Data.DataRow[] rows = this.execution_queue.DateTimeSortedRows;
				
				for (int i = 0; i < rows.Length; i++)
				{
					Database.DbKey row_key = new Database.DbKey (rows[i]);
					
					if (row_key.Id.ClientId == client.ClientId)
					{
						Requests.ExecutionState state = this.execution_queue.GetRequestExecutionState (rows[i]);
						
						//	Comme l'exécution a été faite sur le serveur, il faut ajuster l'état d'exécution
						//	de manière à refléter la réalité :
						
						switch (state)
						{
							case Requests.ExecutionState.ExecutedByClient:
								state = Requests.ExecutionState.ExecutedByServer;
								break;
							
							case Requests.ExecutionState.Conflicting:
								state = Requests.ExecutionState.ConflictingOnServer;
								break;
						}
						
						list.Add (new RequestState (row_key.Id.Value, (int) state));
					}
				}
			}
			
			states = new RequestState[list.Count];
			list.CopyTo (states);
		}
		
		private ClientChangeInfo GetClientChangeInfo(int client_id)
		{
			ClientChangeInfo info;
			
			lock (this.client_changes)
			{
				if (this.client_changes.Contains (client_id))
				{
					info = this.client_changes[client_id] as ClientChangeInfo;
				}
				else
				{
					info = new ClientChangeInfo (client_id);
					this.client_changes[client_id] = info;
				}
			}
			
			return info;
		}
		
		
		private void HandleOrchestratorRequestExecuted(Orchestrator sender, Database.DbId request_id)
		{
			//	Une requête vient d'être exécutée par l'orchestrateur. Si un client
			//	est en attente de modifications d'état de ses requêtes, il faut le
			//	réveiller.
			
			ClientChangeInfo info = this.GetClientChangeInfo (request_id.ClientId);
			
			info.NotifyChange ();
		}
		
		
		#region ClientChangeInfo Class
		private class ClientChangeInfo
		{
			public ClientChangeInfo(int client_id)
			{
				this.client_id = client_id;
				this.change_id = 0;
				this.monitor   = new object ();
			}
			
			
			public int							ClientId
			{
				get
				{
					return this.client_id;
				}
			}
			
			public int							ChangeId
			{
				get
				{
					return this.change_id;
				}
			}
			
			
			public void NotifyChange()
			{
				lock (this.monitor)
				{
					int change = this.change_id + 1;
					
					if (change > 999999999)
					{
						change = 1;
					}
					
					this.change_id = change;
					
					System.Threading.Monitor.PulseAll (this.monitor);
				}
			}
			
			public void WaitChange(int change_id, System.TimeSpan timeout)
			{
				if (change_id != this.change_id)
				{
					return;
				}
				
				bool infinite = (timeout.Ticks < 0);
				
				System.DateTime start_time = System.DateTime.Now;
				System.DateTime stop_time  = start_time.Add (timeout);
				
				for (;;)
				{
					bool      got_event = false;
					const int max_wait  = 30*1000;
					
					lock (this.monitor)
					{
						if (change_id != this.change_id)
						{
							return;
						}
						if (infinite)
						{
							got_event = System.Threading.Monitor.Wait (this.monitor, max_wait);
						}
						else
						{
							timeout = System.TimeSpan.FromTicks (System.Math.Min (stop_time.Ticks - System.DateTime.Now.Ticks, max_wait*10*1000L));
							
							if (timeout.Ticks > 0)
							{
								got_event = System.Threading.Monitor.Wait (this.monitor, timeout);
							}
						}
					}
					
					if (got_event == false)
					{
						break;
					}
				}
			}
			
			
			private int							client_id;
			private int							change_id;
			private object						monitor;
		}
		#endregion
		
		private Orchestrator					orchestrator;
		private ExecutionQueue					execution_queue;
		private System.Collections.Hashtable	client_changes;
	}
}
