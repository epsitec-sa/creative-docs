//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class GroupsTreeTableFiller : AbstractTreeTableFiller<TreeNode>
	{
		public GroupsTreeTableFiller(DataAccessor accessor, AbstractNodesGetter<TreeNode> nodesGetter)
			: base (accessor, nodesGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.Nom;
				yield return ObjectField.Description;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Tree,   250, "Groupe"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 400, "Description"));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var cf = new TreeTableColumnItem<TreeTableCellTree> ();
			var c1 = new TreeTableColumnItem<TreeTableCellString> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodesGetter.Count)
				{
					break;
				}

				var node  = this.nodesGetter[firstRow+i];
				var level = node.Level;
				var type  = node.Type;
				var obj   = this.accessor.GetObject (BaseType.Groups, node.Guid);

				var nom         = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Nom);
				var description = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Description);

				if (this.Timestamp.HasValue &&
					!ObjectCalculator.IsExistingObject (obj, this.Timestamp.Value))
				{
					description = DataDescriptions.OutOfDateName;
				}

				var sf = new TreeTableCellTree   (true, level, type, nom, isSelected: (i == selection));
				var s1 = new TreeTableCellString (true, description,      isSelected: (i == selection));

				cf.AddRow (sf);
				c1.AddRow (s1);
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (cf);
			content.Columns.Add (c1);

			return content;
		}
	}
}
