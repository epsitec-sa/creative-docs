//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// La classe ExecutionService implémente un service de réception de
	/// requêtes via le réseau.
	/// </summary>
	public class ExecutionService : System.MarshalByRefObject, IRequestExecutionService
	{
		public ExecutionService(Orchestrator orchestrator)
		{
			this.orchestrator    = orchestrator;
			this.execution_queue = orchestrator.ExecutionQueue;
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
		
		void IRequestExecutionService.QueryRequestStates(int client_id, out RequestState[] states)
		{
			//	Détermine l'état de toutes les requêtes soumises par le client
			//	spécifié.
			
			System.Data.DataRow[] rows = this.execution_queue.Rows;
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			for (int i = 0; i < rows.Length; i++)
			{
				Database.DbKey key = new Database.DbKey (rows[i]);
				
				if (key.Id.ClientId == client_id)
				{
					list.Add (new RequestState (key.Id.Value, (int) this.execution_queue.GetRequestExecutionState (rows[i])));
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
				Database.DbKey key = new Database.DbKey (rows[i]);
				
				for (int j = 0; j < states.Length; j++)
				{
					if ((states[j].Identifier == key.Id.Value) &&
						(states[j].State == (int)this.execution_queue.GetRequestExecutionState (rows[i])))
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
		
		void IRequestExecutionService.Ping(string text, out string reply)
		{
			System.Diagnostics.Debug.WriteLine ("Server got: " + text);
			reply = System.DateTime.Now.ToLongTimeString ();
		}
		#endregion
		
		public static void StartService(ExecutionService service, int port)
		{
			HttpChannel channel = new HttpChannel (port);
			ChannelServices.RegisterChannel (channel);
			RemotingServices.Marshal (service, "RequestExecutionService.soap");
		}
		
		public static IRequestExecutionService GetRemoteService(string machine, int port)
		{
			string url = string.Format (System.Globalization.CultureInfo.InvariantCulture, "http://{0}:{1}/RequestExecutionService.soap", machine, port);
			
			IRequestExecutionService service = (IRequestExecutionService) System.Activator.GetObject (typeof (IRequestExecutionService), url);
			
			return service;
		}
		
		
		protected Orchestrator					orchestrator;
		protected ExecutionQueue				execution_queue;
	}
}
