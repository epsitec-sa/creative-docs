﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
				foreach (var userField in this.UserFields)
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

				AbstractTreeTableCell.AddColumnDescription (columns, this.UserFields, treeFirst: true);

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			foreach (var userField in this.UserFields)
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

				var cellState1 = (i == selection) ? CellState.Selected : CellState.None;
				var cellState2 = cellState1 | (type == NodeType.Final ? CellState.None : CellState.Unavailable);

				int columnRank = 0;
				foreach (var userField in this.UserFields)
				{
					AbstractTreeTableCell cell;

					if (columnRank == 0)
					{
						var field = (baseType == BaseType.Groups) ? ObjectField.Name : userField.Field;
						var text = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, field, inputValue: true);
						cell = new TreeTableCellTree (level, type, text, cellState1);
					}
					else
					{
						if (userField.Type == FieldType.ComputedAmount)
						{
							//	Pour obtenir la valeur, il faut procéder avec le NodeGetter,
							//	pour tenir compte des cumuls (lorsque des lignes sont compactées).
							var ca = this.NodeGetter.GetValue (obj, node, userField.Field);
							cell = new TreeTableCellComputedAmount (ca, cellState2);
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


		private IEnumerable<UserField> UserFields
		{
			get
			{
				foreach (var userField in accessor.Settings.GetUserFields (BaseType.Assets))
				{
					if (userField.Type == FieldType.ComputedAmount)
					{
						//	Juste avant la première valeur utilisateur, on injecte la valeur comptable.
						//	Le Guid créé à la volée n'est pas utilisé !
						yield return new UserField (DataDescriptions.GetObjectFieldDescription (ObjectField.MainValue), ObjectField.MainValue, FieldType.ComputedAmount, userField.ColumnWidth, null, null, null, 0);
					}

					yield return userField;
				}
			}
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