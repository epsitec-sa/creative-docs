//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class HistoryAccessor
	{
		public HistoryAccessor(DataAccessor accessor, BaseType baseType, Guid objectGuid, Timestamp? timestamp, ObjectField field)
		{
			this.accessor = accessor;
			this.baseType = baseType;

			this.objectField = field;
			this.fieldType = DataAccessor.GetFieldType (field);

			this.content    = new List<List<AbstractSimpleTreeTableCell>> ();
			this.timestamps = new List<Timestamp> ();

			this.InitializeContent (objectGuid, timestamp, field);
		}


		public int								RowsCount
		{
			get
			{
				return this.content.Count ();
			}
		}

		public int								SelectedRow
		{
			get
			{
				return this.selectedRow;
			}
		}

		public List<List<AbstractSimpleTreeTableCell>> Content
		{
			get
			{
				return this.content;
			}
		}

		public int								ColumnsWidth
		{
			get
			{
				return HistoryAccessor.DateColumnWidth
					 + HistoryAccessor.GlyphColumnWidth
					 + this.ValueColumnWidth;
			}
		}


		public Timestamp? GetTimestamp(int row)
		{
			if (row >= 0 && row < this.timestamps.Count)
			{
				return this.timestamps[row];
			}
			else
			{
				return null;
			}
		}


		public TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, HistoryAccessor.DateColumnWidth, "Date"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Glyph,  HistoryAccessor.GlyphColumnWidth, ""));

				switch (this.fieldType)
				{
					case FieldType.Decimal:
						switch (Format.GetFieldFormat (this.objectField))
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

					case FieldType.Guid:
						list.Add (new TreeTableColumnDescription (TreeTableColumnType.Guid, this.ValueColumnWidth, "Valeur"));
						break;

					default:
						list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, this.ValueColumnWidth, "Valeur"));
						break;
				}

				return list.ToArray ();
			}
		}


		private void InitializeContent(Guid objectGuid, Timestamp? timestamp, ObjectField field)
		{
			this.selectedRow = -1;
			this.content.Clear ();
			this.timestamps.Clear ();

			bool put = false;

			var obj = this.accessor.GetObject (this.baseType, objectGuid);
			if (obj != null)
			{
				int count = obj.EventsCount;
				for (int i=0; i<count; i++)
				{
					var e = obj.GetEvent (i);
					var eventTimestamp = e.Timestamp;

					var p = ObjectCalculator.GetObjectSingleProperty (obj, eventTimestamp, field);

					if (p != null)
					{
						if (!put && timestamp != null && timestamp.Value < eventTimestamp)
						{
							var c = this.GetCell (null, timestamp.Value, field);
							this.AddRow (obj, timestamp, timestamp.Value, c);
							put = true;
						}

						if (!put && timestamp != null && timestamp.Value == eventTimestamp)
						{
							put = true;
						}

						var cell = this.GetCell (obj, eventTimestamp, field);
						this.AddRow (obj, timestamp, eventTimestamp, cell);
					}
				}

				if (!put && timestamp != null)
				{
					var c = this.GetCell (null, timestamp.Value, field);
					this.AddRow (obj, timestamp, timestamp.Value, c);
				}
			}
		}

		private AbstractSimpleTreeTableCell GetCell(DataObject obj, Timestamp timestamp, ObjectField field)
		{
			switch (this.fieldType)
			{
				case FieldType.Decimal:
					{
						var value = ObjectCalculator.GetObjectPropertyDecimal (obj, timestamp, field);
						return new SimpleTreeTableCellDecimal (value, Format.GetFieldFormat (field));
					}

				case FieldType.Int:
					{
						var value = ObjectCalculator.GetObjectPropertyInt (obj, timestamp, field);
						return new SimpleTreeTableCellInt (value);
					}

				case FieldType.ComputedAmount:
					{
						var value = ObjectCalculator.GetObjectPropertyComputedAmount (obj, timestamp, field);
						return new SimpleTreeTableCellComputedAmount (value);
					}

				case FieldType.Date:
					{
						var value = ObjectCalculator.GetObjectPropertyDate (obj, timestamp, field);
						return new SimpleTreeTableCellDate (value);
					}

				case FieldType.Guid:
					{
						var value = ObjectCalculator.GetObjectPropertyGuid (obj, timestamp, field);
						return new SimpleTreeTableCellGuid (value);
					}

				default:
					{
						string value = ObjectCalculator.GetObjectPropertyString (obj, timestamp, field);
						return new SimpleTreeTableCellString (value);
					}
			}
		}

		private void AddRow(DataObject obj, Timestamp? selTimestamp, Timestamp addTimestamp, AbstractSimpleTreeTableCell addCell)
		{
			var eventType = EventType.Unknown;
			var e = obj.GetEvent (addTimestamp);
			if (e != null)
			{
				eventType = e.Type;
			}

			var row = new List<AbstractSimpleTreeTableCell> ();

			string d = Helpers.Converters.DateToString (addTimestamp.Date);
			row.Add (new SimpleTreeTableCellString (d));
			row.Add (new SimpleTreeTableCellGlyph (TimelineData.TypeToGlyph (eventType)));
			row.Add (addCell);

			this.content.Add (row);
			this.timestamps.Add (addTimestamp);

			if (selTimestamp != null &&
				selTimestamp.Value == addTimestamp)
			{
				this.selectedRow = this.content.Count-1;
			}
		}


		private int ValueColumnWidth
		{
			get
			{
				switch (this.fieldType)
				{
					case FieldType.Date:
						return 70;

					case FieldType.Int:
						return 50;

					case FieldType.Decimal:
						return 70;

					case FieldType.ComputedAmount:
						return 170;

					case FieldType.String:
						return 150;

					default:
						return 150;

				}
			}
		}


		private static readonly int DateColumnWidth  = 80;
		private static readonly int GlyphColumnWidth = 20;

		private readonly DataAccessor								accessor;
		private readonly BaseType									baseType;
		private readonly ObjectField								objectField;
		private readonly FieldType									fieldType;
		private readonly List<List<AbstractSimpleTreeTableCell>>	content;
		private readonly List<Timestamp>							timestamps;

		private int													selectedRow;
	}
}
