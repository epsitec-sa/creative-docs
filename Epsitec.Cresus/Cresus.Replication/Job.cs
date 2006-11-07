//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Replication
{
	/// <summary>
	/// La classe Job représente une tâche de réplication.
	/// </summary>
	public class Job : Remoting.AbstractOperation
	{
		public Job()
		{
			this.SetLastStep (1);
		}
		
		
		public Remoting.ClientIdentity			Client
		{
			get
			{
				return this.client;
			}
			set
			{
				this.client = value;
			}
		}
		
		public Database.DbId					SyncStartId
		{
			get
			{
				return this.sync_start_id;
			}
			set
			{
				this.sync_start_id = value;
			}
		}
		
		public Database.DbId					SyncEndId
		{
			get
			{
				return this.sync_end_id;
			}
			set
			{
				this.sync_end_id = value;
			}
		}
		
		public byte[]							Data
		{
			get
			{
				return this.sync_data;
			}
		}
		
		public string							Error
		{
			get
			{
				return this.sync_error;
			}
		}
		
		public PullArgsCollection				PullArgs
		{
			get
			{
				return this.pull_args;
			}
			set
			{
				this.pull_args = value;
			}
		}
		
		
		internal void WaitForReady()
		{
			this.WaitForProgress (100);
		}
		
		
		internal void SignalStartedProcessing()
		{
			this.sync_data  = null;
			this.sync_error = null;
			
			this.SetCurrentStep (0);
		}
		
		internal void SignalFinishedProcessing(byte[] data)
		{
			this.sync_data  = data;
			this.sync_error = null;
			
			this.SetCurrentStep (1);
			this.SetProgress (100);
		}
		
		internal void SignalError(string message)
		{
			this.sync_data  = null;
			this.sync_error = message;
			
			this.SetFailed (message);
		}
		
		
		#region PullArgsCollection Class
		public sealed class PullArgsCollection
		{
			public PullArgsCollection(Remoting.PullReplicationArgs[] args)
			{
				this.args = args;
			}
			
			
			public Remoting.PullReplicationArgs	this[Database.DbId id]
			{
				get
				{
					for (int i = 0; i < args.Length; i++)
					{
						if (args[i].TableId == id)
						{
							return args[i];
						}
					}
					
					return null;
				}
			}
			
			public Remoting.PullReplicationArgs	this[Database.DbTable table]
			{
				get
				{
					return this[table.Key.Id];
				}
			}
			
			
			public bool Contains(Database.DbId id)
			{
				for (int i = 0; i < args.Length; i++)
				{
					if (args[i].TableId == id)
					{
						return true;
					}
				}
				
				return false;
			}
			
			public bool Contains(Database.DbTable table)
			{
				return this.Contains (table.Key.Id);
			}
			
			
			Remoting.PullReplicationArgs[]		args;
		}
		#endregion
		
		private Remoting.ClientIdentity			client;
		private Database.DbId					sync_start_id;
		private Database.DbId					sync_end_id;
		private byte[]							sync_data;
		private string							sync_error;
		private PullArgsCollection				pull_args;
	}
}
