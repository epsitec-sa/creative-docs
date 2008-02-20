//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IConnectionService donne accès aux informations sur les
	/// divers services disponibles pour une connexion par le client.
	/// </summary>
	public interface IConnectionService
	{
		void CheckConnectivity(ClientIdentity client);
		
		void QueryAvailableServices(ClientIdentity client, out string[] service_names);
	}
}
