//	Copyright © 2008-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

using System.ServiceModel;

namespace Epsitec.Cresus.Services.Adapters
{
	/// <summary>
	/// The <c>ConnectionServiceAdapter</c> class implements an adapter for the
	/// <see cref="IConnectionService"/>. See also <see cref="AbstractServiceAdapter"/>..
	/// </summary>
	[ServiceBehavior (InstanceContextMode=InstanceContextMode.Single, IncludeExceptionDetailInFaults=true)]
	class ConnectionServiceAdapter : AbstractServiceAdapter<IConnectionService>, IConnectionService
	{
		public ConnectionServiceAdapter(IConnectionService target)
			: base (target)
		{
		}

		#region IConnectionService Members

		public void CheckConnectivity(ClientIdentity client)
		{
			this.target.CheckConnectivity (client);
		}

		public System.Guid[] QueryAvailableServices(ClientIdentity client)
		{
			return this.target.QueryAvailableServices (client);
		}

		#endregion
	}
}
