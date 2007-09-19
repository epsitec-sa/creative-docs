//	Copyright � 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		/// Gets a value indicating whether this accessor is taking data from
		/// resources based on a patch module.
		/// </summary>
		/// <value><c>true</c> if the data are based on a patch module; otherwise, <c>false</c>.</value>
		bool BasedOnPatchModule
		{
			get;
		}

		/// <summary>
		/// Loads resources from the specified resource manager. The resource
		/// manager will be used for all upcoming accesses.
		/// </summary>
		/// <param name="manager">The resource manager.</param>
		void Load(ResourceManager manager);

		/// <summary>
		/// Saves the resources.
		/// </summary>
		/// <param name="saverCallback">The saver callback.</param>
		void Save(ResourceBundleSaver saverCallback);

		/// <summary>
		/// Gets the data broker associated with the specified field. Usually,
		/// this is only meaningful if the field defines a collection of
		/// <see cref="StructuredData"/> items.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="fieldId">The id for the field in the specified container.</param>
		/// <returns>The data broker or <c>null</c>.</returns>
		IDataBroker GetDataBroker(Types.StructuredData container, string fieldId);

		/// <summary>
		/// Gets a list of all available cultures for the specified accessor.
		/// </summary>
		/// <returns>A list of two letter ISO language names.</returns>
		IList<string> GetAvailableCultures();

		/// <summary>
		/// Gets a value indicating whether this accessor contains changes.
		/// </summary>
		/// <value><c>true</c> if this accessor contains changes; otherwise, <c>false</c>.</value>
		bool ContainsChanges
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
		/// Reverts the changes applied to the accessor.
		/// </summary>
		/// <returns>The number of items which have been reverted.</returns>
		int RevertChanges();

		/// <summary>
		/// Notifies the resource accessor that the specified item changed.
		/// </summary>
		/// <param name="item">The item which was modified.</param>
		void NotifyItemChanged(CultureMap item);

		/// <summary>
		/// Notifies the resource accessor that the specified culture data was
		/// just cleared.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		/// <param name="data">The data which is cleared.</param>
		void NotifyCultureDataCleared(CultureMap item, string twoLetterISOLanguageName, Types.StructuredData data);

		/// <summary>
		/// Loads the data for the specified culture into an existing item.
		/// </summary>
		/// <param name="item">The item to update.</param>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		/// <returns>The data loaded from the resources which was stored in the specified item.</returns>
		Types.StructuredData LoadCultureData(CultureMap item, string twoLetterISOLanguageName);
	}
}
