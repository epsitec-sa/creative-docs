//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.DataLayer.Infrastructure;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>CoreDataConnectionManager</c> class maintains an active connection with
	/// the underlying database. Typically, it updates the connection state periodically.
	/// </summary>
	public sealed class ConnectionManager : CoreDataComponent, System.IDisposable
	{
		public ConnectionManager(CoreData data)
			: base (data)
		{
			this.dataInfrastructure = data.DataInfrastructure;
			
			this.keepAliveTimer = new Timer ()
			{
				AutoRepeat = ConnectionManager.KeepAlivePeriodInSeconds,
				Delay = ConnectionManager.KeepAlivePeriodInSeconds,
			};

			this.keepAliveTimer.TimeElapsed += this.HandleKeepAliveTimerTimeElapsed;
		}


		public bool								IsActive
		{
			get
			{
				this.dataInfrastructure.RefreshConnectionInformation ();
				
				var info = this.dataInfrastructure.ConnectionInformation;
				
				return info != null && info.Status == ConnectionStatus.Open;
			}
		}

		public bool								IsReady
		{
			get
			{
				return this.isReady;
			}
		}

		public System.TimeSpan					TimeOffset
		{
			get
			{
				return this.timeOffset;
			}
		}


		public override void ExecuteSetupPhase()
		{
			base.ExecuteSetupPhase ();
			this.Validate ();
		}

		/// <summary>
		/// Validates the connection once the database infrastructure is ready to be
		/// used. This will start the keep alive timer which pulses with a 10 second
		/// period.
		/// </summary>
		public void Validate()
		{
			if (!this.isReady)
			{
				this.OpenConnection ();
				this.StartTimerIfNotRunning ();
				this.KeepAliveConnection ();
			}
		}

		public void ReopenConnection()
		{
			if (this.isReady)
			{
				this.CloseConnection ();
				this.OpenConnection ();
			}
		}

		private void OpenConnection()
		{
			var identity = this.GetIdentity ();
			this.dataInfrastructure.OpenConnection (identity.ToString ());
			
			var databaseTime = this.dataInfrastructure.DbInfrastructure.GetDatabaseTime ();
			var localAppTime = System.DateTime.Now;

			this.timeOffset = localAppTime - databaseTime;

			this.isReady = true;
		}

		private void CloseConnection()
		{
			this.isReady = false;
			this.dataInfrastructure.CloseConnection ();
		}


		private ConnectionUserIdentity GetIdentity()
		{
			return new ConnectionUserIdentity (this.Data.GetActiveUserItemCode ());
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (this.keepAliveTimer.State != TimerState.Disposed)
			{
				this.keepAliveTimer.TimeElapsed -= this.HandleKeepAliveTimerTimeElapsed;
				this.keepAliveTimer.Dispose ();
			}
			
			this.CloseConnection ();
		}

		#endregion

		private void KeepAliveConnection()
		{
			this.dataInfrastructure.KeepConnectionAlive ();
		}

		private void StartTimerIfNotRunning()
		{
			if (this.keepAliveTimer.State == TimerState.Invalid)
			{
				this.keepAliveTimer.Start ();
			}
		}
		
		private void HandleKeepAliveTimerTimeElapsed(object sender)
		{
			this.KeepAliveConnection ();
		}


		private static readonly double KeepAlivePeriodInSeconds = 10.0;

		private readonly DataInfrastructure		dataInfrastructure;
		private readonly Timer					keepAliveTimer;
		
		private bool							isReady;
		private System.TimeSpan					timeOffset;
	}
}