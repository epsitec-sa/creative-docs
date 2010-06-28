//	Copyright © 2008-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	sealed class RequestExecutionServiceAdapter : AbstractServiceAdapter<IRequestExecutionService>, IRequestExecutionService
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

		public RequestState[] QueryRequestStates(ClientIdentity client)
		{
			return this.target.QueryRequestStates (client);
		}

		public int QueryRequestStatesUsingFilter(ClientIdentity client, int changeId, out RequestState[] states)
		{
			return this.target.QueryRequestStatesUsingFilter (client, changeId, out states);
		}

		public void WakeUpQueryRequestStatesUsingFilter(ClientIdentity client)
		{
			this.target.WakeUpQueryRequestStatesUsingFilter (client);
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
