//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Cresus.Remoting;

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

			List<string> serviceNames = new List<string> (this.engine.ServiceNames);
			serviceNames.Sort ();
			service_names = serviceNames.ToArray ();
		}
		#endregion

		public override System.Guid GetServiceId()
		{
			return RemotingServices.ConnectionServiceId;
		}
	}
}
