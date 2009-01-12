//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

using System.Collections.Generic;

namespace Epsitec.Cresus.Services.Engines
{
	/// <summary>
	/// La classe ConnectionEngine permet de s'enquérir au sujet des services
	/// présents sur un serveur et de vérifier l'état de la connexion.
	/// </summary>
	internal sealed class ConnectionEngine : AbstractServiceEngine, Remoting.IConnectionService
	{
		public ConnectionEngine(Engine engine)
			: base (engine)
		{
		}
		
		
		#region IConnectionService Members
		
		public void CheckConnectivity(Remoting.ClientIdentity client)
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("ConnectionEngine: Client '{0}' checked for connectivity.", client));
		}

		public System.Guid[] QueryAvailableServices(Remoting.ClientIdentity client)
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("ConnectionEngine: Client '{0}' asked for available services.", client));

			List<System.Guid> serviceIds = new List<System.Guid> (this.Engine.GetServiceIds ());
			serviceIds.Sort ();
			return serviceIds.ToArray ();
		}
		
		#endregion

		public override System.Guid GetServiceId()
		{
			return RemotingServices.ConnectionServiceId;
		}
	}
}
