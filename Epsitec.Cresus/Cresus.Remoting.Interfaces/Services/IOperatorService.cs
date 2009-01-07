//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceModel;

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>IOperatorService</c> interface allows a client to request
	/// the creation of a fresh client database copy.
	/// </summary>
	[ServiceContract]
	public interface IOperatorService : IRemoteService
	{
		/// <summary>
		/// Creates the roaming client database. This may take some time; the
		/// results can be queried through <see cref="GetRoamingClientData"/>.
		/// </summary>
		/// <param name="clientName">The client name.</param>
		/// <returns>The operation describing the roaming client database creation.</returns>
		[OperationContract]
		ProgressInformation CreateRoamingClient(string clientName);

		/// <summary>
		/// Gets the roaming client data when the operation finishes.
		/// </summary>
		/// <param name="operationId">The operation id.</param>
		/// <param name="client">The client.</param>
		/// <param name="data">The data.</param>
		/// <returns><c>true</c> on succsess; otherwise, <c>false</c>.</returns>
		[OperationContract]
		bool GetRoamingClientData(long operationId, out ClientIdentity client, out byte[] data);
	}
}
