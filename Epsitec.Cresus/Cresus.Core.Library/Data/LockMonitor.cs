//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

[assembly: DependencyClass (typeof (LockMonitor))]

namespace Epsitec.Cresus.Core.Data
{
	using LowLevelLockTransaction = Epsitec.Cresus.DataLayer.Infrastructure.LockTransaction;

	/// <summary>
	/// The <c>LockMonitor</c> class is used to monitor (in the background) the state of
	/// a set of locks.
	/// </summary>
	public sealed class LockMonitor : DependencyObject, IDisposed, IIsDisposed
	{
		public LockMonitor(CoreData data, IEnumerable<string> lockNames)
		{
			this.lockNames          = lockNames.ToArray ();
			this.dataInfrastructure = data.DataInfrastructure;
			this.dataLocker         = data.DataLocker;

			if (this.lockNames.Length > 0)
			{
				this.dataLocker.RegisterLockMonitor (this);
			}
		}


		public LockState						LockState
		{
			get
			{
				if (this.isDisposed)
				{
					return LockState.Disposed;
				}

				return this.lockState;
			}
		}

		public IEnumerable<LockOwner>			LockOwners
		{
			get
			{
				return this.lockOwners as IEnumerable<LockOwner> ?? Enumerable.Empty<LockOwner> ();
			}
		}

		public IEnumerable<string>				LockNames
		{
			get
			{
				return this.lockNames;
			}
		}


		protected override void Dispose(bool disposing)
		{
			bool notify = false;

			if (disposing)
			{
				if (this.isDisposed == false)
				{
					this.isDisposed = true;
					this.dataLocker.UnregisterLockMonitor (this);
					notify = true;
				}
			}

			base.Dispose (disposing);

			if (notify)
			{
				this.OnDisposed ();
			}
		}


		#region IIsDisposed Members

		public bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		#endregion

		#region IDisposed Members

		public event EventHandler  Disposed;

		#endregion

		
		/// <summary>
		/// Updates the state of the lock based on a collection of lock owners.
		/// </summary>
		/// <param name="lockOwners">The lock owners.</param>
		internal void UpdateLockState(IEnumerable<LockOwner> lockOwners)
		{
			LockOwner[] conflicts = lockOwners
				.Where (x => this.lockNames.Contains (x.LockName))
				.ToArray ();

			if (conflicts.Length > 0)
			{
				this.lockOwners = conflicts;
				this.SetLockState (DataLayer.Infrastructure.LockState.Locked);
			}
			else
			{
				this.lockOwners = null;
				this.SetLockState (DataLayer.Infrastructure.LockState.Idle);
			}
		}

		private void SetLockState(LockState lockState)
		{
			if (this.lockState != lockState)
			{
				this.lockState = lockState;
				this.OnLockStateChanged ();
			}
		}

		private void OnDisposed()
		{
			this.Disposed.Raise (this);
		}

		private void OnLockStateChanged()
		{
			this.LockStateChanged.Raise (this);
		}


		public event EventHandler				LockStateChanged;

		public static readonly DependencyProperty LockMonitorProperty = DependencyProperty<LockMonitor>.RegisterAttached<LockMonitor> ("LockMonitor");

		private readonly string[]				lockNames;
		private readonly DataInfrastructure		dataInfrastructure;
		private readonly Locker					dataLocker;

		private bool							isDisposed;
		private LockState						lockState;
		private LockOwner[]						lockOwners;
	}
}
