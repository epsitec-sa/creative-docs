//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Widgets.Tiles;

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
