//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public class GroupsDataFiller : AbstractDataFiller
	{
		public GroupsDataFiller(DataAccessor accessor, BaseType baseType, NavigationTreeTableController controller, AbstractNodeFiller nodeFiller)
			: base (accessor, baseType, controller, nodeFiller)
		{
		}


		public override void UpdateColumns()
		{
			this.controller.SetColumns (GroupsDataFiller.TreeTableColumns, 1);
		}

		public override void UpdateContent(int firstRow, int count, int selection, bool crop = true)
		{
			var cf = new List<TreeTableCellTree> ();
			var c1 = new List<TreeTableCellString> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeFiller.NodesCount)
				{
					break;
				}

				var node  = this.nodeFiller.GetNode (firstRow+i);
				var guid  = node.Guid;
				var level = node.Level;
				var type  = node.Type;
				var obj   = this.accessor.GetObject (this.baseType, guid);

				var family = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Famille);
				var member = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Membre);

				if (this.Timestamp.HasValue &&
					!ObjectCalculator.IsExistingObject (obj, this.Timestamp.Value))
				{
					family = "<i>Inconnu à cette date</i>";
				}

				var sf = new TreeTableCellTree   (true, level, type, family, isSelected: (i == selection));
				var s1 = new TreeTableCellString (true, member, isSelected: (i == selection));

				cf.Add (sf);
				c1.Add (s1);
			}

			{
				int i = 0;
				this.controller.SetColumnCells (i++, cf.ToArray ());
				this.controller.SetColumnCells (i++, c1.ToArray ());
			}
		}


		private static TreeTableColumnDescription[] TreeTableColumns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Tree,   200, "Famille"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 250, "Membre"));

				return list.ToArray ();
			}
		}
	}
}
