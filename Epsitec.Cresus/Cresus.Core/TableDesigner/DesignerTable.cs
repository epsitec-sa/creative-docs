//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.TableDesigner
{
	public class DesignerTable
	{
		public DesignerTable()
		{
			this.dimensions = new List<DesignerDimension> ();
			this.values = new DesignerValues ();
		}


		public List<DesignerDimension> Dimensions
		{
			get
			{
				return this.dimensions;
			}
		}

		public DesignerValues Values
		{
			get
			{
				return this.values;
			}
		}


		public DesignerDimension GetDimension(string code)
		{
			return this.dimensions.Where (x => x.Code == code).FirstOrDefault ();
		}


		public void CleanUp()
		{
			//	Supprime les points doublons dans toutes les dimensions.
			var data = this.ExportValues ();

			foreach (var dimension in this.dimensions)
			{
				dimension.CleanUp ();
			}

			this.ImportValues (data);
		}


		#region Robust import/export
		public RobustData ExportValues()
		{
			//	Exporte toutes les valeurs dans un format robuste, résistant aux modifications des points des dimensions.
			//	Ceci qui permet d'ajuster intelligement les valeurs lors de l'importation, suite à n'importes quelles
			//	modifications des points des dimensions (ajout, suppression, déplacement ou modification). Si un point
			//	décimal est modifié, les valeurs correspondantes sont ajustées par interpolation linéaire.
			//	En revanche, le format ne résiste pas aux modifications des dimensions elles-mêmes (ajout ou suppression
			//	d'une dimension par exemple).
			//
			//	Dans cet exemple à deux dimensions, le point 15 a été ajouté :
			//
			//	         10     20                   10     15     20
			//	      +------+------+             +------+------+------+
			//	  200 | 4.00 | 8.00 |   -->   200 | 4.00 |      | 8.00 |
			//	      +------+------+             +------+------+------+
			//	  300 | 5.00 | 9.00 |         300 | 5.00 |      | 9.00 |
			//	      +------+------+             +------+------+------+
			//
			//	Dans cet exemple à deux dimensions, le point 300 a été modifié en 250 :
			//
			//	         10     20                   10     20
			//	      +------+------+             +------+------+
			//	  200 | 4.00 | 8.00 |   -->   200 | 4.00 | 8.00 |
			//	      +------+------+             +------+------+
			//	  300 | 5.00 | 9.00 |         250 | 4.16 | 7.50 |
			//	      +------+------+             +------+------+

			var data = new RobustData ();

			foreach (var dimension in this.dimensions)
			{
				data.Dimensions.Add (new DesignerDimension (dimension));
			}

			foreach (var pair in this.values.Data)
			{
				string intKey = pair.Key;
				decimal value = pair.Value;

				data.Values.Add (this.IntKeyToStringKey (intKey), value);
			}

			return data;
		}

		public void ImportValues(RobustData data)
		{
			//	Importe toutes les valeurs à partir des données robustes.
			System.Diagnostics.Debug.Assert (data.Dimensions.Count == this.dimensions.Count);

			for (int i=0; i<this.dimensions.Count; i++)
			{
				System.Diagnostics.Debug.Assert (data.Dimensions[i].Code == this.dimensions[i].Code);
			}

			this.values.Clear ();

			foreach (var pair in data.Values)
			{
				string stringKey = pair.Key;
				decimal value = pair.Value;

				int[] intKey;
				if (this.StringKeyToIntKey (data, stringKey, out intKey, ref value))
				{
					this.values.SetValue (intKey, value);
				}
			}
		}


		private string IntKeyToStringKey(string intKey)
		{
			//	Transforme un clé "0.14.2" en une clé "400.1200.STD-1".
			var list = this.IntKeyToStringKeyArray (intKey);
			return string.Join (".", list);
		}

		private bool StringKeyToIntKey(RobustData data, string stringKey, out int[] intKey, ref decimal value)
		{
			return this.StringKeyToIntKey (data.Dimensions, stringKey, out intKey, ref value);
		}

		private static List<string> GetModifiedPoints(List<string> oldPoints, List<string> newPoints)
		{
			var list = new List<string> ();

			foreach (var p in newPoints)
			{
				if (!oldPoints.Contains (p))
				{
					list.Add (p);
				}
			}

			return list;
		}

		private static int Nearest(List<string> points, decimal point)
		{
			decimal delta = decimal.MaxValue;
			int index = -1;

			for (int i = 0; i < points.Count; i++)
			{
				decimal p = decimal.Parse (points[i]);
				decimal d = System.Math.Abs (p-point);

				if (delta > d)
				{
					delta = d;
					index = i;
				}
			}

			return index;
		}


		public class RobustData
		{
			public RobustData()
			{
				this.dimensions = new List<DesignerDimension> ();
				this.values = new Dictionary<string, decimal> ();
			}

			public List<DesignerDimension> Dimensions
			{
				get
				{
					return this.dimensions;
				}
			}

			public Dictionary<string, decimal> Values
			{
				get
				{
					return this.values;
				}
			}

			private readonly List<DesignerDimension>			dimensions;
			private readonly Dictionary<string, decimal>		values;
		}
		#endregion


		public string[] IntKeyToStringKeyArray(string intKey)
		{
			//	Transforme un clé "0.14.2" en une clé "400.1200.STD-1".
			string[] parts = intKey.Split ('.');
			string[] list = new string[parts.Length];

			for (int i=0; i<parts.Length; i++)
			{
				int j = int.Parse (parts[i]);
				list[i] = this.dimensions[i].Points[j];
			}

			return list;
		}

		public bool StringKeyToIntKey(List<DesignerDimension> dimensions, string stringKey, out int[] intKey, ref decimal value)
		{
			//	Transforme un clé "400.1200.STD-1" en une clé "0.14.2".
			//	Si le point n'existe plus, on cherche le plus proche et on ajuste la valeur.
			string[] parts = stringKey.Split ('.');
			intKey = new int[parts.Length];

			for (int i=0; i<parts.Length; i++)
			{
				string s = parts[i];
				int j = this.dimensions[i].Points.IndexOf (s);

				if (j == -1)
				{
					if (!this.dimensions[i].HasDecimal)
					{
						return false;
					}

					decimal initial;
					if (!decimal.TryParse (s, out initial))
					{
						return false;
					}

					var points = DesignerTable.GetModifiedPoints (dimensions[i].Points, this.dimensions[i].Points);
					j = DesignerTable.Nearest (points, initial);

					if (initial == 0 || j == -1)
					{
						return false;
					}

					decimal existing = decimal.Parse (points[j]);

					value *= existing/initial;  // interpolation linéaire

					j = this.dimensions[i].Points.IndexOf (points[j]);
				}

				intKey[i] = j;
			}

			return true;
		}


		private readonly List<DesignerDimension>		dimensions;
		private readonly DesignerValues					values;
	}
}
