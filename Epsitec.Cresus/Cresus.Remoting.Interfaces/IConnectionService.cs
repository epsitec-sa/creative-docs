//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IConnectionService donne accès aux informations sur les
	/// divers services disponibles pour une connexion par le client.
	/// </summary>
	public interface IConnectionService
	{
		void CheckConnectivity(ClientIdentity client_id);
		
		void QueryAvailableServices(ClientIdentity client_id, out string[] service_names);
	}
}
