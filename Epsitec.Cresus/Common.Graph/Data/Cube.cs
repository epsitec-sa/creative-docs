//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Data
{
	public class Cube
	{
		public Cube()
		{
			this.values = new Dictionary<string, double> ();
			this.dimensions = new Dictionary<string, DimensionValues> ();
			this.dimensionNames = new List<string> ();
		}


		public IList<string> DimensionNames
		{
			get
			{
				return this.dimensionNames.AsReadOnly ();
			}
		}


		public void AddTable(Table table)
		{
			string colKey = table.ColumnDimensionKey;
			string rowKey = table.RowDimensionKey;

			if (string.IsNullOrEmpty (colKey))
			{
				throw new System.ArgumentException ("Table has no ColumnDimensionKey");
			}
			if (string.IsNullOrEmpty (rowKey))
			{
				throw new System.ArgumentException ("Table has no RowDimensionKey");
			}

			foreach (var row in table.RowSeries)
			{
				string rowLabel = row.Label;

				foreach (var cell in row.Values)
				{
					DimensionVector vector = new DimensionVector (table.DimensionVector);

					vector.Add (new Dimension (rowKey, rowLabel));
					vector.Add (new Dimension (colKey, cell.Label));

					this.values.Add (vector.Compile (), cell.Value);

					foreach (Dimension dimension in vector)
					{
						DimensionValues values;

						if (this.dimensions.TryGetValue (dimension.Key, out values))
						{
							values.Add (dimension.Value);
						}
						else
						{
							values = new DimensionValues ();
							values.Add (dimension.Value);
							
							this.dimensions.Add (dimension.Key, values);
							this.dimensionNames.Add (dimension.Key);
						}
					}
				}
			}

			this.dimensionNames.Sort ((a, b) => string.CompareOrdinal (a, b));
		}


		public IList<string> GetDimensionValues(string key)
		{
			DimensionValues values;

			if (this.dimensions.TryGetValue (key, out values))
			{
				List<string> list = values.ToList ();
				list.Sort ();
				return list.AsReadOnly ();
			}
			else
			{
				return null;
			}
		}

		public ChartSeries ExtractSeries(params string[] dimensions)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ("^");
			
			List<string> dims = new List<string> (dimensions);
			List<string> axes = new List<string> ();

			foreach (string name in this.dimensionNames)
			{
				string prefix = name + "=";

				int pos = dims.FindIndex (x => x == name);

				if (pos >= 0)
				{
					//	Found an exact match, which means that we have found the dimension
					//	which we will enumerate to produce our sequence.

					if (buffer.Length > 1)
					{
						buffer.Append (':');
					}

					buffer.Append (prefix);
					buffer.Append ("([^:]*)");
					
					dims.RemoveAt (pos);
					axes.Add (name);
					
					continue;
				}

				pos = dims.FindIndex (x => x.StartsWith (prefix));

				if (pos >= 0)
				{
					//	Found an extraction match; just use it as is.

					if (buffer.Length > 1)
					{
						buffer.Append (':');
					}

					buffer.Append (dims[pos]);
					dims.RemoveAt (pos);
					
					continue;
				}

				if (buffer.Length > 1)
				{
					buffer.Append (':');
				}

				buffer.Append (prefix);
				buffer.Append ("[^:]*");
			}

			buffer.Append ('$');

			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex (buffer.ToString (), System.Text.RegularExpressions.RegexOptions.Compiled);
			
			Accumulator accumulator = new Accumulator ();

			foreach (var item in this.values)
			{
				var match = regex.Match (item.Key);

				if (match.Success)
				{
					int num = match.Groups.Count;
					
					if (num > 1)
					{
						string key = "";
						for (int i = 1; i < num; i++)
						{
							if (key.Length > 0)
							{
								key = key + "+";
							}
							key = key + match.Groups[i].Value;
						}
						accumulator.Accumulate (key, item.Value);
					}
				}
			}

			return new ChartSeries (accumulator.Values);
		}

		#region DimensionValues Class

		private sealed class DimensionValues : HashSet<string>
		{
		}

		#endregion


		readonly Dictionary<string, double> values;
		readonly Dictionary<string, DimensionValues> dimensions;
		readonly List<string> dimensionNames;
	}
}
