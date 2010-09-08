//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public sealed class CoreDataConnectionManager : System.IDisposable
	{
		public CoreDataConnectionManager(DataLayer.Infrastructure.DataInfrastructure dataInfrastructure)
		{
			this.dataInfrastructure = dataInfrastructure;
			
			this.keepAliveTimer = new Timer ()
			{
				AutoRepeat = CoreDataConnectionManager.KeepAlivePeriodInSeconds,
				Delay = CoreDataConnectionManager.KeepAlivePeriodInSeconds,
			};

			this.keepAliveTimer.TimeElapsed += this.HandleKeepAliveTimerTimeElapsed;
		}


		public void Validate()
		{
			this.StartTimerIfNotRunning ();
			this.KeepAliveConnection ();
		}


		#region IDisposable Members

		public void Dispose()
		{
			if (this.keepAliveTimer.State != TimerState.Disposed)
			{
				this.keepAliveTimer.TimeElapsed -= this.HandleKeepAliveTimerTimeElapsed;
				this.keepAliveTimer.Dispose ();
			}
		}

		#endregion

		private void KeepAliveConnection()
		{
			this.dataInfrastructure.KeepAlive ();
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

		private readonly DataLayer.Infrastructure.DataInfrastructure dataInfrastructure;
		private readonly Timer keepAliveTimer;
	}
}
