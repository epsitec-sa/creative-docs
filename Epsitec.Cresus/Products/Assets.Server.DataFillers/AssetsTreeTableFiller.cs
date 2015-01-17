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
	public class AssetsTreeTableFiller : AbstractTreeTableFiller<SortableCumulNode>
	{
		public AssetsTreeTableFiller(DataAccessor accessor, INodeGetter<SortableCumulNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
			this.userFields = AssetsLogic.GetUserFields (this.accessor).ToArray ();
		}


		public override SortingInstructions		DefaultSorting
		{
			get
			{
				if (this.userFields.Any ())
				{
					var field = this.userFields.First ().Field;
					return new SortingInstructions (field, SortedType.Ascending, ObjectField.Unknown, SortedType.None);
				}
				else
				{
					return SortingInstructions.Empty;
				}
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

				int columnRank = 0;
				foreach (var userField in this.userFields)
				{
					TreeTableColumnType type;

					if (columnRank == 0)
					{
						type = TreeTableColumnType.Tree;
					}
					else
					{
						type = AbstractTreeTableCell.GetColumnType (userField.Type);
					}

					columns.Add (new TreeTableColumnDescription (userField.Field, type, userField.ColumnWidth, userField.Name));
					columnRank++;
				}

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			foreach (var userField in this.userFields)
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
				foreach (var userField in this.userFields)
				{
					AbstractTreeTableCell cell;

					if (columnRank == 0)  // nom ?
					{
						string text;

						if (baseType == BaseType.Groups)
						{
							var name = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Name, inputValue: true);
							var number = GroupsLogic.GetFullNumber (this.accessor, node.Guid);
							text = GroupsLogic.GetDescription (name, number);
						}
						else
						{
							text = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, userField.Field, inputValue: true);
						}

						cell = new TreeTableCellTree (level, type, text, cellState1);
					}
					else
					{
						if (userField.Type == FieldType.ComputedAmount)
						{
							//	Pour obtenir la valeur, il faut procéder avec le NodeGetter,
							//	pour tenir compte des cumuls (lorsque des lignes sont compactées).
							var v = this.NodeGetter.GetValue (obj, node, userField.Field);
							if (v.HasValue)
							{
								var ca = new ComputedAmount (v);
								cell = new TreeTableCellComputedAmount (ca, cellState2);
							}
							else
							{
								cell = new TreeTableCellComputedAmount (null, cellState2);
							}
						}
						else if (userField.Type == FieldType.AmortizedAmount)
						{
							//	Pour obtenir la valeur, il faut procéder avec le NodeGetter,
							//	pour tenir compte des cumuls (lorsque des lignes sont compactées).
							var v = this.NodeGetter.GetValue (obj, node, userField.Field);
							if (v.HasValue)
							{
								var aa = new AmortizedAmount (v);
								cell = new TreeTableCellAmortizedAmount (aa, cellState2);
							}
							else
							{
								cell = new TreeTableCellAmortizedAmount (null, cellState2);
							}
						}
						else
						{
							cell = AbstractTreeTableCell.CreateTreeTableCell (this.accessor, obj, this.Timestamp, userField, false, cellState2);
						}
					}

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


		private readonly UserField[] userFields;
	}
}
