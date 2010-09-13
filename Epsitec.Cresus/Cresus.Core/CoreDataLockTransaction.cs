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

namespace Epsitec.Cresus.Core
{
	public sealed class CoreDataLockTransaction : System.IDisposable
	{
		public CoreDataLockTransaction(IEnumerable<string> lockNames)
		{
			this.lockNames = new List<string> (lockNames);
		}


		public LockState LockSate
		{
			get
			{
				if (this.isDisposed)
                {
					return LockState.Disposed;
                }

				if (this.lockTransaction == null)
				{
					return LockState.Idle;
				}
				else
				{
					return LockState.Locked;
				}
			}
		}

		
		internal bool Acquire(DataInfrastructure dataInfrastructure)
		{
			System.Diagnostics.Debug.Assert (this.lockTransaction == null);

			this.lockTransaction = this.CreateLockTransaction (dataInfrastructure);

			return this.lockTransaction != null;
		}

		private LockTransaction CreateLockTransaction(DataInfrastructure dataInfrastructure)
		{
			var lockTransaction = dataInfrastructure.CreateLockTransaction (this.lockNames);

			lockTransaction.Lock ();

			if (lockTransaction.State == LockState.Locked)
			{
				return lockTransaction;
			}

			lockTransaction.Dispose ();
			return null;
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (this.lockTransaction != null)
			{
				this.lockTransaction.Dispose ();
				this.lockTransaction = null;
			}

			this.isDisposed = true;
		}

		#endregion

		private readonly List<string> lockNames;
		private LockTransaction lockTransaction;

		private bool isDisposed;
	}
}
