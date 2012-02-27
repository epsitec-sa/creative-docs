//	Copyright © 2010-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors
{
	/// <summary>
	/// The <c>ICollectionAccessor</c> interface is used to access items in an entity
	/// collection.
	/// </summary>
	public interface ICollectionAccessor : IReadOnly
	{
		/// <summary>
		/// Inserts the item at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="item">The item.</param>
		void InsertItem(int index, AbstractEntity item);

		/// <summary>
		/// Adds the item at the end of the collection and returns its index.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns>The index of the inserted item.</returns>
		int AddItem(AbstractEntity item);

		/// <summary>
		/// Replaces the item with another one. If the replacement item is <c>null</c>, then
		/// this is equivalent to <see cref="RemoveItem"/>.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <param name="replacementItem">The replacement item.</param>
		/// <returns><c>true</c> if the item could be found and replaced; otherwise, <c>false</c>.</returns>
		bool ReplaceItem(AbstractEntity item, AbstractEntity replacementItem);

		/// <summary>
		/// Removes the first occurrence of the item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns><c>true</c> if the item was removed; otherwise, <c>false</c>.</returns>
		bool RemoveItem(AbstractEntity item);

		/// <summary>
		/// Gets the item collection.
		/// </summary>
		/// <returns>The item collection.</returns>
		IEnumerable<AbstractEntity> GetItemCollection();
	}
}
