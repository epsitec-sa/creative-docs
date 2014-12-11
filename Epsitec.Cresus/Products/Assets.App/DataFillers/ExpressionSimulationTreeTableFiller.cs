﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.Expression;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public class ExpressionSimulationTreeTableFiller : AbstractTreeTableFiller<ExpressionSimulationNode>
	{
		public ExpressionSimulationTreeTableFiller(DataAccessor accessor, INodeGetter<ExpressionSimulationNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
			this.Title = "Simulation";  // anglais, ne pas traduire
		}


		public override SortingInstructions		DefaultSorting
		{
			get
			{
				return new SortingInstructions (ObjectField.ExpressionSimulationRank, SortedType.Ascending, ObjectField.Unknown, SortedType.None);
			}
		}

		public override int						DefaultDockToLeftCount
		{
			get
			{
				return 2;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (ObjectField.ExpressionSimulationRank,         TreeTableColumnType.Int,    ExpressionSimulationTreeTableFiller.rankWidth,   Res.Strings.DataFillers.ExpressionSimulationTreeTable.Rank.ToString ()));
				list.Add (new TreeTableColumnDescription (ObjectField.ExpressionSimulationDate,         TreeTableColumnType.Date,   ExpressionSimulationTreeTableFiller.dateWidth,   Res.Strings.DataFillers.ExpressionSimulationTreeTable.Date.ToString ()));
				list.Add (new TreeTableColumnDescription (ObjectField.ExpressionSimulationInitial,      TreeTableColumnType.Amount, ExpressionSimulationTreeTableFiller.amountWidth, Res.Strings.DataFillers.ExpressionSimulationTreeTable.Initial.ToString ()));
				list.Add (new TreeTableColumnDescription (ObjectField.ExpressionSimulationAmortization, TreeTableColumnType.Amount, ExpressionSimulationTreeTableFiller.amountWidth, Res.Strings.DataFillers.ExpressionSimulationTreeTable.Amortization.ToString ()));
				list.Add (new TreeTableColumnDescription (ObjectField.ExpressionSimulationFinal,        TreeTableColumnType.Amount, ExpressionSimulationTreeTableFiller.amountWidth, Res.Strings.DataFillers.ExpressionSimulationTreeTable.Final.ToString ()));
				list.Add (new TreeTableColumnDescription (ObjectField.ExpressionSimulationTrace,        TreeTableColumnType.String, ExpressionSimulationTreeTableFiller.traceWidth,  "Trace"));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			for (int i=0; i<6; i++)
			{
				content.Columns.Add (new TreeTableColumnItem ());
			}

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeGetter.Count)
				{
					break;
				}

				var node = this.nodeGetter[firstRow+i];

				var rank    = node.Rank;
				var date    = node.Date;
				var initial = node.InitialAmount;
				var amort   = node.InitialAmount - node.FinalAmount;
				var final   = node.FinalAmount;
				var trace   = ExpressionSimulationTreeTableFiller.ConvertToSingleLine (node.Trace);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var cell1 = new TreeTableCellInt     (rank,    cellState);
				var cell2 = new TreeTableCellDate    (date,    cellState);
				var cell3 = new TreeTableCellDecimal (initial, cellState);
				var cell4 = new TreeTableCellDecimal (amort,   cellState);
				var cell5 = new TreeTableCellDecimal (final,   cellState);
				var cell6 = new TreeTableCellString  (trace,   cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (cell1);
				content.Columns[columnRank++].AddRow (cell2);
				content.Columns[columnRank++].AddRow (cell3);
				content.Columns[columnRank++].AddRow (cell4);
				content.Columns[columnRank++].AddRow (cell5);
				content.Columns[columnRank++].AddRow (cell6);
			}

			return content;
		}


		private static string ConvertToSingleLine(string trace)
		{
			//	Converti "toto<br/>titi<br/>" en "toto / titi".
			if (!string.IsNullOrEmpty (trace))
			{
				const string sep = " / ";

				trace = trace.Replace ("<br/>", sep);

				if (trace.EndsWith (sep))
				{
					trace = trace.Substring (0, trace.Length-sep.Length);
				}
			}

			return trace;
		}


		public const int Width =  // largeur totale, avec un bout de la colonne 'Trace'
			ExpressionSimulationTreeTableFiller.rankWidth +
			ExpressionSimulationTreeTableFiller.dateWidth +
			ExpressionSimulationTreeTableFiller.amountWidth * 3 +
			100;

		private const int rankWidth   = 50;
		private const int dateWidth   = 80;
		private const int amountWidth = 100;
		private const int traceWidth  = 600;
	}
}
