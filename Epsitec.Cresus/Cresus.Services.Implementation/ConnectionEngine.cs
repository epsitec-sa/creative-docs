//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// Summary description for ConnectionEngine.
	/// </summary>
	internal class ConnectionEngine : AbstractServiceEngine, Remoting.IConnectionService
	{
		public ConnectionEngine(Engine engine) : base (engine, "Connection")
		{
		}
		
		
		#region IConnectionService Members
		public void CheckConnectivity(Remoting.ClientIdentity client)
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("ConnectionEngine: Client '{0}' checked for connectivity.", client.Name));
		}
		
		public void QueryAvailableServices(Remoting.ClientIdentity client, out string[] service_names)
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("ConnectionEngine: Client '{0}' asked for available services.", client.Name));
			
			System.Collections.Hashtable hash = this.engine.Services;
			service_names = new string[hash.Count];
			hash.Keys.CopyTo (service_names, 0);
		}
		#endregion
	}
}
