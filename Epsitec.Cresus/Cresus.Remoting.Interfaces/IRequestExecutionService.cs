//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IRequestExecutionService donne acc�s au service d'ex�cution
	/// des requ�tes, comme son nom l'indique.
	/// </summary>
	public interface IRequestExecutionService
	{
		void EnqueueRequest(SerializedRequest[] requests);
		
		void QueryRequestStates(ClientIdentity client_id, out RequestState[] states);
		void ClearRequestStates(RequestState[] states);
	}
}
