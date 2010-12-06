//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	using LowLevelLockTransaction = Epsitec.Cresus.DataLayer.Infrastructure.LockTransaction;

	/// <summary>
	/// The <c>LockMonitor</c> class is used to monitor (in the background) the state of
	/// a set of locks.
	/// </summary>
	public sealed class LockMonitor : System.IDisposable
	{
		public LockMonitor(DataInfrastructure dataInfrastructure, IEnumerable<string> lockNames)
		{
			this.lockNames = new List<string> (lockNames);
			this.dataInfrastructure = dataInfrastructure;
		}


		public LockState LockState
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


		internal void UpdateLockState(IEnumerable<string> lockedItems)
		{
			if (lockedItems.Any (x => this.lockNames.Contains (x)))
			{
				this.lockState = DataLayer.Infrastructure.LockState.Locked;
			}
			else
			{
				this.lockState = DataLayer.Infrastructure.LockState.Idle;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.isDisposed = true;
		}

		#endregion

		private readonly List<string>			lockNames;
		private readonly DataInfrastructure		dataInfrastructure;

		private bool							isDisposed;
		private LockState						lockState;
	}
}
