//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

				foreach (var userField in accessor.Settings.GetUserFields (BaseType.Assets)
					.Where (x => x.Type == FieldType.ComputedAmount))
				{
					yield return userField.Field;
				}

				yield return ObjectField.Name;
				yield return ObjectField.Number;
				yield return ObjectField.Description;

				foreach (var userField in accessor.Settings.GetUserFields (BaseType.Assets)
					.Where (x => x.Type != FieldType.ComputedAmount))
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

				AbstractTreeTableCell.AddColumnDescription (columns,
					accessor.Settings.GetUserFields (BaseType.Assets)
					.Where (x => x.Type == FieldType.ComputedAmount));

				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         180, "Objet"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          50, "N°"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         200, "Description"));

				AbstractTreeTableCell.AddColumnDescription (columns,
					accessor.Settings.GetUserFields (BaseType.Assets)
					.Where (x => x.Type != FieldType.ComputedAmount));

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

			foreach (var userField in accessor.Settings.GetUserFields (BaseType.Assets)
				.Where (x => x.Type == FieldType.ComputedAmount))
			{
				var column  = new TreeTableColumnItem ();
				content.Columns.Add (column);
			}

			for (int i=0; i<3; i++)
			{
				content.Columns.Add (new TreeTableColumnItem ());
			}

			foreach (var userField in accessor.Settings.GetUserFields (BaseType.Assets)
				.Where (x => x.Type != FieldType.ComputedAmount))
			{
				var column  = new TreeTableColumnItem ();
				content.Columns.Add (column);
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
				var value  = ObjectCalculator.GetObjectPropertyComputedAmount (this.DataObject, timestamp, ObjectField.MainValue,   synthetic: false);
				var nom    = ObjectCalculator.GetObjectPropertyString         (this.DataObject, timestamp, ObjectField.Name,        synthetic: false);
				var number = ObjectCalculator.GetObjectPropertyString         (this.DataObject, timestamp, ObjectField.Number,      synthetic: false);
				var desc   = ObjectCalculator.GetObjectPropertyString         (this.DataObject, timestamp, ObjectField.Description, synthetic: false);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var s1 = new TreeTableCellString         (date,   cellState);
				var s2 = new TreeTableCellGlyph          (glyph,  cellState);
				var s3 = new TreeTableCellString         (type,   cellState);
				var s4 = new TreeTableCellComputedAmount (value,  cellState);
				var s7 = new TreeTableCellString         (nom,    cellState);
				var s8 = new TreeTableCellString         (number, cellState);
				var s9 = new TreeTableCellString         (desc,   cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (s1);
				content.Columns[columnRank++].AddRow (s2);
				content.Columns[columnRank++].AddRow (s3);
				content.Columns[columnRank++].AddRow (s4);

				foreach (var userField in accessor.Settings.GetUserFields (BaseType.Assets)
					.Where (x => x.Type == FieldType.ComputedAmount))
				{
					var cell = AbstractTreeTableCell.CreateTreeTableCell (this.DataObject, timestamp, userField, false, cellState, synthetic: false);
					content.Columns[columnRank++].AddRow (cell);
				}

				content.Columns[columnRank++].AddRow (s7);
				content.Columns[columnRank++].AddRow (s8);
				content.Columns[columnRank++].AddRow (s9);

				foreach (var userField in accessor.Settings.GetUserFields (BaseType.Assets)
					.Where (x => x.Type != FieldType.ComputedAmount))
				{
					var cell = AbstractTreeTableCell.CreateTreeTableCell (this.DataObject, timestamp, userField, false, cellState, synthetic: false);
					content.Columns[columnRank++].AddRow (cell);
				}
			}

			return content;
		}
	}
}
