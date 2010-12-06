//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	public sealed class LockOwner
	{
		public LockOwner(DataLayer.Infrastructure.LockOwner owner)
		{
			this.User = ConnectionUserIdentity.Parse (owner.ConnectionIdentity);
			this.LockName = owner.LockName;
		}

		public ConnectionUserIdentity User
		{
			get;
			private set;
		}
		public string LockName
		{
			get;
			private set;
		}
	}
}
