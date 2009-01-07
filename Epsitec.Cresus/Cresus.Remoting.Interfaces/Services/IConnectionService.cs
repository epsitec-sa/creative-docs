//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceModel;

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>IConnectionService</c> interface allows a client to establish a
	/// connection to the services provided by the server.
	/// </summary>
	[ServiceContract]
	public interface IConnectionService : IRemoteService
	{
		/// <summary>
		/// Checks the connectivity.
		/// </summary>
		/// <param name="client">The client identity.</param>
		[OperationContract]
		void CheckConnectivity(ClientIdentity client);

		/// <summary>
		/// Queries the available services.
		/// </summary>
		/// <param name="client">The client identity.</param>
		/// <returns>The ids of the available services.</returns>
		[OperationContract]
		System.Guid[] QueryAvailableServices(ClientIdentity client);
	}
}
