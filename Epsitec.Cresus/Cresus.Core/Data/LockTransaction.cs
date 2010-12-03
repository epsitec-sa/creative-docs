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
	using LowLevelLockTransaction=Epsitec.Cresus.DataLayer.Infrastructure.LockTransaction;
	
	public sealed class LockTransaction : System.IDisposable
	{
		public LockTransaction(IEnumerable<string> lockNames)
		{
			this.lockNames = new List<string> (lockNames);
		}


		public LockState						LockSate
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

		/// <summary>
		/// Gets the identification of the connections who owned the locks of the current instance
		/// when the last try was made to acquire them. The data is returned as a mapping from the
		/// lock names to the connection identifications.
		/// </summary>
		/// <remarks>This data is only available after at least one try has been made to acquire the
		/// locks and if the last try has failed.</remarks>
		/// <exception cref="System.InvalidOperationException">If the data is not available in the current state of this instance.</exception>
		public Dictionary<string, string> LockOwners
		{
			get
			{
				if (this.locksData == null)
				{
					throw new System.InvalidOperationException ("Cannot execute this operation in the current state.");
				}
				
				return this.locksData
						.SelectMany (item => item.Value.Select (l => System.Tuple.Create (l.Item1, item.Key)))
						.ToDictionary (tuple => tuple.Item1, tuple => tuple.Item2);
			}
		}

		/// <summary>
		/// Gets the acquisition time of the locks of the current instance when the last try was
		/// made to acquire them. The data is returned as a mapping from the lock names to their
		/// creation time.
		/// </summary>
		/// <remarks>This data is only available after at least one try has been made to acquire the
		/// locks and if the last try has failed.</remarks>
		/// <exception cref="System.InvalidOperationException">If the data is not available in the current state of this instance.</exception>
		public Dictionary<string, System.DateTime> LockCreationTimes
		{
			get
			{
				if (this.locksData == null)
				{
					throw new System.InvalidOperationException ("Cannot execute this operation in the current state.");
				}
				
				return this.locksData
						.SelectMany (item => item.Value)
						.ToDictionary (tuple => tuple.Item1, tuple => tuple.Item2);
			}
		}
				
		internal bool Acquire(DataInfrastructure dataInfrastructure)
		{
			System.Diagnostics.Debug.Assert (this.lockTransaction == null);

			var lockTransaction = this.CreateLockTransaction (dataInfrastructure);

			if (lockTransaction.State == LockState.Locked)
			{
				this.locksData = null;
				this.lockTransaction = lockTransaction;
			}
			else
			{
				this.locksData = lockTransaction.GetLockOwners ();
				this.lockTransaction = null;

				lockTransaction.Dispose ();
			}

			return this.lockTransaction != null;
		}

		private LowLevelLockTransaction CreateLockTransaction(DataInfrastructure dataInfrastructure)
		{
			var lockTransaction = dataInfrastructure.CreateLockTransaction (this.lockNames);

			lockTransaction.Lock ();

			return lockTransaction;
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
		private LowLevelLockTransaction lockTransaction;
		private Dictionary<string, List<System.Tuple<string, System.DateTime>>> locksData;


		private bool isDisposed;
	}
}
