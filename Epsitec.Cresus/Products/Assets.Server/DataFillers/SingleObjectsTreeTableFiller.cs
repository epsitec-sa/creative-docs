//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class SingleObjectsTreeTableFiller : AbstractTreeTableFiller<TreeNode>
	{
		public SingleObjectsTreeTableFiller(DataAccessor accessor, AbstractNodesGetter<TreeNode> nodesGetter)
			: base (accessor, nodesGetter)
		{
		}


		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Tree, 180, "Objet"));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var cf = new TreeTableColumnItem<TreeTableCellTree> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodesGetter.Count)
				{
					break;
				}

				var node  = this.nodesGetter[firstRow+i];
				var level = node.Level;
				var type  = node.Type;
				var obj   = this.accessor.GetObject (node.BaseType, node.Guid);

				var nom = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Nom);

				if (this.Timestamp.HasValue &&
					!ObjectCalculator.IsExistingObject (obj, this.Timestamp.Value))
				{
					nom = DataDescriptions.OutOfDateName;
				}

				var sf = new TreeTableCellTree (true, level, type, nom, isSelected: (i == selection));

				cf.AddRow (sf);
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (cf);

			return content;
		}
	}
}
