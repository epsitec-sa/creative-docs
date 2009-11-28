//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Data
{
	public sealed class DimensionVector : System.Collections.IEnumerable, IEnumerable<Dimension>, System.IEquatable<DimensionVector>
	{
		public DimensionVector()
		{
			this.dimensions = new List<Dimension> ();
		}

		public DimensionVector(string compiledVector)
			: this ()
		{
			string[] items = compiledVector.Split (DimensionVector.KeyValueSeparator, DimensionVector.DimensionSeparator);

			System.Diagnostics.Debug.Assert ((items.Length % 2) == 0);

			for (int i = 0; i < items.Length; i += 2)
			{
				this.dimensions.Add (new Dimension (items[i+0], items[i+1]));
			}
		}

		public DimensionVector(DimensionVector vector)
			: this ()
		{
			this.dimensions.AddRange (vector.dimensions);
		}

		
		public IEnumerable<string> Keys
		{
			get
			{
				return this.dimensions.Select (x => x.Key);
			}
		}

		public string this[string key]
		{
			get
			{
				int index = this.dimensions.FindIndex (x => x.Key == key);

				if (index < 0)
				{
					return null;
				}
				else
				{
					return this.dimensions[index].Value;
				}
			}
		}

		
		public void Add(string key, string value)
		{
			this.Add (new Dimension (key, value));
		}
		
		public void Add(Dimension item)
		{
			//	Find the last item in the list which is smaller than the one to be
			//	inserted and insert after it.
			
			int index = this.dimensions.FindLastIndex (x => item.CompareTo (x) > 0) + 1;

			if (index > 0)
			{
				if (this.dimensions[index-1].Key == item.Key)
				{
					throw new System.ArgumentException ("Duplicate dimension");
				}
			}

			this.dimensions.Insert (index, item);
		}

		public void Add(DimensionVector vector)
		{
			if (this.dimensions.Count == 0)
			{
				this.dimensions.AddRange (vector.dimensions);
			}
			else
			{
				foreach (Dimension item in vector)
				{
					this.Add (item);
				}
			}
		}

		public string Compile()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (var item in this.dimensions)
			{
				if (buffer.Length > 0)
				{
					buffer.Append (DimensionVector.DimensionSeparator);
				}
				buffer.Append (item.Key);
				buffer.Append (DimensionVector.KeyValueSeparator);
				buffer.Append (item.Value);
			}

			return buffer.ToString ();
		}


		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.dimensions.GetEnumerator ();
		}

		#endregion

		#region IEnumerable<Dimension> Members

		IEnumerator<Dimension> IEnumerable<Dimension>.GetEnumerator()
		{
			return this.dimensions.GetEnumerator ();
		}

		#endregion

		#region IEquatable<DimensionVector> Members

		public bool Equals(DimensionVector other)
		{
			return this.Compile () == other.Compile ();
		}

		#endregion


		public const char						KeyValueSeparator	= '~'; //'\u2E30';
		public const char						DimensionSeparator	= '£'; //'\u2E31';
		
		private readonly						List<Dimension> dimensions;
	}
}
