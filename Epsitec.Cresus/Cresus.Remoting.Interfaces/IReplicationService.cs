//	Copyright � 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IReplicationService donne acc�s au service de r�plication.
	/// </summary>
	public interface IReplicationService
	{
		void AcceptReplication(ClientIdentity client, long sync_start_id, long sync_end_id, out IOperation operation);
		void PullReplication(ClientIdentity client, long sync_start_id, long sync_end_id, PullReplicationArgs[] args, out IOperation operation);
		
		void GetReplicationData(IOperation operation, out byte[] data);
	}
}
