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

		internal bool Acquire(DataInfrastructure dataInfrastructure)
		{
			return dataInfrastructure.TryCreateLockTransaction (this.lockNames, out this.lockTransaction);
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (this.lockTransaction != null)
			{
				this.lockTransaction.Dispose ();
				this.lockTransaction = null;
			}
		}

		#endregion

		private readonly List<string> lockNames;
		private LockTransaction lockTransaction;
	}
}
