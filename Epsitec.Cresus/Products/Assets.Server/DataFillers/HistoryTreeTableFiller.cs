//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class HistoryTreeTableFiller : AbstractTreeTableFiller<GuidNode>
	{
		public HistoryTreeTableFiller(DataAccessor accessor, AbstractNodeGetter<GuidNode> nodeGetter, ObjectField field)
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
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Date,  HistoryTreeTableFiller.DateColumnWidth, "Date"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Glyph, HistoryTreeTableFiller.GlyphColumnWidth, ""));

				switch (this.accessor.GetFieldType (this.field))
				{
					case FieldType.Decimal:
						switch (Format.GetFieldFormat (this.field))
						{
							case DecimalFormat.Rate:
								list.Add (new TreeTableColumnDescription (TreeTableColumnType.Rate, this.ValueColumnWidth, "Valeur"));
								break;

							case DecimalFormat.Amount:
								list.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, this.ValueColumnWidth, "Valeur"));
								break;

							default:
								list.Add (new TreeTableColumnDescription (TreeTableColumnType.Decimal, this.ValueColumnWidth, "Valeur"));
								break;
						}
						break;

					case FieldType.Date:
						list.Add (new TreeTableColumnDescription (TreeTableColumnType.Date, this.ValueColumnWidth, "Valeur"));
						break;

					case FieldType.Int:
						list.Add (new TreeTableColumnDescription (TreeTableColumnType.Int, this.ValueColumnWidth, "Valeur"));
						break;

					case FieldType.ComputedAmount:
						list.Add (new TreeTableColumnDescription (TreeTableColumnType.DetailedComputedAmount, this.ValueColumnWidth, "Valeur"));
						break;

					case FieldType.GuidGroup:
					case FieldType.GuidPerson:
						list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, this.ValueColumnWidth, "Valeur"));
						break;

					case FieldType.GuidRatio:
						list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, this.ValueColumnWidth, "Valeur"));
						break;

					default:
						list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, this.ValueColumnWidth, "Valeur"));
						break;
				}

				return list.ToArray ();
			}
		}


		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var c1 = new TreeTableColumnItem ();
			var c2 = new TreeTableColumnItem ();

			int i = 0;
			foreach (var e in this.GetEvents (firstRow, count))
			{
				var date  = e.Timestamp.Date;
				var glyph = TimelineData.TypeToGlyph (e.Type);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;
				var s1 = new TreeTableCellDate  (date,  cellState);
				var s2 = new TreeTableCellGlyph (glyph, cellState);
				i++;

				c1.AddRow (s1);
				c2.AddRow (s2);
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (c1);
			content.Columns.Add (c2);
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

				case FieldType.GuidGroup:
					this.PutGuidGroup (content, firstRow, count, selection);
					break;

				case FieldType.GuidPerson:
					this.PutGuidPerson (content, firstRow, count, selection);
					break;

				case FieldType.GuidRatio:
					this.PutGuidRatio (content, firstRow, count, selection);
					break;
			}
		}

		private void PutString(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			var columnItem = new TreeTableColumnItem ();

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
				columnItem.AddRow (cell);
			}

			content.Columns.Add (columnItem);
		}

		private void PutDecimal(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			var columnItem = new TreeTableColumnItem ();

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
				columnItem.AddRow (cell);
			}

			content.Columns.Add (columnItem);
		}

		private void PutDate(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			var columnItem = new TreeTableColumnItem ();

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
				columnItem.AddRow (cell);
			}

			content.Columns.Add (columnItem);
		}

		private void PutInt(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			var columnItem = new TreeTableColumnItem ();

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
				columnItem.AddRow (cell);
			}

			content.Columns.Add (columnItem);
		}

		private void PutComputedAmount(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			var columnItem = new TreeTableColumnItem ();

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
				columnItem.AddRow (cell);
			}

			content.Columns.Add (columnItem);
		}

		private void PutGuidGroup(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			var columnItem = new TreeTableColumnItem ();

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
				columnItem.AddRow (cell);
			}

			content.Columns.Add (columnItem);
		}

		private void PutGuidPerson(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			var columnItem = new TreeTableColumnItem ();

			int i = 0;
			foreach (var e in this.GetEvents (firstRow, count))
			{
				var value = Guid.Empty;

				var property = e.GetProperty (this.field) as DataGuidProperty;
				if (property != null)
				{
					value = property.Value;
				}

				var text = PersonsLogic.GetFullName (this.accessor, value);
				var cellState = (i++ == selection) ? CellState.Selected : CellState.None;
				var cell = new TreeTableCellString (text, cellState);
				columnItem.AddRow (cell);
			}

			content.Columns.Add (columnItem);
		}

		private void PutGuidRatio(TreeTableContentItem content, int firstRow, int count, int selection)
		{
			var columnItem = new TreeTableColumnItem ();

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
				columnItem.AddRow (cell);
			}

			content.Columns.Add (columnItem);
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
					return 170;

				case FieldType.GuidGroup:
				case FieldType.GuidPerson:
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
