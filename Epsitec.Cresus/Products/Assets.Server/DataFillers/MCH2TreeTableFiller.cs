//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class MCH2TreeTableFiller : AbstractTreeTableFiller<CumulNode>
	{
		public MCH2TreeTableFiller(DataAccessor accessor, AbstractNodeGetter<CumulNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				foreach (var column in this.OrderedColumns)
				{
					yield return ObjectField.MCH2Report + (int) column;
				}
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				foreach (var column in this.OrderedColumns)
				{
					var type  = this.GetColumnType  (column);
					var width = this.GetColumnWidth (column);
					var name  = this.GetColumnName  (column);

					columns.Add (new TreeTableColumnDescription (type, width, name));
				}

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			foreach (var column in this.OrderedColumns)
			{
				content.Columns.Add (new TreeTableColumnItem ());
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

				var cellState1 = (i == selection) ? CellState.Selected : CellState.None;
				var cellState2 = cellState1 | (type == NodeType.Final ? CellState.None : CellState.Unavailable);

				int columnRank = 0;
				foreach (var column in this.OrderedColumns)
				{
					AbstractTreeTableCell cell;

					if (column == Column.Name)
					{
						var field = (baseType == BaseType.Groups) ? ObjectField.Name : this.accessor.GlobalSettings.GetMainStringField (BaseType.Assets);
						var text = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, field, inputValue: true);
						cell = new TreeTableCellTree (level, type, text, cellState1);
					}
					else
					{
						cell = new TreeTableCellDecimal (null, cellState2);
					}

					content.Columns[columnRank++].AddRow (cell);
				}
			}

			return content;
		}


		private int GetColumnWidth(Column colum)
		{
			if (colum == MCH2TreeTableFiller.Column.Name)
			{
				return 200;
			}
			else
			{
				return 100;
			}
		}

		private TreeTableColumnType GetColumnType(Column colum)
		{
			if (colum == MCH2TreeTableFiller.Column.Name)
			{
				return TreeTableColumnType.Tree;
			}
			else
			{
				return TreeTableColumnType.Amount;
			}
		}

		private string GetColumnName(Column colum)
		{
			switch (colum)
			{
				case Column.Name:
					return "Catégorie";

				case Column.InitialState:
					return "Etat initial";

				case Column.Inputs:
					return "Entrées";

				case Column.Reorganizations:
					return "Réorganisations";

				case Column.Outputs:
					return "Sorties";

				case Column.FinalState:
					return "Etat final";

				case Column.Amortizations:
					return "Amortissements";

				case Column.Revaluations:
					return "Réévaluations";

				case Column.Revalorizations:
					return "Revalorisations";

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown Columns {0}", colum));
			}
		}

		private IEnumerable<Column> OrderedColumns
		{
			get
			{
				yield return Column.Name;
				yield return Column.InitialState;
				yield return Column.Inputs;
				yield return Column.Reorganizations;
				yield return Column.Outputs;
				yield return Column.FinalState;
				yield return Column.Amortizations;
				yield return Column.Revaluations;
				yield return Column.Revalorizations;
			}
		}

		private enum Column
		{
			Name,

			InitialState,
			Inputs,
			Reorganizations,
			Outputs,

			FinalState,
			Amortizations,
			Revaluations,
			Revalorizations,
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
