//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.NodesGetter;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public class NavigationTreeTableFiller : AbstractTreeTableFiller<NavigationNode>
	{
		public NavigationTreeTableFiller(DataAccessor accessor, AbstractNodesGetter<NavigationNode> nodesGetter)
			: base (accessor, nodesGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.Titre;
				yield return ObjectField.Nom;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 200, "Vue"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 300, "Objet"));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var c1 = new TreeTableColumnItem<TreeTableCellString> ();
			var c2 = new TreeTableColumnItem<TreeTableCellString> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodesGetter.Count)
				{
					break;
				}

				var node  = this.nodesGetter[firstRow+i];

				string name = StaticDescriptions.GetViewTypeDescription (node.ViewType);
				string desc = node.Description;
				bool isSelected = (i == selection);

				var s1 = new TreeTableCellString (true, name, isSelected: isSelected);
				var s2 = new TreeTableCellString (true, desc, isSelected: isSelected);

				c1.AddRow (s1);
				c2.AddRow (s2);
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (c1);
			content.Columns.Add (c2);

			return content;
		}


		public const int TotalWidth = 200+300;
	}
}
