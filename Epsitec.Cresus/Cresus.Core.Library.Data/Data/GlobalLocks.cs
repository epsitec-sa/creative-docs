//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data
{
	/// <summary>
	/// The <c>GlobalLocks</c> class provides the <see cref="GlobalLock"/> singletons which
	/// describe global locks.
	/// </summary>
	public static class GlobalLocks
	{
		public static readonly GlobalLock UserManagement = new GlobalLock ("Global:UserManagement");
	}
}
