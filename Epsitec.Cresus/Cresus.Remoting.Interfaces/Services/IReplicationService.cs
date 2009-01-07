//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceModel;

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// The <c>IReplicationService</c> interface allows a client to get
	/// replication data from the server.
	/// </summary>
	[ServiceContract]
	public interface IReplicationService : IRemoteService
	{
		[OperationContract]
		ProgressInformation AcceptReplication(ClientIdentity client, long sync_start_id, long sync_end_id);

		[OperationContract]
		ProgressInformation PullReplication(ClientIdentity client, long sync_start_id, long sync_end_id, PullReplicationArgs[] args);

		[OperationContract]
		byte[] GetReplicationData(long operationId);
	}
}
