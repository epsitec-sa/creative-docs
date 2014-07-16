﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class MCH2SummaryTreeTableFiller : AbstractTreeTableFiller<SortableCumulNode>
	{
		public MCH2SummaryTreeTableFiller(DataAccessor accessor, INodeGetter<SortableCumulNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public Timestamp						InitialTimestamp;
		public Timestamp						FinalTimestamp;


		public override SortingInstructions		DefaultSorting
		{
			get
			{
				return new SortingInstructions (ObjectField.MCH2Report, SortedType.Ascending, ObjectField.Unknown, SortedType.None);
			}
		}

		public override int						DefaultDockToLeftCount
		{
			get
			{
				return 1;
			}
		}

		public IEnumerable<ExtractionInstructions> UsedExtractionInstructions
		{
			//	Retourne la liste des instructions d'extraction, dans lesquelles on
			//	retrouve InitialTimestamp et FinalTimestamp.
			get
			{
				foreach (var column in this.OrderedColumns)
				{
					var ei = this.GetExtractionInstructions (column);
					if (!ei.IsEmpty)
					{
						yield return ei;
					}
				}
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				foreach (var column in this.OrderedColumns)
				{
					var field = ObjectField.MCH2Report + (int) column;
					var type  = this.GetColumnType  (column);
					var width = this.GetColumnWidth (column);
					var name  = this.GetColumnName  (column);

					columns.Add (new TreeTableColumnDescription (field, type, width, name));
				}

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			foreach (var column in this.OrderedColumns)
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
				foreach (var column in this.OrderedColumns)
				{
					AbstractTreeTableCell cell;

					if (column == Column.Name)
					{
						var field = (baseType == BaseType.Groups) ? ObjectField.Name : this.accessor.GlobalSettings.GetMainStringField (BaseType.Assets);
						var text = ObjectProperties.GetObjectPropertyString (obj, this.FinalTimestamp, field, inputValue: true);
						cell = new TreeTableCellTree (level, type, text, cellState1);
					}
					else
					{
						var value = this.GetColumnValue (node, obj, column);
						cell = new TreeTableCellDecimal (value, cellState2);
					}

					content.Columns[columnRank++].AddRow (cell);
				}
			}

			return content;
		}


		private decimal? GetColumnValue(SortableCumulNode node, DataObject obj, Column column)
		{
			//	Calcule la valeur d'une colonne.
			var field = ObjectField.MCH2Report + (int) column;

			//	Pour obtenir la valeur, il faut procéder avec le NodeGetter,
			//	pour tenir compte des cumuls (lorsque des lignes sont compactées).
			var v = this.NodeGetter.GetValue (obj, node, field);
			if (v.HasValue)
			{
				return v.Value;
			}
			else
			{
				return null;
			}
		}


		private ExtractionInstructions GetExtractionInstructions(Column column)
		{
			//	Retourne les instructions d'extraction permettant de peupler une colonne.
			var field = ObjectField.MCH2Report + (int) column;

			switch (column)
			{
				case Column.InitialState:
					return new ExtractionInstructions (field,
						ExtractionAmount.StateAt,
						new DateRange (System.DateTime.MinValue, this.InitialTimestamp.Date.AddDays (1)),
						EventType.Unknown);

				case Column.Inputs:
					return new ExtractionInstructions (field,
						ExtractionAmount.Filtered,
						new DateRange (this.InitialTimestamp.Date.AddDays (1), this.FinalTimestamp.Date.AddDays (1)),
						EventType.Input);

				case Column.Reorganizations:
					return new ExtractionInstructions (field,
						ExtractionAmount.Filtered,
						new DateRange (this.InitialTimestamp.Date.AddDays (1), this.FinalTimestamp.Date.AddDays (1)),
						EventType.Modification);

				case Column.Outputs:
					return new ExtractionInstructions (field,
						ExtractionAmount.Filtered,
						new DateRange (this.InitialTimestamp.Date.AddDays (1), this.FinalTimestamp.Date.AddDays (1)),
						EventType.Output);

				case Column.FinalState:
					return new ExtractionInstructions (field,
						ExtractionAmount.StateAt,
						new DateRange (System.DateTime.MinValue, this.FinalTimestamp.Date.AddDays (1)),
						EventType.Unknown);

				case Column.AmortizationsAuto:
					return new ExtractionInstructions (field,
						ExtractionAmount.Amortizations,
						new DateRange (this.InitialTimestamp.Date.AddDays (1), this.FinalTimestamp.Date.AddDays (1)),
						EventType.AmortizationAuto);

				case Column.AmortizationsExtra:
					return new ExtractionInstructions (field,
						ExtractionAmount.Amortizations,
						new DateRange (this.InitialTimestamp.Date.AddDays (1), this.FinalTimestamp.Date.AddDays (1)),
						EventType.AmortizationExtra);

				case Column.Revaluations:
					return new ExtractionInstructions (field,
						ExtractionAmount.Filtered,
						new DateRange (this.InitialTimestamp.Date.AddDays (1), this.FinalTimestamp.Date.AddDays (1)),
						EventType.Revaluation);

				case Column.Revalorizations:
					return new ExtractionInstructions (field,
						ExtractionAmount.Filtered,
						new DateRange (this.InitialTimestamp.Date.AddDays (1), this.FinalTimestamp.Date.AddDays (1)),
						EventType.Revalorization);

				default:
					return ExtractionInstructions.Empty;
			}
		}

		private int GetColumnWidth(Column column)
		{
			switch (column)
			{
				case Column.Name:
					return 200;

				case Column.InitialState:
				case Column.FinalState:
					return 150;

				default:
					return 100;
			}
		}

		private TreeTableColumnType GetColumnType(Column column)
		{
			if (column == MCH2SummaryTreeTableFiller.Column.Name)
			{
				return TreeTableColumnType.Tree;
			}
			else
			{
				return TreeTableColumnType.Amount;
			}
		}

		private string GetColumnName(Column column)
		{
			switch (column)
			{
				case Column.Name:
					return "Catégorie";  // nom de l'objet d'immobilisation, selon MCH2

				case Column.InitialState:
					var id = TypeConverters.DateToString (this.InitialTimestamp.Date);
					return string.Format ("Etat initial au {0}", id);

				case Column.Inputs:
					return "Entrées";

				case Column.Reorganizations:
					return "Réorganisations";

				case Column.Outputs:
					return "Sorties";

				case Column.FinalState:
					var fd = TypeConverters.DateToString (this.FinalTimestamp.Date);
					return string.Format ("Etat final au {0}", fd);

				case Column.AmortizationsAuto:
					return "Amort. ord.";

				case Column.AmortizationsExtra:
					return "Amort. extra.";

				case Column.Revaluations:
					return "Réévaluations";

				case Column.Revalorizations:
					return "Revalorisations";

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown Columns {0}", column));
			}
		}

		private IEnumerable<Column> OrderedColumns
		{
			get
			{
				yield return Column.Name;
				yield return Column.InitialState;
				yield return Column.Inputs;
				yield return Column.Reorganizations;
				yield return Column.Outputs;
				yield return Column.FinalState;
				yield return Column.AmortizationsAuto;
				yield return Column.AmortizationsExtra;
				yield return Column.Revaluations;
				yield return Column.Revalorizations;
			}
		}

		private enum Column
		{
			Name,

			InitialState,
			Inputs,
			Reorganizations,
			Outputs,

			FinalState,
			AmortizationsAuto,
			AmortizationsExtra,
			Revaluations,
			Revalorizations,
		}


		//?private MCH2SummaryNodeGetter NodeGetter
		//?{
		//?	get
		//?	{
		//?		return this.nodeGetter as MCH2SummaryNodeGetter;
		//?	}
		//?}
		private ObjectsNodeGetter NodeGetter
		{
			get
			{
				return this.nodeGetter as ObjectsNodeGetter;
			}
		}
	}
}
