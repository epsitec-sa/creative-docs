//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>INotifyCollectionChangedProvider</c> interface gives access to
	/// the <see cref="INotifyCollectionChanged"/> interface.
	/// </summary>
	public interface INotifyCollectionChangedProvider
	{
		/// <summary>
		/// Gets the <see cref="INotifyCollectionChanged"/> interface which can
		/// be used to get the <c>CollectionChanged</c> events for the source.
		/// </summary>
		/// <returns>The <see cref="INotifyCollectionChanged"/> interface.</returns>
		INotifyCollectionChanged GetNotifyCollectionChangedSource();
	}
}
