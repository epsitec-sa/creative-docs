//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	public interface ISuspendCollectionChanged
	{
		/// <summary>
		/// Temporarily disables all change notifications. Any changes which
		/// happen until <c>Dispose</c> is called on the returned object will
		/// not generate events until they are re-enabled; they will be fired
		/// at that moment (and only once).
		/// </summary>
		/// <returns>An object you will have to <c>Dispose</c> in order to re-enable
		/// the notifications and fire the accumulated events.</returns>
		System.IDisposable SuspendNotifications();
	}
}
