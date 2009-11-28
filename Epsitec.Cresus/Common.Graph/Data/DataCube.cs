//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;

namespace Epsitec.Common.Graph.Data
{
	public class DataCube
	{
		public DataCube()
		{
			this.values = new Dictionary<string, double> ();
			this.dimensions = new Dictionary<string, DimensionValues> ();
			this.dimensionNames = new List<string> ();
			this.naturalTableDimensionNames = new List<string> ();
		}


		public string Information
		{
			get;
			set;
		}


		public IList<string> DimensionNames
		{
			get
			{
				return this.dimensionNames.AsReadOnly ();
			}
		}

		public IList<string> NaturalTableDimensionNames
		{
			get
			{
				return this.naturalTableDimensionNames.AsReadOnly ();
			}
		}



		public bool AddTable(DataTable table)
		{
			string colKey = table.ColumnDimensionKey;
			string rowKey = table.RowDimensionKey;

			if (this.dimensionNames.Count > 0)
			{
				var testVector = new DimensionVector ();

				testVector.Add (table.DimensionVector);
				testVector.Add (colKey, "?");
				testVector.Add (rowKey, "?");

				var v1 = string.Join (":", testVector.Keys.ToArray ());
				var v2 = string.Join (":", this.dimensionNames.ToArray ());

				if (v1 != v2)
				{
					return false;
				}
			}

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
					this.AddDimensionVector(vector);
				}
			}

			this.dimensionNames.Sort ((a, b) => string.CompareOrdinal (a, b));
			this.DefineNaturalTableDimensionNames (table.RowDimensionKey, table.ColumnDimensionKey);

			return true;
		}

		public bool AddCube(DataCube cube)
		{
			if (cube == null)
			{
				return false;
			}

			if (this.dimensionNames.Count > 0)
			{
				var v1 = string.Join (":", cube.dimensionNames.ToArray ());
				var v2 = string.Join (":", this.dimensionNames.ToArray ());

				if (v1 != v2)
				{
					return false;
				}
			}
			
			foreach (var value in cube.values)
			{
				var vector = new DimensionVector (value.Key);

				this.values[vector.Compile ()] = value.Value;
				this.AddDimensionVector (vector);
			}
			
			this.dimensionNames.Sort ((a, b) => string.CompareOrdinal (a, b));

			if (this.naturalTableDimensionNames.Count == 0)
			{
				this.naturalTableDimensionNames.AddRange (cube.NaturalTableDimensionNames);
			}

			return true;
		}

		public void DefineNaturalTableDimensionNames(string rows, string columns)
		{
			this.naturalTableDimensionNames.Clear ();
			
			this.naturalTableDimensionNames.Add (rows);
			this.naturalTableDimensionNames.Add (columns);
		}

		public void Clear()
		{
			this.values.Clear ();
			this.dimensions.Clear ();
			this.dimensionNames.Clear ();
			this.naturalTableDimensionNames.Clear ();
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

		public ChartSeries ExtractChartSeries(params string[] dimensions)
		{
			return new ChartSeries (this.Accumulate (dimensions).Values);
		}

		public DataTable ExtractNaturalDataTable()
		{
			return this.ExtractDataTable (this.naturalTableDimensionNames[0], this.naturalTableDimensionNames[1]);
		}

		public DataTable ExtractDataTable(params string[] dimensions)
		{
			List<string> axes;

			var accumulator = this.Accumulate (dimensions, out axes);

			if (axes.Count != 2)
			{
				throw new System.ArgumentException ("Invalid number of axes for table extraction");
			}

			DataTable table = new DataTable ();

			table.RowDimensionKey    = axes[0];
			table.ColumnDimensionKey = axes[1];

			HashSet<string> rowLabels = new HashSet<string> ();
			HashSet<string> colLabels = new HashSet<string> ();

			foreach (var item in accumulator.Values)
			{
				string[] keys = item.Label.Split (DataCube.LabelSeparator);

				rowLabels.Add (keys[0]);
				colLabels.Add (keys[1]);
			}

			table.DefineRowLabels (rowLabels.OrderBy (x => x));
			table.DefineColumnLabels (colLabels.OrderBy (x => x));

			foreach (var item in accumulator.Values)
			{
				string[] keys = item.Label.Split (DataCube.LabelSeparator);

				table[keys[0], keys[1]] = item.Value;
			}

			return table;
		}


		public void Save(System.IO.TextWriter stream)
		{
			stream.WriteLine ("Epsitec.Common.Graph.Data.DataCube");
			stream.WriteLine ("DataFormat:V1");

			if (this.Information != null)
			{
				stream.WriteLine ("Section:Information");
				stream.WriteLine (DataCube.Escape (this.Information));
			}

			foreach (var annotation in this.GetAnnotations ())
			{
				stream.WriteLine ("Section:Annotation");
				stream.WriteLine (DataCube.Escape (annotation));
			}

			stream.WriteLine ("Section:DimensionNames");
			this.dimensionNames.ForEach (name => stream.WriteLine (DataCube.Escape (name)));
			
			stream.WriteLine ("Section:NaturalTableDimensionNames");
			this.naturalTableDimensionNames.ForEach (name => stream.WriteLine (DataCube.Escape (name)));
			
			stream.WriteLine ("Section:DimensionValues");
			this.dimensions.ForEach (
				pair =>
				{
					stream.Write (DataCube.Escape (pair.Key));
					stream.Write ("=");
					stream.WriteLine (pair.Value.ToString ());
				});

			stream.WriteLine ("Section:Values");
			this.values.ForEach (
				pair =>
				{
					stream.Write (DataCube.Escape (pair.Key));
					stream.Write ("=");
					stream.WriteLine (pair.Value.ToString (System.Globalization.CultureInfo.InvariantCulture));
				});

			stream.WriteLine ("Section:End");
		}

		public void Restore(System.IO.TextReader stream)
		{
			var line1 = stream.ReadLine ();
			var line2 = stream.ReadLine ();

			if ((line1 != "Epsitec.Common.Graph.Data.DataCube") ||
				(line2 != "DataFormat:V1"))
			{
				throw new System.FormatException ();
			}

			System.Action<string> processor = null;

			while (true)
			{
				var line = stream.ReadLine ();

				if (line.StartsWith ("Section:"))
				{
					string token = line.Substring (8);

					switch (token)
					{
						case "Information":
							processor = x => this.Information = DataCube.Unescape (x);
							break;

						case "Annotation":
							processor = x => this.AddAnnotation (DataCube.Unescape (x));
							break;

						case "DimensionNames":
							processor = x => this.dimensionNames.Add (DataCube.Unescape (x));
							break;

						case "NaturalTableDimensionNames":
							processor = x => this.naturalTableDimensionNames.Add (DataCube.Unescape (x));
							break;

						case "DimensionValues":
							processor =
								x =>
								{
									int pos = x.IndexOf ('=');
									var key = DataCube.Unescape (x.Substring (0, pos));
									var val = DataCube.Unescape (x.Substring (pos+1));
									this.dimensions.Add (key, DimensionValues.Parse (val));
								};
							break;

						case "Values":
							processor =
								x =>
								{
									int pos = x.IndexOf ('=');
									var key = DataCube.Unescape (x.Substring (0, pos));
									var val = double.Parse (DataCube.Unescape (x.Substring (pos+1)), System.Globalization.CultureInfo.InvariantCulture);
									this.values.Add (key, val);
								};
							break;

						case "End":
							return;

						default:
							throw new System.FormatException ();
					}
				}
				else
				{
					if (processor == null)
					{
						throw new System.FormatException ();
					}

					processor (line);
				}
			}
		}


		public static string CleanUpLabel(string label)
		{
			return DataCube.CleanUpLabelPrefixOnly (label).Trim ().Replace ('\t', ' ');
		}

		public static string CleanUpLabelPrefixOnly(string label)
		{
			int pos = label.LastIndexOf (Data.DataCube.LabelSortPrefixSeparator);

			if (pos < 0)
			{
				return label;
			}
			else
			{
				return label.Substring (pos+1);
			}
		}

		protected virtual IEnumerable<string> GetAnnotations()
		{
			yield break;
		}

		protected virtual void AddAnnotation(string annotation)
		{
		}

		private void AddDimensionVector(DimensionVector vector)
		{
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

		private Accumulator Accumulate(string[] dimensions)
		{
			List<string> axes;
			return this.Accumulate (dimensions, out axes);
		}
		
		private Accumulator Accumulate(string[] dimensions, out List<string> axes)
		{
			List<string> dims;
			
			var regex = this.GetExtractionRegex (dimensions, out dims, out axes);

			int[] groupIndexMap = DataCube.GetGroupIndexMapAndUpdateAxes (dims, axes);

			Accumulator accumulator = new Accumulator ();
			System.Text.StringBuilder key = new System.Text.StringBuilder ();

			int expectedGroupsInRegexMatch = groupIndexMap.Length + 1;

			foreach (var item in this.values)
			{
				var match = regex.Match (item.Key);

				if (match.Success)
				{
					int num = match.Groups.Count;

					System.Diagnostics.Debug.Assert (num == expectedGroupsInRegexMatch);

					if (num == expectedGroupsInRegexMatch)
					{
						key.Length = 0;

						foreach (int groupIndex in groupIndexMap)
						{
							if (key.Length > 0)
							{
								key.Append (DataCube.LabelSeparator);
							}

							key.Append (match.Groups[groupIndex].Value);
						}
						
						accumulator.Accumulate (key.ToString (), item.Value);
					}
				}
			}

			return accumulator;
		}

		private System.Text.RegularExpressions.Regex GetExtractionRegex(string[] dimensions, out List<string> dims, out List<string> axes)
		{
			dims = new List<string> (dimensions);
			axes = new List<string> ();

			System.Text.StringBuilder buffer = new System.Text.StringBuilder ("^");
			
			foreach (string name in this.dimensionNames)
			{
				string prefix = name + "=";

				//	Is this dimension specified as an extraction axe, such as "foo" in the dimensions
				//	vector provided by the caller ?
				
				int pos = dims.FindIndex (x => x == name);

				if (pos >= 0)
				{
					//	Found an exact match, which means that we have found the dimension
					//	which we will enumerate to produce our sequence. Extract "foo=*".

					if (buffer.Length > 1)
					{
						buffer.Append (DimensionVector.DimensionSeparator);
					}

					buffer.Append (RegexFactory.Escape (name));
					buffer.Append (DimensionVector.KeyValueSeparator);
					buffer.Append ("([^" + DimensionVector.DimensionSeparator.ToString () + "]*)");

					axes.Add (name);

					continue;
				}

				//	Is this dimension specified as an extraction criterion, such as "foo=123" in the
				//	dimensions vector provided by the caller ?

				pos = dims.FindIndex (x => x.StartsWith (prefix));

				if (pos >= 0)
				{
					//	Found an extraction match; just use it as is. Extract "foo=123".

					if (buffer.Length > 1)
					{
						buffer.Append (DimensionVector.DimensionSeparator);
					}

					buffer.Append (RegexFactory.Escape (name));
					buffer.Append (DimensionVector.KeyValueSeparator);
					buffer.Append (RegexFactory.Escape (dims[pos].Substring (prefix.Length)));
					dims.RemoveAt (pos);

					continue;
				}

				//	Else, the dimension was not provided by the caller as something he is interested
				//	in; Extract the dimension "foo=*".

				if (buffer.Length > 1)
				{
					buffer.Append (DimensionVector.DimensionSeparator);
				}

				buffer.Append (RegexFactory.Escape (prefix));
				buffer.Append ("([^" + DimensionVector.DimensionSeparator.ToString () + "]*)");
			}

			buffer.Append ('$');

			return new System.Text.RegularExpressions.Regex (buffer.ToString (), System.Text.RegularExpressions.RegexOptions.Compiled);
		}

		private static int[] GetGroupIndexMapAndUpdateAxes(List<string> dims, List<string> axes)
		{
			int[] groupIndexes = new int[axes.Count];

			for (int i = 0; i < groupIndexes.Length; i++)
			{
				groupIndexes[i] = dims.IndexOf (axes[i]) + 1;
			}

			axes.Clear ();
			axes.AddRange (dims);
			
			return groupIndexes;
		}

		
		private static string Escape(string text)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (char c in text)
			{
				switch (c)
				{
					case '\\':
						buffer.Append ("\\\\");
						break;

					case ':':
						buffer.Append ("\\.");
						break;

					case ';':
						buffer.Append ("\\,");
						break;

					case '=':
						buffer.Append ("\\-");
						break;

					case '\n':
						buffer.Append ("\\n");
						break;

					default:
						buffer.Append (c);
						break;

				}
			}

			return buffer.ToString ();
		}

		private static string Unescape(string text)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			bool escape = false;

			foreach (char c in text)
			{
				if (escape)
				{
					switch (c)
					{
						case '\\':
							buffer.Append ('\\');
							break;

						case '.':
							buffer.Append (':');
							break;

						case ',':
							buffer.Append (';');
							break;

						case '-':
							buffer.Append ('=');
							break;

						case 'n':
							buffer.Append ('\n');
							break;

						default:
							throw new System.InvalidOperationException ("Unexpected escape sequence \\" + escape);
					}

					escape = false;
				}
				else if (c == '\\')
				{
					escape = true;
				}
				else
				{
					buffer.Append (c);
				}
			}

			return buffer.ToString ();
		}


		#region DimensionValues Class

		private sealed class DimensionValues : HashSet<string>
		{
			public override string ToString()
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				buffer.Append (this.Count.ToString (System.Globalization.CultureInfo.InvariantCulture));
				this.ForEach (x => buffer.Append (";" + DataCube.Escape (x)));
				return buffer.ToString ();
			}

			public void Restore(string line)
			{
				line.Split (';').Skip (1).ForEach (x => this.Add (DataCube.Unescape (x)));
			}

			public static DimensionValues Parse(string s)
			{
				DimensionValues values = new DimensionValues ();
				values.Restore (s);
				return values;
			}
		}

		#endregion


		public const char LabelSeparator = '+';
		public const char LabelSortPrefixSeparator = '¦';

		readonly Dictionary<string, double> values;
		readonly Dictionary<string, DimensionValues> dimensions;
		readonly List<string> dimensionNames;
		readonly List<string> naturalTableDimensionNames;
	}
}
