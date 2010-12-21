//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Services;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{


	/// <summary>
	/// The <c>ConnectionInformation</c> class provides access to the data about high level
	/// connections to the database.
	/// Once it has been opened with the Open() method, it must be kept alive regularly with calls
	/// to the KeepAlive() method until it is closed by the Close() method. A connection has a status
	/// which indicates its state. This status might get out of synchronization with the database and
	/// can be refresh with the RefreshStatus() method.
	/// </summary>
	public sealed class ConnectionInformation
	{
		
		
		/// <summary>
		/// Creates a new <c>ConnectionInformation</c>.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> object used to communicate with the database.</param>
		/// <param name="identity">The identity that describes the connection.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dbInfrastructure"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="identity"/> is <c>null</c> or empty.</exception>
		internal ConnectionInformation(DbInfrastructure dbInfrastructure, string identity)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			identity.ThrowIfNullOrEmpty ("identity");

			this.dbInfrastructure = dbInfrastructure;
			this.connectionId = null;
			this.ConnectionIdentity = identity;
			this.Status = ConnectionStatus.NotYetOpen;
		}


		/// <summary>
		/// The id that uniquely identifies the connection.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">If the connection has never been opened.</exception>
		public DbId ConnectionId
		{
			get
			{
				if (!this.connectionId.HasValue)
				{
					throw new System.InvalidOperationException ("ConnectionId is not yet defined.");
				}

				return this.connectionId.Value;
			}
		}


		/// <summary>
		/// The identity that describes the connection.
		/// </summary>
		public string ConnectionIdentity
		{
			get;
			private set;
		}


		/// <summary>
		/// The status that defines the state in which the connection is.
		/// </summary>
		/// <remarks>
		/// The status might get out of synchronization with the real state in the database if the
		/// connection is interrupted. A call to RefrechStatus() will synchronize it.
		/// </remarks>
		public ConnectionStatus Status
		{
			get;
			private set;
		}


		/// <summary>
		/// The <see cref="DbConnectionManager"/> object used to access the low level connection
		/// data.
		/// </summary>
		private DbConnectionManager ConnectionManager
		{
			get
			{
				return this.dbInfrastructure.ServiceManager.ConnectionManager;
			}
		}


		/// <summary>
		/// Opens the connection.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">If the connection has already been opened.</exception>
		internal void Open()
		{
			if (this.Status != ConnectionStatus.NotYetOpen)
			{
				throw new System.InvalidOperationException ("Invalid status for operation.");
			}

			using (DbTransaction transaction = this.CreateWriteTransaction ())
			{
				this.connectionId = this.ConnectionManager.OpenConnection (this.ConnectionIdentity).Id;

				this.RefreshStatus ();

				transaction.Commit ();
			}
		}


		/// <summary>
		/// Closes the connection.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">If the connection is not open.</exception>
		internal void Close()
		{
			using (DbTransaction transaction = this.CreateWriteTransaction ())
			{
				this.RefreshStatus ();

				if (this.Status != ConnectionStatus.Open)
				{
					throw new System.InvalidOperationException ("Invalid status for operation.");
				}

				this.ConnectionManager.CloseConnection (this.ConnectionId);

				this.RefreshStatus ();

				transaction.Commit ();
			}
		}


		/// <summary>
		/// Keeps the connection alive, which avoids interruptions.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">If the connection is not open.</exception>
		internal void KeepAlive()
		{
			using (DbTransaction transaction = this.CreateWriteTransaction ())
			{
				this.RefreshStatus ();

				if (this.Status != ConnectionStatus.Open)
				{
					throw new System.InvalidOperationException ("Invalid status for operation.");
				}

				this.ConnectionManager.KeepConnectionAlive (this.ConnectionId);

				this.RefreshStatus ();

				transaction.Commit ();
			}
		}


		/// <summary>
		/// Refreshes the status of the connection with its current state in the database.
		/// </summary>
		internal void RefreshStatus()
		{
			if (this.connectionId.HasValue)
			{
				DbConnectionStatus status = this.ConnectionManager.GetConnection (this.ConnectionId).Status;

				this.Status = this.ConvertStatus (status);
			}
		}


		/// <summary>
		/// Kills the connections which are inactive for a given amount of time.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> object used to communicate with the database.</param>
		/// <param name="timeOutValue">The amount of time required by a connection to become considered as inactive.</param>
		/// <returns><c>true</c> if an inactive connection has been interrupted, <c>false</c> if no connection has been interrupted.</returns>
		/// <exception cref="System.ArgumentNullException">If <see cref="DbInfrastructure"/> is <c>null</c>.</exception>
		internal static bool InterruptDeadConnections(DbInfrastructure dbInfrastructure, System.TimeSpan timeOutValue)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");

			return dbInfrastructure.ServiceManager.ConnectionManager.InterruptDeadConnections (timeOutValue);
		}
		

		/// <summary>
		/// Converts a <see cref="DbConnectionStatus"/> to the equivalent <see cref="ConnectionStatus"/>.
		/// </summary>
		/// <param name="status">The <see cref="DbConnectionStatus"/> to convert.</param>
		/// <returns>The equivalent <see cref="ConnectionStatus"/>.</returns>
		private ConnectionStatus ConvertStatus(DbConnectionStatus status)
		{
			ConnectionStatus convertedStatus;

			switch (status)
			{
				case DbConnectionStatus.Open:
					convertedStatus = ConnectionStatus.Open;
					break;

				case DbConnectionStatus.Closed:
					convertedStatus =  ConnectionStatus.Closed;
					break;

				case DbConnectionStatus.Interrupted:
					convertedStatus =  ConnectionStatus.Interrupted;
					break;

				default:
					throw new System.NotSupportedException ();
			}

			return convertedStatus;
		}


		/// <summary>
		/// Creates the <see cref="DbTransaction"/> that must be used for the read write transaction
		/// with the database.
		/// </summary>
		/// <returns>The <see cref="DbTransaction"/> object.</returns>
		private DbTransaction CreateWriteTransaction()
		{
			List<DbTable> tablesToLock = new List<DbTable> ()
			{
				dbInfrastructure.ResolveDbTable (Tags.TableConnection),
			};

			return dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite, tablesToLock);
		}


		/// <summary>
		/// The id of the connection with the database.
		/// </summary>
		private DbId? connectionId;


		/// <summary>
		/// The <see cref="DbInfrastructure"/> object used to communicate with the database.
		/// </summary>
		private DbInfrastructure dbInfrastructure;


	}


}