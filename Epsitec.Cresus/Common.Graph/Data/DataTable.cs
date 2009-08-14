//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Data
{
	public class DataTable
	{
		public DataTable()
		{
			this.columnLabels = new List<string> ();
			this.rowLabels = new List<string> ();
			this.rows = new List<double?[]> ();
			this.dimensionVector = new DimensionVector ();
		}


		public int ColumnCount
		{
			get
			{
				return this.columnLabels.Count;
			}
		}

		public int RowCount
		{
			get
			{
				return this.rowLabels.Count;
			}
		}

		public string ColumnDimensionKey
		{
			get
			{
				return this.columnDimensionKey ?? "";
			}
			set
			{
				this.columnDimensionKey = value;
			}
		}

		public string RowDimensionKey
		{
			get
			{
				return this.rowDimensionKey ?? "";
			}
			set
			{
				this.rowDimensionKey = value;
			}
		}

		public DimensionVector DimensionVector
		{
			get
			{
				return this.dimensionVector;
			}
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

		public IEnumerable<ChartSeries> RowSeries
		{
			get
			{
				for (int i = 0; i < this.rows.Count; i++)
				{
					yield return this.GetRowSeries (i);
				}
			}
		}

		public double? this[string row, string col]
		{
			get
			{
				int rowIndex = this.FindRowIndex (row);
				int colIndex = this.FindColumnIndex (col);

				if ((rowIndex < 0) ||
					(colIndex < 0))
				{
					throw new System.ArgumentOutOfRangeException ();
				}

				if ((rowIndex < this.rows.Count) &&
					(colIndex < this.rows[rowIndex].Length))
				{
					return this.rows[rowIndex][colIndex];
				}
				else
				{
					return null;
				}
			}
			set
			{
				int rowIndex = this.FindRowIndex (row);
				int colIndex = this.FindColumnIndex (col);

				if ((rowIndex < 0) ||
					(colIndex < 0))
				{
					throw new System.ArgumentOutOfRangeException ();
				}
				
				while (this.rows.Count <= rowIndex)
				{
					this.rows.Add (new double?[this.columnLabels.Count]);
				}

				if (colIndex >= this.rows[rowIndex].Length)
				{
					double?[] copy = new double?[this.columnLabels.Count];
					this.rows[rowIndex].CopyTo (copy, 0);
					this.rows[rowIndex] = copy;
				}
				
				this.rows[rowIndex][colIndex] = value;
			}
		}

		
		public void DefineColumnLabels(IEnumerable<string> labels)
		{
			this.columnLabels.Clear ();
			this.columnLabels.AddRange (labels);
		}

		public void DefineRowLabels(IEnumerable<string> labels)
		{
			this.rowLabels.Clear ();
			this.rowLabels.AddRange (labels);
		}

		public int FindRowIndex(string label)
		{
			return this.rowLabels.IndexOf (label);
		}

		public int FindColumnIndex(string label)
		{
			return this.columnLabels.IndexOf (label);
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
		private readonly DimensionVector dimensionVector;
		private string columnDimensionKey;
		private string rowDimensionKey;
	}
}
