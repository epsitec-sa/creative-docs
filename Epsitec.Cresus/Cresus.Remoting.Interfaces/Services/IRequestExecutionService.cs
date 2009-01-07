//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceModel;

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IRequestExecutionService donne accès au service d'exécution
	/// des requêtes, comme son nom l'indique.
	/// </summary>
	[ServiceContract]
	public interface IRequestExecutionService : IRemoteService
	{
		[OperationContract]
		void EnqueueRequest(ClientIdentity client, SerializedRequest[] requests);

		[OperationContract]
		RequestState[] QueryRequestStates(ClientIdentity client);

		[OperationContract]
		int QueryRequestStatesUsingFilter(ClientIdentity client, int changeId, System.TimeSpan timeout, out RequestState[] states);

		[OperationContract]
		void RemoveRequestStates(ClientIdentity client, RequestState[] states);

		[OperationContract]
		void RemoveAllRequestStates(ClientIdentity client);
	}
}
