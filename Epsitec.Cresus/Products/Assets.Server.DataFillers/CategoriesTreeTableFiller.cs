﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

				columns.Add (new TreeTableColumnDescription (ObjectField.Name,             TreeTableColumnType.String, 180, "Catégorie"));
				columns.Add (new TreeTableColumnDescription (ObjectField.Number,           TreeTableColumnType.String,  50, "N°"));
				columns.Add (new TreeTableColumnDescription (ObjectField.AmortizationRate, TreeTableColumnType.Rate,    80, "Taux"));
				columns.Add (new TreeTableColumnDescription (ObjectField.AmortizationType, TreeTableColumnType.String,  80, "Type"));
				columns.Add (new TreeTableColumnDescription (ObjectField.Periodicity,      TreeTableColumnType.String, 100, "Périodicité"));
				columns.Add (new TreeTableColumnDescription (ObjectField.Prorata,          TreeTableColumnType.String, 100, "Prorata"));
				columns.Add (new TreeTableColumnDescription (ObjectField.Round,            TreeTableColumnType.Amount, 100, "Arrondi"));
				columns.Add (new TreeTableColumnDescription (ObjectField.ResidualValue,    TreeTableColumnType.Amount, 120, "Valeur résiduelle"));

				columns.Add (new TreeTableColumnDescription (ObjectField.Account1,         TreeTableColumnType.String, 150, "Contrepartie achat"));
				columns.Add (new TreeTableColumnDescription (ObjectField.Account2,         TreeTableColumnType.String, 150, "Contrepartie vente"));
				columns.Add (new TreeTableColumnDescription (ObjectField.Account3,         TreeTableColumnType.String, 150, "Compte d'immob."));
				columns.Add (new TreeTableColumnDescription (ObjectField.Account4,         TreeTableColumnType.String, 150, "Compte d'amort."));
				columns.Add (new TreeTableColumnDescription (ObjectField.Account5,         TreeTableColumnType.String, 150, "Charge d'amort."));
				columns.Add (new TreeTableColumnDescription (ObjectField.Account6,         TreeTableColumnType.String, 150, "Compte réévaluation"));

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			for (int i=0; i<8+6; i++)
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

				var cell21 = new TreeTableCellString (this.GetAccount (obj, ObjectField.Account1), cellState);
				var cell22 = new TreeTableCellString (this.GetAccount (obj, ObjectField.Account2), cellState);
				var cell23 = new TreeTableCellString (this.GetAccount (obj, ObjectField.Account3), cellState);
				var cell24 = new TreeTableCellString (this.GetAccount (obj, ObjectField.Account4), cellState);
				var cell25 = new TreeTableCellString (this.GetAccount (obj, ObjectField.Account5), cellState);
				var cell26 = new TreeTableCellString (this.GetAccount (obj, ObjectField.Account6), cellState);
				//var cell27 = new TreeTableCellString (this.GetAccount (obj, ObjectField.Account7), cellState);
				//var cell28 = new TreeTableCellString (this.GetAccount (obj, ObjectField.Account8), cellState);

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
				//content.Columns[columnRank++].AddRow (cell27);
				//content.Columns[columnRank++].AddRow (cell28);
			}

			return content;
		}

		private string GetAccount(DataObject obj, ObjectField field)
		{
			var guid = ObjectProperties.GetObjectPropertyGuid (obj, this.Timestamp, field);
			return AccountsLogic.GetSummary (this.accessor, guid);
		}
	}
}
