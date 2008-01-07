//	Copyright © 2006-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>INotifyCollectionChanged</c> interface defines a <c>CollectionChanged</c> event.
	/// </summary>
	public interface INotifyCollectionChanged
	{
		/// <summary>
		/// Occurs when the collection changes, either by adding, replacing or
		/// removing items.
		/// </summary>
		event EventHandler<CollectionChangedEventArgs> CollectionChanged;
	}
}
