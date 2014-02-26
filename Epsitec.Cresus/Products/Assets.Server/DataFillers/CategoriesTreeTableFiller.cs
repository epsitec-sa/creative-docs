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

				yield return ObjectField.Compte1;
				yield return ObjectField.Compte2;
				yield return ObjectField.Compte3;
				yield return ObjectField.Compte4;
				yield return ObjectField.Compte5;
				yield return ObjectField.Compte6;
				yield return ObjectField.Compte7;
				yield return ObjectField.Compte8;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 180, "Catégorie"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 50, "N°"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Rate, 80, "Taux"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 80, "Type"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 100, "Périodicité"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 100, "Prorata"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, 100, "Arrondi"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, 120, "Valeur résiduelle"));

				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 150, "Comnpte 1"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 150, "Comnpte 2"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 150, "Comnpte 3"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 150, "Comnpte 4"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 150, "Comnpte 5"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 150, "Comnpte 6"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 150, "Comnpte 7"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 150, "Comnpte 8"));

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			for (int i=0; i<8+8; i++)
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

				var name   = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Name, inputValue: true);
				var number = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Number);
				var rate   = ObjectProperties.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.AmortizationRate);
				var type   = ObjectProperties.GetObjectPropertyInt (obj, this.Timestamp, ObjectField.AmortizationType);
				var period = ObjectProperties.GetObjectPropertyInt (obj, this.Timestamp, ObjectField.Periodicity);
				var prorat = ObjectProperties.GetObjectPropertyInt (obj, this.Timestamp, ObjectField.Prorata);
				var round  = ObjectProperties.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.Round);
				var resid  = ObjectProperties.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.ResidualValue);

				var t = EnumDictionaries.GetAmortizationTypeName (type);
				var c = EnumDictionaries.GetPeriodicityName (period);
				var r = EnumDictionaries.GetProrataTypeName (prorat);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var cell11 = new TreeTableCellString  (name,   cellState);
				var cell12 = new TreeTableCellString  (number, cellState);
				var cell13 = new TreeTableCellDecimal (rate,   cellState);
				var cell14 = new TreeTableCellString  (t,      cellState);
				var cell15 = new TreeTableCellString  (c,      cellState);
				var cell16 = new TreeTableCellString  (r,      cellState);
				var cell17 = new TreeTableCellDecimal (round,  cellState);
				var cell18 = new TreeTableCellDecimal (resid,  cellState);

				var cell21 = new TreeTableCellString (this.GetCompte (obj, ObjectField.Compte1), cellState);
				var cell22 = new TreeTableCellString (this.GetCompte (obj, ObjectField.Compte2), cellState);
				var cell23 = new TreeTableCellString (this.GetCompte (obj, ObjectField.Compte3), cellState);
				var cell24 = new TreeTableCellString (this.GetCompte (obj, ObjectField.Compte4), cellState);
				var cell25 = new TreeTableCellString (this.GetCompte (obj, ObjectField.Compte5), cellState);
				var cell26 = new TreeTableCellString (this.GetCompte (obj, ObjectField.Compte6), cellState);
				var cell27 = new TreeTableCellString (this.GetCompte (obj, ObjectField.Compte7), cellState);
				var cell28 = new TreeTableCellString (this.GetCompte (obj, ObjectField.Compte8), cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (cell11);
				content.Columns[columnRank++].AddRow (cell12);
				content.Columns[columnRank++].AddRow (cell13);
				content.Columns[columnRank++].AddRow (cell14);
				content.Columns[columnRank++].AddRow (cell15);
				content.Columns[columnRank++].AddRow (cell16);
				content.Columns[columnRank++].AddRow (cell17);
				content.Columns[columnRank++].AddRow (cell18);

				content.Columns[columnRank++].AddRow (cell21);
				content.Columns[columnRank++].AddRow (cell22);
				content.Columns[columnRank++].AddRow (cell23);
				content.Columns[columnRank++].AddRow (cell24);
				content.Columns[columnRank++].AddRow (cell25);
				content.Columns[columnRank++].AddRow (cell26);
				content.Columns[columnRank++].AddRow (cell27);
				content.Columns[columnRank++].AddRow (cell28);
			}

			return content;
		}

		private string GetCompte(DataObject obj, ObjectField field)
		{
			var guid = ObjectProperties.GetObjectPropertyGuid (obj, this.Timestamp, field);
			return AccountsLogic.GetSummary (this.accessor, guid);
		}
	}
}
