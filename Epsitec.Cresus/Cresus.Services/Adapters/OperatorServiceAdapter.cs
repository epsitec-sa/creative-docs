//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

using System.ServiceModel;

namespace Epsitec.Cresus.Services.Adapters
{
	[ServiceBehavior (InstanceContextMode=InstanceContextMode.Single, IncludeExceptionDetailInFaults=true)]
	class OperatorServiceAdapter : AbstractServiceAdapter, IOperatorService
	{
		public OperatorServiceAdapter(IOperatorService target)
		{
			this.target = target;
		}

		private readonly IOperatorService target;

		#region IOperatorService Members

		public void CreateRoamingClient(string client_name, out ProgressInformation operation)
		{
			this.target.CreateRoamingClient (client_name, out operation);
		}

		public void GetRoamingClientData(long operationId, out ClientIdentity client, out byte[] compressed_data)
		{
			this.target.GetRoamingClientData (operationId, out client, out compressed_data);
		}

		#endregion

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
