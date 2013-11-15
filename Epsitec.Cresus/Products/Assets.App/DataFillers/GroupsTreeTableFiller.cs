//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public class GroupsTreeTableFiller : AbstractTreeTableFiller
	{
		public GroupsTreeTableFiller(DataAccessor accessor, BaseType baseType, NavigationTreeTableController controller, AbstractNodesGetter<TreeNode> nodesGetter)
			: base (accessor, baseType, controller)
		{
			this.nodesGetter = nodesGetter;
		}


		public override void UpdateColumns()
		{
			this.controller.SetColumns (GroupsTreeTableFiller.TreeTableColumns, 1);
		}

		public override void UpdateContent(int firstRow, int count, int selection)
		{
			var cf = new List<TreeTableCellTree> ();
			var c1 = new List<TreeTableCellString> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodesGetter.Count)
				{
					break;
				}

				var node  = this.nodesGetter[firstRow+i];
				var guid  = node.Guid;
				var level = node.Level;
				var type  = node.Type;
				var obj   = this.accessor.GetObject (this.baseType, guid);

				var nom    = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Nom);
				var family = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Famille);

				if (this.Timestamp.HasValue &&
					!ObjectCalculator.IsExistingObject (obj, this.Timestamp.Value))
				{
					family = DataDescriptions.OutOfDateName;
				}

				var sf = new TreeTableCellTree   (true, level, type, nom, isSelected: (i == selection));
				var s1 = new TreeTableCellString (true, family, isSelected: (i == selection));

				cf.Add (sf);
				c1.Add (s1);
			}

			{
				int i = 0;
				//-this.controller.SetColumnCells (i++, cf.ToArray ());
				//-this.controller.SetColumnCells (i++, c1.ToArray ());
			}
		}


		private static TreeTableColumnDescription[] TreeTableColumns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Tree,   250, "Membre"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 150, "Famille"));

				return list.ToArray ();
			}
		}


		private AbstractNodesGetter<TreeNode> nodesGetter;
	}
}
