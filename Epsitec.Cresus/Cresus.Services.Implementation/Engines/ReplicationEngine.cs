//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Remoting;
using Epsitec.Cresus.Requests;

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// La classe ReplicationEngine implémente un service de réplication. Tout
	/// le travail se fait dans Replication.ServerEngine.
	/// </summary>
	internal sealed class ReplicationEngine : AbstractServiceEngine, IReplicationService
	{
		public ReplicationEngine(Engine engine)
			: base (engine)
		{
			this.replicator = new Replication.ServerEngine (engine.Infrastructure);
		}
		
		
		#region IReplicationService Members
		public ProgressInformation AcceptReplication(ClientIdentity client, long sync_start_id, long sync_end_id)
		{
			//	Signale au serveur que le client accepte des données relatives à une réplication.
			
			//	Crée un "job" pour Repliaction.ServerEngine; c'est là que tout le travail
			//	se fera, par un processus séparé.
			
			Replication.Job job = new Replication.Job ();
			
			job.Client      = client;
			job.SyncStartId = sync_start_id;
			job.SyncEndId   = sync_end_id;
			
			this.replicator.Enqueue (job);
			
			//	Maintenant que le système de réplication a reçu notre requête, il n'y a plus
			//	qu'à attendre que le travail soit fait...
			
			return job.GetProgressInformation ();
		}

		public ProgressInformation PullReplication(ClientIdentity client, long sync_start_id, long sync_end_id, PullReplicationChunk[] args)
		{
			//	Signale au serveur que le client demande une réplication explicite.
			
			//	Crée un "job" pour Repliaction.ServerEngine; c'est là que tout le travail
			//	se fera, par un processus séparé.
			
			Replication.Job job = new Replication.Job ();
			
			job.Client      = client;
			job.SyncStartId = sync_start_id;
			job.SyncEndId   = sync_end_id;
			job.PullArgs    = new Replication.Job.PullArgsCollection (args);
			
			this.replicator.Enqueue (job);
			
			//	Maintenant que le système de réplication a reçu notre requête, il n'y a plus
			//	qu'à attendre que le travail soit fait...
			
			return job.GetProgressInformation ();
		}
		
		public byte[] GetReplicationData(long operationId)
		{
			//	Récupère le résultat de l'opération de réplication.

			Replication.Job job = OperationManager.Resolve<Replication.Job> (operationId);
			
			if (job == null)
			{
				throw new System.ArgumentException ("Operation mismatch.");
			}
			
			job.WaitForProgress (100);
			
			Engine.ThrowExceptionBasedOnStatus (job.ProgressState);
			
			if (job.Error != null)
			{
				System.Diagnostics.Debug.WriteLine (job.Error);
			}
			
			return job.Data;
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

		public override System.Guid GetServiceId()
		{
			return RemotingServices.ReplicationServiceId;
		}
		
		
		private Replication.ServerEngine		replicator;
	}
}
