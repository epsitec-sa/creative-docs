//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	internal class ReadWriteLock : System.Threading.ReaderWriterLockSlim
	{
		public ReadWriteLock()
			: base (System.Threading.LockRecursionPolicy.NoRecursion)
		{
		}
	};
}
