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
			this.values          = new Dictionary<string, decimal?> ();
			this.shortTitles     = new List<Dictionary<int, FormattedText>> ();
			this.fullTitles      = new List<Dictionary<int, FormattedText>> ();
			this.maxCoords       = new List<int> ();
			this.dimensionTitles = new List<FormattedText> ();
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

			cube.SetShortTitle (0, 0, "X0");
			cube.SetShortTitle (0, 1, "X1");
			cube.SetShortTitle (0, 2, "X2");
			cube.SetShortTitle (0, 3, "X3");

			cube.SetShortTitle (1, 0, "Y0");
			cube.SetShortTitle (1, 1, "Y1");
			cube.SetShortTitle (1, 2, "Y2");

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

			System.Diagnostics.Debug.Assert (cube.GetShortTitle (0, 1) == "X1");
			System.Diagnostics.Debug.Assert (cube.GetShortTitle (1, 1) == "Y1");
			System.Diagnostics.Debug.Assert (cube.GetShortTitle (1, 2) == "Y2");
			System.Diagnostics.Debug.Assert (cube.GetShortTitle (1, 3) == FormattedText.Null);

			System.Diagnostics.Debug.Assert (cube.GetCount (0) == 4);
			System.Diagnostics.Debug.Assert (cube.GetCount (1) == 3);
		}


		public void FilteredCopy(Cube src, int dimension1, int dimension2, List<FormattedText> filter1, List<FormattedText> filter2, int[] constants)
		{
			//	Copie un cube de dimensions quelconques dans un cube à 2 dimensions, en filtrant les valeurs.
			//	Le cube ainsi obtenu est "prêt à l'emploi" pour le dessin.
			this.Clear ();

			var indexes1 = Cube.GetFilterIndexes (src.GetCount (dimension1), src.shortTitles[dimension1], filter1);
			var indexes2 = Cube.GetFilterIndexes (src.GetCount (dimension2), src.shortTitles[dimension2], filter2);

			this.Dimensions = 2;

			for (int i = 0; i < indexes1.Count; i++)
			{
				this.SetShortTitle (0, i, src.GetShortTitle (dimension1, indexes1[i]));
				this.SetFullTitle  (0, i, src.GetFullTitle  (dimension1, indexes1[i]));
			}

			for (int i = 0; i < indexes2.Count; i++)
			{
				this.SetShortTitle (1, i, src.GetShortTitle (dimension2, indexes2[i]));
				this.SetFullTitle  (1, i, src.GetFullTitle  (dimension2, indexes2[i]));
			}

			int nx = src.GetCount (dimension1);
			for (int x = 0; x < nx; x++)
			{
				int ny = src.GetCount (dimension2);
				for (int y = 0; y < ny; y++)
				{
					var key = src.GetFilterKey (dimension1, dimension2, indexes1, indexes2, constants, x, y);
					if (key != null)
					{
						var value = src.GetValue (key);
						this.SetValue (x, y, value);
					}
				}
			}
		}

		private static List<int> GetFilterIndexes(int n, Dictionary<int, FormattedText> titles, List<FormattedText> filter)
		{
			var indexes = new List<int> ();

			for (int i = 0; i < n; i++)
			{
				FormattedText title;
				if (titles.TryGetValue (i, out title))
				{
					if (!filter.Contains (title))
					{
						indexes.Add (i);
					}
				}
				else
				{
					indexes.Add (i);
				}
			}

			return indexes;
		}

		private int[] GetFilterKey(int dimension1, int dimension2, List<int> indexes1, List<int> indexes2, int[] constants, int x, int y)
		{
			if (x >= indexes1.Count || y >= indexes2.Count)
			{
				return null;
			}

			var key = new List<int> ();

			for (int d = 0; d < this.dimensions; d++)
			{
				if (d == dimension1)
				{
					key.Add (indexes1[x]);
				}
				else if (d == dimension2)
				{
					key.Add (indexes2[y]);
				}
				else
				{
					if (constants == null || d >= constants.Length)
					{
						key.Add (0);
					}
					else
					{
						key.Add (constants[d]);
					}
				}
			}

			return key.ToArray ();
		}


		public void ThresholdCopy(Cube src, decimal threshold)
		{
			System.Diagnostics.Debug.Assert (src.Dimensions == 2);
			this.Clear ();
			this.Dimensions = 2;

			int nx = src.GetCount (0);
			int ny = src.GetCount (1);

			var used = new List<int> ();
			bool hasOther = false;
			for (int x = 0; x < nx; x++)
			{
				decimal sum = 0;
				for (int y = 0; y < ny; y++)
				{
					sum += System.Math.Abs (src.GetValue (x, y).GetValueOrDefault ());
				}

				for (int y = 0; y < ny; y++)
				{
					var value = System.Math.Abs (src.GetValue (x, y).GetValueOrDefault ());

					if (sum != 0 && value/sum >= threshold)
					{
						if (!used.Contains (y))
						{
							used.Add (y);
						}
						else
						{
							if (value != 0)
							{
								hasOther = true;
							}
						}
					}
				}
			}
			used.Sort ();

			for (int x = 0; x < nx; x++)
			{
				decimal sum = 0;
				for (int y = 0; y < ny; y++)
				{
					sum += System.Math.Abs (src.GetValue (x, y).GetValueOrDefault ());
				}

				decimal others = 0;
				for (int y = 0; y < ny; y++)
				{
					var value = System.Math.Abs (src.GetValue (x, y).GetValueOrDefault ());

					if (sum != 0 && value/sum >= threshold)
					{
						int u = used.IndexOf (y);
						this.SetValue (x, u, value);
					}
					else
					{
						others += value;
					}
				}

				if (hasOther)
				{
					this.SetValue (x, used.Count, others);
				}
			}

			for (int x = 0; x < nx; x++)
			{
				this.SetShortTitle (0, x, src.GetShortTitle (0, x));
				this.SetFullTitle  (0, x, src.GetFullTitle  (0, x));
			}

			for (int u = 0; u < used.Count; u++)
			{
				int y = used[u];
				this.SetShortTitle (1, u, src.GetShortTitle (1, y));
				this.SetFullTitle  (1, u, src.GetFullTitle  (1, y));
			}

			if (hasOther)
			{
				this.SetShortTitle (1, used.Count, "Autres");
			}
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
					this.shortTitles.Clear ();
					this.fullTitles.Clear ();
					this.dimensionTitles.Clear ();

					for (int i = 0; i < this.dimensions; i++)
					{
						this.maxCoords.Add (-1);
						this.shortTitles.Add (new Dictionary<int, FormattedText> ());
						this.fullTitles.Add (new Dictionary<int, FormattedText> ());
						this.dimensionTitles.Add (FormattedText.Empty);
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
				this.shortTitles[i].Clear ();
				this.fullTitles[i].Clear ();
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.values.Count == 0;
			}
		}

		public FormattedText GetDimensionTitle(int dimension)
		{
			System.Diagnostics.Debug.Assert (dimension >= 0 && dimension < this.dimensions);
			return this.dimensionTitles[dimension];
		}

		public void SetDimensionTitle(int dimension, FormattedText title)
		{
			System.Diagnostics.Debug.Assert (dimension >= 0 && dimension < this.dimensions);
			this.dimensionTitles[dimension] = title;
		}


		public FormattedText GetTitle(int dimension, int coordIndex)
		{
			//	Retourne un titre, le complet s'il existe, sinon le court.
			var title = this.GetFullTitle (dimension, coordIndex);

			if (title.IsNullOrEmpty)
			{
				title = this.GetShortTitle (dimension, coordIndex);
			}

			return title;
		}


		public FormattedText GetShortTitle(int dimension, int coordIndex)
		{
			System.Diagnostics.Debug.Assert (dimension >= 0 && dimension < this.dimensions);

			FormattedText title;
			if (this.shortTitles[dimension].TryGetValue (coordIndex, out title))
			{
				return title;
			}
			else
			{
				return FormattedText.Null;
			}
		}

		public void SetShortTitle(int dimension, int coordIndex, FormattedText title)
		{
			System.Diagnostics.Debug.Assert (dimension >= 0 && dimension < this.dimensions);
			this.maxCoords[dimension] = System.Math.Max (this.maxCoords[dimension], coordIndex);
			this.shortTitles[dimension][coordIndex] = title;
		}


		public FormattedText GetFullTitle(int dimension, int coordIndex)
		{
			System.Diagnostics.Debug.Assert (dimension >= 0 && dimension < this.dimensions);

			FormattedText title;
			if (this.fullTitles[dimension].TryGetValue (coordIndex, out title))
			{
				return title;
			}
			else
			{
				return FormattedText.Null;
			}
		}

		public void SetFullTitle(int dimension, int coordIndex, FormattedText title)
		{
			System.Diagnostics.Debug.Assert (dimension >= 0 && dimension < this.dimensions);
			this.maxCoords[dimension] = System.Math.Max (this.maxCoords[dimension], coordIndex);
			this.fullTitles[dimension][coordIndex] = title;
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
		private readonly List<Dictionary<int, FormattedText>>	shortTitles;
		private readonly List<Dictionary<int, FormattedText>>	fullTitles;
		private readonly List<FormattedText>					dimensionTitles;

		private int												dimensions;
	}
}
