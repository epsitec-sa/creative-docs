//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Settings
{
	/// <summary>
	/// Summary description for Locals.
	/// </summary>
	public sealed class Locals : AbstractBase
	{
		internal Locals(DbInfrastructure infrastructure, DbTransaction transaction)
		{
			this.Setup (infrastructure, transaction, Locals.Name);
		}
		
		
		internal static string					Name
		{
			get
			{
				return "CR_SETTINGS_LOCALS";
			}
		}
		
		
		public int								ClientId
		{
			get
			{
				return this.client_id;
			}
			set
			{
				if (this.client_id != value)
				{
					this.client_id = value;
					this.NotifyPropertyChanged ("ClientId");
				}
			}
		}
		
		public bool								IsServer
		{
			get
			{
				return this.is_server;
			}
			set
			{
				if (this.is_server != value)
				{
					this.is_server = value;
					this.NotifyPropertyChanged ("IsServer");
				}
			}
		}
		
		public long								SyncLogId
		{
			get
			{
				return this.sync_log_id;
			}
			set
			{
				if (this.sync_log_id != value)
				{
					this.sync_log_id = value;
					this.NotifyPropertyChanged ("SyncLogId");
				}
			}
		}
		
		
		private int								client_id;
		private bool							is_server;
		private long							sync_log_id;
	}
}
