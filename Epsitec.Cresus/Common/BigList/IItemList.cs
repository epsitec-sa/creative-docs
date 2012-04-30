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

		int										Count
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

		void Reset();
	}
}