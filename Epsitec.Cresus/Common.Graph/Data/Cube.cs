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

		private sealed class DimensionValues : HashSet<string>
		{
		}

		readonly Dictionary<string, double> values;
		readonly Dictionary<string, DimensionValues> dimensions;
		readonly List<string> dimensionNames;
	}
}
