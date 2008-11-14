//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.ServiceModel;

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IReplicationService donne accès au service de réplication.
	/// </summary>
	[ServiceContract]
	public interface IReplicationService : IRemoteService
	{
		[OperationContract]
		void AcceptReplication(ClientIdentity client, long sync_start_id, long sync_end_id, out IOperation operation);

		[OperationContract]
		void PullReplication(ClientIdentity client, long sync_start_id, long sync_end_id, PullReplicationArgs[] args, out IOperation operation);

		[OperationContract]
		void GetReplicationData(IOperation operation, out byte[] data);
	}
}
