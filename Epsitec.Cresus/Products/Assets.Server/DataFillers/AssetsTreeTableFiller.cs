//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class AssetsTreeTableFiller : AbstractTreeTableFiller<CumulNode>
	{
		public AssetsTreeTableFiller(DataAccessor accessor, AbstractNodeGetter<CumulNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.Name;
				yield return ObjectField.Number;
				yield return ObjectField.MainValue;

				foreach (var userField in accessor.Settings.GetUserFields (BaseType.Assets))
				{
					yield return userField.Field;
				}
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Tree,           180, "Objet"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          50, "N°"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 110, "Valeur comptable"));

				AbstractTreeTableCell.AddColumnDescription (columns, accessor.Settings.GetUserFields (BaseType.Assets));

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			for (int i=0; i<3; i++)
			{
				content.Columns.Add (new TreeTableColumnItem ());
			}

			foreach (var userField in accessor.Settings.GetUserFields (BaseType.Assets))
			{
				var column  = new TreeTableColumnItem ();
				content.Columns.Add (column);
			}

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeGetter.Count)
				{
					break;
				}

				var node     = this.nodeGetter[firstRow+i];
				var guid     = node.Guid;
				var baseType = node.BaseType;
				var level    = node.Level;
				var type     = node.Type;

				var obj = this.accessor.GetObject (baseType, guid);

				var nom     = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Name, inputValue: true);
				var numéro  = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Number);
				var valeur1 = this.NodeGetter.GetValue (obj, node, ObjectField.MainValue);

				var cellState1 = (i == selection) ? CellState.Selected : CellState.None;
				var cellState2 = cellState1 | (type == NodeType.Final ? CellState.None : CellState.Unavailable);

				var s1 = new TreeTableCellTree           (level, type, nom, cellState1);
				var s2 = new TreeTableCellString         (numéro,           cellState1);
				var s3 = new TreeTableCellComputedAmount (valeur1,          cellState2);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (s1);
				content.Columns[columnRank++].AddRow (s2);
				content.Columns[columnRank++].AddRow (s3);

				foreach (var userField in accessor.Settings.GetUserFields (BaseType.Assets))
				{
					var cell = AbstractTreeTableCell.CreateTreeTableCell (obj, this.Timestamp, userField, false, cellState2);
					content.Columns[columnRank++].AddRow (cell);
				}
			}

			return content;
		}


		private ObjectsNodeGetter NodeGetter
		{
			get
			{
				return this.nodeGetter as ObjectsNodeGetter;
			}
		}
	}
}
