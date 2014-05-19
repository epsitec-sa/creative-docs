//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class HistoryTreeTableFiller : AbstractTreeTableFiller<GuidNode>
	{
		public HistoryTreeTableFiller(DataAccessor accessor, INodeGetter<GuidNode> nodeGetter, ObjectField field)
			: base (accessor, nodeGetter)
		{
			this.field = field;
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.Unknown;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			//	Retourne les 3 colonnes d'un historique, à savoir:
			//	1) La date
			//	2) Le glyph
			//	3) La valeur d'un type variable
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Date,  HistoryTreeTableFiller.DateColumnWidth, "Date"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Glyph, HistoryTreeTableFiller.GlyphColumnWidth, ""));

				switch (this.accessor.GetFieldType (this.field))
				{
					case FieldType.Decimal:
						switch (Format.GetFieldFormat (this.field))
						{
							case DecimalFormat.Rate:
								columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Rate, this.ValueColumnWidth, "Valeur"));
								break;

							case DecimalFormat.Amount:
								columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, this.ValueColumnWidth, "Valeur"));
								break;

							default:
								columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Decimal, this.ValueColumnWidth, "Valeur"));
								break;
						}
						break;

					case FieldType.Date:
						columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Date, this.ValueColumnWidth, "Valeur"));
						break;

					case FieldType.Int:
						columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Int, this.ValueColumnWidth, "Valeur"));
						break;

					case FieldType.ComputedAmount:
						columns.Add (new TreeTableColumnDescription (TreeTableColumnType.DetailedComputedAmount, this.ValueColumnWidth, "Valeur"));
						break;

					case FieldType.AmortizedAmount:
						columns.Add (new TreeTableColumnDescription (TreeTableColumnType.DetailedAmortizedAmount, this.ValueColumnWidth, "Valeur"));
						break;

					case FieldType.GuidGroup:
					case FieldType.GuidPerson:
					case FieldType.GuidAccount:
						columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, this.ValueColumnWidth, "Valeur"));
						break;

					case FieldType.GuidRatio:
						columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, this.ValueColumnWidth, "Valeur"));
						break;

					default:
						columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, this.ValueColumnWidth, "Valeur"));
						break;
				}

				return columns.ToArray ();
			}
		}


		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			for (int i=0; i<3; i++)
			{
				content.Columns.Add (new TreeTableColumnItem ());
			}

			int row = 0;
			foreach (var e in this.GetEvents (firstRow, count))
			{
				var date  = e.Timestamp.Date;
				var glyph = TimelineData.TypeToGlyph (e.Type);

				var cellState = (row == selection) ? CellState.Selected : CellState.None;

				var cell1 = new TreeTableCellDate  (date,  cellState);
				var cell2 = new TreeTableCellGlyph (glyph, cellState);
				row++;

				content.Columns[0].AddRow (cell1);
				content.Columns[1].AddRow (cell2);
			}

			this.PutValue(content, firstRow, count, selection);

			return content;
		}

		private void PutValue(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			//	Peuple la 3ème colonne dont le type varie.
			//	Je n'ai pas trouvé comment faire plus simple...
			switch (this.accessor.GetFieldType (this.field))
			{
				case FieldType.String:
					this.PutString (content, firstRow, count, selection);
					break;

				case FieldType.Decimal:
					this.PutDecimal (content, firstRow, count, selection);
					break;

				case FieldType.Date:
					this.PutDate (content, firstRow, count, selection);
					break;

				case FieldType.Int:
					this.PutInt (content, firstRow, count, selection);
					break;

				case FieldType.ComputedAmount:
					this.PutComputedAmount (content, firstRow, count, selection);
					break;

				case FieldType.AmortizedAmount:
					this.PutAmortizedAmount (content, firstRow, count, selection);
					break;

				case FieldType.GuidGroup:
					this.PutGuidGroup (content, firstRow, count, selection);
					break;

				case FieldType.GuidPerson:
					this.PutGuidPerson (content, firstRow, count, selection);
					break;

				case FieldType.GuidAccount:
					this.PutGuidAccount (content, firstRow, count, selection);
					break;

				case FieldType.GuidRatio:
					this.PutGuidRatio (content, firstRow, count, selection);
					break;
			}
		}

		private void PutString(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			int i = 0;
			foreach (var e in this.GetEvents (firstRow, count))
			{
				string value = null;

				var property = e.GetProperty (this.field) as DataStringProperty;
				if (property != null)
				{
					value = property.Value;
				}

				var cellState = (i++ == selection) ? CellState.Selected : CellState.None;
				var cell = new TreeTableCellString (value, cellState);

				content.Columns[2].AddRow (cell);
			}
		}

		private void PutDecimal(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			int i = 0;
			foreach (var e in this.GetEvents (firstRow, count))
			{
				decimal? value = null;

				var property = e.GetProperty (this.field) as DataDecimalProperty;
				if (property != null)
				{
					value = property.Value;
				}

				var cellState = (i++ == selection) ? CellState.Selected : CellState.None;
				var cell = new TreeTableCellDecimal (value, cellState);

				content.Columns[2].AddRow (cell);
			}
		}

		private void PutDate(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			int i = 0;
			foreach (var e in this.GetEvents (firstRow, count))
			{
				System.DateTime? value = null;

				var property = e.GetProperty (this.field) as DataDateProperty;
				if (property != null)
				{
					value = property.Value;
				}

				var cellState = (i++ == selection) ? CellState.Selected : CellState.None;
				var cell = new TreeTableCellDate (value, cellState);

				content.Columns[2].AddRow (cell);
			}
		}

		private void PutInt(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			int i = 0;
			foreach (var e in this.GetEvents (firstRow, count))
			{
				int? value = null;

				var property = e.GetProperty (this.field) as DataIntProperty;
				if (property != null)
				{
					value = property.Value;
				}

				var cellState = (i++ == selection) ? CellState.Selected : CellState.None;
				var cell = new TreeTableCellInt (value, cellState);

				content.Columns[2].AddRow (cell);
			}
		}

		private void PutComputedAmount(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			int i = 0;
			foreach (var e in this.GetEvents (firstRow, count))
			{
				ComputedAmount? value = null;

				var property = e.GetProperty (this.field) as DataComputedAmountProperty;
				if (property != null)
				{
					value = property.Value;
				}

				var cellState = (i++ == selection) ? CellState.Selected : CellState.None;
				var cell = new TreeTableCellComputedAmount (value, cellState);

				content.Columns[2].AddRow (cell);
			}
		}

		private void PutAmortizedAmount(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			int i = 0;
			foreach (var e in this.GetEvents (firstRow, count))
			{
				AmortizedAmount? value = null;

				var property = e.GetProperty (this.field) as DataAmortizedAmountProperty;
				if (property != null)
				{
					value = property.Value;
				}

				var cellState = (i++ == selection) ? CellState.Selected : CellState.None;
				var cell = new TreeTableCellAmortizedAmount (value, cellState);

				content.Columns[2].AddRow (cell);
			}
		}

		private void PutGuidGroup(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			int i = 0;
			foreach (var e in this.GetEvents (firstRow, count))
			{
				var value = Guid.Empty;

				var property = e.GetProperty (this.field) as DataGuidProperty;
				if (property != null)
				{
					value = property.Value;
				}

				var text = GroupsLogic.GetFullName (this.accessor, value);
				var cellState = (i++ == selection) ? CellState.Selected : CellState.None;
				var cell = new TreeTableCellString (text, cellState);

				content.Columns[2].AddRow (cell);
			}
		}

		private void PutGuidPerson(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			int i = 0;
			foreach (var e in this.GetEvents (firstRow, count))
			{
				var value = Guid.Empty;

				var property = e.GetProperty (this.field) as DataGuidProperty;
				if (property != null)
				{
					value = property.Value;
				}

				var text = PersonsLogic.GetSummary (this.accessor, value);
				var cellState = (i++ == selection) ? CellState.Selected : CellState.None;
				var cell = new TreeTableCellString (text, cellState);

				content.Columns[2].AddRow (cell);
			}
		}

		private void PutGuidAccount(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			int i = 0;
			foreach (var e in this.GetEvents (firstRow, count))
			{
				var value = Guid.Empty;

				var property = e.GetProperty (this.field) as DataGuidProperty;
				if (property != null)
				{
					value = property.Value;
				}

				var text = AccountsLogic.GetSummary (this.accessor, value);
				var cellState = (i++ == selection) ? CellState.Selected : CellState.None;
				var cell = new TreeTableCellString (text, cellState);

				content.Columns[2].AddRow (cell);
			}
		}

		private void PutGuidRatio(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			int i = 0;
			foreach (var e in this.GetEvents (firstRow, count))
			{
				var value = GuidRatio.Empty;

				var property = e.GetProperty (this.field) as DataGuidRatioProperty;
				if (property != null)
				{
					value = property.Value;
				}

				var text = GroupsLogic.GetFullName (this.accessor, value);
				var cellState = (i++ == selection) ? CellState.Selected : CellState.None;
				var cell = new TreeTableCellString (text, cellState);

				content.Columns[2].AddRow (cell);
			}
		}

		private IEnumerable<DataEvent> GetEvents(int firstRow, int count)
		{
			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeGetter.Count)
				{
					break;
				}

				var node = this.nodeGetter[firstRow+i];
				var e = this.DataObject.GetEvent (node.Guid);
				yield return e;
			}
		}


		public int ValueColumnWidth
		{
			get
			{
				return this.GetValueColumnWidth (this.field);
			}
		}

		public int GetValueColumnWidth(ObjectField field)
		{
			switch (this.accessor.GetFieldType (field))
			{
				case FieldType.Date:
					return 70;

				case FieldType.Int:
					return 50;

				case FieldType.Decimal:
					return 70;

				case FieldType.ComputedAmount:
				case FieldType.AmortizedAmount:
					return 170;

				case FieldType.GuidGroup:
				case FieldType.GuidPerson:
				case FieldType.GuidAccount:
					return 300;

				case FieldType.GuidRatio:
					return 350;

				case FieldType.String:
					return 150;

				default:
					return 150;

			}
		}


		public static readonly int DateColumnWidth  = 80;
		public static readonly int GlyphColumnWidth = 20;

		private readonly ObjectField	field;
	}
}
