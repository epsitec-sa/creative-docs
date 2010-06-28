//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;
using Epsitec.Cresus.Requests;
using System.Collections.Generic;

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// La classe RequestExecutionEngine implémente un service de réception de
	/// requêtes via le réseau.
	/// </summary>
	internal sealed class RequestExecutionEngine : AbstractServiceEngine, IRequestExecutionService
	{
		public RequestExecutionEngine(Engine engine)
			: base (engine)
		{
			this.orchestrator    = this.Engine.Orchestrator;
			this.executionQueue = this.orchestrator.ExecutionQueue;
			this.clientChanges  = new System.Collections.Hashtable ();

			this.orchestrator.RequestExecuted += this.HandleOrchestratorRequestExecuted;

			System.Diagnostics.Debug.Assert (this.executionQueue.IsRunningAsServer);
		}


		public override System.Guid GetServiceId()
		{
			return RemotingServices.RequestExecutionServiceId;
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
				ids[i]  = new Database.DbId (requests[i].RequestId);
			}
			
			//	Vérifie que tous les IDs proviennent bien du même client. C'est un test
			//	de plausibilité, pour voir si tout s'est bien passé...
			
			if (n > 1)
			{
				int clientId = ids[0].ClientId;
				
				for (int i = 1; i < n; i++)
				{
					if (ids[i].ClientId != clientId)
					{
						throw new System.InvalidOperationException (string.Format ("Request {0} has ID {1}/{2}; client ID should be {3}.", i, ids[i].ClientId, ids[i].LocalId, clientId));
					}
				}
			}
			
			this.executionQueue.Enqueue (null, data, ids);
		}
		
		RequestState[] IRequestExecutionService.QueryRequestStates(Remoting.ClientIdentity client)
		{
			return this.InternalQueryRequestStates (client);
		}
		
		int IRequestExecutionService.QueryRequestStatesUsingFilter(ClientIdentity client, int changeId, out RequestState[] states)
		{
			//	Retourne les informations sur les états uniquement en cas de changement
			//	ou si le temps imparti est écoulé.
			
			//	De manière interne, le serveur conserve une table qui fait le lien entre
			//	chaque client avec lequel il a été en contact et le compteur de changement
			//	associé :
			
			ClientChangeInfo info = this.GetClientChangeInfo (client.Id);
			
			//	Attend jusqu'à ce que l'état soit différent de 'change_id' (ou que le temps
			//	imparti soit écoulé) :
			
			info.WaitChange (changeId);
			
			//	L'appelant va être informé de la nouvelle valeur du compteur de changements.
			//	Il faut considérer ce compteur comme une valeur "opaque"; il n'a pas de sens
			//	à l'extérieur du serveur !
			
			changeId = info.ChangeId;
			
			states = this.InternalQueryRequestStates (client);
			
			return changeId;
		}

		void IRequestExecutionService.WakeUpQueryRequestStatesUsingFilter(ClientIdentity client)
		{
			ClientChangeInfo info = this.GetClientChangeInfo (client.Id);

			info.WakeUp ();
		}
		
		void IRequestExecutionService.RemoveRequestStates(ClientIdentity client, RequestState[] states)
		{
			//	Supprime de la queue les requêtes dont l'état correspond à celui
			//	décrit.
			
			lock (this.executionQueue)
			{
				List<System.Data.DataRow> list = new List<System.Data.DataRow> ();
				System.Data.DataRow[]     rows = this.executionQueue.GetDateTimeSortedRows ();
				
				System.Diagnostics.Debug.WriteLine ("RemoveRequestStates: ");
				
				for (int i = 0; i < states.Length; i++)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("  {0}: {1} in state {2}", i, states[i].RequestId, (Requests.ExecutionState)states[i].State));
				}
				
				for (int i = 0; i < rows.Length; i++)
				{
					Database.DbKey rowKey   = new Database.DbKey (rows[i]);
					ExecutionState rowState = this.executionQueue.GetRequestExecutionState (rows[i]);
						
					//	Comme l'exécution est faite sur le serveur, il faut ajuster l'état d'exécution
					//	de manière à refléter la réalité :
					
					switch (rowState)
					{
						case Requests.ExecutionState.ExecutedByClient:
							rowState = Requests.ExecutionState.ExecutedByServer;
							break;
						
						case Requests.ExecutionState.Conflicting:
							rowState = Requests.ExecutionState.ConflictingOnServer;
							break;
					}
					
					for (int j = 0; j < states.Length; j++)
					{
						if ((states[j].RequestId == rowKey.Id.Value) &&
							(states[j].State == (int)rowState))
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
					this.executionQueue.RemoveRequestRows (list);
				}
			}
		}
		
		void IRequestExecutionService.RemoveAllRequestStates(ClientIdentity client)
		{
			lock (this.executionQueue)
			{
				List<System.Data.DataRow> list = new List<System.Data.DataRow> ();
				System.Data.DataRow[]     rows = this.executionQueue.GetDateTimeSortedRows ();
				
				System.Diagnostics.Debug.WriteLine ("RemoveAllRequestStates for client " + client.ToString ());
				
				for (int i = 0; i < rows.Length; i++)
				{
					Database.DbKey row_key   = new Database.DbKey (rows[i]);
					ExecutionState row_state = this.executionQueue.GetRequestExecutionState (rows[i]);
					
					if (row_key.Id.ClientId == client.Id)
					{
						System.Diagnostics.Debug.WriteLine (string.Format (" - {0} in state {1}", row_key.Id.Value, row_state));
						list.Add (rows[i]);
					}
				}
				if (list.Count > 0)
				{
					this.executionQueue.RemoveRequestRows (list);
				}
			}
		}
		#endregion

		private RequestState[] InternalQueryRequestStates(Remoting.ClientIdentity client)
		{
			//	Détermine l'état de toutes les requêtes soumises par le client
			//	spécifié.

			List<RequestState> list = new List<RequestState> ();
			
			lock (this.executionQueue)
			{
				System.Data.DataRow[] rows = this.executionQueue.GetDateTimeSortedRows ();
				
				for (int i = 0; i < rows.Length; i++)
				{
					Database.DbKey rowKey = new Database.DbKey (rows[i]);
					
					if (rowKey.Id.ClientId == client.Id)
					{
						Requests.ExecutionState state = this.executionQueue.GetRequestExecutionState (rows[i]);
						
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
						
						list.Add (new RequestState (rowKey.Id.Value, (int) state));
					}
				}
			}

			return list.ToArray ();
		}
		
		private ClientChangeInfo GetClientChangeInfo(int clientId)
		{
			ClientChangeInfo info;
			
			lock (this.clientChanges)
			{
				if (this.clientChanges.Contains (clientId))
				{
					info = this.clientChanges[clientId] as ClientChangeInfo;
				}
				else
				{
					info = new ClientChangeInfo (clientId);
					this.clientChanges[clientId] = info;
				}
			}
			
			return info;
		}
		
		
		private void HandleOrchestratorRequestExecuted(Orchestrator sender, Database.DbId requestId)
		{
			//	Une requête vient d'être exécutée par l'orchestrateur. Si un client
			//	est en attente de modifications d'état de ses requêtes, il faut le
			//	réveiller.
			
			ClientChangeInfo info = this.GetClientChangeInfo (requestId.ClientId);
			
			info.NotifyChange ();
		}
		
		
		#region ClientChangeInfo Class
		private class ClientChangeInfo
		{
			public ClientChangeInfo(int clientId)
			{
				this.clientId = clientId;
				this.changeId = 0;
				this.monitor  = new object ();
			}
			
			
			public int							ClientId
			{
				get
				{
					return this.clientId;
				}
			}
			
			public int							ChangeId
			{
				get
				{
					return this.changeId;
				}
			}
			
			
			public void NotifyChange()
			{
				lock (this.monitor)
				{
					int change = this.changeId + 1;
					
					if (change > 999999999)
					{
						change = 1;
					}
					
					this.changeId = change;
					System.Threading.Monitor.PulseAll (this.monitor);
				}
			}

			public void WaitChange(int changeId)
			{
				if (changeId != this.changeId)
				{
					return;
				}
				
				lock (this.monitor)
				{
					if (changeId != this.changeId)
					{
						return;
					}
					
					System.Threading.Monitor.Wait (this.monitor);
				}
			}

			public void WakeUp()
			{
				lock (this.monitor)
				{
					System.Threading.Monitor.PulseAll (this.monitor);
				}
			}

			readonly object						monitor;
			readonly int						clientId;
			int									changeId;
		}
		#endregion
		
		private Orchestrator					orchestrator;
		private ExecutionQueue					executionQueue;
		private System.Collections.Hashtable	clientChanges;
	}
}
