//	Copyright © 2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>INotifyCurrentChanged</c> interface defines a <c>CurrentChanged</c> event.
	/// </summary>
	public interface INotifyCurrentChanged
	{
		/// <summary>
		/// Occurs when the current item in a collection changes.
		/// <remarks>Subscribing to this event is thread safe.</remarks>
		/// </summary>
		event Support.EventHandler CurrentChanged;

		/// <summary>
		/// Occurs when the current item in a collection is about to change.
		/// <remarks>Subscribing to this event is thread safe.</remarks>
		/// </summary>
		event Support.EventHandler<CurrentChangingEventArgs> CurrentChanging;
	}
}
