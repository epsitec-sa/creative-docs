//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

using System.ServiceModel;

namespace Epsitec.Cresus.Services.Adapters
{
	[ServiceBehavior (InstanceContextMode=InstanceContextMode.Single, IncludeExceptionDetailInFaults=true)]
	class RequestExecutionServiceAdapter : AbstractServiceAdapter, IRequestExecutionService
	{
		public RequestExecutionServiceAdapter(IRequestExecutionService target)
		{
			this.target = target;
		}

		private readonly IRequestExecutionService target;

		#region IRequestExecutionService Members

		public void EnqueueRequest(ClientIdentity client, SerializedRequest[] requests)
		{
			this.target.EnqueueRequest (client, requests);
		}

		public void QueryRequestStates(ClientIdentity client, out RequestState[] states)
		{
			this.target.QueryRequestStates (client, out states);
		}

		public void QueryRequestStatesUsingFilter(ClientIdentity client, ref int change_id, System.TimeSpan timeout, out RequestState[] states)
		{
			this.target.QueryRequestStatesUsingFilter (client, ref change_id, timeout, out states);
		}

		public void RemoveRequestStates(ClientIdentity client, RequestState[] states)
		{
			this.target.RemoveRequestStates (client, states);
		}

		public void RemoveAllRequestStates(ClientIdentity client)
		{
			this.target.RemoveAllRequestStates (client);
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
