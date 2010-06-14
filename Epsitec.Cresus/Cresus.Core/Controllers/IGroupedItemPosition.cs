//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	public interface IGroupedItemPosition
	{
		/// <summary>
		/// Gets or sets the index of the item within a group.
		/// </summary>
		/// <value>The index of the item within a group.</value>
		int GroupedItemIndex
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the number of items in the same group as this item.
		/// </summary>
		/// <value>The number of items in the same group as this item.</value>
		int GroupedItemCount
		{
			get;
		}
	}
}
