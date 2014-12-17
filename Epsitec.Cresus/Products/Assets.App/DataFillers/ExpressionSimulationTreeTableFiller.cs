//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
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
			this.Title = Res.Strings.DataFillers.ExpressionSimulation.Title.ToString ();
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
				list.Add (new TreeTableColumnDescription (ObjectField.ExpressionSimulationType,         TreeTableColumnType.Glyph,  ExpressionSimulationTreeTableFiller.typeWidth,   null));
				list.Add (new TreeTableColumnDescription (ObjectField.ExpressionSimulationInitial,      TreeTableColumnType.Amount, ExpressionSimulationTreeTableFiller.amountWidth, Res.Strings.DataFillers.ExpressionSimulationTreeTable.Initial.ToString ()));
				list.Add (new TreeTableColumnDescription (ObjectField.ExpressionSimulationAmortization, TreeTableColumnType.Amount, ExpressionSimulationTreeTableFiller.amountWidth, Res.Strings.DataFillers.ExpressionSimulationTreeTable.Amortization.ToString ()));
				list.Add (new TreeTableColumnDescription (ObjectField.ExpressionSimulationFinal,        TreeTableColumnType.Amount, ExpressionSimulationTreeTableFiller.amountWidth, Res.Strings.DataFillers.ExpressionSimulationTreeTable.Final.ToString ()));

				list.Add (new TreeTableColumnDescription (ObjectField.ExpressionSimulationDebug+0, TreeTableColumnType.Date,   ExpressionSimulationTreeTableFiller.dateWidth,   "InputDate"));
				list.Add (new TreeTableColumnDescription (ObjectField.ExpressionSimulationDebug+1, TreeTableColumnType.Amount, ExpressionSimulationTreeTableFiller.amountWidth, "InputAmount"));
				list.Add (new TreeTableColumnDescription (ObjectField.ExpressionSimulationDebug+2, TreeTableColumnType.Date,   ExpressionSimulationTreeTableFiller.dateWidth,   "BaseDate"));
				list.Add (new TreeTableColumnDescription (ObjectField.ExpressionSimulationDebug+3, TreeTableColumnType.Amount, ExpressionSimulationTreeTableFiller.amountWidth, "BaseAmount"));
				list.Add (new TreeTableColumnDescription (ObjectField.ExpressionSimulationDebug+4, TreeTableColumnType.Amount, ExpressionSimulationTreeTableFiller.amountWidth, "InitialAmount"));

				list.Add (new TreeTableColumnDescription (ObjectField.ExpressionSimulationTrace, TreeTableColumnType.String, ExpressionSimulationTreeTableFiller.traceWidth, Res.Strings.DataFillers.ExpressionSimulationTreeTable.Trace.ToString ()));
				list.Add (new TreeTableColumnDescription (ObjectField.ExpressionSimulationError, TreeTableColumnType.String, ExpressionSimulationTreeTableFiller.errorWidth, Res.Strings.DataFillers.ExpressionSimulationTreeTable.Error.ToString ()));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			for (int i=0; i<6+5+2; i++)
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

				var rank    = node.Rank.HasValue ? node.Rank+1 : null;  // 1..n
				var date    = node.Date;
				var type    = TimelineData.TypeToGlyph (node.EventType);
				var initial = node.InitialAmount;
				var amort   = node.Amortization;
				var final   = node.FinalAmount;
				var trace   = ExpressionSimulationTreeTableFiller.ConvertTraceToSingleLine (node.Trace);
				var error   = ExpressionSimulationTreeTableFiller.ConvertTraceToSingleLine (node.Error);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var cell11 = new TreeTableCellInt     (rank,    cellState);
				var cell12 = new TreeTableCellDate    (date,    cellState);
				var cell13 = new TreeTableCellGlyph   (type,    cellState);
				var cell14 = new TreeTableCellDecimal (initial, cellState);
				var cell15 = new TreeTableCellDecimal (amort,   cellState);
				var cell16 = new TreeTableCellDecimal (final,   cellState);

				System.DateTime? inputDate     = null;
				decimal?         inputAmount   = null;
				System.DateTime? baseDate      = null;
				decimal?         baseAmount    = null;
				decimal?         initialAmount = null;

				if (!node.Details.IsEmpty)
				{
					inputDate     = node.Details.History.InputDate;
					inputAmount   = node.Details.History.InputAmount;
					baseDate      = node.Details.History.BaseDate;
					baseAmount    = node.Details.History.BaseAmount;
					initialAmount = node.Details.History.InitialAmount;
				}

				var cell21 = new TreeTableCellDate    (inputDate,     cellState);
				var cell22 = new TreeTableCellDecimal (inputAmount,   cellState);
				var cell23 = new TreeTableCellDate    (baseDate,      cellState);
				var cell24 = new TreeTableCellDecimal (baseAmount,    cellState);
				var cell25 = new TreeTableCellDecimal (initialAmount, cellState);

				var cell31 = new TreeTableCellString (trace, cellState);
				var cell32 = new TreeTableCellString (trace, cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (cell11);
				content.Columns[columnRank++].AddRow (cell12);
				content.Columns[columnRank++].AddRow (cell13);
				content.Columns[columnRank++].AddRow (cell14);
				content.Columns[columnRank++].AddRow (cell15);
				content.Columns[columnRank++].AddRow (cell16);

				content.Columns[columnRank++].AddRow (cell21);
				content.Columns[columnRank++].AddRow (cell22);
				content.Columns[columnRank++].AddRow (cell23);
				content.Columns[columnRank++].AddRow (cell24);
				content.Columns[columnRank++].AddRow (cell25);

				content.Columns[columnRank++].AddRow (cell31);
				content.Columns[columnRank++].AddRow (cell32);
			}

			return content;
		}


		public static string ConvertTraceToSingleLine(string trace)
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
			ExpressionSimulationTreeTableFiller.typeWidth +
			ExpressionSimulationTreeTableFiller.amountWidth * 3;

		private const int rankWidth   = 50;
		private const int dateWidth   = 80;
		private const int typeWidth   = 20;
		private const int amountWidth = 100;
		private const int traceWidth  = 400;
		private const int errorWidth  = 200;
	}
}
