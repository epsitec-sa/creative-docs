//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.Helpers;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public class EventsCategoriesTreeTableFiller : AbstractTreeTableFiller<GuidNode>
	{
		public EventsCategoriesTreeTableFiller(DataAccessor accessor, AbstractNodesGetter<GuidNode> nodesGetter)
			: base (accessor, nodesGetter)
		{
		}


		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  70, "Date"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Glyph,   20, ""));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 110, "Type"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Rate,    80, "Taux"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  80, "Type"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 100, "Périodicité"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, 120, "Valeur résiduelle"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 180, "Catégorie"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  50, "N°"));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var c1 = new TreeTableColumnItem<TreeTableCellString> ();
			var c2 = new TreeTableColumnItem<TreeTableCellGlyph> ();
			var c3 = new TreeTableColumnItem<TreeTableCellString> ();
			var c4 = new TreeTableColumnItem<TreeTableCellDecimal> ();
			var c5 = new TreeTableColumnItem<TreeTableCellString> ();
			var c6 = new TreeTableColumnItem<TreeTableCellString> ();
			var c7 = new TreeTableColumnItem<TreeTableCellDecimal> ();
			var c8 = new TreeTableColumnItem<TreeTableCellString> ();
			var c9 = new TreeTableColumnItem<TreeTableCellString> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodesGetter.Count)
				{
					break;
				}

				var node = this.nodesGetter[firstRow+i];
				var guid = node.Guid;
				var e    = this.DataObject.GetEvent (guid);

				var timestamp  = e.Timestamp;
				var eventType  = e.Type;

				var date   = TypeConverters.DateToString (timestamp.Date);
				var glyph  = TimelineData.TypeToGlyph (eventType);
				var type   = DataDescriptions.GetEventDescription (eventType);
				var taux   = ObjectCalculator.GetObjectPropertyDecimal (this.DataObject, timestamp, ObjectField.TauxAmortissement);
				var typeAm = ObjectCalculator.GetObjectPropertyString  (this.DataObject, timestamp, ObjectField.TypeAmortissement);
				var period = ObjectCalculator.GetObjectPropertyString  (this.DataObject, timestamp, ObjectField.Périodicité);
				var residu = ObjectCalculator.GetObjectPropertyDecimal (this.DataObject, timestamp, ObjectField.ValeurRésiduelle);
				var nom    = ObjectCalculator.GetObjectPropertyString  (this.DataObject, timestamp, ObjectField.Nom);
				var numéro = ObjectCalculator.GetObjectPropertyString  (this.DataObject, timestamp, ObjectField.Numéro);

				var s1 = new TreeTableCellString  (true, date,   isSelected: (i == selection));
				var s2 = new TreeTableCellGlyph   (true, glyph,  isSelected: (i == selection));
				var s3 = new TreeTableCellString  (true, type,   isSelected: (i == selection));
				var s4 = new TreeTableCellDecimal (true, taux,   isSelected: (i == selection));
				var s5 = new TreeTableCellString  (true, typeAm, isSelected: (i == selection));
				var s6 = new TreeTableCellString  (true, period, isSelected: (i == selection));
				var s7 = new TreeTableCellDecimal (true, residu, isSelected: (i == selection));
				var s8 = new TreeTableCellString  (true, nom,    isSelected: (i == selection));
				var s9 = new TreeTableCellString  (true, numéro, isSelected: (i == selection));

				c1.AddRow (s1);
				c2.AddRow (s2);
				c3.AddRow (s3);
				c4.AddRow (s4);
				c5.AddRow (s5);
				c6.AddRow (s6);
				c7.AddRow (s7);
				c8.AddRow (s8);
				c9.AddRow (s9);
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
			content.Columns.Add (c9);

			return content;
		}
	}
}
