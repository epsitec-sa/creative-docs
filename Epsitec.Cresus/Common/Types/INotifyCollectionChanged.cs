//	Copyright © 2006-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
