//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceModel;

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IConnectionService donne accès aux informations sur les
	/// divers services disponibles pour une connexion par le client.
	/// </summary>
	[ServiceContract]
	public interface IConnectionService : IRemoteService
	{
		[OperationContract]
		void CheckConnectivity(ClientIdentity client);

		[OperationContract]
		System.Guid[] QueryAvailableServices(ClientIdentity client);
	}
}
