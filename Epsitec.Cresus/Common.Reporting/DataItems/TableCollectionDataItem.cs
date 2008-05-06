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
//-			System.Diagnostics.Debug.Assert (vectorSetting != null);

			this.vectorSetting = vectorSetting;
			this.title = collectionSetting.Title;
		}


		/// <summary>
		/// Gets the columns definiton, stored as a vector setting.
		/// </summary>
		/// <value>The columns.</value>
		public override Settings.VectorSetting Columns
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
		public FormattedText Title
		{
			get
			{
				return this.title;
			}
		}

		public override DataView.DataItem GetVirtualItem(VirtualNodeType virtualNodeType)
		{
			TitleDataItem item = null;

			switch (virtualNodeType)
			{
				case VirtualNodeType.Header1:
					item = new TitleDataItem (this.DataView.Context, this.title, DataItemClass.TableHeader1);
					break;

				case VirtualNodeType.Header2:
					item = new TitleDataItem (this.DataView.Context, this.GetColumnTitles (), DataItemClass.TableHeader2);
					break;

				case VirtualNodeType.Footer1:
					item = new TitleDataItem (this.DataView.Context, this.title, DataItemClass.TableFooter1);
					break;

				case VirtualNodeType.Footer2:
					item = new TitleDataItem (this.DataView.Context, this.GetColumnTitles (), DataItemClass.TableFooter2);
					break;
			}

			if (item == null)
			{
				throw new System.InvalidOperationException (string.Format ("Invalid virtual node type {0} for table", virtualNodeType));
			}

			DataView.RegisterItem (this.DataView, item, DataView.GetVirtualNodeId (virtualNodeType));

			return item;
		}

		private IEnumerable<FormattedText> GetColumnTitles()
		{
			foreach (var item in this.Columns.Values)
			{
				yield return item.Title;
			}
		}

		private readonly Settings.VectorSetting vectorSetting;
		private readonly FormattedText title;
	}
}
