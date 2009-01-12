//	Copyright © 2008-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	sealed class ReplicationServiceAdapter : AbstractServiceAdapter<IReplicationService>, IReplicationService
	{
		public ReplicationServiceAdapter(IReplicationService target)
			: base (target)
		{
		}

		#region IReplicationService Members

		public ProgressInformation AcceptReplication(ClientIdentity client, long sync_start_id, long sync_end_id)
		{
			return this.target.AcceptReplication (client, sync_start_id, sync_end_id);
		}

		public ProgressInformation PullReplication(ClientIdentity client, long sync_start_id, long sync_end_id, PullReplicationChunk[] args)
		{
			return this.target.PullReplication (client, sync_start_id, sync_end_id, args);
		}

		public byte[] GetReplicationData(long operationId)
		{
			return this.target.GetReplicationData (operationId);
		}

		#endregion
	}
}
