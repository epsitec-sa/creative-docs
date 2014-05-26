//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class EventsAssetsTreeTableFiller : AbstractTreeTableFiller<SortableNode>
	{
		public EventsAssetsTreeTableFiller(DataAccessor accessor, INodeGetter<SortableNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				foreach (var userField in this.UserFields)
				{
					TreeTableColumnType type;
				
					if (userField.Field == ObjectField.EventGlyph)
					{
						type = TreeTableColumnType.Glyph;
					}
					else
					{
						type = AbstractTreeTableCell.GetColumnType (userField.Type);
					}

					columns.Add (new TreeTableColumnDescription (userField.Field, type, userField.ColumnWidth, userField.Name));
				}

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			foreach (var userField in this.UserFields)
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

				var timestamp = e.Timestamp;
				var eventType = e.Type;

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				int columnRank = 0;

				foreach (var userField in this.UserFields)
				{
					AbstractTreeTableCell cell;

					if (userField.Field == ObjectField.EventDate)
					{
						var date = TypeConverters.DateToString (timestamp.Date);
						cell = new TreeTableCellString (date, cellState);
					}
					else if (userField.Field == ObjectField.EventGlyph)
					{
						var glyph= TimelineData.TypeToGlyph (eventType);
						cell = new TreeTableCellGlyph (glyph, cellState);
					}
					else if (userField.Field == ObjectField.EventType)
					{
						var type = DataDescriptions.GetEventDescription (eventType);
						cell = new TreeTableCellString (type, cellState);
					}
					else
					{
						cell = AbstractTreeTableCell.CreateTreeTableCell (this.accessor, this.DataObject, timestamp, userField, false, cellState, synthetic: false);
					}

					content.Columns[columnRank++].AddRow (cell);
				}
			}

			return content;
		}

		private IEnumerable<UserField> UserFields
		{
			get
			{
				yield return new UserField ("Date", ObjectField.EventDate,  FieldType.String,  70, null, null, null, 0);
				yield return new UserField ("",     ObjectField.EventGlyph, FieldType.String,  20, null, null, null, 0);
				yield return new UserField ("Type", ObjectField.EventType,  FieldType.String, 110, null, null, null, 0);

				foreach (var userField in AssetsLogic.GetUserFields (this.accessor))
				{
					yield return userField;
				}
			}
		}
	}
}
