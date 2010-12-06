//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	using DataInfrastructure=Epsitec.Cresus.DataLayer.Infrastructure.DataInfrastructure;
	
	/// <summary>
	/// The <c>Locker</c> class is used to request a lock on one or several
	/// items. It returns a <see cref="LockTransaction"/> to describe the
	/// lock (which might or might not have been taken).
	/// </summary>
	public sealed class Locker : System.IDisposable
	{
		public Locker(DataInfrastructure dataInfrastructure)
		{
			this.dataInfrastructure = dataInfrastructure;
		}

		
		public void Validate()
		{
			if (!this.isReady)
			{
				this.isReady = true;
			}
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
			return new LockMonitor (this.dataInfrastructure, lockNames);
		}

		
		internal static string GetEntityLockName(DataContext context, AbstractEntity entity)
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
		
		private readonly DataInfrastructure		dataInfrastructure;

		private bool isReady;
	}
}