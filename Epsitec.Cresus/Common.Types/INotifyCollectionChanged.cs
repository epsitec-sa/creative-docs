//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	using CollectionChangedEventHandler=Epsitec.Common.Support.EventHandler<CollectionChangedEventArgs>;

	/// <summary>
	/// The <c>INotifyCollectionChanged</c> interface defines a <c>CollectionChanged</c> event.
	/// </summary>
	public interface INotifyCollectionChanged
	{
		/// <summary>
		/// Occurs when the collection changes, either by adding or removing items.
		/// </summary>
		event CollectionChangedEventHandler CollectionChanged;
	}
}
