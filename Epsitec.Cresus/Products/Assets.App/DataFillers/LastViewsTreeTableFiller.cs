//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.NodeGetters;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public class LastViewsTreeTableFiller : AbstractTreeTableFiller<LastViewNode>
	{
		public LastViewsTreeTableFiller(DataAccessor accessor, AbstractNodesGetter<LastViewNode> nodesGetter)
			: base (accessor, nodesGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.LastViewsPin;
				yield return ObjectField.LastViewsType;
				yield return ObjectField.LastViewsPage;
				yield return ObjectField.LastViewsDate;
				yield return ObjectField.LastViewsDescription;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Pin,    LastViewsTreeTableFiller.TypeColumnWidth, ""));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Icon,   LastViewsTreeTableFiller.TypeColumnWidth, ""));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, LastViewsTreeTableFiller.PageColumnWidth, "Page"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Date,   LastViewsTreeTableFiller.DateColumnWidth, "Date"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, LastViewsTreeTableFiller.DescColumnWidth, "Nom"));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var c1 = new TreeTableColumnItem<TreeTableCellInt> ();
			var c2 = new TreeTableColumnItem<TreeTableCellString> ();
			var c3 = new TreeTableColumnItem<TreeTableCellString> ();
			var c4 = new TreeTableColumnItem<TreeTableCellDate> ();
			var c5 = new TreeTableColumnItem<TreeTableCellString> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodesGetter.Count)
				{
					break;
				}

				var node  = this.nodesGetter[firstRow+i];

				int    pin  = node.Pin ? 1 : 0;
				string icon = StaticDescriptions.GetViewTypeIcon (node.ViewType);
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

				bool isSelected = (i == selection);

				var s1 = new TreeTableCellInt    (true, pin,  isSelected: isSelected);
				var s2 = new TreeTableCellString (true, icon, isSelected: isSelected);
				var s3 = new TreeTableCellString (true, type, isSelected: isSelected);
				var s4 = new TreeTableCellDate   (true, date, isSelected: isSelected);
				var s5 = new TreeTableCellString (true, desc, isSelected: isSelected);

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
		private const int TypeColumnWidth =  29;
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
