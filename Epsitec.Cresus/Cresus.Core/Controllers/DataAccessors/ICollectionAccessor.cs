//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public interface ICollectionAccessor
	{
		void InsertItem(int index, AbstractEntity item);
		void AddItem(AbstractEntity item);
		bool RemoveItem(AbstractEntity item);

		/// <summary>
		/// Gets the item collection.
		/// </summary>
		/// <returns>The item collection.</returns>
		System.Collections.IList GetItemCollection();
	}
}
