//	Copyright � 2004-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		public void AcceptReplication(ClientIdentity client, long sync_start_id, long sync_end_id, out IOperation operation)
		{
			//	Signale au serveur que le client accepte des donn�es relatives � une r�plication.
			
			//	Cr�e un "job" pour Repliaction.ServerEngine; c'est l� que tout le travail
			//	se fera, par un processus s�par�.
			
			Replication.Job job = new Replication.Job ();
			
			job.Client      = client;
			job.SyncStartId = sync_start_id;
			job.SyncEndId   = sync_end_id;
			
			this.replicator.Enqueue (job);
			
			//	Maintenant que le syst�me de r�plication a re�u notre requ�te, il n'y a plus
			//	qu'� attendre que le travail soit fait...
			
			operation = job;
		}
		
		public void PullReplication(ClientIdentity client, long sync_start_id, long sync_end_id, PullReplicationArgs[] args, out IOperation operation)
		{
			//	Signale au serveur que le client demande une r�plication explicite.
			
			//	Cr�e un "job" pour Repliaction.ServerEngine; c'est l� que tout le travail
			//	se fera, par un processus s�par�.
			
			Replication.Job job = new Replication.Job ();
			
			job.Client      = client;
			job.SyncStartId = sync_start_id;
			job.SyncEndId   = sync_end_id;
			job.PullArgs    = new Replication.Job.PullArgsCollection (args);
			
			this.replicator.Enqueue (job);
			
			//	Maintenant que le syst�me de r�plication a re�u notre requ�te, il n'y a plus
			//	qu'� attendre que le travail soit fait...
			
			operation = job;
		}
		
		public void GetReplicationData(IOperation operation, out byte[] data)
		{
			//	R�cup�re le r�sultat de l'op�ration de r�plication.

			if (operation == null)
			{
				throw new System.ArgumentNullException ("operation");
			}
			
			Replication.Job job = operation as Replication.Job;
			
			if (job == null)
			{
				throw new System.ArgumentException ("Operation mismatch.");
			}
			
			job.WaitForProgress (100);
			
			this.ThrowExceptionBasedOnStatus (job.ProgressStatus);
			
			if (job.Error != null)
			{
				System.Diagnostics.Debug.WriteLine (job.Error);
			}
			
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
