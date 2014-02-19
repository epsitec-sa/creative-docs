//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class UserFieldsTreeTableFiller : AbstractTreeTableFiller<UserFieldNode>
	{
		public UserFieldsTreeTableFiller(DataAccessor accessor, AbstractNodeGetter<UserFieldNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.Name;
				yield return ObjectField.EventType;
				yield return ObjectField.Description;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 150, "Nom"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 130, "Type"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Int,     50, "Lg max"));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var c0  = new TreeTableColumnItem<TreeTableCellString> ();
			var c1  = new TreeTableColumnItem<TreeTableCellString> ();
			var c2  = new TreeTableColumnItem<TreeTableCellInt> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeGetter.Count)
				{
					break;
				}

				var node  = this.nodeGetter[firstRow+i];
				var userField = this.accessor.Mandat.Settings.GetUserField (node.Field);

				var text0  = userField.Name;
				var text1  = EnumDictionaries.GetFieldTypeName (userField.Type);
				var text2  = userField.MaxLength;

				var s0  = new TreeTableCellString (true, text0,  isSelected: (i == selection));
				var s1  = new TreeTableCellString (true, text1,  isSelected: (i == selection));
				var s2  = new TreeTableCellInt    (true, text2,  isSelected: (i == selection));

				c0.AddRow (s0);
				c1.AddRow (s1);
				c2.AddRow (s2);
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (c0);
			content.Columns.Add (c1);
			content.Columns.Add (c2);

			return content;
		}
	}
}
