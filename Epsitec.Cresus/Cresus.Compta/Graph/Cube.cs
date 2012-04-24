//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta;

using System.Collections.Generic;

namespace Epsitec.Cresus.Compta.Graph
{
	/// <summary>
	/// Cette classe permet de stocker des données numériques à n dimensions, en vue de les représenter graphiquement.
	/// </summary>
	public class Cube
	{
		public Cube()
		{
			this.values    = new Dictionary<string, decimal?> ();
			this.titles    = new List<Dictionary<int,FormattedText>>();
			this.maxCoords = new List<int> ();
		}


		static Cube()
		{
			//	Auto-test intégré.
			var cube = new Cube ();
			cube.Dimensions = 2;

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

			cube.SetTitle (0, 0, "X0");
			cube.SetTitle (0, 1, "X1");
			cube.SetTitle (0, 2, "X2");
			cube.SetTitle (0, 3, "X3");

			cube.SetTitle (1, 0, "Y0");
			cube.SetTitle (1, 1, "Y1");
			cube.SetTitle (1, 2, "Y2");

			System.Diagnostics.Debug.Assert (cube.Dimensions == 2);
			System.Diagnostics.Debug.Assert (cube.GetCount (0) == 4);
			System.Diagnostics.Debug.Assert (cube.GetCount (1) == 3);

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

			System.Diagnostics.Debug.Assert (cube.GetTitle (0, 1) == "X1");
			System.Diagnostics.Debug.Assert (cube.GetTitle (1, 1) == "Y1");
			System.Diagnostics.Debug.Assert (cube.GetTitle (1, 2) == "Y2");
			System.Diagnostics.Debug.Assert (cube.GetTitle (1, 3) == FormattedText.Null);

			System.Diagnostics.Debug.Assert (cube.GetCount (0) == 4);
			System.Diagnostics.Debug.Assert (cube.GetCount (1) == 3);
		}


		public int Dimensions
		{
			get
			{
				return this.dimensions;
			}
			set
			{
				System.Diagnostics.Debug.Assert (value >= 1 && value <= 10);

				if (this.dimensions != value)
				{
					this.dimensions = value;

					this.maxCoords.Clear ();
					this.titles.Clear ();

					for (int i = 0; i < this.dimensions; i++)
					{
						this.maxCoords.Add (-1);
						this.titles.Add (new Dictionary<int, FormattedText> ());
					}
				}
			}
		}


		public void Clear()
		{
			//	Vide le contenu du cube, sans modifier le nombre de dimensions.
			this.values.Clear ();

			for (int i = 0; i < this.dimensions; i++)
			{
				this.maxCoords[i] = -1;
				this.titles[i].Clear ();
			}
		}


		public FormattedText GetTitle(int dimension, int coordIndex)
		{
			System.Diagnostics.Debug.Assert (dimension >= 0 && dimension < this.dimensions);

			FormattedText title;
			if (this.titles[dimension].TryGetValue (coordIndex, out title))
			{
				return title;
			}
			else
			{
				return FormattedText.Null;
			}
		}

		public void SetTitle(int dimension, int coordIndex, FormattedText title)
		{
			System.Diagnostics.Debug.Assert (dimension >= 0 && dimension < this.dimensions);
			this.maxCoords[dimension] = System.Math.Max (this.maxCoords[dimension], coordIndex);
			this.titles[dimension][coordIndex] = title;
		}


		public int GetCount(int dimension)
		{
			System.Diagnostics.Debug.Assert (dimension >= 0 && dimension < this.dimensions);
			return this.maxCoords[dimension]+1;
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
			this.UpdateMaxCoords (x);
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
			this.UpdateMaxCoords (x, y);
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
			this.UpdateMaxCoords (x, y, z);
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
			this.UpdateMaxCoords (coords);
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


		private void UpdateMaxCoords(int x)
		{
			System.Diagnostics.Debug.Assert (this.dimensions == 1);
			this.maxCoords[0] = System.Math.Max (this.maxCoords[0], x);
		}

		private void UpdateMaxCoords(int x, int y)
		{
			System.Diagnostics.Debug.Assert (this.dimensions == 2);
			this.maxCoords[0] = System.Math.Max (this.maxCoords[0], x);
			this.maxCoords[1] = System.Math.Max (this.maxCoords[1], y);
		}

		private void UpdateMaxCoords(int x, int y, int z)
		{
			System.Diagnostics.Debug.Assert (this.dimensions == 3);
			this.maxCoords[0] = System.Math.Max (this.maxCoords[0], x);
			this.maxCoords[1] = System.Math.Max (this.maxCoords[1], y);
			this.maxCoords[2] = System.Math.Max (this.maxCoords[2], z);
		}

		private void UpdateMaxCoords(params int[] coords)
		{
			System.Diagnostics.Debug.Assert (this.dimensions == coords.Length);
			for (int i = 0; i < coords.Length; i++)
			{
				this.maxCoords[i] = System.Math.Max (this.maxCoords[i], coords[i]);
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


		private readonly Dictionary<string, decimal?>			values;
		private readonly List<int>								maxCoords;
		private readonly List<Dictionary<int, FormattedText>>	titles;

		private int												dimensions;
	}
}
