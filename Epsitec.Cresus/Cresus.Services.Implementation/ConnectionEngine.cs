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
		public void CheckConnectivity(string client_id)
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("ConnectionEngine: Client '{0}' checked for connectivity.", client_id));
		}
		
		public void QueryAvailableServices(string client_id, out string[] service_names)
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("ConnectionEngine: Client '{0}' asked for available services.", client_id));
			
			System.Collections.Hashtable hash = this.engine.Services;
			service_names = new string[hash.Count];
			hash.Keys.CopyTo (service_names, 0);
		}
		#endregion
	}
}
