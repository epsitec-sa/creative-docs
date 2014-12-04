//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class ArgumentsTreeTableFiller : AbstractTreeTableFiller<SortableNode>
	{
		public ArgumentsTreeTableFiller(DataAccessor accessor, INodeGetter<SortableNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override SortingInstructions		DefaultSorting
		{
			get
			{
				return new SortingInstructions (ObjectField.Name, SortedType.Ascending, ObjectField.Unknown, SortedType.None);
			}
		}

		public override int						DefaultDockToLeftCount
		{
			get
			{
				return 1;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				columns.Add (new TreeTableColumnDescription (ObjectField.Name,             TreeTableColumnType.String, ArgumentsTreeTableFiller.nameWidth,     Res.Strings.ArgumentsTreeTableFiller.Name.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.Description,      TreeTableColumnType.String, ArgumentsTreeTableFiller.descWidth,     Res.Strings.ArgumentsTreeTableFiller.Description.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.ArgumentType,     TreeTableColumnType.String, ArgumentsTreeTableFiller.typeWidth,     Res.Strings.ArgumentsTreeTableFiller.Type.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.ArgumentNullable, TreeTableColumnType.String, ArgumentsTreeTableFiller.nullWidth,     Res.Strings.ArgumentsTreeTableFiller.Nullable.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.ArgumentVariable, TreeTableColumnType.String, ArgumentsTreeTableFiller.variableWidth, Res.Strings.ArgumentsTreeTableFiller.Variable.ToString ()));

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			for (int i=0; i<5; i++)
			{
				content.Columns.Add (new TreeTableColumnItem ());
			}

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeGetter.Count)
				{
					break;
				}

				var node  = this.nodeGetter[firstRow+i];
				var guid  = node.Guid;
				var obj   = this.accessor.GetObject (BaseType.Arguments, guid);

				var name     = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Name, inputValue: true);
				var desc     = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Description);
				var type     = ObjectProperties.GetObjectPropertyInt    (obj, this.Timestamp, ObjectField.ArgumentType);
				var nullable = ObjectProperties.GetObjectPropertyInt    (obj, this.Timestamp, ObjectField.ArgumentNullable);
				var variable = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, ObjectField.ArgumentVariable);

				var t = EnumDictionaries.GetArgumentTypeName (type);
				var n = (nullable == 1) ?
					Res.Strings.ArgumentsTreeTableFiller.NullableTrue.ToString () :
					Res.Strings.ArgumentsTreeTableFiller.NullableFalse.ToString ();

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var cell1 = new TreeTableCellString (name,     cellState);
				var cell2 = new TreeTableCellString (desc,     cellState);
				var cell3 = new TreeTableCellString (t,        cellState);
				var cell4 = new TreeTableCellString (n,        cellState);
				var cell5 = new TreeTableCellString (variable, cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (cell1);
				content.Columns[columnRank++].AddRow (cell2);
				content.Columns[columnRank++].AddRow (cell3);
				content.Columns[columnRank++].AddRow (cell4);
				content.Columns[columnRank++].AddRow (cell5);
			}

			return content;
		}


		private const int nameWidth     = 150;
		private const int descWidth     = 250;
		private const int typeWidth     = 120;
		private const int nullWidth     =  80;
		private const int variableWidth = 150;

		public const int totalWidth =
			ArgumentsTreeTableFiller.nameWidth +
			ArgumentsTreeTableFiller.descWidth +
			ArgumentsTreeTableFiller.typeWidth +
			ArgumentsTreeTableFiller.nullWidth +
			ArgumentsTreeTableFiller.variableWidth;
	}
}
