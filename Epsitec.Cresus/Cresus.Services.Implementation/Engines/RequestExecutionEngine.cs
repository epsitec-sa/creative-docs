//	Copyright � 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;
using Epsitec.Cresus.Requests;
using System.Collections.Generic;

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// La classe RequestExecutionEngine impl�mente un service de r�ception de
	/// requ�tes via le r�seau.
	/// </summary>
	internal sealed class RequestExecutionEngine : AbstractServiceEngine, IRequestExecutionService
	{
		public RequestExecutionEngine(Engine engine)
			: base (engine)
		{
			this.orchestrator    = this.Engine.Orchestrator;
			this.execution_queue = this.orchestrator.ExecutionQueue;
			this.client_changes  = new System.Collections.Hashtable ();

			this.orchestrator.RequestExecuted += new RequestExecutedCallback (this.HandleOrchestratorRequestExecuted);

			System.Diagnostics.Debug.Assert (this.execution_queue.IsRunningAsServer);
		}


		public override System.Guid GetServiceId()
		{
			return RemotingServices.RequestExecutionServiceId;
		}
		
		#region IRequestExecutionService Members
		void IRequestExecutionService.EnqueueRequest(ClientIdentity client, SerializedRequest[] requests)
		{
			//	Place une s�rie de requ�tes dans la queue.
			
			int n = requests.Length;
			
			byte[][]        data = new byte[n][];
			Database.DbId[] ids  = new Database.DbId[n];
			
			for (int i = 0; i < requests.Length; i++)
			{
				data[i] = requests[i].Data;
				ids[i]  = new Database.DbId (requests[i].RequestId);
			}
			
			//	V�rifie que tous les IDs proviennent bien du m�me client. C'est un test
			//	de plausibilit�, pour voir si tout s'est bien pass�...
			
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
			
			this.execution_queue.Enqueue (null, data, ids);
		}
		
		RequestState[] IRequestExecutionService.QueryRequestStates(Remoting.ClientIdentity client)
		{
			return this.InternalQueryRequestStates (client);
		}
		
		int IRequestExecutionService.QueryRequestStatesUsingFilter(ClientIdentity client, int change_id, System.TimeSpan timeout, out RequestState[] states)
		{
			//	Retourne les informations sur les �tats uniquement en cas de changement
			//	ou si le temps imparti est �coul�.
			
			//	De mani�re interne, le serveur conserve une table qui fait le lien entre
			//	chaque client avec lequel il a �t� en contact et le compteur de changement
			//	associ� :
			
			ClientChangeInfo info = this.GetClientChangeInfo (client.Id);
			
			//	Attend jusqu'� ce que l'�tat soit diff�rent de 'change_id' (ou que le temps
			//	imparti soit �coul�) :
			
			info.WaitChange (change_id, timeout);
			
			//	L'appelant va �tre inform� de la nouvelle valeur du compteur de changements.
			//	Il faut consid�rer ce compteur comme une valeur "opaque"; il n'a pas de sens
			//	� l'ext�rieur du serveur !
			
			change_id = info.ChangeId;
			
			states = this.InternalQueryRequestStates (client);
			
			return change_id;
		}
		
		void IRequestExecutionService.RemoveRequestStates(ClientIdentity client, RequestState[] states)
		{
			//	Supprime de la queue les requ�tes dont l'�tat correspond � celui
			//	d�crit.
			
			lock (this.execution_queue)
			{
				List<System.Data.DataRow> list = new List<System.Data.DataRow> ();
				System.Data.DataRow[]     rows = this.execution_queue.DateTimeSortedRows;
				
				System.Diagnostics.Debug.WriteLine ("RemoveRequestStates: ");
				
				for (int i = 0; i < states.Length; i++)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("  {0}: {1} in state {2}", i, states[i].RequestId, (Requests.ExecutionState)states[i].State));
				}
				
				for (int i = 0; i < rows.Length; i++)
				{
					Database.DbKey row_key   = new Database.DbKey (rows[i]);
					ExecutionState row_state = this.execution_queue.GetRequestExecutionState (rows[i]);
						
					//	Comme l'ex�cution est faite sur le serveur, il faut ajuster l'�tat d'ex�cution
					//	de mani�re � refl�ter la r�alit� :
					
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
						if ((states[j].RequestId == row_key.Id.Value) &&
							(states[j].State == (int)row_state))
						{
							list.Add (rows[i]);
							
							//	Si c'�tait le seul �l�ment restant � supprimer de la table, on s'arr�te
							//	tout de suite.
							
							if (states.Length == 1)
							{
								goto end;
							}
							
							//	Retire l'�l�ment que l'on vient de supprimer de la liste des �l�ments
							//	� supprimer :
							
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
			lock (this.execution_queue)
			{
				List<System.Data.DataRow> list = new List<System.Data.DataRow> ();
				System.Data.DataRow[]     rows = this.execution_queue.DateTimeSortedRows;
				
				System.Diagnostics.Debug.WriteLine ("RemoveAllRequestStates for client " + client.ToString ());
				
				for (int i = 0; i < rows.Length; i++)
				{
					Database.DbKey row_key   = new Database.DbKey (rows[i]);
					ExecutionState row_state = this.execution_queue.GetRequestExecutionState (rows[i]);
					
					if (row_key.Id.ClientId == client.Id)
					{
						System.Diagnostics.Debug.WriteLine (string.Format (" - {0} in state {1}", row_key.Id.Value, row_state));
						list.Add (rows[i]);
					}
				}
				if (list.Count > 0)
				{
					this.execution_queue.RemoveRequests (list);
				}
			}
		}
		#endregion

		private RequestState[] InternalQueryRequestStates(Remoting.ClientIdentity client)
		{
			//	D�termine l'�tat de toutes les requ�tes soumises par le client
			//	sp�cifi�.

			List<RequestState> list = new List<RequestState> ();
			
			lock (this.execution_queue)
			{
				System.Data.DataRow[] rows = this.execution_queue.DateTimeSortedRows;
				
				for (int i = 0; i < rows.Length; i++)
				{
					Database.DbKey row_key = new Database.DbKey (rows[i]);
					
					if (row_key.Id.ClientId == client.Id)
					{
						Requests.ExecutionState state = this.execution_queue.GetRequestExecutionState (rows[i]);
						
						//	Comme l'ex�cution a �t� faite sur le serveur, il faut ajuster l'�tat d'ex�cution
						//	de mani�re � refl�ter la r�alit� :
						
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

			return list.ToArray ();
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
			//	Une requ�te vient d'�tre ex�cut�e par l'orchestrateur. Si un client
			//	est en attente de modifications d'�tat de ses requ�tes, il faut le
			//	r�veiller.
			
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
