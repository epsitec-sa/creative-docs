//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Replication
{
	/// <summary>
	/// La classe Job repr�sente une t�che de r�plication.
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
		
		
		
		private Remoting.ClientIdentity			client;
		private Database.DbId					sync_start_id;
		private Database.DbId					sync_end_id;
		private byte[]							sync_data;
		private string							sync_error;
	}
}
