//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Replication
{
	/// <summary>
	/// Summary description for Job.
	/// </summary>
	public class Job
	{
		public Job()
		{
			this.wait_event = new System.Threading.AutoResetEvent (false);
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
			set
			{
				this.sync_data = value;
			}
		}
		
		public string							Error
		{
			get
			{
				return this.error;
			}
			set
			{
				this.error = value;
			}
		}
		
		
		public void SignalReady()
		{
			this.wait_event.Set ();
		}
		
		public void WaitForReady()
		{
			Common.Support.Sync.Wait (this.wait_event);
		}
		
		
		private Remoting.ClientIdentity			client;
		private Database.DbId					sync_start_id;
		private Database.DbId					sync_end_id;
		private byte[]							sync_data;
		private string							error;
		private System.Threading.AutoResetEvent	wait_event;
	}
}
