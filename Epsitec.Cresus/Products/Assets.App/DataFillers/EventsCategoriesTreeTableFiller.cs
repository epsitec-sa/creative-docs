//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public class EventsCategoriesTreeTableFiller : AbstractTreeTableFiller
	{
		public EventsCategoriesTreeTableFiller(DataAccessor accessor, BaseType baseType, NavigationTreeTableController controller, AbstractNodesGetter<GuidNode> nodesGetter)
			: base (accessor, baseType, controller)
		{
			this.nodesGetter = nodesGetter;
		}


		public override void UpdateColumns()
		{
			this.controller.SetColumns (EventsCategoriesTreeTableFiller.TreeTableColumns, 1);
		}

		public override void UpdateContent(int firstRow, int count, int selection)
		{
			var c1 = new List<TreeTableCellString> ();
			var c2 = new List<TreeTableCellGlyph> ();
			var c3 = new List<TreeTableCellString> ();
			var c4 = new List<TreeTableCellDecimal> ();
			var c5 = new List<TreeTableCellString> ();
			var c6 = new List<TreeTableCellString> ();
			var c7 = new List<TreeTableCellDecimal> ();
			var c8 = new List<TreeTableCellString> ();
			var c9 = new List<TreeTableCellString> ();

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

				c1.Add (s1);
				c2.Add (s2);
				c3.Add (s3);
				c4.Add (s4);
				c5.Add (s5);
				c6.Add (s6);
				c7.Add (s7);
				c8.Add (s8);
				c9.Add (s9);
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
		}


		private static TreeTableColumnDescription[] TreeTableColumns
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


		private AbstractNodesGetter<GuidNode> nodesGetter;
	}
}
