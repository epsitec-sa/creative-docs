//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	/// <summary>
	/// The <c>IItemList</c> interface defines the basic properties of an item list.
	/// </summary>
	public interface IItemList
	{
		ItemListFeatures						Features
		{
			get;
		}

		IList<ItemListMark>						Marks
		{
			get;
		}

		int										ItemCount
		{
			get;
		}

		int										ActiveIndex
		{
			get;
			set;
		}

		int										FocusedIndex
		{
			get;
			set;
		}

		ItemListSelection						Selection
		{
			get;
		}

		ItemCache								Cache
		{
			get;
		}

		/// <summary>
		/// Resets the list active index, focused index and visible frame, if any. This does
		/// not affect the item cache.
		/// </summary>
		void ResetList();
	}
}