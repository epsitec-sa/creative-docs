//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Remoting;
using Epsitec.Cresus.Requests;

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// La classe RequestExecutionEngine impl�mente un service de r�ception de
	/// requ�tes via le r�seau.
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
			//	Place une s�rie de requ�tes dans la queue.
			
			int n = requests.Length;
			
			byte[][]        data = new byte[n][];
			Database.DbId[] ids  = new Database.DbId[n];
			
			for (int i = 0; i < requests.Length; i++)
			{
				data[i] = requests[i].Data;
				ids[i]  = new Database.DbId (requests[i].Identifier);
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
			
			this.execution_queue.Enqueue (data, ids);
		}
		
		void IRequestExecutionService.QueryRequestStates(Remoting.ClientIdentity client, out RequestState[] states)
		{
			//	D�termine l'�tat de toutes les requ�tes soumises par le client
			//	sp�cifi�.
			
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
					
					//	Comme l'ex�cution a �t� faite sur le serveur, il faut ajuster l'�tat d'ex�cution
					//	de mani�re � refl�ter la r�alit� :
					
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
			//	Supprime de la queue les requ�tes dont l'�tat correspond � celui
			//	d�crit.
			
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
						
						//	Si c'�tait le seul �l�ment restant � supprimer de la table, on s'arr�te
						//	tout de suite.
						
						if (states.Length == 1)
						{
							return;
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
		}
		#endregion
		
		private Orchestrator					orchestrator;
		private ExecutionQueue					execution_queue;
	}
}
