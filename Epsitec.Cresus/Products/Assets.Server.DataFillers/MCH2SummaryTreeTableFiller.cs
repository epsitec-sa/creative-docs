//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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


		public DateRange						DateRange;


		public override SortingInstructions		DefaultSorting
		{
			get
			{
				return new SortingInstructions (this.MainStringField, SortedType.Ascending, ObjectField.Unknown, SortedType.None);
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
					ObjectField field;

					if (column == Column.Name)
					{
						field = this.MainStringField;
					}
					else
					{
						field = ObjectField.MCH2Report + (int) column;
					}

					var type    = this.GetColumnType    (column);
					var width   = this.GetColumnWidth   (column);
					var name    = this.GetColumnName    (column);
					var tooltip = this.GetColumnTooltip (column);

					columns.Add (new TreeTableColumnDescription (field, type, width, name, tooltip));
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
						string text;

						if (baseType == BaseType.Groups)
						{
							var name = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Name, inputValue: true);
							var number = GroupsLogic.GetFullNumber (this.accessor, node.Guid);
							text = GroupsLogic.GetDescription (name, number);
						}
						else
						{
							text = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, this.MainStringField, inputValue: true);
						}

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
					//	Avec une période du 01.01.2014 au 31.12.2014, on cherche l'état avant
					//	le premier janvier, donc au 31.12.2013 23:59:59.
					return new ExtractionInstructions (field,
						ExtractionAmount.StateAt,
						new DateRange (System.DateTime.MinValue, this.DateRange.IncludeFrom.AddTicks (-1)),
						EventType.Unknown);

				case Column.Inputs:
					return new ExtractionInstructions (field,
						ExtractionAmount.DeltaFiltered,
						this.DateRange,
						EventType.Input);

				case Column.Reorganizations:
					return new ExtractionInstructions (field,
						ExtractionAmount.DeltaFiltered,
						this.DateRange,
						EventType.Modification);

				case Column.Decreases:
					return new ExtractionInstructions (field,
						ExtractionAmount.LastFiltered,  // le type DeltaFiltered semble mal adapté ?
						this.DateRange,
						EventType.Decrease);

				case Column.Increases:
					return new ExtractionInstructions (field,
						ExtractionAmount.LastFiltered,  // le type DeltaFiltered semble mal adapté ?
						this.DateRange,
						EventType.Increase);

				case Column.Outputs:
					return new ExtractionInstructions (field,
						ExtractionAmount.DeltaFiltered,
						this.DateRange,
						EventType.Output);

				case Column.AmortizationsAuto:
					return new ExtractionInstructions (field,
						ExtractionAmount.Amortizations,
						this.DateRange,
						EventType.AmortizationAuto);

				case Column.AmortizationsExtra:
					return new ExtractionInstructions (field,
						ExtractionAmount.Amortizations,
						this.DateRange,
						EventType.AmortizationExtra);

				case Column.FinalState:
					//	Avec une période du 01.01.2014 au 31.12.2014, on cherche l'état après
					//	le 31 décembre. Comme la date "au" est exclue dans un DateRange, la date
					//	ExcludeTo vaut 01.01.2015. On cherche donc l'état au 31.12.2014 23:59:59.
					return new ExtractionInstructions (field,
						ExtractionAmount.StateAt,
						new DateRange (System.DateTime.MinValue, this.DateRange.ExcludeTo.Date.AddTicks (-1)),
						EventType.Unknown);

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
					return Res.Strings.Enum.MCH2Summary.Column.Name.Text.ToString ();

				case Column.InitialState:
					return string.Format (Res.Strings.Enum.MCH2Summary.Column.InitialState.Text.ToString (), this.InitialDate);

				case Column.Inputs:
					return Res.Strings.Enum.MCH2Summary.Column.Inputs.Text.ToString ();

				case Column.Reorganizations:
					return Res.Strings.Enum.MCH2Summary.Column.Reorganizations.Text.ToString ();

				case Column.Decreases:
					return Res.Strings.Enum.MCH2Summary.Column.Decreases.Text.ToString ();

				case Column.Increases:
					return Res.Strings.Enum.MCH2Summary.Column.Increases.Text.ToString ();

				case Column.Outputs:
					return Res.Strings.Enum.MCH2Summary.Column.Outputs.Text.ToString ();

				case Column.AmortizationsAuto:
					return Res.Strings.Enum.MCH2Summary.Column.AmortizationsAuto.Text.ToString ();

				case Column.AmortizationsExtra:
					return Res.Strings.Enum.MCH2Summary.Column.AmortizationsExtra.Text.ToString ();

				case Column.FinalState:
					return string.Format (Res.Strings.Enum.MCH2Summary.Column.FinalState.Text.ToString (), this.FinalDate);

				default:
					return null;
			}
		}

		private string GetColumnTooltip(Column column)
		{
			switch (column)
			{
				case Column.Name:
					return Res.Strings.Enum.MCH2Summary.Column.Name.Tooltip.ToString ();

				case Column.InitialState:
					return string.Format (Res.Strings.Enum.MCH2Summary.Column.InitialState.Tooltip.ToString (), this.InitialDateTooltip);

				case Column.Inputs:
					return Res.Strings.Enum.MCH2Summary.Column.Inputs.Tooltip.ToString ();

				case Column.Reorganizations:
					return Res.Strings.Enum.MCH2Summary.Column.Reorganizations.Tooltip.ToString ();

				case Column.Decreases:
					return Res.Strings.Enum.MCH2Summary.Column.Decreases.Tooltip.ToString ();

				case Column.Increases:
					return Res.Strings.Enum.MCH2Summary.Column.Increases.Tooltip.ToString ();

				case Column.Outputs:
					return Res.Strings.Enum.MCH2Summary.Column.Outputs.Tooltip.ToString ();

				case Column.AmortizationsAuto:
					return Res.Strings.Enum.MCH2Summary.Column.AmortizationsAuto.Tooltip.ToString ();

				case Column.AmortizationsExtra:
					return Res.Strings.Enum.MCH2Summary.Column.AmortizationsExtra.Tooltip.ToString ();

				case Column.FinalState:
					return string.Format (Res.Strings.Enum.MCH2Summary.Column.FinalState.Tooltip.ToString (), this.FinalDateTooltip);

				default:
					return null;
			}
		}

		private string InitialDate
		{
			get
			{
				if (this.DateRange.IsEmpty)
				{
					return "?";
				}
				else
				{
					return TypeConverters.DateToString (this.DateRange.IncludeFrom);
				}
			}
		}

		private string FinalDate
		{
			get
			{
				if (this.DateRange.IsEmpty)
				{
					return "?";
				}
				else
				{
					return TypeConverters.DateToString (this.DateRange.ExcludeTo.AddDays (-1));
				}
			}
		}

		private string InitialDateTooltip
		{
			get
			{
				if (this.DateRange.IsEmpty)
				{
					return "?";
				}
				else
				{
					var date = TypeConverters.DateToString (this.DateRange.IncludeFrom.AddDays (-1));
					return string.Format (Res.Strings.MCH2SummaryTreeTableFiller.DateOneMinuteToMidnight.ToString (), date);  // 31.12.xx à 23h59
				}
			}
		}

		private string FinalDateTooltip
		{
			get
			{
				if (this.DateRange.IsEmpty)
				{
					return "?";
				}
				else
				{
					var date = TypeConverters.DateToString (this.DateRange.ExcludeTo.AddDays (-1));
					return string.Format (Res.Strings.MCH2SummaryTreeTableFiller.DateOneMinuteToMidnight.ToString (), date);  // 31.12.xx à 23h59
				}
			}
		}

		private IEnumerable<Column> OrderedColumns
		{
			//	Retourne les colonnes visibles, dans le bon ordre.
			get
			{
				yield return Column.Name;
				yield return Column.InitialState;
				yield return Column.Inputs;
//?				yield return Column.Reorganizations;  // l'événement de modification ne modifie jamais la valeur comptable
				yield return Column.Decreases;
				yield return Column.Increases;
				yield return Column.Outputs;
				yield return Column.AmortizationsAuto;
				yield return Column.AmortizationsExtra;
				yield return Column.FinalState;
			}
		}

		private ObjectField MainStringField
		{
			get
			{
				return this.accessor.GetMainStringField (BaseType.Assets);
			}
		}

		private enum Column
		{
			Name,

			InitialState,
			Inputs,
			Reorganizations,
			Decreases,
			Increases,
			Outputs,
			AmortizationsAuto,
			AmortizationsExtra,
			FinalState,
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
