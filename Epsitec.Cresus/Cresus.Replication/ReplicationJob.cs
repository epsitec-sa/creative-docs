//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Replication
{
	/// <summary>
	/// The <c>ReplicationJob</c> class represents a single replication job.
	/// </summary>
	public sealed class ReplicationJob : Remoting.AbstractOperation
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReplicationJob"/> class.
		/// </summary>
		public ReplicationJob()
		{
			this.SetLastExpectedStep (1);
		}


		/// <summary>
		/// Gets or sets the client indentity.
		/// </summary>
		/// <value>The client identity.</value>
		public Remoting.ClientIdentity Client
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the synchronization start id.
		/// </summary>
		/// <value>The synchronization start id.</value>
		public Database.DbId SyncStartId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the synchronization end id.
		/// </summary>
		/// <value>The synchronization end id.</value>
		public Database.DbId SyncEndId
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the replication data, produced by the server replication engine.
		/// </summary>
		/// <value>The replication data or <c>null</c>.</value>
		public byte[]							Data
		{
			get
			{
				return this.syncData;
			}
		}

		/// <summary>
		/// Gets the replication error.
		/// </summary>
		/// <value>The replication error or <c>null</c>.</value>
		public string							Error
		{
			get
			{
				return this.syncError;
			}
		}

		/// <summary>
		/// Gets or sets the pull arguments.
		/// </summary>
		/// <value>The pull arguments.</value>
		public PullArguments PullArguments
		{
			get;
			set;
		}
		
		
		
		internal void WaitForReady()
		{
			this.WaitForProgress (100);
		}
		
		internal void NotifyStartProcessing()
		{
			this.syncData  = null;
			this.syncError = null;
			
			this.SetCurrentStep (0);
		}
		
		internal void NotifyDoneProcessing(byte[] data)
		{
			this.syncData  = data;
			this.syncError = null;
			
			this.SetCurrentStep (1);
			this.SetProgress (100);
		}
		
		internal void NotifyError(string message)
		{
			this.syncData  = null;
			this.syncError = message;
			
			this.SetFailed (message);
		}
		
		
		
		private byte[]							syncData;
		private string							syncError;
	}
}
