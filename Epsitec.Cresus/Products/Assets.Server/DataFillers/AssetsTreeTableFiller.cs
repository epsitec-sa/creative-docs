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
				yield return ObjectField.Value1;
				yield return ObjectField.Value2;
				yield return ObjectField.Maintenance;
				yield return ObjectField.Color;
				yield return ObjectField.SerialNumber;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Tree,           180, "Objet"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          50, "N°"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur comptable"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur assurance"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur imposable"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         120, "Maintenance"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          60, "Couleur"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         200, "Numéro de série"));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var cf = new TreeTableColumnItem ();
			var c1 = new TreeTableColumnItem ();
			var c2 = new TreeTableColumnItem ();
			var c3 = new TreeTableColumnItem ();
			var c4 = new TreeTableColumnItem ();
			var c5 = new TreeTableColumnItem ();
			var c6 = new TreeTableColumnItem ();
			var c7 = new TreeTableColumnItem ();

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

				var valeur1     = this.NodeGetter.GetValue (obj, node, ObjectField.MainValue);
				var valeur2     = this.NodeGetter.GetValue (obj, node, ObjectField.Value1);
				var valeur3     = this.NodeGetter.GetValue (obj, node, ObjectField.Value2);

				var nom         = AssetCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Name, inputValue: true);
				var numéro      = AssetCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Number);
				var maintenance = AssetCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Maintenance);
				var couleur     = AssetCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Color);
				var série       = AssetCalculator.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.SerialNumber);

				var cellState1 = (i == selection) ? CellState.Selected : CellState.None;
				var cellState2 = cellState1 | (type == NodeType.Final ? CellState.None : CellState.Unavailable);

				var sf = new TreeTableCellTree           (level, type, nom, cellState1);
				var s1 = new TreeTableCellString         (numéro,           cellState1);
				var s2 = new TreeTableCellComputedAmount (valeur1,          cellState2);
				var s3 = new TreeTableCellComputedAmount (valeur2,          cellState2);
				var s4 = new TreeTableCellComputedAmount (valeur3,          cellState2);
				var s5 = new TreeTableCellString         (maintenance,      cellState2);
				var s6 = new TreeTableCellString         (couleur,          cellState2);
				var s7 = new TreeTableCellString         (série,            cellState2);

				cf.AddRow (sf);
				c1.AddRow (s1);
				c2.AddRow (s2);
				c3.AddRow (s3);
				c4.AddRow (s4);
				c5.AddRow (s5);
				c6.AddRow (s6);
				c7.AddRow (s7);
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (cf);
			content.Columns.Add (c1);
			content.Columns.Add (c2);
			content.Columns.Add (c3);
			content.Columns.Add (c4);
			content.Columns.Add (c5);
			content.Columns.Add (c6);
			content.Columns.Add (c7);

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
