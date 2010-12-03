//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.DataLayer.Infrastructure;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	using LowLevelLockTransaction=Epsitec.Cresus.DataLayer.Infrastructure.LockTransaction;
	
	public sealed class LockTransaction : System.IDisposable
	{
		public LockTransaction(DataInfrastructure dataInfrastructure, IEnumerable<string> lockNames)
		{
			this.dataInfrastructure = dataInfrastructure;
			this.lockNames = new List<string> (lockNames);
			this.lockTransaction = this.dataInfrastructure.CreateLockTransaction (this.lockNames);
		}

		/// <summary>
		/// Gets the current state of this instance.
		/// </summary>
		public LockState						LockSate
		{
			get
			{
				if (this.isDisposed)
				{
					return LockState.Disposed;
				}
				else
				{
					return this.lockTransaction.State;
				}
			}
		}

		/// <summary>
		/// Gets the identification of the connections who owned the locks of the current instance
		/// when the last call to <see cref="LockTransaction.Poll"/> was made. The data is returned
		/// as a mapping from the lock names to the connection identifications.
		/// </summary>
		/// <remarks>This data is only available after a call to <see cref="LockTransaction.Poll"/>
		/// has been made, and the data might be outdated if the real state of the locks has been
		/// changed in the database since that call.</remarks>
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
		/// Gets the acquisition time of the locks of the current instance when the last call to
		/// <see cref="LockTransaction.Poll"/> was made. The data is returned as a mapping from the
		/// lock names to their creation time.
		/// </summary>
		/// <remarks>This data is only available after a call to <see cref="LockTransaction.Poll"/>
		/// has been made, and the data might be outdated if the real state of the locks has been
		/// changed in the database since that call.</remarks>
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
		
		/// <summary>
		/// Tries to acquire the locks of the current instance.
		/// </summary>
		/// <returns><c>true</c> if the locks have been acquired, <c>false</c> if they don't.</returns>
		/// <exception cref="System.InvalidOperationException">If the locks have already been acquired by this instance or if this instance has been disposed.</exception>
		internal bool Acquire()
		{
			if (this.LockSate != LockState.Idle)
			{
				throw new System.InvalidOperationException ("Cannot execute this operation in the current state.");
			}
			
			return this.lockTransaction.Lock ();
		}

		/// <summary>
		/// Checks if the locks of the current instance can be acquired by the connection of the
		/// current instance and populates the fields with the data about the lock owners and
		/// creation time
		/// </summary>
		/// <returns><c>true</c> if the locks can be acquired (or have been acquired), <c>false</c> if they cannot.</returns>
		/// <exception cref="System.InvalidOperationException">If this instance has been disposed.</exception>
		internal bool Poll()
		{
			if (this.LockSate == LockState.Disposed)
			{
				throw new System.InvalidOperationException ("Cannot execute this operation in the current state.");
			}

			this.locksData = this.lockTransaction.GetLockOwners ();

			string id = this.dataInfrastructure.ConnectionInformation.ConnectionIdentity;

			return !this.locksData.Any ()
				|| (this.locksData.Count == 1 && this.locksData.ContainsKey (id));
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

		private readonly DataInfrastructure dataInfrastructure;
		private readonly List<string> lockNames;
		private LowLevelLockTransaction lockTransaction;
		private Dictionary<string, List<System.Tuple<string, System.DateTime>>> locksData;
		
		private bool isDisposed;
	}
}
