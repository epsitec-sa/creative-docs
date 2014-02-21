//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class CategoriesTreeTableFiller : AbstractTreeTableFiller<SortableNode>
	{
		public CategoriesTreeTableFiller(DataAccessor accessor, AbstractNodeGetter<SortableNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.Name;
				yield return ObjectField.Number;
				yield return ObjectField.AmortizationRate;
				yield return ObjectField.AmortizationType;
				yield return ObjectField.Periodicity;
				yield return ObjectField.Prorata;
				yield return ObjectField.Round;
				yield return ObjectField.ResidualValue;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 180, "Catégorie"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  50, "N°"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Rate,    80, "Taux"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  80, "Type"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 100, "Périodicité"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 100, "Prorata"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, 100, "Arrondi"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, 120, "Valeur résiduelle"));

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			for (int i=0; i<8; i++)
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
				var obj   = this.accessor.GetObject (BaseType.Categories, guid);

				var nom    = ObjectProperties.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Name, inputValue: true);
				var numéro = ObjectProperties.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Number);
				var taux   = ObjectProperties.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.AmortizationRate);
				var type   = ObjectProperties.GetObjectPropertyInt     (obj, this.Timestamp, ObjectField.AmortizationType);
				var period = ObjectProperties.GetObjectPropertyInt     (obj, this.Timestamp, ObjectField.Periodicity);
				var prorat = ObjectProperties.GetObjectPropertyInt     (obj, this.Timestamp, ObjectField.Prorata);
				var round  = ObjectProperties.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.Round);
				var residu = ObjectProperties.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.ResidualValue);

				var t = EnumDictionaries.GetAmortizationTypeName (type);
				var c = EnumDictionaries.GetPeriodicityName (period);
				var r = EnumDictionaries.GetProrataTypeName (prorat);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;
				var s0 = new TreeTableCellString  (nom,    cellState);
				var s1 = new TreeTableCellString  (numéro, cellState);
				var s2 = new TreeTableCellDecimal (taux,   cellState);
				var s3 = new TreeTableCellString  (t,      cellState);
				var s4 = new TreeTableCellString  (c,      cellState);
				var s5 = new TreeTableCellString  (r,      cellState);
				var s6 = new TreeTableCellDecimal (round,  cellState);
				var s7 = new TreeTableCellDecimal (residu, cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (s0);
				content.Columns[columnRank++].AddRow (s1);
				content.Columns[columnRank++].AddRow (s2);
				content.Columns[columnRank++].AddRow (s3);
				content.Columns[columnRank++].AddRow (s4);
				content.Columns[columnRank++].AddRow (s5);
				content.Columns[columnRank++].AddRow (s6);
				content.Columns[columnRank++].AddRow (s7);
			}

			return content;
		}
	}
}
