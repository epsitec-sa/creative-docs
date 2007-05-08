//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>IResourceAccessor</c> interface provides a unified access to very
	/// different resources.
	/// </summary>
	public interface IResourceAccessor
	{
		/// <summary>
		/// Gets the collection of <see cref="CultureMap"/> items.
		/// </summary>
		/// <value>The collection of <see cref="CultureMap"/> items.</value>
		CultureMapList Collection
		{
			get;
		}

		/// <summary>
		/// Gets the data broker associated with the root data stored in the
		/// collection.
		/// </summary>
		/// <value>The data broker.</value>
		IDataBroker DataBroker
		{
			get;
		}

		/// <summary>
		/// Creates a new item which can then be added to the collection.
		/// </summary>
		/// <returns>A new <see cref="CultureMap"/> item.</returns>
		CultureMap CreateItem();

		/// <summary>
		/// Persists the changes to the underlying data store.
		/// </summary>
		/// <returns>The number of items which have been persisted.</returns>
		int PersistChanges();

		/// <summary>
		/// Notifies the resource accessor that the specified item changed.
		/// </summary>
		/// <param name="item">The item which was modified.</param>
		void NotifyItemChanged(CultureMap item);


		Types.StructuredData LoadCultureData(CultureMap item, string twoLetterISOLanguageName);
	}
}
