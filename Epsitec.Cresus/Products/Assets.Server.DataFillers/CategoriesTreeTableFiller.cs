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

				columns.Add (new TreeTableColumnDescription (ObjectField.Name,                  TreeTableColumnType.String,  180, Res.Strings.CategoriesTreeTableFiller.Name.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.Number,                TreeTableColumnType.String,   50, Res.Strings.CategoriesTreeTableFiller.Number.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AmortizationMethod,    TreeTableColumnType.String,   70, Res.Strings.CategoriesTreeTableFiller.AmortizationMethod.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AmortizationRate,      TreeTableColumnType.Rate,     50, Res.Strings.CategoriesTreeTableFiller.AmortizationRate.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AmortizationYearCount, TreeTableColumnType.Decimal,  60, Res.Strings.CategoriesTreeTableFiller.AmortizationYearCount.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AmortizationType,      TreeTableColumnType.String,   80, Res.Strings.CategoriesTreeTableFiller.AmortizationType.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.Periodicity,           TreeTableColumnType.String,  100, Res.Strings.CategoriesTreeTableFiller.Periodicity.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.Prorata,               TreeTableColumnType.String,  100, Res.Strings.CategoriesTreeTableFiller.Prorata.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.Round,                 TreeTableColumnType.Amount,   70, Res.Strings.CategoriesTreeTableFiller.Round.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.ResidualValue,         TreeTableColumnType.Amount,  100, Res.Strings.CategoriesTreeTableFiller.ResidualValue.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.Expression,            TreeTableColumnType.String,  200, Res.Strings.CategoriesTreeTableFiller.Expression.ToString ()));

				foreach (var field in DataAccessor.AccountFields)
				{
					columns.Add (new TreeTableColumnDescription (field, TreeTableColumnType.String, 150, DataDescriptions.GetObjectFieldDescription (field)));
				}

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			int columnCount = 11 + DataAccessor.AccountFields.Count ();
			for (int i=0; i<columnCount; i++)
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
				var method = ObjectProperties.GetObjectPropertyInt     (obj, this.Timestamp, ObjectField.AmortizationMethod);
				var rate   = ObjectProperties.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.AmortizationRate);
				var years  = ObjectProperties.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.AmortizationYearCount);
				var type   = ObjectProperties.GetObjectPropertyInt     (obj, this.Timestamp, ObjectField.AmortizationType);
				var period = ObjectProperties.GetObjectPropertyInt     (obj, this.Timestamp, ObjectField.Periodicity);
				var prorat = ObjectProperties.GetObjectPropertyInt     (obj, this.Timestamp, ObjectField.Prorata);
				var round  = ObjectProperties.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.Round);
				var resid  = ObjectProperties.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.ResidualValue);
				var exp    = ObjectProperties.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Expression);

				if (method.HasValue)
				{
					var am = (AmortizationMethod) method;

					CategoriesTreeTableFiller.Hide (am, ObjectField.AmortizationRate,      ref rate);
					CategoriesTreeTableFiller.Hide (am, ObjectField.AmortizationYearCount, ref years);
					CategoriesTreeTableFiller.Hide (am, ObjectField.AmortizationType,      ref type);
					CategoriesTreeTableFiller.Hide (am, ObjectField.Periodicity,           ref period);
					CategoriesTreeTableFiller.Hide (am, ObjectField.Prorata,               ref prorat);
					CategoriesTreeTableFiller.Hide (am, ObjectField.Round,                 ref round);
					CategoriesTreeTableFiller.Hide (am, ObjectField.ResidualValue,         ref resid);
				}

				var m = EnumDictionaries.GetAmortizationMethodSummary (method);
				var t = EnumDictionaries.GetAmortizationTypeName (type);
				var c = EnumDictionaries.GetPeriodicityName (period);
				var r = EnumDictionaries.GetProrataTypeName (prorat);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var cell11 = new TreeTableCellString  (name,   cellState);
				var cell12 = new TreeTableCellString  (number, cellState);
				var cell13 = new TreeTableCellString  (m,      cellState);
				var cell14 = new TreeTableCellDecimal (rate,   cellState);
				var cell15 = new TreeTableCellDecimal (years,  cellState);
				var cell16 = new TreeTableCellString  (t,      cellState);
				var cell17 = new TreeTableCellString  (c,      cellState);
				var cell18 = new TreeTableCellString  (r,      cellState);
				var cell19 = new TreeTableCellDecimal (round,  cellState);
				var cell20 = new TreeTableCellDecimal (resid,  cellState);
				var cell21 = new TreeTableCellString  (exp,    cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (cell11);
				content.Columns[columnRank++].AddRow (cell12);
				content.Columns[columnRank++].AddRow (cell13);
				content.Columns[columnRank++].AddRow (cell14);
				content.Columns[columnRank++].AddRow (cell15);
				content.Columns[columnRank++].AddRow (cell16);
				content.Columns[columnRank++].AddRow (cell17);
				content.Columns[columnRank++].AddRow (cell18);
				content.Columns[columnRank++].AddRow (cell19);
				content.Columns[columnRank++].AddRow (cell20);
				content.Columns[columnRank++].AddRow (cell21);

				foreach (var field in DataAccessor.AccountFields)
				{
					var cell = new TreeTableCellString (this.GetAccount (obj, field), cellState);
					content.Columns[columnRank++].AddRow (cell);
				}
			}

			return content;
		}

		private string GetAccount(DataObject obj, ObjectField field)
		{
			return ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, field);
		}


		private static void Hide(AmortizationMethod method, ObjectField field, ref decimal? value)
		{
			if (Amortizations.IsHidden (method, field))
			{
				value = null;
			}
		}

		private static void Hide(AmortizationMethod method, ObjectField field, ref int? value)
		{
			if (Amortizations.IsHidden (method, field))
			{
				value = null;
			}
		}
	}
}
