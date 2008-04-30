//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting.DataItems
{
	/// <summary>
	/// The <c>TableCollectionDataItem</c> class represents items which map to
	/// a collection of items (rows), also known as a table.
	/// </summary>
	class TableCollectionDataItem : CollectionDataItem
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TableCollectionDataItem"/> class.
		/// </summary>
		/// <param name="context">The data view context.</param>
		/// <param name="collection">The collection of items.</param>
		public TableCollectionDataItem(DataViewContext context, System.Collections.IList collection)
			: base (context, collection)
		{
		}

		/// <summary>
		/// Gets the type of the item.
		/// </summary>
		/// <value>The type of the item.</value>
		public override DataItemType ItemType
		{
			get
			{
				return DataItemType.Table;
			}
		}
	}
}
