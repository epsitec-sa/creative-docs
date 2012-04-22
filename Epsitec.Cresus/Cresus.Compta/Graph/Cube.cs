//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta;

using System.Collections.Generic;

namespace Epsitec.Cresus.Compta.Graph
{
	public class Cube
	{
		public Cube(int dimensions)
		{
			System.Diagnostics.Debug.Assert (dimensions >= 1 && dimensions <= 10);
			this.dimensions = dimensions;
			this.values = new Dictionary<string, decimal?> ();
		}


		static Cube()
		{
			//	Auto-test intégré.
			var cube = new Cube (2);

			cube.SetValue (0, 0, 10.0m);
			cube.SetValue (1, 0, 11.0m);
			cube.SetValue (2, 0, 12.0m);
			cube.SetValue (3, 0, 13.0m);

			cube.SetValue (0, 1, 100.0m);
			cube.SetValue (1, 1, 101.0m);
			cube.SetValue (2, 1, 102.0m);
			cube.SetValue (3, 1, 103.0m);

			cube.SetValue (0, 2, 1000.0m);
			cube.SetValue (1, 2, 1001.0m);
			cube.SetValue (2, 2, 1002.0m);
			cube.SetValue (3, 2, 1003.0m);

			System.Diagnostics.Debug.Assert (cube.GetValue (0, 0) == 10.0m);
			System.Diagnostics.Debug.Assert (cube.GetValue (1, 0) == 11.0m);
			System.Diagnostics.Debug.Assert (cube.GetValue (1, 2) == 1001.0m);
			System.Diagnostics.Debug.Assert (cube.GetValue (0, 3) == null);
			System.Diagnostics.Debug.Assert (cube.GetValue (4, 0) == null);

			decimal min, max;

			cube.GetMinMax (null, null, out min, out max);
			System.Diagnostics.Debug.Assert (min == 10.0m && max == 1003.0m);

			cube.GetMinMax (1, null, out min, out max);
			System.Diagnostics.Debug.Assert (min == 11.0m && max == 1001.0m);

			cube.GetMinMax (null, 2, out min, out max);
			System.Diagnostics.Debug.Assert (min == 1000.0m && max == 1003.0m);

			cube.GetMinMax (3, 1, out min, out max);
			System.Diagnostics.Debug.Assert (min == 103.0m && max == 103.0m);
		}


		public void Clear()
		{
			this.values.Clear ();
		}


		public void GetMinMax(int? x, out decimal min, out decimal max)
		{
			int?[] coords = { x };
			this.GetMinMax (out min, out max, coords);
		}

		public void GetMinMax(int? x, int? y, out decimal min, out decimal max)
		{
			int?[] coords = { x, y };
			this.GetMinMax (out min, out max, coords);
		}

		public void GetMinMax(int? x, int? y, int? z, out decimal min, out decimal max)
		{
			int?[] coords = { x, y, z };
			this.GetMinMax (out min, out max, coords);
		}

		public void GetMinMax(out decimal min, out decimal max, params int?[] coords)
		{
			System.Diagnostics.Debug.Assert (this.dimensions == coords.Length);
			min = decimal.MaxValue;
			max = decimal.MinValue;

			foreach (var pair in this.values)
			{
				var value = pair.Value;

				if (value.HasValue)
				{
					var key = pair.Key.Split (';');
					bool take = true;

					for (int i = 0; i < this.dimensions; i++)
					{
						if (coords[i].HasValue)
						{
							int c = int.Parse (key[i]);
							if (c != coords[i].Value)
							{
								take = false;
								break;
							}
						}
					}

					if (take)
					{
						min = System.Math.Min (min, value.Value);
						max = System.Math.Max (max, value.Value);
					}
				}
			}
		}


		public decimal? GetValue(int x)
		{
			System.Diagnostics.Debug.Assert (this.dimensions == 1);
			return this.GetValue (Cube.CoordsToKey (x));
		}

		public void SetValue(int x, decimal? value)
		{
			System.Diagnostics.Debug.Assert (this.dimensions == 1);
			this.SetValue (Cube.CoordsToKey (x), value);
		}


		public decimal? GetValue(int x, int y)
		{
			System.Diagnostics.Debug.Assert (this.dimensions == 2);
			return this.GetValue (Cube.CoordsToKey (x, y));
		}

		public void SetValue(int x, int y, decimal? value)
		{
			System.Diagnostics.Debug.Assert (this.dimensions == 2);
			this.SetValue (Cube.CoordsToKey (x, y), value);
		}


		public decimal? GetValue(int x, int y, int z)
		{
			System.Diagnostics.Debug.Assert (this.dimensions == 3);
			return this.GetValue (Cube.CoordsToKey (x, y, z));
		}

		public void SetValue(int x, int y, int z, decimal? value)
		{
			System.Diagnostics.Debug.Assert (this.dimensions == 3);
			this.SetValue (Cube.CoordsToKey (x, y, z), value);
		}


		public decimal? GetValue(params int[] coords)
		{
			System.Diagnostics.Debug.Assert (this.dimensions == coords.Length);
			return this.GetValue (Cube.CoordsToKey (coords));
		}

		public void SetValue(decimal? value, params int[] coords)
		{
			System.Diagnostics.Debug.Assert (this.dimensions == coords.Length);
			this.SetValue (Cube.CoordsToKey (coords), value);
		}


		private decimal? GetValue(string key)
		{
			decimal? value;
			if (this.values.TryGetValue (key, out value))
			{
				return value;
			}
			else
			{
				return null;
			}
		}

		private void SetValue(string key, decimal? value)
		{
			if (value.HasValue)
			{
				this.values[key] = value;
			}
			else
			{
				if (this.values.ContainsKey (key))
				{
					this.values.Remove (key);
				}
			}
		}


		private static string CoordsToKey(int x)
		{
			System.Diagnostics.Debug.Assert (x >= 0);
			return x.ToString (System.Globalization.CultureInfo.InvariantCulture);
		}

		private static string CoordsToKey(int x, int y)
		{
			System.Diagnostics.Debug.Assert (x >= 0 && y >= 0);
			return x.ToString (System.Globalization.CultureInfo.InvariantCulture) + ";"
				 + y.ToString (System.Globalization.CultureInfo.InvariantCulture);
		}

		private static string CoordsToKey(int x, int y, int z)
		{
			System.Diagnostics.Debug.Assert (x >= 0 && y >= 0 && z >= 0);
			return x.ToString (System.Globalization.CultureInfo.InvariantCulture) + ";"
				 + y.ToString (System.Globalization.CultureInfo.InvariantCulture) + ";"
				 + z.ToString (System.Globalization.CultureInfo.InvariantCulture);
		}

		private static string CoordsToKey(params int[] coords)
		{
			if (coords.Length == 1)
			{
				return Cube.CoordsToKey (coords[0]);
			}
			else if (coords.Length == 2)
			{
				return Cube.CoordsToKey (coords[0], coords[1]);
			}
			else if (coords.Length == 3)
			{
				return Cube.CoordsToKey (coords[0], coords[1], coords[2]);
			}
			else
			{
				var list = new List<string> ();

				foreach (var coord in coords)
				{
					System.Diagnostics.Debug.Assert (coord >= 0);
					list.Add (coord.ToString (System.Globalization.CultureInfo.InvariantCulture));
				}

				return string.Join (";", list);
			}
		}


		private readonly int							dimensions;
		private readonly Dictionary<string, decimal?>	values;
	}
}
