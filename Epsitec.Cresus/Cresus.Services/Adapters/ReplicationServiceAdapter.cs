//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;

using System.ServiceModel;

namespace Epsitec.Cresus.Services.Adapters
{
	[ServiceBehavior (InstanceContextMode=InstanceContextMode.Single, IncludeExceptionDetailInFaults=true)]
	class ReplicationServiceAdapter : AbstractServiceAdapter, IReplicationService
	{
		public ReplicationServiceAdapter(IReplicationService target)
		{
			this.target = target;
		}

		private readonly IReplicationService target;

		#region IReplicationService Members

		public void AcceptReplication(ClientIdentity client, long sync_start_id, long sync_end_id, out IOperation operation)
		{
			IOperation op;
			this.target.AcceptReplication (client, sync_start_id, sync_end_id, out op);
			operation = new OperationWrapper (op);
		}

		public void PullReplication(ClientIdentity client, long sync_start_id, long sync_end_id, PullReplicationArgs[] args, out IOperation operation)
		{
			IOperation op;
			this.target.PullReplication (client, sync_start_id, sync_end_id, args, out op);
			operation = new OperationWrapper (op);
		}

		public void GetReplicationData(IOperation operation, out byte[] data)
		{
			OperationWrapper wr = (OperationWrapper) operation;
			this.target.GetReplicationData (wr.Target, out data);
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
