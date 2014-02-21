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
				yield return ObjectField.Value1;
				yield return ObjectField.Value2;
				yield return ObjectField.Name;
				yield return ObjectField.Number;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          70, "Date"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Glyph,           20, ""));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         110, "Type"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur comptable"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur assurance"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur imposable"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         180, "Objet"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          50, "N°"));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var c1  = new TreeTableColumnItem ();
			var c2  = new TreeTableColumnItem ();
			var c3  = new TreeTableColumnItem ();
			var c4  = new TreeTableColumnItem ();
			var c5  = new TreeTableColumnItem ();
			var c6  = new TreeTableColumnItem ();
			var c7  = new TreeTableColumnItem ();
			var c8  = new TreeTableColumnItem ();

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

				var date    = TypeConverters.DateToString (timestamp.Date);
				var glyph   = TimelineData.TypeToGlyph (eventType);
				var type    = DataDescriptions.GetEventDescription (eventType);
				var valeur1 = AssetCalculator.GetObjectPropertyComputedAmount (this.DataObject, timestamp, ObjectField.MainValue,     synthetic: false);
				var valeur2 = AssetCalculator.GetObjectPropertyComputedAmount (this.DataObject, timestamp, ObjectField.Value1,     synthetic: false);
				var valeur3 = AssetCalculator.GetObjectPropertyComputedAmount (this.DataObject, timestamp, ObjectField.Value2,     synthetic: false);
				var nom     = AssetCalculator.GetObjectPropertyString         (this.DataObject, timestamp, ObjectField.Name,         synthetic: false);
				var numéro  = AssetCalculator.GetObjectPropertyString         (this.DataObject, timestamp, ObjectField.Number,      synthetic: false);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;
				var s1 = new TreeTableCellString         (date,    cellState);
				var s2 = new TreeTableCellGlyph          (glyph,   cellState);
				var s3 = new TreeTableCellString         (type,    cellState);
				var s4 = new TreeTableCellComputedAmount (valeur1, cellState);
				var s5 = new TreeTableCellComputedAmount (valeur2, cellState);
				var s6 = new TreeTableCellComputedAmount (valeur3, cellState);
				var s7 = new TreeTableCellString         (nom,     cellState);
				var s8 = new TreeTableCellString         (numéro,  cellState);

				c1.AddRow (s1);
				c2.AddRow (s2);
				c3.AddRow (s3);
				c4.AddRow (s4);
				c5.AddRow (s5);
				c6.AddRow (s6);
				c7.AddRow (s7);
				c8.AddRow (s8);
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (c1);
			content.Columns.Add (c2);
			content.Columns.Add (c3);
			content.Columns.Add (c4);
			content.Columns.Add (c5);
			content.Columns.Add (c6);
			content.Columns.Add (c7);
			content.Columns.Add (c8);

			return content;
		}
	}
}
