//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.Helpers;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public class EventsObjectsTreeTableFiller : AbstractTreeTableFiller
	{
		public EventsObjectsTreeTableFiller(DataAccessor accessor, BaseType baseType, NavigationTreeTableController controller, AbstractNodesGetter<GuidNode> nodesGetter)
			: base (accessor, baseType, controller)
		{
			this.nodesGetter = nodesGetter;
		}


		public override void UpdateColumns()
		{
			this.controller.SetColumns (EventsObjectsTreeTableFiller.TreeTableColumns, 1);
		}

		public override void UpdateContent(int firstRow, int count, int selection)
		{
			var c1 = new List<TreeTableCellString> ();
			var c2 = new List<TreeTableCellGlyph> ();
			var c3 = new List<TreeTableCellString> ();
			var c4 = new List<TreeTableCellComputedAmount> ();
			var c5 = new List<TreeTableCellComputedAmount> ();
			var c6 = new List<TreeTableCellComputedAmount> ();
			var c7 = new List<TreeTableCellString> ();
			var c8 = new List<TreeTableCellString> ();
			var c9 = new List<TreeTableCellString> ();
			var c10 = new List<TreeTableCellString> ();
			var c11 = new List<TreeTableCellString> ();

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

				var date        = TypeConverters.DateToString (timestamp.Date);
				var glyph       = TimelineData.TypeToGlyph (eventType);
				var type        = DataDescriptions.GetEventDescription (eventType);
				var valeur1     = ObjectCalculator.GetObjectPropertyComputedAmount (this.DataObject, timestamp, ObjectField.Valeur1);
				var valeur2     = ObjectCalculator.GetObjectPropertyComputedAmount (this.DataObject, timestamp, ObjectField.Valeur2);
				var valeur3     = ObjectCalculator.GetObjectPropertyComputedAmount (this.DataObject, timestamp, ObjectField.Valeur3);
				var nom         = ObjectCalculator.GetObjectPropertyString         (this.DataObject, timestamp, ObjectField.Nom);
				var numéro      = ObjectCalculator.GetObjectPropertyString         (this.DataObject, timestamp, ObjectField.Numéro);
				var responsable = ObjectCalculator.GetObjectPropertyString         (this.DataObject, timestamp, ObjectField.Responsable);
				var couleur     = ObjectCalculator.GetObjectPropertyString         (this.DataObject, timestamp, ObjectField.Couleur);
				var série       = ObjectCalculator.GetObjectPropertyString         (this.DataObject, timestamp, ObjectField.NuméroSérie);

				var s1 = new TreeTableCellString         (true, date,        isSelected: (i == selection));
				var s2 = new TreeTableCellGlyph          (true, glyph,       isSelected: (i == selection));
				var s3 = new TreeTableCellString         (true, type,        isSelected: (i == selection));
				var s4 = new TreeTableCellComputedAmount (true, valeur1,     isSelected: (i == selection));
				var s5 = new TreeTableCellComputedAmount (true, valeur2,     isSelected: (i == selection));
				var s6 = new TreeTableCellComputedAmount (true, valeur3,     isSelected: (i == selection));
				var s7 = new TreeTableCellString         (true, responsable, isSelected: (i == selection));
				var s8 = new TreeTableCellString         (true, couleur,     isSelected: (i == selection));
				var s9 = new TreeTableCellString         (true, série,       isSelected: (i == selection));
				var s10 = new TreeTableCellString        (true, nom,         isSelected: (i == selection));
				var s11 = new TreeTableCellString        (true, numéro,      isSelected: (i == selection));

				c1.Add (s1);
				c2.Add (s2);
				c3.Add (s3);
				c4.Add (s4);
				c5.Add (s5);
				c6.Add (s6);
				c7.Add (s7);
				c8.Add (s8);
				c9.Add (s9);
				c10.Add (s10);
				c11.Add (s11);
			}

			int c = 0;
			this.controller.SetColumnCells (c++, c1.ToArray ());
			this.controller.SetColumnCells (c++, c2.ToArray ());
			this.controller.SetColumnCells (c++, c3.ToArray ());
			this.controller.SetColumnCells (c++, c4.ToArray ());
			this.controller.SetColumnCells (c++, c5.ToArray ());
			this.controller.SetColumnCells (c++, c6.ToArray ());
			this.controller.SetColumnCells (c++, c7.ToArray ());
			this.controller.SetColumnCells (c++, c8.ToArray ());
			this.controller.SetColumnCells (c++, c9.ToArray ());
			this.controller.SetColumnCells (c++, c10.ToArray ());
			this.controller.SetColumnCells (c++, c11.ToArray ());
		}


		private static TreeTableColumnDescription[] TreeTableColumns
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
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         120, "Responsable"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          60, "Couleur"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         200, "Numéro de série"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         180, "Objet"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          50, "N°"));

				return list.ToArray ();
			}
		}


		private AbstractNodesGetter<GuidNode> nodesGetter;
	}
}
