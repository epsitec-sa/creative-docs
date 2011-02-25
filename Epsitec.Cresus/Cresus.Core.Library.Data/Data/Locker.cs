//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	using DataInfrastructure=Epsitec.Cresus.DataLayer.Infrastructure.DataInfrastructure;
	using Timer=Epsitec.Common.Widgets.Timer;
	
	/// <summary>
	/// The <c>Locker</c> class is used to request a lock on one or several
	/// items. It returns a <see cref="LockTransaction"/> to describe the
	/// lock (which might or might not have been taken).
	/// </summary>
	public sealed class Locker : CoreDataComponent, System.IDisposable
	{
		public Locker(CoreData data)
			: base (data)
		{
			this.dataInfrastructure = this.Data.DataInfrastructure;
			this.lockMonitors = new WeakList<LockMonitor> ();
			this.lockMonitorTimer = new Timer ();
			this.lockMonitorTimer.TimeElapsed += this.HandleLockMonitorTimerTimeElapsed;
			this.lockMonitorTimer.Delay      = 0.5;
			this.lockMonitorTimer.AutoRepeat = 2.5;
		}

		
		public LockTransaction RequestLock(params string[] lockNames)
		{
			IList<LockOwner> foreignLockOwners;
			return this.RequestLock ((IEnumerable<string>) lockNames, out foreignLockOwners);
		}

		public LockTransaction RequestLock(IEnumerable<string> lockNames, out IList<LockOwner> foreignLockOwners)
		{
			var lockTransaction = new LockTransaction (this.dataInfrastructure, lockNames);
			
			if (lockTransaction.Acquire ())
			{
				foreignLockOwners = null;
				return lockTransaction;
			}
			else
			{
				foreignLockOwners = lockTransaction.ForeignLockOwners;
				lockTransaction.Dispose ();
				
				return null;
			}
		}
		
		public bool AreAllLocksAvailable(params string[] lockNames)
		{
			return this.AreAllLocksAvailable ((IEnumerable<string>) lockNames);
		}

		public bool AreAllLocksAvailable(IEnumerable<string> lockNames)
		{
			return this.dataInfrastructure.AreAllLocksAvailable (lockNames);
		}


		public LockMonitor CreateLockMonitor(params string[] lockNames)
		{
			return this.CreateLockMonitor ((IEnumerable<string>) lockNames);
		}

		public LockMonitor CreateLockMonitor(IEnumerable<string> lockNames)
		{
			return new LockMonitor (this.Data, lockNames);
		}


		internal void RegisterLockMonitor(LockMonitor lockMonitor)
		{
			lock (this.lockMonitors)
			{
				this.lockMonitors.Add (lockMonitor);
				this.ClearLockMonitorNames ();
				this.lockMonitorTimer.Start ();
			}
		}

		internal void UnregisterLockMonitor(LockMonitor lockMonitor)
		{
			lock (this.lockMonitors)
			{
				this.lockMonitors.Remove (lockMonitor);
				this.ClearLockMonitorNames ();
			}
		}

		
		public static string GetEntityLockName(DataContext context, AbstractEntity entity)
		{
			var key = context.GetNormalizedEntityKey (entity);

			if (key.HasValue)
			{
				return key.Value.ToString ();
			}
			else
			{
				return null;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion

		private void ClearLockMonitorNames()
		{
			this.lockMonitorNames = null;
		}

		private string[] GetLockMonitorNames()
		{
			if (this.lockMonitorNames == null)
			{
				this.lockMonitorNames = this.lockMonitors
					.SelectMany (x => x.LockNames)
					.Distinct ()
					.ToArray ();
			}

			return this.lockMonitorNames;
		}

		private void HandleLockMonitorTimerTimeElapsed(object sender)
		{
			string[]      names;
			LockMonitor[] monitors;

			lock (this.lockMonitors)
			{
				names = this.GetLockMonitorNames ();
				
				if (names.Length == 0)
				{
					this.lockMonitorTimer.Stop ();
					return;
				}

				monitors = this.lockMonitors.ToArray ();
			}

			var owners = new List<LockOwner> ();
			
			using (var lockTransaction = new LockTransaction (this.dataInfrastructure, names))
			{
				lockTransaction.Poll ();
				owners.AddRange (lockTransaction.ForeignLockOwners);
			}

			monitors.ForEach (x => x.UpdateLockState (owners));
		}


		private readonly DataInfrastructure		dataInfrastructure;
		private readonly WeakList<LockMonitor>	lockMonitors;
		private readonly Timer					lockMonitorTimer;

		private string[]						lockMonitorNames;
	}
}