//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceModel;

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IOperatorService g�re la cr�ation de bases de donn�es
	/// "client" et leur dur�e de vie.
	/// </summary>
	[ServiceContract]
	public interface IOperatorService : IRemoteService
	{
		[OperationContract]
		void CreateRoamingClient(string clientName, out IOperation operation);

		[OperationContract]
		void GetRoamingClientData(IOperation operation, out ClientIdentity client, out byte[] data);
	}
}
