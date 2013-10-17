//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class HistoryAccessor
	{
		public HistoryAccessor(DataAccessor accessor, Guid objectGuid, Timestamp? timestamp, int field)
		{
			this.accessor = accessor;

			this.fieldType = this.accessor.GetFieldType ((ObjectField) field);

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
				return HistoryAccessor.DateColumnWidth + HistoryAccessor.ValueColumnWidth;
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

				if (this.fieldType == FieldType.Amount ||
					this.fieldType == FieldType.Int)
				{
					list.Add (new TreeTableColumnDescription (TreeTableColumnType.Decimal, HistoryAccessor.ValueColumnWidth, "Valeur"));
				}
				else if (this.fieldType == FieldType.Rate)
				{
					list.Add (new TreeTableColumnDescription (TreeTableColumnType.Rate, HistoryAccessor.ValueColumnWidth, "Valeur"));
				}
				else
				{
					list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, HistoryAccessor.ValueColumnWidth, "Valeur"));
				}

				return list.ToArray ();
			}
		}


		private void InitializeContent(Guid objectGuid, Timestamp? timestamp, int field)
		{
			this.selectedRow = -1;
			this.content.Clear ();
			this.timestamps.Clear ();

			bool put = false;

			int count = this.accessor.GetObjectEventsCount (objectGuid);
			for (int i=0; i<count; i++)
			{
				var eventTimestamp = this.accessor.GetObjectEventTimestamp (objectGuid, i);
				if (eventTimestamp.HasValue)
				{
					var properties = this.accessor.GetObjectSingleProperties (objectGuid, eventTimestamp.Value);

					var state = DataAccessor.GetPropertyState (properties, field);
					if (state != PropertyState.Undefined)
					{
						if (!put && timestamp != null && timestamp.Value < eventTimestamp.Value)
						{
							var c = this.GetCell (null, field);
							this.AddRow (timestamp, timestamp.Value, c);
							put = true;
						}

						if (!put && timestamp != null && timestamp.Value == eventTimestamp.Value)
						{
							put = true;
						}

						var cell = this.GetCell (properties, field);
						this.AddRow (timestamp, eventTimestamp.Value, cell);
					}
				}
			}

			if (!put && timestamp != null)
			{
				var c = this.GetCell (null, field);
				this.AddRow (timestamp, timestamp.Value, c);
			}
		}

		private AbstractSimpleTreeTableCell GetCell(IEnumerable<AbstractDataProperty> properties, int field)
		{
			if (this.fieldType == FieldType.Amount ||
				this.fieldType == FieldType.Int    ||
				this.fieldType == FieldType.Rate)
			{
				var value = DataAccessor.GetDecimalProperty (properties, field);
				return new SimpleTreeTableCellDecimal (value);
			}
			else
			{
				string s = DataAccessor.GetStringProperty (properties, field);
				return new SimpleTreeTableCellString (s);
			}
		}

		private void AddRow(Timestamp? selTimestamp, Timestamp addTimestamp, AbstractSimpleTreeTableCell addCell)
		{
			var row = new List<AbstractSimpleTreeTableCell> ();

			string d = Helpers.Converters.DateToString (addTimestamp.Date);
			row.Add (new SimpleTreeTableCellString (d));
			row.Add (addCell);

			this.content.Add (row);
			this.timestamps.Add (addTimestamp);

			if (selTimestamp != null &&
				selTimestamp.Value == addTimestamp)
			{
				this.selectedRow = this.content.Count-1;
			}
		}


		private static readonly int DateColumnWidth  = 80;
		private static readonly int ValueColumnWidth = 150;

		private readonly DataAccessor								accessor;
		private readonly FieldType									fieldType;
		private readonly List<List<AbstractSimpleTreeTableCell>>	content;
		private readonly List<Timestamp>							timestamps;

		private int													selectedRow;
	}
}
