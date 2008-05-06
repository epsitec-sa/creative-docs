//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting.DataItems
{
	/// <summary>
	/// The <c>TitleDataItem</c> class represents items which map to
	/// entities, visible as a vector of data items.
	/// </summary>
	class TitleDataItem : CollectionDataItem
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TitleDataItem"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="title">The title.</param>
		/// <param name="itemClass">The item class.</param>
		public TitleDataItem(DataViewContext context, string title, DataItemClass itemClass)
			: base (context, new List<string> () { title })
		{
			this.ItemClass = itemClass;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TitleDataItem"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="titles">The titles.</param>
		/// <param name="itemClass">The item class.</param>
		public TitleDataItem(DataViewContext context, IEnumerable<string> titles, DataItemClass itemClass)
			: base (context, new List<string> (titles))
		{
			this.ItemClass = itemClass;
		}

		/// <summary>
		/// Gets the type of the item.
		/// </summary>
		/// <value>Always <c>DataItemType.Vector</c>.</value>
		public override DataItemType ItemType
		{
			get
			{
				return DataItemType.Vector;
			}
		}
	}
}
