//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Remoting;
using Epsitec.Cresus.Requests;

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// La classe ReplicationEngine impl�mente un service de r�plication. Tout
	/// le travail se fait dans Replication.ServerEngine.
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
			//	Cr�e un "job" pour Repliaction.ServerEngine; c'est l� que tout le travail
			//	se fera, par un processus s�par� :
			
			Replication.Job job = new Replication.Job ();
			
			job.Client = client;
			job.SyncStartId = sync_start_id;
			job.SyncEndId   = sync_end_id;
			
			this.replicator.Enqueue (job);
			
			//	Maintenant que le syst�me de r�plication a re�u notre requ�te, il n'y a plus
			//	qu'� attendre que le travail soit fait pour retourner � l'appelant :
			
			job.WaitForReady ();
			
			data = job.Data;
		}
		#endregion
		
		
		protected override void Dispose(bool disposing)
		{
			if (this.replicator != null)
			{
				this.replicator.Dispose ();
				this.replicator = null;
			}
			
			base.Dispose (disposing);
		}
		
		
		private Replication.ServerEngine		replicator;
	}
}
