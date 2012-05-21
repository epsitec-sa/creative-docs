//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Data
{
	public class DataTable : System.IEquatable<DataTable>
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
			this.Insert (this.rows.Count, label, row);
		}

		public void Add(string label, IEnumerable<ChartValue> collection)
		{
			this.Insert (this.rows.Count, label, collection);
		}



		public ChartSeries SumRows(IEnumerable<int> rows, System.Func<int, ChartSeries> map)
		{
			Accumulator accumulator = new Accumulator ();
			List<string> labels = new List<string> ();

			foreach (int row in rows)
			{
				var series = (map == null) ? this.GetRowSeries (row) : map (row);

				accumulator.Accumulate (series.Values);
				labels.Add (series.Label);
			}

			if (labels.Count > 0)
			{
				var result = new ChartSeries (accumulator.Values)
				{
					Label = string.Join (DataCube.LabelSeparator.ToString (), labels.ToArray ())
				};

				return result;
			}
			else
			{
				return null;
			}
		}

		public void Insert(int index, string label, IEnumerable<double?> row)
		{
			double?[] rowValues = row.ToArray ();

			if (rowValues.Length != this.columnLabels.Count)
			{
				throw new System.ArgumentException ("Size mismatch", "row");
			}

			this.rowLabels.Insert (index, label);
			this.rows.Insert (index, rowValues);
		}

		public void Insert(int index, string label, IEnumerable<ChartValue> collection)
		{
			double?[] rowValues = new double?[this.columnLabels.Count];

			this.rowLabels.Insert (index, label);
			this.rows.Insert (index, rowValues);

			foreach (var item in collection)
			{
				this[label, item.Label] = item.Value;
			}
		}

		public void RemoveRows(IEnumerable<int> rows)
		{
			int count = 0;
			int last  = -1;

			foreach (int row in rows)
			{
				if (row <= last)
				{
					throw new System.ArgumentException ("Invalid rows : must be sorted in ascending order");
				}
				last = row;
			}

			foreach (int row in rows)
			{
				int index = row - count;
				count += 1;
				
				this.rowLabels.RemoveAt (index);
				this.rows.RemoveAt (index);
			}
		}


		public IEnumerable<ChartSeries> GetRowSeries()
		{
			for (int i = 0; i < this.RowCount; i++)
			{
				yield return this.GetRowSeries (i);
			}
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

		#region IEquatable<DataTable> Members

		public bool Equals(DataTable other)
		{
			if ((this.ColumnCount != other.ColumnCount) ||
				(this.RowCount != other.RowCount) ||
				(this.ColumnDimensionKey != other.ColumnDimensionKey) ||
				(this.RowDimensionKey != other.RowDimensionKey))
			{
				return false;
			}

			//	TODO: verify label & dimension equality

			for (int i = 0; i < this.rows.Count; i++)
			{
				if (DataTable.EqualRows (this.rows[i], other.rows[i]) == false)
				{
					return false;
				}
			}

			return true;
		}

		private static bool EqualRows(double?[] a, double?[] b)
		{
			if (a.Length != b.Length)
			{
				return false;
			}

			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[i])
				{
					if ((a[i].HasValue) &&
						(b[i].HasValue) &&
						(double.IsNaN (a[i].Value)) &&
						(double.IsNaN (b[i].Value)))
					{
						continue;
					}

					return false;
				}
			}

			return true;
		}

		#endregion


		public static DataTable LoadFromData(System.Data.DataTable table, System.Converter<List<string>, IEnumerable<string>> columnMapper, System.Func<System.Data.DataRow, KeyValuePair<string, IEnumerable<double?>>> converter)
		{
			DataTable output = new DataTable ();

			List<string> columns = new List<string> ();

			foreach (System.Data.DataColumn column in table.Columns)
			{
				columns.Add (column.ColumnName);
			}

			output.DefineColumnLabels (columnMapper (columns));

			foreach (System.Data.DataRow row in table.Rows)
			{
				var data = converter (row);

				if ((data.Key != null) &&
					(data.Value != null))
				{
					output.Add (data.Key, data.Value);
				}
			}

			return output;
		}

		
		private readonly List<string> columnLabels;
		private readonly List<string> rowLabels;

		private readonly List<double?[]> rows;
		private readonly DimensionVector dimensionVector;
		private string columnDimensionKey;
		private string rowDimensionKey;
	}
}
