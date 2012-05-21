//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>IReadOnlyLock</c> interface can be used to lock/unlock a collection
	/// in order to make it read-only.
	/// </summary>
	public interface IReadOnlyLock : IReadOnly
	{
		void Lock();
		void Unlock();
	}
}
