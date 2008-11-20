//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

using System.ServiceModel;

namespace Epsitec.Cresus.Services.Adapters
{
	/// <summary>
	/// The <c>RequestExecutionServiceAdapter</c> class implements an adapter for the
	/// <see cref="IRequestExecutionService"/>. See also <see cref="AbstractServiceAdapter"/>..
	/// </summary>
	[ServiceBehavior (InstanceContextMode=InstanceContextMode.Single, IncludeExceptionDetailInFaults=true)]
	class RequestExecutionServiceAdapter : AbstractServiceAdapter<IRequestExecutionService>, IRequestExecutionService
	{
		public RequestExecutionServiceAdapter(IRequestExecutionService target)
			: base (target)
		{
		}

		#region IRequestExecutionService Members

		public void EnqueueRequest(ClientIdentity client, SerializedRequest[] requests)
		{
			this.target.EnqueueRequest (client, requests);
		}

		public void QueryRequestStates(ClientIdentity client, out RequestState[] states)
		{
			this.target.QueryRequestStates (client, out states);
		}

		public void QueryRequestStatesUsingFilter(ClientIdentity client, ref int changeId, System.TimeSpan timeout, out RequestState[] states)
		{
			this.target.QueryRequestStatesUsingFilter (client, ref changeId, timeout, out states);
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
	}
}
