//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.DataLayer.Infrastructure;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Factories;

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
				this.DataInfrastructure.RefreshConnectionData ();
				
				var info = this.DataInfrastructure.Connection;
				
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

		private DataInfrastructure DataInfrastructure
		{
			get
			{
				return this.Host.DataInfrastructure;
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
			this.DataInfrastructure.OpenConnection (identity.ToString ());
			
			var databaseTime = this.DataInfrastructure.GetDatabaseTime ();
			var localAppTime = System.DateTime.Now;

			this.timeOffset = localAppTime - databaseTime;

			this.isReady = true;
		}

		private void CloseConnection()
		{
			this.isReady = false;
			this.DataInfrastructure.CloseConnection ();
		}


		private ConnectionUserIdentity GetIdentity()
		{
			return new ConnectionUserIdentity (this.Host.GetActiveUserItemCode ());
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
//-			System.Diagnostics.Debug.WriteLine ("KeepAlive pulsed");

			this.DataInfrastructure.KeepConnectionAlive ();

			if (this.Host.EnableConnectionRecycling)
			{
				this.DataInfrastructure.KillDeadConnections(System.TimeSpan.FromSeconds(30));
			}
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


		#region Factory Class

		private sealed class Factory : ICoreDataComponentFactory
		{
			#region ICoreDataComponentFactory Members

			public bool CanCreate(CoreData data)
			{
				return data.IsReady;
			}

			public CoreDataComponent Create(CoreData data)
			{
				return new ConnectionManager (data);
			}

			public System.Type GetComponentType()
			{
				return typeof (ConnectionManager);
			}

			#endregion
		}

		#endregion

		private static readonly double KeepAlivePeriodInSeconds = 10.0;

		private readonly Timer					keepAliveTimer;
		
		private bool							isReady;
		private System.TimeSpan					timeOffset;
	}
}