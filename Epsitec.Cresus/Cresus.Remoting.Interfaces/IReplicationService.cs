//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IReplicationService donne accès au service de réplication.
	/// </summary>
	public interface IReplicationService
	{
		void AcceptReplication(ClientIdentity client, long sync_start_id, long sync_end_id, out byte[] data);
	}
}
