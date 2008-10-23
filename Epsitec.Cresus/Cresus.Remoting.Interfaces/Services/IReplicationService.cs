//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IReplicationService donne acc�s au service de r�plication.
	/// </summary>
	public interface IReplicationService : IRemotingService
	{
		void AcceptReplication(ClientIdentity client, long sync_start_id, long sync_end_id, out IOperation operation);
		void PullReplication(ClientIdentity client, long sync_start_id, long sync_end_id, PullReplicationArgs[] args, out IOperation operation);
		
		void GetReplicationData(IOperation operation, out byte[] data);
	}
}
