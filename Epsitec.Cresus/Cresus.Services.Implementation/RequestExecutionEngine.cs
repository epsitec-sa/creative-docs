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
			
			System.Diagnostics.Debug.Assert (this.execution_queue.IsRunningAsServer);
		}
		
		
		#region IRequestExecutionService Members
		void IRequestExecutionService.EnqueueRequest(SerializedRequest[] requests)
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
			//	Détermine l'état de toutes les requêtes soumises par le client
			//	spécifié.
			
			System.Data.DataRow[] rows = this.execution_queue.Rows;
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			for (int i = 0; i < rows.Length; i++)
			{
				if (rows[i].RowState == System.Data.DataRowState.Deleted)
				{
					continue;
				}
				
				Database.DbKey row_key = new Database.DbKey (rows[i]);
				
				if (row_key.Id.ClientId == client.ClientId)
				{
					Requests.ExecutionState state = this.execution_queue.GetRequestExecutionState (rows[i]);
					
					//	Comme l'exécution a été faite sur le serveur, il faut ajuster l'état d'exécution
					//	de manière à refléter la réalité :
					
					if (state == Requests.ExecutionState.ExecutedByClient)
					{
						state = Requests.ExecutionState.ExecutedByServer;
					}
					
					list.Add (new RequestState (row_key.Id.Value, (int) state));
				}
			}
			
			states = new RequestState[list.Count];
			
			list.CopyTo (states);
		}
		
		void IRequestExecutionService.ClearRequestStates(RequestState[] states)
		{
			//	Supprime de la queue les requêtes dont l'état correspond à celui
			//	décrit.
			
			System.Data.DataRow[] rows = this.execution_queue.Rows;
			
			for (int i = 0; i < rows.Length; i++)
			{
				if (rows[i].RowState == System.Data.DataRowState.Deleted)
				{
					continue;
				}
				
				Database.DbKey row_key   = new Database.DbKey (rows[i]);
				ExecutionState row_state = this.execution_queue.GetRequestExecutionState (rows[i]);
					
				if (row_state == Requests.ExecutionState.ExecutedByClient)
				{
					row_state = Requests.ExecutionState.ExecutedByServer;
				}
				
				for (int j = 0; j < states.Length; j++)
				{
					if ((states[j].Identifier == row_key.Id.Value) &&
						(states[j].State == (int)row_state))
					{
						Database.DbRichCommand.KillRow (rows[i]);
						
						//	Si c'était le seul élément restant à supprimer de la table, on s'arrête
						//	tout de suite.
						
						if (states.Length == 1)
						{
							return;
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
		}
		#endregion
		
		private Orchestrator					orchestrator;
		private ExecutionQueue					execution_queue;
	}
}
