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
					object old_value = this.client_id;
					object new_value = value;
					
					this.client_id = value;
					
					this.NotifyPropertyChanged ("ClientId", old_value, new_value);
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
					object old_value = this.is_server;
					object new_value = value;
					
					this.is_server = value;
					
					this.NotifyPropertyChanged ("IsServer", old_value, new_value);
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
					object old_value = this.sync_log_id;
					object new_value = value;
					
					this.sync_log_id = value;
					
					this.NotifyPropertyChanged ("SyncLogId", old_value, new_value);
				}
			}
		}
		
		
		private int								client_id;
		private bool							is_server;
		private long							sync_log_id;
	}
}
