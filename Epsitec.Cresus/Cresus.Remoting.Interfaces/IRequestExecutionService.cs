//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		void ClearRequestStates(ClientIdentity client, RequestState[] states);
	}
}
