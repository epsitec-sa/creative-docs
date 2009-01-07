//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		/// <summary>
		/// Starts a full replication; the client specifies that it accepts replication
		/// data. The data can be fetched by <see cref="GetReplicationData"/>.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="syncStartId">The sync start id.</param>
		/// <param name="syncEndId">The sync end id.</param>
		/// <returns></returns>
		[OperationContract]
		ProgressInformation AcceptReplication(ClientIdentity client, long syncStartId, long syncEndId);

		/// <summary>
		/// Starts a parametrized replication; the client specifies for which tables it
		/// whishes to get the replication data. The data can be fetched by
		/// <see cref="GetReplicationData"/>.
		/// </summary>
		/// <param name="client">The client.</param>
		/// <param name="syncStartId">The sync start id.</param>
		/// <param name="syncEndId">The sync end id.</param>
		/// <param name="chunks">The chunks.</param>
		/// <returns></returns>
		[OperationContract]
		ProgressInformation PullReplication(ClientIdentity client, long syncStartId, long syncEndId, PullReplicationChunk[] chunks);

		/// <summary>
		/// Gets the replication data for a previously started replication job.
		/// </summary>
		/// <param name="operationId">The operation id.</param>
		/// <returns>The replication data.</returns>
		[OperationContract]
		byte[] GetReplicationData(long operationId);
	}
}
