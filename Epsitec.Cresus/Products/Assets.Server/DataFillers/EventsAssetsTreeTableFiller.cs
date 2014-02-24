﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.Helpers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class EventsAssetsTreeTableFiller : AbstractTreeTableFiller<SortableNode>
	{
		public EventsAssetsTreeTableFiller(DataAccessor accessor, AbstractNodeGetter<SortableNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.EventDate;
				yield return ObjectField.EventGlyph;
				yield return ObjectField.EventType;
				yield return ObjectField.MainValue;

				foreach (var userField in accessor.Settings.GetUserFields (BaseType.Assets))
				{
					yield return userField.Field;
				}
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          70, "Date"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Glyph,           20, ""));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         110, "Type"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur comptable"));

				AbstractTreeTableCell.AddColumnDescription (columns, accessor.Settings.GetUserFields (BaseType.Assets));

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			for (int i=0; i<4; i++)
			{
				content.Columns.Add (new TreeTableColumnItem ());
			}

			foreach (var userField in accessor.Settings.GetUserFields (BaseType.Assets))
			{
				content.Columns.Add (new TreeTableColumnItem ());
			}

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeGetter.Count)
				{
					break;
				}

				var node = this.nodeGetter[firstRow+i];
				var e    = this.DataObject.GetEvent (node.Guid);
				System.Diagnostics.Debug.Assert (e != null);

				var timestamp  = e.Timestamp;
				var eventType  = e.Type;

				var date   = TypeConverters.DateToString (timestamp.Date);
				var glyph  = TimelineData.TypeToGlyph (eventType);
				var type   = DataDescriptions.GetEventDescription (eventType);
				var value  = ObjectProperties.GetObjectPropertyComputedAmount (this.DataObject, timestamp, ObjectField.MainValue,   synthetic: false);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var cell1 = new TreeTableCellString         (date,  cellState);
				var cell2 = new TreeTableCellGlyph          (glyph, cellState);
				var cell3 = new TreeTableCellString         (type,  cellState);
				var cell4 = new TreeTableCellComputedAmount (value, cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (cell1);
				content.Columns[columnRank++].AddRow (cell2);
				content.Columns[columnRank++].AddRow (cell3);
				content.Columns[columnRank++].AddRow (cell4);

				foreach (var userField in accessor.Settings.GetUserFields (BaseType.Assets))
				{
					var cell = AbstractTreeTableCell.CreateTreeTableCell (this.accessor, this.DataObject, timestamp, userField, false, cellState, synthetic: false);
					content.Columns[columnRank++].AddRow (cell);
				}
			}

			return content;
		}
	}
}