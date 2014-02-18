//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class SingleCategoriesTreeTableFiller : AbstractTreeTableFiller<SortableNode>
	{
		public SingleCategoriesTreeTableFiller(DataAccessor accessor, AbstractNodeGetter<SortableNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.Name;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 200, "Catégories"));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var c0 = new TreeTableColumnItem<TreeTableCellString> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeGetter.Count)
				{
					break;
				}

				var node = this.nodeGetter[firstRow+i];
				var obj  = this.accessor.GetObject (BaseType.Categories, node.Guid);

				var nom = AssetCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Name, inputValue: true);

				var s0 = new TreeTableCellString (true, nom, isSelected: (i == selection));

				c0.AddRow (s0);
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (c0);

			return content;
		}
	}
}
