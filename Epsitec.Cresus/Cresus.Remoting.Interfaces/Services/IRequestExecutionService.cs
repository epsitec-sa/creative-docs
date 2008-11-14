//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceModel;

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IRequestExecutionService donne acc�s au service d'ex�cution
	/// des requ�tes, comme son nom l'indique.
	/// </summary>
	[ServiceContract]
	public interface IRequestExecutionService : IRemoteService
	{
		[OperationContract]
		void EnqueueRequest(ClientIdentity client, SerializedRequest[] requests);

		[OperationContract]
		void QueryRequestStates(ClientIdentity client, out RequestState[] states);

		[OperationContract]
		void QueryRequestStatesUsingFilter(ClientIdentity client, ref int change_id, System.TimeSpan timeout, out RequestState[] states);

		[OperationContract]
		void RemoveRequestStates(ClientIdentity client, RequestState[] states);

		[OperationContract]
		void RemoveAllRequestStates(ClientIdentity client);
	}
}
