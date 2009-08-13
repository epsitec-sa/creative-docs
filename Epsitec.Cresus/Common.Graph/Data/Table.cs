//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Data
{
	public class Table
	{
		public Table()
		{
			this.columnLabels = new List<string> ();
			this.rowLabels = new List<string> ();
			this.rows = new List<double?[]> ();
		}

		public IList<string> ColumnLabels
		{
			get
			{
				return this.columnLabels;
			}
		}

		public IList<string> RowLabels
		{
			get
			{
				return this.rowLabels;
			}
		}

		public void Add(string label, IEnumerable<double?> row)
		{
			this.rowLabels.Add (label);
			this.rows.Add (row.ToArray ());
		}

		
		public ChartSeries GetRowSeries(int index)
		{
			ChartSeries series = new ChartSeries ()
			{
				Label = this.rowLabels[index]
			};

			double?[] values = this.rows[index];
			
			int count = this.columnLabels.Count;

			System.Diagnostics.Debug.Assert (values.Length == count);
			
			for (int i = 0; i < count; i++)
			{
				double? value = values[i];
				string  label = this.columnLabels[i];

				if (value.HasValue)
				{
					series.Values.Add (new ChartValue (label, value.Value));
				}
			}

			return series;
		}

		public ChartSeries GetColumnSeries(int index)
		{
			ChartSeries series = new ChartSeries ()
			{
				Label = this.columnLabels[index]
			};

			int count = this.rowLabels.Count;

			for (int i = 0; i < count; i++)
			{
				double? value = this.rows[i][index];
				string  label = this.rowLabels[i];

				if (value.HasValue)
				{
					series.Values.Add (new ChartValue (label, value.Value));
				}
			}

			return series;
		}


		private readonly List<string> columnLabels;
		private readonly List<string> rowLabels;

		private readonly List<double?[]> rows;
	}
}
