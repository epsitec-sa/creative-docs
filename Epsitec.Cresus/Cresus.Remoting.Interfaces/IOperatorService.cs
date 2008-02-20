//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IOperatorService gère la création de bases de données
	/// "client" et leur durée de vie.
	/// </summary>
	public interface IOperatorService
	{
		void CreateRoamingClient(string client_name, out IOperation operation);
		
		void GetRoamingClientData(IOperation operation, out ClientIdentity client, out byte[] compressed_data);
	}
}
