//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Remoting;
using Epsitec.Cresus.Requests;

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// La classe ReplicationEngine implémente un service de réplication.
	/// </summary>
	internal sealed class ReplicationEngine : AbstractServiceEngine, IReplicationService
	{
		public ReplicationEngine(Engine engine) : base (engine, "Replication")
		{
			this.replicator = new Replication.ServerEngine (engine.Infrastructure);
		}
		
		
		#region IReplicationService Members
		public void AcceptReplication(ClientIdentity client, long sync_start_id, long sync_end_id, out byte[] data)
		{
			Replication.Job job = new Replication.Job ();
			
			job.Client = client;
			job.SyncStartId = sync_start_id;
			job.SyncEndId   = sync_end_id;
			
			this.replicator.Enqueue (job);
			job.WaitForReady ();
			
			data = job.Data;
		}
		#endregion
		
		
		private Replication.ServerEngine		replicator;
	}
}
