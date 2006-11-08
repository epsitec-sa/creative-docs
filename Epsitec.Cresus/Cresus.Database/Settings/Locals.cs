//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Settings
{
	/// <summary>
	/// The <c>Locals</c> class stores local settings, backed by a database
	/// table.
	/// </summary>
	public sealed class Locals : AbstractBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Locals"/> class.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="transaction">The transaction.</param>
		internal Locals(DbInfrastructure infrastructure, DbTransaction transaction)
		{
			this.AttachAndLoad (infrastructure, transaction, Locals.Name);
		}

		/// <summary>
		/// Gets or sets the client id.
		/// </summary>
		/// <value>The client id.</value>
		public int								ClientId
		{
			get
			{
				return this.clientId;
			}
			set
			{
				if (this.clientId != value)
				{
					object oldValue = this.clientId;
					object newValue = value;
					
					this.clientId = value;
					
					this.NotifyPropertyChanged ("ClientId", oldValue, newValue);
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this is a server.
		/// </summary>
		/// <value><c>true</c> if this is a server; otherwise, <c>false</c>.</value>
		public bool								IsServer
		{
			get
			{
				return this.isServer;
			}
			set
			{
				if (this.isServer != value)
				{
					object oldValue = this.isServer;
					object newValue = value;
					
					this.isServer = value;
					
					this.NotifyPropertyChanged ("IsServer", oldValue, newValue);
				}
			}
		}

		/// <summary>
		/// Gets or sets the synchronization log id.
		/// </summary>
		/// <value>The synchronization log id.</value>
		public long								SyncLogId
		{
			get
			{
				return this.syncLogId;
			}
			set
			{
				if (this.syncLogId != value)
				{
					object oldValue = this.syncLogId;
					object newValue = value;
					
					this.syncLogId = value;
					
					this.NotifyPropertyChanged ("SyncLogId", oldValue, newValue);
				}
			}
		}
		
		internal static readonly string			Name = "CR_SETTINGS_LOCALS";
		
		private int								clientId;
		private bool							isServer;
		private long							syncLogId;
	}
}
