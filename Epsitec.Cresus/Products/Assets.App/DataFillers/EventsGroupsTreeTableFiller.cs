//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public class EventsGroupsTreeTableFiller : AbstractTreeTableFiller
	{
		public EventsGroupsTreeTableFiller(DataAccessor accessor, BaseType baseType, NavigationTreeTableController controller, AbstractNodesGetter<GuidNode> nodesGetter)
			: base (accessor, baseType, controller)
		{
			this.nodesGetter = nodesGetter;
		}


		public override void UpdateColumns()
		{
			this.controller.SetColumns (EventsGroupsTreeTableFiller.TreeTableColumns, 1);
		}

		public override void UpdateContent(int firstRow, int count, int selection)
		{
			var c1 = new List<TreeTableCellString> ();
			var c2 = new List<TreeTableCellGlyph> ();
			var c3 = new List<TreeTableCellString> ();
			var c4 = new List<TreeTableCellString> ();
			var c5 = new List<TreeTableCellString> ();

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

				var date   = Helpers.Converters.DateToString (timestamp.Date);
				var glyph  = TimelineData.TypeToGlyph (eventType);
				var type   = DataDescriptions.GetEventDescription (eventType);
				var nom    = ObjectCalculator.GetObjectPropertyString (this.DataObject, timestamp, ObjectField.Nom);
				var family = ObjectCalculator.GetObjectPropertyString (this.DataObject, timestamp, ObjectField.Famille);

				var s1 = new TreeTableCellString (true, date,   isSelected: (i == selection));
				var s2 = new TreeTableCellGlyph  (true, glyph,  isSelected: (i == selection));
				var s3 = new TreeTableCellString (true, type,   isSelected: (i == selection));
				var s4 = new TreeTableCellString (true, nom,    isSelected: (i == selection));
				var s5 = new TreeTableCellString (true, family, isSelected: (i == selection));

				c1.Add (s1);
				c2.Add (s2);
				c3.Add (s3);
				c4.Add (s4);
				c5.Add (s5);
			}

			int c = 0;
			this.controller.SetColumnCells (c++, c1.ToArray ());
			this.controller.SetColumnCells (c++, c2.ToArray ());
			this.controller.SetColumnCells (c++, c3.ToArray ());
			this.controller.SetColumnCells (c++, c4.ToArray ());
			this.controller.SetColumnCells (c++, c5.ToArray ());
		}


		private static TreeTableColumnDescription[] TreeTableColumns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  70, "Date"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Glyph,   20, ""));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 110, "Type"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 250, "Membre"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 150, "Famille"));

				return list.ToArray ();
			}
		}


		private AbstractNodesGetter<GuidNode> nodesGetter;
	}
}
