//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IRequestExecutionService donne accès au service d'exécution
	/// des requêtes, comme son nom l'indique.
	/// </summary>
	public interface IRequestExecutionService
	{
		void EnqueueRequest(ClientIdentity client, SerializedRequest[] requests);
		
		void QueryRequestStates(ClientIdentity client, out RequestState[] states);
		void QueryRequestStates(ClientIdentity client, ref int change_id, System.TimeSpan timeout, out RequestState[] states);
		
		void RemoveRequestStates(ClientIdentity client, RequestState[] states);
		void RemoveAllRequestStates(ClientIdentity client);
	}
}
