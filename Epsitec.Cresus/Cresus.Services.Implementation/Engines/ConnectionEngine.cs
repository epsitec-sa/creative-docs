//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

using System.Collections.Generic;

namespace Epsitec.Cresus.Services.Engines
{
	/// <summary>
	/// The <c>ConnectionEngine</c> class implements the <see cref="IConnectionService"/>
	/// interface.
	/// </summary>
	internal sealed class ConnectionEngine : AbstractServiceEngine, IConnectionService
	{
		public ConnectionEngine(Engine engine)
			: base (engine)
		{
		}
		
		
		#region IConnectionService Members
		
		public void CheckConnectivity(ClientIdentity client)
		{
			System.Diagnostics.Debug.WriteLine (string.Format ("ConnectionEngine: Client '{0}' checked for connectivity.", client));
		}

		public System.Guid[] QueryAvailableServices(ClientIdentity client)
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
