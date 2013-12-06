//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.Helpers;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class EventsObjectsTreeTableFiller : AbstractTreeTableFiller<SortableNode>
	{
		public EventsObjectsTreeTableFiller(DataAccessor accessor, AbstractNodesGetter<SortableNode> nodesGetter)
			: base (accessor, nodesGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.EventDate;
				yield return ObjectField.EventGlyph;
				yield return ObjectField.EventType;
				yield return ObjectField.ValeurComptable;
				yield return ObjectField.Valeur1;
				yield return ObjectField.Valeur2;
				yield return ObjectField.Nom;
				yield return ObjectField.Numéro;
				yield return ObjectField.Maintenance;
				yield return ObjectField.Couleur;
				yield return ObjectField.NuméroSérie;
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
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         120, "Maintenance"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          60, "Couleur"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         200, "Numéro de série"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         180, "Objet"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          50, "N°"));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var c1  = new TreeTableColumnItem<TreeTableCellString> ();
			var c2  = new TreeTableColumnItem<TreeTableCellGlyph> ();
			var c3  = new TreeTableColumnItem<TreeTableCellString> ();
			var c4  = new TreeTableColumnItem<TreeTableCellComputedAmount> ();
			var c5  = new TreeTableColumnItem<TreeTableCellComputedAmount> ();
			var c6  = new TreeTableColumnItem<TreeTableCellComputedAmount> ();
			var c7  = new TreeTableColumnItem<TreeTableCellString> ();
			var c8  = new TreeTableColumnItem<TreeTableCellString> ();
			var c9  = new TreeTableColumnItem<TreeTableCellString> ();
			var c10 = new TreeTableColumnItem<TreeTableCellString> ();
			var c11 = new TreeTableColumnItem<TreeTableCellString> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodesGetter.Count)
				{
					break;
				}

				var node = this.nodesGetter[firstRow+i];
				var e    = this.DataObject.GetEvent (node.Guid);

				var timestamp  = e.Timestamp;
				var eventType  = e.Type;

				var date        = TypeConverters.DateToString (timestamp.Date);
				var glyph       = TimelineData.TypeToGlyph (eventType);
				var type        = DataDescriptions.GetEventDescription (eventType);
				var valeur1     = ObjectCalculator.GetObjectPropertyComputedAmount (this.DataObject, timestamp, ObjectField.ValeurComptable,     synthetic: false);
				var valeur2     = ObjectCalculator.GetObjectPropertyComputedAmount (this.DataObject, timestamp, ObjectField.Valeur1,     synthetic: false);
				var valeur3     = ObjectCalculator.GetObjectPropertyComputedAmount (this.DataObject, timestamp, ObjectField.Valeur2,     synthetic: false);
				var nom         = ObjectCalculator.GetObjectPropertyString         (this.DataObject, timestamp, ObjectField.Nom,         synthetic: false);
				var numéro      = ObjectCalculator.GetObjectPropertyString         (this.DataObject, timestamp, ObjectField.Numéro,      synthetic: false);
				var maintenance = ObjectCalculator.GetObjectPropertyString         (this.DataObject, timestamp, ObjectField.Maintenance, synthetic: false);
				var couleur     = ObjectCalculator.GetObjectPropertyString         (this.DataObject, timestamp, ObjectField.Couleur,     synthetic: false);
				var série       = ObjectCalculator.GetObjectPropertyString         (this.DataObject, timestamp, ObjectField.NuméroSérie, synthetic: false);

				var s1 = new TreeTableCellString         (true, date,        isSelected: (i == selection));
				var s2 = new TreeTableCellGlyph          (true, glyph,       isSelected: (i == selection));
				var s3 = new TreeTableCellString         (true, type,        isSelected: (i == selection));
				var s4 = new TreeTableCellComputedAmount (true, valeur1,     isSelected: (i == selection));
				var s5 = new TreeTableCellComputedAmount (true, valeur2,     isSelected: (i == selection));
				var s6 = new TreeTableCellComputedAmount (true, valeur3,     isSelected: (i == selection));
				var s7 = new TreeTableCellString         (true, maintenance, isSelected: (i == selection));
				var s8 = new TreeTableCellString         (true, couleur,     isSelected: (i == selection));
				var s9 = new TreeTableCellString         (true, série,       isSelected: (i == selection));
				var s10 = new TreeTableCellString        (true, nom,         isSelected: (i == selection));
				var s11 = new TreeTableCellString        (true, numéro,      isSelected: (i == selection));

				c1.AddRow (s1);
				c2.AddRow (s2);
				c3.AddRow (s3);
				c4.AddRow (s4);
				c5.AddRow (s5);
				c6.AddRow (s6);
				c7.AddRow (s7);
				c8.AddRow (s8);
				c9.AddRow (s9);
				c10.AddRow (s10);
				c11.AddRow (s11);
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
			content.Columns.Add (c10);
			content.Columns.Add (c11);

			return content;
		}
	}
}
