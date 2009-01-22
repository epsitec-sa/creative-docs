//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceModel;

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>IRequestExecutionService</c> interface allows the client to manage
	/// low level requests intended for the request execution engine.
	/// </summary>
	[ServiceContract]
	public interface IRequestExecutionService : IRemoteService
	{
		/// <summary>
		/// Enqueues the specified requests.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="requests">The requests.</param>
		[OperationContract]
		void EnqueueRequest(ClientIdentity client, SerializedRequest[] requests);

		/// <summary>
		/// Queries the request states.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <returns>The request states.</returns>
		[OperationContract]
		RequestState[] QueryRequestStates(ClientIdentity client);

		/// <summary>
		/// Queries the request states using a filter.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="changeId">The change id.</param>
		/// <param name="states">The states.</param>
		/// <returns>The current change id.</returns>
		[OperationContract]
		int QueryRequestStatesUsingFilter(ClientIdentity client, int changeId, out RequestState[] states);

		/// <summary>
		/// Wakes up the threads waiting in method <see cref="QueryRequestStatesUsingFiler"/>.
		/// </summary>
		/// <param name="client">The client.</param>
		[OperationContract]
		void WakeUpQueryRequestStatesUsingFilter(ClientIdentity client);

		/// <summary>
		/// Removes the specified request states.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="states">The request states.</param>
		[OperationContract]
		void RemoveRequestStates(ClientIdentity client, RequestState[] states);

		/// <summary>
		/// Removes all request states.
		/// </summary>
		/// <param name="client">The client.</param>
		[OperationContract]
		void RemoveAllRequestStates(ClientIdentity client);
	}
}
