//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Core.Business
{
	/// <summary>
	/// The <c>LockingPolicy</c> enumeration is used by <see cref="IBusinessContext.SaveChanges"/>
	/// to decide what to do with the locks.
	/// </summary>
	public enum LockingPolicy
	{
		KeepLock,
		ReleaseLock,
	}
}
