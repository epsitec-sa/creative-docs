//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// La classe ConnectionEngine permet de s'enquérir au sujet des services
	/// présents sur un serveur et de vérifier l'état de la connexion.
	/// </summary>
	internal sealed class ConnectionEngine : AbstractServiceEngine, Remoting.IConnectionService
	{
		public ConnectionEngine(Engine engine) : base (engine, "Connection")
		{
		}
		
		
		#region IConnectionService Members
		public void CheckConnectivity(Remoting.ClientIdentity client)
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("ConnectionEngine: Client '{0}' checked for connectivity.", client));
		}
		
		public void QueryAvailableServices(Remoting.ClientIdentity client, out string[] service_names)
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("ConnectionEngine: Client '{0}' asked for available services.", client));
			
			System.Collections.Hashtable hash = this.engine.Services;
			service_names = new string[hash.Count];
			hash.Keys.CopyTo (service_names, 0);
			System.Array.Sort (service_names);
		}
		#endregion
	}
}
