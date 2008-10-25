//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IOperatorService gère la création de bases de données
	/// "client" et leur durée de vie.
	/// </summary>
	public interface IOperatorService : IRemoteService
	{
		void CreateRoamingClient(string clientName, out IOperation operation);
		
		void GetRoamingClientData(IOperation operation, out ClientIdentity client, out byte[] data);
	}
}
