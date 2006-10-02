//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	using CollectionChangedEventHandler=Epsitec.Common.Support.EventHandler<CollectionChangedEventArgs>;

	/// <summary>
	/// The <c>INotifyCollectionChanged</c> interface defines a <c>CollectionChanged</c> event.
	/// </summary>
	public interface INotifyCollectionChanged
	{
		event CollectionChangedEventHandler CollectionChanged;
	}
}
