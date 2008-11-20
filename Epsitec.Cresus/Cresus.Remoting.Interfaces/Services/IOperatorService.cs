//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceModel;

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IOperatorService gère la création de bases de données
	/// "client" et leur durée de vie.
	/// </summary>
	[ServiceContract]
	public interface IOperatorService : IRemoteService
	{
		[OperationContract]
		void CreateRoamingClient(string clientName, out ProgressInformation operation);

		[OperationContract]
		void GetRoamingClientData(long operationId, out ClientIdentity client, out byte[] data);
	}
}
