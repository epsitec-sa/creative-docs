//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbClientRecord</c> class represents a database client. The records
	/// are managed by <see cref="DbClientManager"/>.
	/// </summary>
	public sealed class DbClientRecord
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbClientRecord"/> class.
		/// </summary>
		public DbClientRecord()
			: this ("", 0, 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbClientRecord"/> class.
		/// </summary>
		/// <param name="clientName">Name of the client.</param>
		/// <param name="clientId">The client id.</param>
		/// <param name="syncLogId">The synchronization log id.</param>
		public DbClientRecord(string clientName, int clientId, long syncLogId)
		{
			this.clientName         = clientName;
			this.clientId           = clientId;
			this.syncLogId          = syncLogId;
			this.creationDateTime   = System.DateTime.UtcNow;
			this.connectionDateTime = this.creationDateTime;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbClientRecord"/> class.
		/// </summary>
		/// <param name="clientName">Name of the client.</param>
		/// <param name="clientId">The client id.</param>
		/// <param name="syncLogId">The synchronization log id.</param>
		/// <param name="creationDateTime">The creation date time.</param>
		/// <param name="connectionDateTime">The connection date time.</param>
		public DbClientRecord(string clientName, int clientId, long syncLogId, System.DateTime creationDateTime, System.DateTime connectionDateTime)
		{
			this.clientName         = clientName;
			this.clientId           = clientId;
			this.syncLogId          = syncLogId;
			this.creationDateTime   = creationDateTime;
			this.connectionDateTime = connectionDateTime;
		}


		/// <summary>
		/// Gets the name of the client.
		/// </summary>
		/// <value>The name of the client.</value>
		public string							ClientName
		{
			get
			{
				return this.clientName;
			}
		}

		/// <summary>
		/// Gets the client id.
		/// </summary>
		/// <value>The client id.</value>
		public int								ClientId
		{
			get
			{
				return this.clientId;
			}
		}

		/// <summary>
		/// Gets the synchronization log id.
		/// </summary>
		/// <value>The synchronization log id.</value>
		public long								SyncLogId
		{
			get
			{
				return this.syncLogId;
			}
		}

		/// <summary>
		/// Gets the creation date time.
		/// </summary>
		/// <value>The creation date time.</value>
		public System.DateTime					CreationDateTime
		{
			get
			{
				return this.creationDateTime;
			}
		}

		/// <summary>
		/// Gets the last connection date time.
		/// </summary>
		/// <value>The last connection date time.</value>
		public System.DateTime					ConnectionDateTime
		{
			get
			{
				return this.connectionDateTime;
			}
		}


		/// <summary>
		/// Saves client record to an empty row.
		/// </summary>
		/// <param name="row">The row.</param>
		internal void SaveToEmptyRow(System.Data.DataRow row)
		{
			row.BeginEdit ();
			row[Tags.ColumnClientName]    = this.ClientName;
			row[Tags.ColumnClientId]      = this.ClientId;
			row[Tags.ColumnClientSync]    = this.SyncLogId;
			row[Tags.ColumnClientCreDate] = this.CreationDateTime;
			row[Tags.ColumnClientConDate] = this.ConnectionDateTime;
			row.EndEdit ();
		}

		/// <summary>
		/// Updates the row based on the client record.
		/// </summary>
		/// <param name="row">The row.</param>
		internal void UpdateRow(System.Data.DataRow row)
		{
			row.BeginEdit ();
			row[Tags.ColumnClientSync]    = this.SyncLogId;
			row[Tags.ColumnClientConDate] = this.ConnectionDateTime;
			row.EndEdit ();
		}

		/// <summary>
		/// Loads the client record from the row.
		/// </summary>
		/// <param name="row">The row.</param>
		internal void LoadFromRow(System.Data.DataRow row)
		{
			this.clientName         = InvariantConverter.ToString (row[Tags.ColumnClientName]);
			this.clientId           = InvariantConverter.ToInt (row[Tags.ColumnClientId]);
			this.syncLogId          = InvariantConverter.ToLong (row[Tags.ColumnClientSync]);
			this.creationDateTime   = InvariantConverter.ToDateTime (row[Tags.ColumnClientCreDate]);
			this.connectionDateTime = InvariantConverter.ToDateTime (row[Tags.ColumnClientConDate]);
		}
		
		private string							clientName;
		private int								clientId;
		private long							syncLogId;
		private System.DateTime					creationDateTime;
		private System.DateTime					connectionDateTime;
	}
}
