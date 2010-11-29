//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	public sealed class GlobalLock : System.IEquatable<GlobalLock>
	{
		public GlobalLock(string lockId)
		{
			if (lockId == null)
			{
				throw new System.ArgumentNullException ("lockId");
			}

			this.lockId = lockId;
		}


		public string LockId
		{
			get
			{
				return this.lockId;
			}
		}


		#region IEquatable<GlobalLock> Members

		public bool Equals(GlobalLock other)
		{
			return (other != null) && (this.lockId == other.lockId);
		}

		#endregion

		public override string ToString()
		{
			return this.lockId;
		}

		public override int GetHashCode()
		{
			return this.lockId.GetHashCode ();
		}

		public override bool Equals(object obj)
		{
			if (obj is GlobalLock)
			{
				return this.Equals ((GlobalLock) obj);
			}

			return false;
		}
		
		private readonly string lockId;
	}
}
