//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{


	public sealed class ConnectionInformation
	{

		// TODO Comment this class.
		// Marc


		internal ConnectionInformation(DbInfrastructure dbInfrastructure, string identity)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			identity.ThrowIfNullOrEmpty ("identity");

			this.dbInfrastructure = dbInfrastructure;
			this.connectionId = null;
			this.ConnectionIdentity = identity;
			this.Status = ConnectionStatus.NotYetOpen;
		}


		public long ConnectionId
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


		public string ConnectionIdentity
		{
			get;
			private set;
		}


		public ConnectionStatus Status
		{
			get;
			private set;
		}


		private DbConnectionManager ConnectionManager
		{
			get
			{
				return this.dbInfrastructure.ConnectionManager;
			}
		}


		internal void Open()
		{
			if (this.Status != ConnectionStatus.NotYetOpen)
			{
				throw new System.InvalidOperationException ("Invalid status for operation.");
			}

			using (DbTransaction transaction = this.CreateWriteTransaction ())
			{
				this.connectionId = this.ConnectionManager.OpenConnection (this.ConnectionIdentity);

				this.RefreshStatus ();

				transaction.Commit ();
			}
		}


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


		internal void RefreshStatus()
		{
			if (this.connectionId.HasValue)
			{
				DbConnectionStatus status = this.ConnectionManager.GetConnectionStatus (this.ConnectionId);

				this.Status = this.ConvertStatus (status);
			}
		}


		internal static bool InterruptDeadConnections(DbInfrastructure dbInfrastructure)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");

			return dbInfrastructure.ConnectionManager.InterruptDeadConnections ();
		}
		

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


		private DbTransaction CreateWriteTransaction()
		{
			List<DbTable> tablesToLock = new List<DbTable> ()
			{
				dbInfrastructure.ResolveDbTable (Tags.TableConnection),
			};

			return dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite, tablesToLock);
		}


		private long? connectionId;


		private DbInfrastructure dbInfrastructure;


	}


}