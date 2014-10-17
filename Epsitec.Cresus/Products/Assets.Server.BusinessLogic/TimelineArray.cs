//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public class TimelineArray
	{
		public TimelineArray()
		{
			this.columns = new List<TimelineColumn> ();
			this.rowsLabel = new List<string> ();
		}

		public List<string> RowsLabel
		{
			get
			{
				return this.rowsLabel;
			}
		}

		public int RowsCount
		{
			get
			{
				return this.rowsCount;
			}
		}

		public int ColumnsCount
		{
			get
			{
				return this.columns.Count;
			}
		}

		public TimelineCell GetCell(int row, int column)
		{
			if (column < this.ColumnsCount && row < this.RowsCount)
			{
				return this.columns[column][row];
			}
			else
			{
				return TimelineCell.Empty;
			}
		}

		public void Clear(int rowsCount, TimelinesMode mode, DateRange range)
		{
			this.rowsCount = rowsCount;
			this.mode      = mode;
			this.range     = range;

			this.columns.Clear ();
		}

		public int FindColumnIndex(Timestamp timestamp)
		{
			return this.columns.FindIndex (x => x.Timestamp == timestamp);
		}

		public IEnumerable<TimelineColumn> Columns
		{
			get
			{
				return this.columns;
			}
		}

		public TimelineColumn GetColumn(int column)
		{
			if (column >= 0 && column < this.columns.Count)
			{
				return this.columns[column];
			}
			else
			{
				return null;
			}
		}

		public TimelineColumn GetColumn(Timestamp timestamp, bool grouped)
		{
			//	Retourne la colonne à utiliser pour un Timestamp donné.
			//	Si elle n'existe pas, elle est créée.
			var adjusted = this.Adjust (timestamp);
			var column = this.columns.Where (x => this.Adjust (x.Timestamp) == adjusted).FirstOrDefault ();

			if (column == null)
			{
				column = new TimelineColumn (this.rowsCount, timestamp, grouped);

				//	Les colonnes sont triées chronologiquement. Il faut donc insérer
				//	la nouvelle colonne à la bonne place.
				int i = this.columns.Where (x => x.Timestamp < timestamp).Count ();
				this.columns.Insert (i, column);
			}

			return column;
		}

		private Timestamp Adjust(Timestamp timestamp)
		{
			if (this.mode == TimelinesMode.GroupedByMonth && !range.IsInside (timestamp.Date))
			{
				var date = new System.DateTime(timestamp.Date.Year, timestamp.Date.Month, 1);
				timestamp = new Timestamp (date, 0);
			}

			return timestamp;
		}

		private readonly List<TimelineColumn>	columns;
		private readonly List<string>		rowsLabel;
		private int							rowsCount;
		private TimelinesMode				mode;
		private DateRange					range;
	}
}