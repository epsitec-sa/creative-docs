﻿//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

namespace Epsitec.Cresus.Services.Adapters
{
	class ConnectionServiceAdapter : AbstractServiceAdapter, IConnectionService
	{
		public ConnectionServiceAdapter(IConnectionService target)
		{
			this.target = target;
		}

		#region IConnectionService Members

		public void CheckConnectivity(ClientIdentity client)
		{
			this.target.CheckConnectivity (client);
		}

		public void QueryAvailableServices(ClientIdentity client, out string[] service_names)
		{
			this.target.QueryAvailableServices (client, out service_names);
		}

		#endregion

		private readonly IConnectionService target;

		#region IRemotingService Members

		public System.Guid GetServiceId()
		{
			throw new System.NotImplementedException ();
		}

		public string GetServiceName()
		{
			throw new System.NotImplementedException ();
		}

		#endregion
	}
}
