//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IRequestExecutionService donne acc�s au service d'ex�cution
	/// des requ�tes, comme son nom l'indique.
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
