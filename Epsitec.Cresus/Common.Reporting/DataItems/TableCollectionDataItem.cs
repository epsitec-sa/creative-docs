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
		public TableCollectionDataItem(DataViewContext context, System.Collections.IList collection, Settings.CollectionSetting collectionSetting, Settings.VectorSetting vectorSetting)
			: base (context, collection)
		{
			System.Diagnostics.Debug.Assert (collectionSetting != null);
			System.Diagnostics.Debug.Assert (vectorSetting != null);

			this.vectorSetting = vectorSetting;
			this.title = collectionSetting.Title;
		}


		/// <summary>
		/// Gets the columns definiton, stored as a vector setting.
		/// </summary>
		/// <value>The columns.</value>
		public Settings.VectorSetting Columns
		{
			get
			{
				return this.vectorSetting;
			}
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

		/// <summary>
		/// Gets the title for this table. The title is represented using
		/// formatted text.
		/// </summary>
		/// <value>The title, as formatted text.</value>
		public string Title
		{
			get
			{
				return this.title;
			}
		}

		private readonly Settings.VectorSetting vectorSetting;
		private readonly string title;
	}
}
