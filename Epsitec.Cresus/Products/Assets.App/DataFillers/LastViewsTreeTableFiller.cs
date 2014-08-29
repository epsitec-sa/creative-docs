//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.NodeGetters;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public class LastViewsTreeTableFiller : AbstractTreeTableFiller<LastViewNode>
	{
		public LastViewsTreeTableFiller(DataAccessor accessor, INodeGetter<LastViewNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override SortingInstructions		DefaultSorting
		{
			get
			{
				return new SortingInstructions (ObjectField.LastViewsPin, SortedType.Ascending, ObjectField.Unknown, SortedType.None);
			}
		}

		public override int						DefaultDockToLeftCount
		{
			get
			{
				return 0;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (ObjectField.LastViewsPin,         TreeTableColumnType.Pin,    LastViewsTreeTableFiller.TypeColumnWidth, ""));
				list.Add (new TreeTableColumnDescription (ObjectField.LastViewsType,        TreeTableColumnType.Icon,   LastViewsTreeTableFiller.TypeColumnWidth, Res.Strings.DataFillers.LastViewsTreeTable.Type.ToString ()));
				list.Add (new TreeTableColumnDescription (ObjectField.LastViewsPage,        TreeTableColumnType.String, LastViewsTreeTableFiller.PageColumnWidth, Res.Strings.DataFillers.LastViewsTreeTable.Page.ToString ()));
				list.Add (new TreeTableColumnDescription (ObjectField.LastViewsDate,        TreeTableColumnType.Date,   LastViewsTreeTableFiller.DateColumnWidth, Res.Strings.DataFillers.LastViewsTreeTable.Date.ToString ()));
				list.Add (new TreeTableColumnDescription (ObjectField.LastViewsDescription, TreeTableColumnType.String, LastViewsTreeTableFiller.DescColumnWidth, Res.Strings.DataFillers.LastViewsTreeTable.Description.ToString ()));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var c1 = new TreeTableColumnItem ();
			var c2 = new TreeTableColumnItem ();
			var c3 = new TreeTableColumnItem ();
			var c4 = new TreeTableColumnItem ();
			var c5 = new TreeTableColumnItem ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeGetter.Count)
				{
					break;
				}

				var node = this.nodeGetter[firstRow+i];

				int    pin  = node.Pin ? 1 : 0;
				string icon = StaticDescriptions.GetViewTypeIcon (node.ViewType.Kind);
				string type = StaticDescriptions.GetObjectPageDescription (node.PageType);
				string desc = node.Description;

				System.DateTime? date;
				if (node.Timestamp.HasValue)
				{
					date = node.Timestamp.Value.Date;
				}
				else
				{
					date = null;
				}

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var s1 = new TreeTableCellInt    (pin,  cellState);
				var s2 = new TreeTableCellString (icon, cellState);
				var s3 = new TreeTableCellString (type, cellState);
				var s4 = new TreeTableCellDate   (date, cellState);
				var s5 = new TreeTableCellString (desc, cellState);

				c1.AddRow (s1);
				c2.AddRow (s2);
				c3.AddRow (s3);
				c4.AddRow (s4);
				c5.AddRow (s5);
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (c1);
			content.Columns.Add (c2);
			content.Columns.Add (c3);
			content.Columns.Add (c4);
			content.Columns.Add (c5);

			return content;
		}


		private const int PinColumnWidth  =  29;
		private const int TypeColumnWidth =  32;
		private const int PageColumnWidth = 100;
		private const int DateColumnWidth =  70;
		private const int DescColumnWidth = 250;

		public const int TotalWidth = LastViewsTreeTableFiller.PinColumnWidth
									+ LastViewsTreeTableFiller.TypeColumnWidth
									+ LastViewsTreeTableFiller.PageColumnWidth
									+ LastViewsTreeTableFiller.DateColumnWidth
									+ LastViewsTreeTableFiller.DescColumnWidth;
	}
}
