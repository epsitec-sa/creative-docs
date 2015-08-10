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
			this.userColumns = new List<UserColumn> ();
			this.InitializeUserColumns ();
		}


		public DateRange						DateRange;
		public bool								DirectMode;


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
						if (column < Column.User)
						{
							field = ObjectField.MCH2Report + (int) column;
						}
						else
						{
							var userColumn = this.userColumns[(column-Column.User)];
							field = userColumn.Field;
						}
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
						var columnType = this.GetColumnType (column);

						switch (columnType)
						{
							case TreeTableColumnType.Date:
								{
									var v = value as DateCumulValue;
									if (v != null && v.IsRange)
									{
										cell = new TreeTableCellString ("...", cellState2);
									}
									else
									{
										cell = new TreeTableCellDate ((v == null) ? null : v.MinValue, cellState2);
									}
								}
								break;

							case TreeTableColumnType.String:
								{
									var v = value as StringCumulValue;
									cell = new TreeTableCellString ((v == null) ? null : v.Value, cellState2);
								}
								break;

							case TreeTableColumnType.Amount:
								{
									var v = value as DecimalCumulValue;
									cell = new TreeTableCellDecimal ((v == null) ? null : v.Value, cellState2);
								}
								break;

							default:
								throw new System.InvalidOperationException (string.Format ("Unknown TreeTableColumnType {0}", columnType.ToString ()));
						}
					}

					content.Columns[columnRank++].AddRow (cell);
				}
			}

			return content;
		}


		private AbstractCumulValue GetColumnValue(SortableCumulNode node, DataObject obj, Column column)
		{
			//	Calcule la valeur d'une colonne.
			ObjectField field;

			if (column < Column.User)
			{
				field = ObjectField.MCH2Report + (int) column;
			}
			else  // colonne supplémentaire ?
			{
				var userColumn = this.userColumns[(column-Column.User)];
				field = userColumn.Field;
			}

			//	Pour obtenir la valeur, il faut procéder avec le NodeGetter,
			//	pour tenir compte des cumuls (lorsque des lignes sont compactées).
			return this.NodeGetter.GetValue (obj, node, field);
		}


		private ExtractionInstructions GetExtractionInstructions(Column column)
		{
			//	Retourne les instructions d'extraction permettant de peupler une colonne.
			if (column >= Column.User)
			{
				var userColumn = this.userColumns[(column-Column.User)];

				return new ExtractionInstructions (userColumn.Field,
						ExtractionAmount.UserColumn,
						new DateRange (System.DateTime.MinValue, this.DateRange.ExcludeTo.Date.AddTicks (-1)),
						false);
			}

			var field = ObjectField.MCH2Report + (int) column;

			switch (column)
			{
				//	Mode direct: Etat d'entrée.
				case Column.InitialState:
					//	Avec une période du 01.01.2014 au 31.12.2014, on cherche l'état avant
					//	le premier janvier, donc au 31.12.2013 23:59:59.
					return new ExtractionInstructions (field,
						ExtractionAmount.StateAt,
						new DateRange (System.DateTime.MinValue, this.DateRange.IncludeFrom.AddTicks (-1)),
						false);

				//	Mode direct et indirect: Valeur selon les événements d'entrée de financement préalable.
				case Column.PreInputs:
					return new ExtractionInstructions (field,
						ExtractionAmount.DeltaSum,
						this.DateRange,
						false,
						EventType.PreInput);

				//	Mode direct: Entrées de mises en service.
				case Column.Inputs:
					return new ExtractionInstructions (field,
						ExtractionAmount.DeltaSum,
						this.DateRange,
						false,
						EventType.Input);

				//	Mode indirect: Valeurs à neuf.
				case Column.ReplacementValues:
					return new ExtractionInstructions (field,
						ExtractionAmount.LastFiltered,
						new DateRange (System.DateTime.MinValue, this.DateRange.ExcludeTo.Date.AddTicks (-1)),
						false,
						EventType.Input);

				//	Mode indirect: Somme des réévaluations et revalorisations.
				case Column.DeltaDecreasesIncreases:
					return new ExtractionInstructions (field,
						ExtractionAmount.DeltaSum,
						new DateRange (System.DateTime.MinValue, this.DateRange.ExcludeTo.Date.AddTicks (-1)),
						true,
						EventType.Increase, EventType.Decrease);

				//	Mode indirect: Somme des corrections.
				case Column.DeltaAdjusts:
					return new ExtractionInstructions (field,
						ExtractionAmount.DeltaSum,
						new DateRange (System.DateTime.MinValue, this.DateRange.ExcludeTo.Date.AddTicks (-1)),
						true,
						EventType.Adjust);

				//	Mode indirect: Somme des amortissements.
				case Column.DeltaAmortizations:
					return new ExtractionInstructions (field,
						ExtractionAmount.DeltaSum,
						new DateRange (System.DateTime.MinValue, this.DateRange.ExcludeTo.Date.AddTicks (-1)),
						true,
						EventType.AmortizationAuto, EventType.AmortizationExtra, EventType.AmortizationPreview, EventType.AmortizationSuppl);

				//	Normalement plus utilisé.
				case Column.Reorganizations:
					return new ExtractionInstructions (field,
						ExtractionAmount.DeltaSum,
						this.DateRange,
						false,
						EventType.Modification);

				//	Mode direct: Réévaluations.
				case Column.Decreases:
					return new ExtractionInstructions (field,
						this.DirectMode ? ExtractionAmount.LastFiltered : ExtractionAmount.DeltaSum,
						this.DateRange,
						true,
						EventType.Decrease);

				//	Mode direct: Revaloristions.
				case Column.Increases:
					return new ExtractionInstructions (field,
						this.DirectMode ? ExtractionAmount.LastFiltered : ExtractionAmount.DeltaSum,
						this.DateRange,
						false,
						EventType.Increase);

				//	Mode direct: Corrections.
				case Column.Adjusts:
					return new ExtractionInstructions (field,
						this.DirectMode ? ExtractionAmount.LastFiltered : ExtractionAmount.DeltaSum,
						this.DateRange,
						false,
						EventType.Adjust);

				//	Mode direct et indirect: Sorties.
				case Column.Outputs:
					return new ExtractionInstructions (field,
						ExtractionAmount.DeltaSum,
						this.DateRange,
						true,
						EventType.Output);

				//	Mode direct: Amortissements automatiques.
				case Column.AmortizationsAuto:
					return new ExtractionInstructions (field,
						ExtractionAmount.DeltaSum,
						this.DateRange,
						true,
						EventType.AmortizationAuto, EventType.AmortizationPreview);

				//	Mode direct: Amortissements extraordinaires.
				case Column.AmortizationsExtra:
					return new ExtractionInstructions (field,
						ExtractionAmount.DeltaSum,
						this.DateRange,
						true,
						EventType.AmortizationExtra);

				//	Mode direct: Amortissements supplémentaires.
				case Column.AmortizationsSuppl:
					return new ExtractionInstructions (field,
						ExtractionAmount.DeltaSum,
						this.DateRange,
						true,
						EventType.AmortizationSuppl);

				//	Mode direct et indirect: Valeurs finales.
				case Column.FinalState:
					//	Avec une période du 01.01.2014 au 31.12.2014, on cherche l'état après
					//	le 31 décembre. Comme la date "au" est exclue dans un DateRange, la date
					//	ExcludeTo vaut 01.01.2015. On cherche donc l'état au 31.12.2014 23:59:59.
					return new ExtractionInstructions (field,
						ExtractionAmount.StateAt,
						new DateRange (System.DateTime.MinValue, this.DateRange.ExcludeTo.Date.AddTicks (-1)),
						false);

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
			if (column == Column.Name)
			{
				return TreeTableColumnType.Tree;
			}
			else if (column >= Column.User)
			{
				var userColumn = this.userColumns[(column-Column.User)];

				switch (userColumn.Type)
				{
					case FieldType.Date:
						return TreeTableColumnType.Date;

					case FieldType.String:
						return TreeTableColumnType.String;

					default:
						return TreeTableColumnType.Amount;
				}
			}
			else
			{
				return TreeTableColumnType.Amount;
			}
		}

		private string GetColumnName(Column column)
		{
			if (column >= Column.User)
			{
				//	Retourne le nom d'une colonne supplémentaire.
				return this.userColumns[(column-Column.User)].Name;
			}

			switch (column)
			{
				case Column.Name:
					return Res.Strings.Enum.MCH2Summary.Column.Name.Text.ToString ();

				case Column.InitialState:
					return string.Format (Res.Strings.Enum.MCH2Summary.Column.InitialState.Text.ToString (), this.InitialDate);

				case Column.PreInputs:
					return Res.Strings.Enum.MCH2Summary.Column.PreInputs.Text.ToString ();

				case Column.Inputs:
					return Res.Strings.Enum.MCH2Summary.Column.Inputs.Text.ToString ();

				case Column.ReplacementValues:
					return Res.Strings.Enum.MCH2Summary.Column.ReplacementValues.Text.ToString ();

				case Column.DeltaAdjusts:
					return Res.Strings.Enum.MCH2Summary.Column.DeltaAdjusts.Text.ToString ();

				case Column.DeltaDecreasesIncreases:
					return Res.Strings.Enum.MCH2Summary.Column.DeltaDecreasesIncreases.Text.ToString ();

				case Column.DeltaAmortizations:
					return Res.Strings.Enum.MCH2Summary.Column.DeltaAmortizations.Text.ToString ();

				case Column.Reorganizations:
					return Res.Strings.Enum.MCH2Summary.Column.Reorganizations.Text.ToString ();

				case Column.Decreases:
					return Res.Strings.Enum.MCH2Summary.Column.Decreases.Text.ToString ();

				case Column.Increases:
					return Res.Strings.Enum.MCH2Summary.Column.Increases.Text.ToString ();

				case Column.Adjusts:
					return Res.Strings.Enum.MCH2Summary.Column.Adjusts.Text.ToString ();

				case Column.Outputs:
					return Res.Strings.Enum.MCH2Summary.Column.Outputs.Text.ToString ();

				case Column.AmortizationsAuto:
					return Res.Strings.Enum.MCH2Summary.Column.AmortizationsAuto.Text.ToString ();

				case Column.AmortizationsExtra:
					return Res.Strings.Enum.MCH2Summary.Column.AmortizationsExtra.Text.ToString ();

				case Column.AmortizationsSuppl:
					return Res.Strings.Enum.MCH2Summary.Column.AmortizationsSuppl.Text.ToString ();

				case Column.FinalState:
					return string.Format (Res.Strings.Enum.MCH2Summary.Column.FinalState.Text.ToString (), this.FinalDate);

				default:
					return null;
			}
		}

		private string GetColumnTooltip(Column column)
		{
			if (column >= Column.User)
			{
				//	Retourne le tooltip d'une colonne supplémentaire, par exemple "Valeur fiscale le 31.12.2015 à 23h59".
				var userColumn = this.userColumns[(column-Column.User)];
				return string.Format (Res.Strings.Enum.MCH2Summary.Column.UserColumn.Tooltip.ToString (), userColumn.Name, this.FinalDateTooltip);
			}

			switch (column)
			{
				case Column.Name:
					return Res.Strings.Enum.MCH2Summary.Column.Name.Tooltip.ToString ();

				case Column.InitialState:
					return string.Format (Res.Strings.Enum.MCH2Summary.Column.InitialState.Tooltip.ToString (), this.InitialDateTooltip);

				case Column.PreInputs:
					return Res.Strings.Enum.MCH2Summary.Column.PreInputs.Tooltip.ToString ();

				case Column.Inputs:
					return Res.Strings.Enum.MCH2Summary.Column.Inputs.Tooltip.ToString ();

				case Column.ReplacementValues:
					return Res.Strings.Enum.MCH2Summary.Column.ReplacementValues.Tooltip.ToString ();

				case Column.DeltaAdjusts:
					return Res.Strings.Enum.MCH2Summary.Column.DeltaAdjusts.Tooltip.ToString ();

				case Column.DeltaDecreasesIncreases:
					return Res.Strings.Enum.MCH2Summary.Column.DeltaDecreasesIncreases.Tooltip.ToString ();

				case Column.DeltaAmortizations:
					return Res.Strings.Enum.MCH2Summary.Column.DeltaAmortizations.Tooltip.ToString ();

				case Column.Reorganizations:
					return Res.Strings.Enum.MCH2Summary.Column.Reorganizations.Tooltip.ToString ();

				case Column.Decreases:
					return this.DirectMode ?
						Res.Strings.Enum.MCH2Summary.Column.Decreases.Direct.Tooltip.ToString () :
						Res.Strings.Enum.MCH2Summary.Column.Decreases.Indirect.Tooltip.ToString ();

				case Column.Increases:
					return this.DirectMode ?
						Res.Strings.Enum.MCH2Summary.Column.Increases.Direct.Tooltip.ToString () :
						Res.Strings.Enum.MCH2Summary.Column.Increases.Indirect.Tooltip.ToString ();

				case Column.Adjusts:
					return this.DirectMode ?
						Res.Strings.Enum.MCH2Summary.Column.Adjusts.Direct.Tooltip.ToString () :
						Res.Strings.Enum.MCH2Summary.Column.Adjusts.Indirect.Tooltip.ToString ();

				case Column.Outputs:
					return Res.Strings.Enum.MCH2Summary.Column.Outputs.Tooltip.ToString ();

				case Column.AmortizationsAuto:
					return Res.Strings.Enum.MCH2Summary.Column.AmortizationsAuto.Tooltip.ToString ();

				case Column.AmortizationsExtra:
					return Res.Strings.Enum.MCH2Summary.Column.AmortizationsExtra.Tooltip.ToString ();

				case Column.AmortizationsSuppl:
					return Res.Strings.Enum.MCH2Summary.Column.AmortizationsSuppl.Tooltip.ToString ();

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
				if (this.DirectMode)  // mode direct ?
				{
					yield return Column.Name;
					yield return Column.InitialState;
					yield return Column.PreInputs;
					yield return Column.Inputs;
//?					yield return Column.Reorganizations;  // l'événement de modification ne modifie jamais la valeur comptable
					yield return Column.Decreases;
					yield return Column.Increases;
					yield return Column.Adjusts;
					yield return Column.Outputs;
					yield return Column.AmortizationsAuto;
					yield return Column.AmortizationsExtra;
					yield return Column.AmortizationsSuppl;
					yield return Column.FinalState;
				}
				else  // mode indirect ?
				{
					yield return Column.Name;
					yield return Column.PreInputs;
					yield return Column.ReplacementValues;
					yield return Column.DeltaDecreasesIncreases;
					yield return Column.DeltaAdjusts;
					yield return Column.DeltaAmortizations;
					yield return Column.Outputs;
					yield return Column.AmortizationsAuto;
					yield return Column.AmortizationsExtra;
					yield return Column.AmortizationsSuppl;
					yield return Column.FinalState;
				}

				foreach (var userField in this.userColumns)
				{
					yield return userField.Column;
				}
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
			PreInputs,
			Inputs,
			ReplacementValues,
			DeltaAdjusts,
			DeltaDecreasesIncreases,
			DeltaAmortizations,
			Reorganizations,
			Decreases,
			Increases,
			Adjusts,
			Outputs,
			AmortizationsAuto,
			AmortizationsExtra,
			AmortizationsSuppl,
			FinalState,
			User,					// +n, pour toutes les colonnes de l'utilisateur
		}


		private ObjectsNodeGetter NodeGetter
		{
			get
			{
				return this.nodeGetter as ObjectsNodeGetter;
			}
		}


		private void InitializeUserColumns()
		{
			this.userColumns.Clear ();

			int i = 0;
			foreach (var userField in accessor.UserFieldsAccessor.GetUserFields (BaseType.AssetsUserFields)
				.Where   (x => x.MCH2SummaryOrder.HasValue)
				.OrderBy (x => x.MCH2SummaryOrder.Value))
			{
				if (userField.MCH2SummaryOrder.HasValue)
				{
					var userColumn = new UserColumn (userField.Field, Column.User + (i++), userField.Name, userField.Type);
					this.userColumns.Add(userColumn);
				}
			}
		}


		private class UserColumn
		{
			public UserColumn(ObjectField field, Column column, string name, FieldType type)
			{
				this.Field  = field;
				this.Column = column;
				this.Name   = name;
				this.Type   = type;
			}

			public readonly ObjectField			Field;
			public readonly Column				Column;
			public readonly string				Name;
			public readonly FieldType			Type;
		}


		private readonly List<UserColumn>		userColumns;
	}
}
