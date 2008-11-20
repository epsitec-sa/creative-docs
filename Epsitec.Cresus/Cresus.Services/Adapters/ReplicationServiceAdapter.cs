//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

using System.ServiceModel;

namespace Epsitec.Cresus.Services.Adapters
{
	/// <summary>
	/// The <c>ReplicationServiceAdapter</c> class implements an adapter for the
	/// <see cref="IReplicationService"/>. See also <see cref="AbstractServiceAdapter"/>..
	/// </summary>
	[ServiceBehavior (InstanceContextMode=InstanceContextMode.Single, IncludeExceptionDetailInFaults=true)]
	class ReplicationServiceAdapter : AbstractServiceAdapter<IReplicationService>, IReplicationService
	{
		public ReplicationServiceAdapter(IReplicationService target)
			: base (target)
		{
		}

		#region IReplicationService Members

		public void AcceptReplication(ClientIdentity client, long sync_start_id, long sync_end_id, out ProgressInformation operation)
		{
			this.target.AcceptReplication (client, sync_start_id, sync_end_id, out operation);
		}

		public void PullReplication(ClientIdentity client, long sync_start_id, long sync_end_id, PullReplicationArgs[] args, out ProgressInformation operation)
		{
			this.target.PullReplication (client, sync_start_id, sync_end_id, args, out operation);
		}

		public void GetReplicationData(long operationId, out byte[] data)
		{
			this.target.GetReplicationData (operationId, out data);
		}

		#endregion
	}
}
