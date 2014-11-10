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
	public class CategoriesTreeTableFiller : AbstractTreeTableFiller<SortableNode>
	{
		public CategoriesTreeTableFiller(DataAccessor accessor, INodeGetter<SortableNode> nodeGetter)
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

				columns.Add (new TreeTableColumnDescription (ObjectField.Name,             TreeTableColumnType.String, 180, Res.Strings.CategoriesTreeTableFiller.Name.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.Number,           TreeTableColumnType.String,  50, Res.Strings.CategoriesTreeTableFiller.Number.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AmortizationRate, TreeTableColumnType.Rate,    80, Res.Strings.CategoriesTreeTableFiller.AmortizationRate.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AmortizationType, TreeTableColumnType.String,  80, Res.Strings.CategoriesTreeTableFiller.AmortizationType.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.Periodicity,      TreeTableColumnType.String, 100, Res.Strings.CategoriesTreeTableFiller.Periodicity.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.Prorata,          TreeTableColumnType.String, 100, Res.Strings.CategoriesTreeTableFiller.Prorata.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.Round,            TreeTableColumnType.Amount, 100, Res.Strings.CategoriesTreeTableFiller.Round.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.ResidualValue,    TreeTableColumnType.Amount, 120, Res.Strings.CategoriesTreeTableFiller.ResidualValue.ToString ()));

				columns.Add (new TreeTableColumnDescription (ObjectField.AccountPurchaseDebit,           TreeTableColumnType.String, 150, Res.Strings.CategoriesTreeTableFiller.AccountPurchaseDebit.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AccountPurchaseCredit,          TreeTableColumnType.String, 150, Res.Strings.CategoriesTreeTableFiller.AccountPurchaseCredit.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AccountSaleDebit,               TreeTableColumnType.String, 150, Res.Strings.CategoriesTreeTableFiller.AccountSaleDebit.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AccountSaleCredit,              TreeTableColumnType.String, 150, Res.Strings.CategoriesTreeTableFiller.AccountSaleCredit.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AccountAmortizationAutoDebit,   TreeTableColumnType.String, 150, Res.Strings.CategoriesTreeTableFiller.AccountAmortizationAutoDebit.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AccountAmortizationAutoCredit,  TreeTableColumnType.String, 150, Res.Strings.CategoriesTreeTableFiller.AccountAmortizationAutoCredit.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AccountAmortizationExtraDebit,  TreeTableColumnType.String, 150, Res.Strings.CategoriesTreeTableFiller.AccountAmortizationExtraDebit.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AccountAmortizationExtraCredit, TreeTableColumnType.String, 150, Res.Strings.CategoriesTreeTableFiller.AccountAmortizationExtraCredit.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AccountIncreaseDebit,           TreeTableColumnType.String, 150, Res.Strings.CategoriesTreeTableFiller.AccountIncreaseDebit.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AccountIncreaseCredit,          TreeTableColumnType.String, 150, Res.Strings.CategoriesTreeTableFiller.AccountIncreaseCredit.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AccountDecreaseDebit,           TreeTableColumnType.String, 150, Res.Strings.CategoriesTreeTableFiller.AccountDecreaseDebit.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AccountDecreaseCredit,          TreeTableColumnType.String, 150, Res.Strings.CategoriesTreeTableFiller.AccountDecreaseCredit.ToString ()));

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			for (int i=0; i<8+12; i++)
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

				var name   = ObjectProperties.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Name, inputValue: true);
				var number = ObjectProperties.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Number);
				var rate   = ObjectProperties.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.AmortizationRate);
				var type   = ObjectProperties.GetObjectPropertyInt     (obj, this.Timestamp, ObjectField.AmortizationType);
				var period = ObjectProperties.GetObjectPropertyInt     (obj, this.Timestamp, ObjectField.Periodicity);
				var prorat = ObjectProperties.GetObjectPropertyInt     (obj, this.Timestamp, ObjectField.Prorata);
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

				var cell201 = new TreeTableCellString (this.GetAccount (obj, ObjectField.AccountPurchaseDebit),           cellState);
				var cell202 = new TreeTableCellString (this.GetAccount (obj, ObjectField.AccountPurchaseCredit),          cellState);
				var cell203 = new TreeTableCellString (this.GetAccount (obj, ObjectField.AccountSaleDebit),               cellState);
				var cell204 = new TreeTableCellString (this.GetAccount (obj, ObjectField.AccountSaleCredit),              cellState);
				var cell205 = new TreeTableCellString (this.GetAccount (obj, ObjectField.AccountAmortizationAutoDebit),   cellState);
				var cell206 = new TreeTableCellString (this.GetAccount (obj, ObjectField.AccountAmortizationAutoCredit),  cellState);
				var cell207 = new TreeTableCellString (this.GetAccount (obj, ObjectField.AccountAmortizationExtraDebit),  cellState);
				var cell208 = new TreeTableCellString (this.GetAccount (obj, ObjectField.AccountAmortizationExtraCredit), cellState);
				var cell209 = new TreeTableCellString (this.GetAccount (obj, ObjectField.AccountIncreaseDebit),           cellState);
				var cell210 = new TreeTableCellString (this.GetAccount (obj, ObjectField.AccountIncreaseCredit),          cellState);
				var cell211 = new TreeTableCellString (this.GetAccount (obj, ObjectField.AccountDecreaseDebit),           cellState);
				var cell212 = new TreeTableCellString (this.GetAccount (obj, ObjectField.AccountDecreaseCredit),          cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (cell11);
				content.Columns[columnRank++].AddRow (cell12);
				content.Columns[columnRank++].AddRow (cell13);
				content.Columns[columnRank++].AddRow (cell14);
				content.Columns[columnRank++].AddRow (cell15);
				content.Columns[columnRank++].AddRow (cell16);
				content.Columns[columnRank++].AddRow (cell17);
				content.Columns[columnRank++].AddRow (cell18);

				content.Columns[columnRank++].AddRow (cell201);
				content.Columns[columnRank++].AddRow (cell202);
				content.Columns[columnRank++].AddRow (cell203);
				content.Columns[columnRank++].AddRow (cell204);
				content.Columns[columnRank++].AddRow (cell205);
				content.Columns[columnRank++].AddRow (cell206);
				content.Columns[columnRank++].AddRow (cell207);
				content.Columns[columnRank++].AddRow (cell208);
				content.Columns[columnRank++].AddRow (cell209);
				content.Columns[columnRank++].AddRow (cell210);
				content.Columns[columnRank++].AddRow (cell211);
				content.Columns[columnRank++].AddRow (cell212);
			}

			return content;
		}

		private string GetAccount(DataObject obj, ObjectField field)
		{
			return ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, field);
		}
	}
}
